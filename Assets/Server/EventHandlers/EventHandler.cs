using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Threading.Tasks;
using UnityEngine;

namespace GameServer {
    public static class EventHandler {
        private delegate void PacketHandler (Client client, Packet packet);
        private static Dictionary<int, PacketHandler> packetHandlers = new Dictionary<int, PacketHandler> () {
            {
            (int) ClientPackets.VersionCheck, VersionCheck
            }, {
            (int) ClientPackets.Login,
            Login
            }, {
            (int) ClientPackets.Logout,
            Logout
            }, {
            (int) ClientPackets.PlayerMoved,
            PlayerMoved
            }, {
            (int) ClientPackets.ChatMessage,
            ChatMessage
            }, {
            (int) ClientPackets.PlayerDataRequest,
            PlayerDataRequest
            }, {
            (int) ClientPackets.ItemPickupDataRequest,
            ItemPickupDataRequest
            }, {
            (int) ClientPackets.BankDataRequest,
            BankDataRequest
            }, {
            (int) ClientPackets.ItemDropped,
            ItemDropped
            }, {
            (int) ClientPackets.ItemPickedUp,
            ItemPickedUp
            }, {
            (int) ClientPackets.BankDeposit,
            BankDeposit
            }, {
            (int) ClientPackets.BankWithdraw,
            BankWithdraw
            },
        };

        public static void HandlePacket (Client client, Packet packet) {
            int packetID = packet.ReadInt ();
            Console.Log (Enum.GetName (typeof (ClientPackets), packetID), "green");
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
                    PacketSender.OtherPlayerLoggedIn (client);
                    PacketSender.LoginAccepted (client);
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

        private static void ItemPickupDataRequest (Client client, Packet packet) {
            PacketSender.ItemPickupData(client);
        }

        private static void BankDataRequest (Client client, Packet packet) { 
            PacketSender.BankDataRequest(client);
        }
        private static void ItemDropped (Client client, Packet packet) {
            int pickupID = packet.ReadInt ();
            Vector3 position = packet.ReadVector ();
            int itemID = packet.ReadInt ();

            client.player.RemoveItem (itemID);

            ItemPickup pickup = ItemDatabase.SpawnItemPickup (itemID, 1, position, pickupID);
            PacketSender.ItemDropped (client, pickupID, position, itemID);
        }
        private static void ItemPickedUp (Client client, Packet packet) {
            int pickupID = packet.ReadInt ();
            int count = packet.ReadInt ();

            ItemPickup pickup = ItemPickup.Get (pickupID);
            Item item = pickup.item;
            client.player.AddItem (ItemDatabase.GetItem (item.id, count));

            pickup.TakeItem (count);
            PacketSender.ItemPickedUp (client, pickup.id, count);
        }
        private static void BankDeposit (Client client, Packet packet) {
            int bankID = packet.ReadInt ();
            int itemID = packet.ReadInt ();

            // TODO: Add to player's bank
            client.player.RemoveItem (itemID);
        }
        private static void BankWithdraw (Client client, Packet packet) {
            int bankID = packet.ReadInt ();
            int itemID = packet.ReadInt ();

            // TODO: Remove from player's bank
            client.player.AddItem (ItemDatabase.GetItem (itemID));
        }

    }
}