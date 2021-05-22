using System;
using System.Threading.Tasks;
using UnityEngine;

namespace GameServer {
    public class ClientDatabaseData {
        public Vector3 position = Vector3.zero;
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

            player = (OtherPlayer) GameManager.SpawnPlayer (data.position, Quaternion.identity, id);
            loggedIn = true;
            Debug.Log ($"[{id}] Logged in");
            return true;
        }

        public void Logout () {
            if (!loggedIn) return;

            Database.WriteUser (user.UserId, GetWriteableData ());
            GameManager.DestroyPlayer (id);
            player = null;

            PacketSender.LogoutSuccessful (this);
            PacketSender.OtherPlayerLoggedOut (this);

            Debug.Log ($"[{id}] Logging out");
            loggedIn = false;
        }

        private ClientDatabaseData GetWriteableData () {
            ClientDatabaseData data = new ClientDatabaseData ();

            data.position = player.transform.position;

            return data;
        }

        public void Disconnect () {
            Logout ();

            tcp.Disconnect ();
            udp.Disconnect ();
            Debug.Log ($"[{id}] Disconnected");
        }
    }
}