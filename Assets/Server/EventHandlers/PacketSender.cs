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

        private static void BroadcastLoggedIn (Packet packet, Client exceptClient = null) {
            packet.WriteLength ();
            for (int clientID = 0; clientID < Server.maxPlayers; clientID++) {
                Client client = Server.GetClient (clientID);
                if (client == null || !client.loggedIn) continue;

                if (client != exceptClient) {
                    client.tcp.SendData (packet);
                }
            }
        }

        #region Packets
        #region Admin
        public static void ConnectedTCP (Client client) {
            using (Packet packet = new Packet (ServerPackets.ConnectedTCP)) {
                packet.Write (client.id);
                SendTCPData (client, packet);
            }
        }

        public static void VersionAccepted (Client client) {
            using (Packet packet = new Packet (ServerPackets.VersionAccepted)) {
                SendTCPData (client, packet);
            }
        }

        public static void VersionDenied (Client client) {
            using (Packet packet = new Packet (ServerPackets.VersionDenied)) {
                Console.Log ($"[{client.id}] Failed version check");
                SendTCPData (client, packet);
            }
        }

        public static void LoginAccepted (Client client) {
            using (Packet packet = new Packet (ServerPackets.LoginAccepted)) {
                SendTCPData (client, packet);
            }
        }

        public static void PlayerData (Client client) {
            using (Packet packet = new Packet (ServerPackets.PlayerData)) {
                Person player = client.player;
                packet.Write (player.transform.position);

                Client[] otherPlayers = Server.GetOtherClients (client);
                packet.Write (otherPlayers.Length);

                foreach (Client otherClient in otherPlayers) {
                    packet.Write (otherClient.id);
                    packet.Write (otherClient.player.transform.position);
                    packet.Write (otherClient.player.transform.rotation);
                }

                packet.Write(player.inventory);

                SendTCPData (client, packet);
            }
        }

        public static void LoginDenied (Client client) {
            // TODO: Send over message (Incorrect username, incorrect password etc)
            using (Packet packet = new Packet (ServerPackets.LoginDenied)) {
                SendTCPData (client, packet);
            }
        }

        public static void LogoutSuccessful (Client client) {
            using (Packet packet = new Packet (ServerPackets.LogoutSuccessful)) {
                SendTCPData (client, packet);
            }
        }

        public static void OtherPlayerLoggedIn (Client client) {
            using (Packet packet = new Packet (ServerPackets.OtherPlayerLoggedIn)) {
                packet.Write (client.id);
                packet.Write (client.player.transform.position);
                BroadcastLoggedIn (packet, client);
            }
        }

        public static void OtherPlayerLoggedOut (Client client) {
            using (Packet packet = new Packet (ServerPackets.OtherPlayerLoggedOut)) {
                packet.Write (client.id);
                BroadcastLoggedIn (packet, client);
            }
        }

        public static void PlayerPosition (Client client, Vector3 position) {
            using (Packet packet = new Packet (ServerPackets.PlayerPosition)) {
                packet.Write (position);
                SendTCPData (client, packet);
            }
        }

        public static void OtherPlayerMoved (Client client, Vector3 destination) {
            using (Packet packet = new Packet (ServerPackets.OtherPlayerMoved)) {
                packet.Write (client.id);
                packet.Write (destination);
                BroadcastLoggedIn (packet, client);
            }
        }

        public static void ChatMessage (Client client, string message) {
            using (Packet packet = new Packet (ServerPackets.ChatMessage)) {
                packet.Write (message);
                BroadcastLoggedIn (packet, client);
            }
        }
        #endregion

        #region Items
        public static void ItemPickupData (Client client) {
            using (Packet packet = new Packet (ServerPackets.ItemPickupData)) {
                packet.Write (client.id);
                BroadcastLoggedIn (packet, client);
            }
        }

        public static void BankData (Client client) {
            using (Packet packet = new Packet (ServerPackets.BankData)) {
                packet.Write (client.id);
                BroadcastLoggedIn (packet, client);
            }
        }

        public static void ItemDropped (Client client, int pickupID, Vector3 position, int itemID) {
            using (Packet packet = new Packet (ServerPackets.ItemDropped)) {
                packet.Write (pickupID);
                packet.Write (position);
                packet.Write (itemID);
                BroadcastLoggedIn (packet, client);
            }
        }

        public static void ItemPickedUp (Client client, int pickupID, int removedCount) {
            using (Packet packet = new Packet (ServerPackets.ItemPickedUp)) {
                packet.Write (pickupID);
                packet.Write (removedCount);
                BroadcastLoggedIn (packet, client);
            }
        }
        #endregion
        #endregion
    }
}