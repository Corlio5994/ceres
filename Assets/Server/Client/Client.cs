using System;
using System.Threading.Tasks;
using UnityEngine;

namespace GameServer {    
    public struct ClientDatabaseData {
        public Vector3 position;
    }

    public partial class Client {
        public readonly int id;
        private const int dataBufferSize = 4096;
        public bool loggedIn { get; private set; } = false;
        public Firebase.Auth.FirebaseUser user { get; private set; }

        public OtherPlayer player;

        public Client (int id) {
            this.id = id;
            tcp = new TCP (this);
            udp = new UDP (this);
        }

        public async Task<bool> Login (string email, string password) {
            if (loggedIn) return false;

            user = await Authoriser.LoginUser (email, password);
            if (user == null) return false;

            ClientDatabaseData data = await Database.GetUser (user.UserId);

            loggedIn = true;
            player = (OtherPlayer) GameManager.SpawnPlayer (data.position, Quaternion.identity, id);
            return loggedIn;
        }

        public async void Logout () {
            if (!loggedIn) return;
            loggedIn = false;

            Database.WriteUser(this);
            ThreadManager.ExecuteOnMainThread (() => {
                GameManager.DestroyPlayer (id);
            });
            
            PacketSender.LogoutSuccessful (this);
            PacketSender.OtherPlayerLoggedOut (this);
        }

        public void Disconnect () {
            if (loggedIn)
                Logout ();

            tcp.Disconnect ();
            udp.Disconnect ();
            Debug.Log ($"[{id}] Disconnected");
        }
    }
}