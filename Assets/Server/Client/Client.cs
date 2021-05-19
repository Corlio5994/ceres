using System;
using UnityEngine;

namespace GameServer {
    public partial class Client {
        public readonly int id;
        private const int dataBufferSize = 4096;
        public bool loggedIn { get; private set; } = false;

        public OtherPlayer player { get; private set; }

        public Client (int id) {
            this.id = id;
            tcp = new TCP (this);
            udp = new UDP (this);
        }

        public void Login () {
            loggedIn = true;
            player = (OtherPlayer) GameManager.SpawnPlayer (Vector3.zero, Quaternion.identity, id);
            PacketSender.LoginAccepted (this);
            PacketSender.OtherPlayerLoggedIn (this);
        }

        public void Logout () {
            loggedIn = false;

            ThreadManager.ExecuteOnMainThread (() => {
                GameManager.DestroyPlayer (id);
            });
            player = null;
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