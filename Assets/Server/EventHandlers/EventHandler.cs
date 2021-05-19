using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

namespace GameServer {
    public static class EventHandler {
        private delegate void PacketHandler (Client client, Packet packet);
        private static Dictionary<int, PacketHandler> packetHandlers = new Dictionary<int, PacketHandler> () {
            { (int) ClientPackets.Login, Login },
            { (int) ClientPackets.Logout, Logout },
            { (int) ClientPackets.PlayerMoved, PlayerMoved },
            { (int) ClientPackets.ChatMessage, ChatMessage },
            { (int) ClientPackets.PlayerDataRequest, PlayerDataRequest }
        };

        public static void HandlePacket (Client client, Packet packet) {
            int packetID = packet.ReadInt ();
            packetHandlers[packetID] (client, packet);
        }

        private static void Login (Client client, Packet packet) {
            string username = packet.ReadString ();
            string password = packet.ReadString ();

            // TODO: Verify password

            client.Login();
        }

        private static void Logout(Client client, Packet packet) {
            client.Logout();
            Debug.Log($"[{client.id}] Logging out");
            PacketSender.OtherPlayerLoggedOut(client);
        }

        private static void PlayerMoved(Client client, Packet packet) {
            Vector3 destination = packet.ReadVector();
            client.player.SetDestination(destination);
            PacketSender.OtherPlayerMoved(client, destination);
        }

        private static void ChatMessage(Client client, Packet packet) {
            string message = packet.ReadString();
            PacketSender.ChatMessage(client, message);
        }

        private static void PlayerDataRequest(Client client, Packet packet) {
            PacketSender.PlayerData(client);
        }
    }
}