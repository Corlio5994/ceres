using UnityEngine;

namespace GameServer {
    public static class PacketSender {
        private static void SendTCPData (Client client, Packet packet) {
            packet.WriteLength ();
            client.tcp.SendData (packet);
        }

        private static void BroadcastTCPData (Packet packet, Client exceptClient = null) {
            packet.WriteLength ();
            for (int clientID = 0; clientID < Server.maxPlayers; clientID++) {
                Client client = Server.GetClient (clientID);
                if (client == null) continue;

                if (client != exceptClient) {
                    client.tcp.SendData (packet);
                }
            }
        }

        private static void SendUDPData (Client client, Packet packet) {
            packet.WriteLength ();
            client.udp.SendData (packet);
        }

        private static void SendUDPDataToAll (Packet packet, Client exceptClient = null) {
            packet.WriteLength ();
            for (int clientID = 0; clientID < Server.maxPlayers; clientID++) {
                Client client = Server.GetClient (clientID);

                if (client != exceptClient) {
                    client.udp.SendData (packet);
                }
            }
        }

        #region Packets
        public static void ConnectedTCP (Client client) {
            using (Packet packet = new Packet ((int) ServerPackets.ConnectedTCP)) {
                packet.Write (client.id);
                SendTCPData (client, packet);
            }
        }

        public static void ConnectedUDP (Client client) {
            using (Packet packet = new Packet ((int) ServerPackets.ConnectedUDP)) {
                SendTCPData (client, packet);
            }
        }

        public static void VersionAccepted (Client client) {
            using (Packet packet = new Packet ((int) ServerPackets.VersionAccepted)) {
                SendTCPData (client, packet);
            }
        }

        public static void VersionDenied (Client client) {
            using (Packet packet = new Packet ((int) ServerPackets.VersionDenied)) {
                SendTCPData (client, packet);
            }
        }

        public static void LoginAccepted (Client client) {
            using (Packet packet = new Packet ((int) ServerPackets.LoginAccepted)) {
                SendTCPData (client, packet);
            }
        }

        public static void PlayerData (Client client) {
            using (Packet packet = new Packet ((int) ServerPackets.PlayerData)) {
                packet.Write (client.player.transform.position);

                Client[] otherPlayers = Server.GetOtherClients (client);
                packet.Write (otherPlayers.Length);

                foreach (Client otherClient in otherPlayers) {
                    packet.Write (otherClient.id);
                    packet.Write (otherClient.player.transform.position);
                    packet.Write (otherClient.player.transform.rotation);
                }

                SendTCPData (client, packet);
            }
        }

        public static void LoginDenied (Client client) {
            // TODO: Send over message (Incorrect username, incorrect password etc)
            using (Packet packet = new Packet ((int) ServerPackets.LoginDenied)) {
                SendTCPData (client, packet);
            }
        }

        public static void LogoutSuccessful (Client client) {
            using (Packet packet = new Packet ((int) ServerPackets.LogoutSuccessful)) {
                SendTCPData (client, packet);
            }
        }

        public static void OtherPlayerLoggedIn (Client client) {
            using (Packet packet = new Packet ((int) ServerPackets.OtherPlayerLoggedIn)) {
                packet.Write (client.id);
                packet.Write (client.player.transform.position);
                BroadcastTCPData (packet, client);
            }
        }

        public static void OtherPlayerLoggedOut (Client client) {
            using (Packet packet = new Packet ((int) ServerPackets.OtherPlayerLoggedOut)) {
                packet.Write (client.id);
                BroadcastTCPData (packet, client);
            }
        }

        public static void PlayerPosition (Client client, Vector3 position) {
            using (Packet packet = new Packet ((int) ServerPackets.PlayerPosition)) {
                packet.Write (position);
                SendTCPData (client, packet);
            }
        }

        public static void OtherPlayerMoved (Client client, Vector3 destination) {
            using (Packet packet = new Packet ((int) ServerPackets.OtherPlayerMoved)) {
                packet.Write (client.id);
                packet.Write (destination);
                BroadcastTCPData (packet, client);
            }
        }

        public static void ChatMessage (Client client, string message) {
            using (Packet packet = new Packet ((int) ServerPackets.ChatMessage)) {
                packet.Write (message);
                BroadcastTCPData (packet, client);
            }
        }
        #endregion
    }
}