using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using System.Threading.Tasks;

namespace GameServer {
    public static class EventHandler {
        private delegate void PacketHandler (Client client, Packet packet);
        private static Dictionary<int, PacketHandler> packetHandlers = new Dictionary<int, PacketHandler> () { {
                (int) ClientPackets.VersionCheck, VersionCheck }, {
                (int) ClientPackets.Login, Login }, {
                (int) ClientPackets.Logout, Logout }, {
                (int) ClientPackets.PlayerMoved, PlayerMoved }, {
                (int) ClientPackets.ChatMessage, ChatMessage }, {
                (int) ClientPackets.PlayerDataRequest, PlayerDataRequest }
        };

        public static void HandlePacket (Client client, Packet packet) {
            int packetID = packet.ReadInt ();
            packetHandlers[packetID] (client, packet);
        }

        private static void VersionCheck (Client client, Packet packet) {
            string version = packet.ReadString ();

            if (version == Constants.version)
                PacketSender.VersionAccepted (client);
            else
                PacketSender.VersionDenied (client);
        }

        private static void Login (Client client, Packet packet) {
            string email = packet.ReadString ();
            string password = packet.ReadString ();

            client.Login (email, password).ContinueWith (task => {
                if (task.Result) {
                    PacketSender.LoginAccepted (client);
                    PacketSender.OtherPlayerLoggedIn (client);
                } else {
                    PacketSender.LoginDenied (client);
                }
            }, TaskContinuationOptions.ExecuteSynchronously);
        }

        private static void Logout (Client client, Packet packet) {
            client.Logout ();
        }

        private static void PlayerMoved (Client client, Packet packet) {
            Vector3 destination = packet.ReadVector ();
            client.player.SetDestination (destination);
            PacketSender.OtherPlayerMoved (client, destination);
        }

        private static void ChatMessage (Client client, Packet packet) {
            string message = packet.ReadString ();
            PacketSender.ChatMessage (client, message);
        }

        private static void PlayerDataRequest (Client client, Packet packet) {
            PacketSender.PlayerData (client);
        }
    }
}