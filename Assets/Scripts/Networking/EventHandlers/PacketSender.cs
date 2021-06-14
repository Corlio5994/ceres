using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PacketSender {
    private static void SendTCPData (Packet packet) {
        packet.WriteLength ();
        Client.TCP.SendData (packet);
    }

    #region Packets
    public static void VersionCheck () {
        using (Packet packet = new Packet (ClientPackets.VersionCheck)) {
            packet.Write (Constants.version);
            SendTCPData (packet);
        }
    }

    public static void Login (string username, string password) {
        using (Packet packet = new Packet (ClientPackets.Login)) {
            packet.Write (username);
            packet.Write (password); // TODO: Encrypt the password

            SendTCPData (packet);
        }
    }

    public static void Logout () {
        using (Packet packet = new Packet (ClientPackets.Logout)) {
            SendTCPData (packet);
        }
    }

    public static void PlayerMoved (Vector3 destination) {
        using (Packet packet = new Packet (ClientPackets.PlayerMoved)) {
            packet.Write (destination);
            SendTCPData (packet);
        }
    }

    public static void ChatMessage (string message) {
        using (Packet packet = new Packet (ClientPackets.ChatMessage)) {
            packet.Write (message);
            SendTCPData (packet);
        }
    }

    public static void PlayerDataRequest () {
        using (Packet packet = new Packet (ClientPackets.PlayerDataRequest)) {
            SendTCPData (packet);
        }
    }

    public static void ItemPickupDataRequest () {
        using (Packet packet = new Packet (ClientPackets.ItemPickupDataRequest)) {
            SendTCPData (packet);
        }
    }

    public static void BankDataRequest () {
        using (Packet packet = new Packet (ClientPackets.BankDataRequest)) {
            SendTCPData (packet);
        }
    }

    public static void ItemDropped (ItemPickup pickup) {
        using (Packet packet = new Packet (ClientPackets.ItemDropped)) {
            packet.Write (pickup.id);
            packet.Write (pickup.transform.position);
            packet.Write (pickup.item.id);
            SendTCPData (packet);
        }
    }

    public static void ItemPickedUp (ItemPickup pickup, int count) {
        using (Packet packet = new Packet (ClientPackets.ItemPickedUp)) {
            packet.Write (pickup.id);
            packet.Write (count);
            SendTCPData (packet);
        }
    }

    public static void BankDeposit (Bank bank, Item item) {
        using (Packet packet = new Packet (ClientPackets.BankDeposit)) {
            packet.Write (bank.id);
            packet.Write (item.id);
            SendTCPData (packet);
        }
    }

    public static void BankWithdraw (Bank bank, Item item) {
        using (Packet packet = new Packet (ClientPackets.BankWithdraw)) {
            packet.Write (bank.id);
            packet.Write (item.id);
            SendTCPData (packet);
        }
    }
    #endregion
}