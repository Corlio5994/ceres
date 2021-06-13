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
                if (client == null || client.loggedIn) continue;

                if (client != exceptClient) {
                    client.tcp.SendData (packet);
                }
            }
        }

        #region Packets
        #region Admin
        public static void ConnectedTCP (Client client) {
            using (Packet packet = new Packet ((int) ServerPackets.ConnectedTCP)) {
                packet.Write (client.id);
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
                Console.Log ($"[{client.id}] Failed version check");
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
                BroadcastLoggedIn (packet, client);
            }
        }

        public static void OtherPlayerLoggedOut (Client client) {
            using (Packet packet = new Packet ((int) ServerPackets.OtherPlayerLoggedOut)) {
                packet.Write (client.id);
                BroadcastLoggedIn (packet, client);
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
                BroadcastLoggedIn (packet, client);
            }
        }

        public static void ChatMessage (Client client, string message) {
            using (Packet packet = new Packet ((int) ServerPackets.ChatMessage)) {
                packet.Write (message);
                BroadcastLoggedIn (packet, client);
            }
        }
        #endregion

        #region Items
        public static void ItemPickupData (Client client) {
            using (Packet packet = new Packet ((int) ServerPackets.OtherPlayerLoggedOut)) {
                packet.Write (client.id);
                BroadcastLoggedIn (packet, client);
            }
        }

        public static void ContainerData (Client client) {
            using (Packet packet = new Packet ((int) ServerPackets.OtherPlayerLoggedOut)) {
                packet.Write (client.id);
                BroadcastLoggedIn (packet, client);
            }
        }

        public static void ItemDropped (Client client, ItemPickup pickup) {
            using (Packet packet = new Packet ((int) ServerPackets.OtherPlayerLoggedOut)) {
                packet.Write (pickup.id);
                packet.Write (pickup.transform.position);
                packet.Write (pickup.item.id);
                BroadcastLoggedIn (packet, client);
            }
        }

        public static void ItemPickedUp (Client client, ItemPickup pickup, int removedCount) {
            using (Packet packet = new Packet ((int) ServerPackets.OtherPlayerLoggedOut)) {
                packet.Write (pickup.id);
                packet.Write (removedCount);
                BroadcastLoggedIn (packet, client);
            }
        }

        public static void ContainerDeposit (Client client, int containerID, int itemID) {
            using (Packet packet = new Packet ((int) ServerPackets.OtherPlayerLoggedOut)) {
                packet.Write (containerID);
                packet.Write (itemID);
                BroadcastLoggedIn (packet, client);
            }
        }

        public static void ContainerWithdraw (Client client, int containerID, int itemID) {
            using (Packet packet = new Packet ((int) ServerPackets.OtherPlayerLoggedOut)) {
                packet.Write (containerID);
                packet.Write (itemID);
                BroadcastLoggedIn (packet, client);
            }
        }
        #endregion
        #endregion
    }
}