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
        using (Packet packet = new Packet ((int) ClientPackets.VersionCheck)) {
            packet.Write (Constants.version);
            SendTCPData (packet);
        }
    }

    public static void Login (string username, string password) {
        using (Packet packet = new Packet ((int) ClientPackets.Login)) {
            packet.Write (username);
            packet.Write (password); // TODO: Encrypt the password

            SendTCPData (packet);
        }
    }

    public static void Logout () {
        using (Packet packet = new Packet ((int) ClientPackets.Logout)) {
            SendTCPData (packet);
        }
    }

    public static void PlayerMoved (Vector3 destination, bool queued = false) {
        using (Packet packet = new Packet ((int) ClientPackets.PlayerMoved)) {
            packet.Write (destination);
            SendTCPData (packet);
        }
    }

    public static void ChatMessage (string message) {
        using (Packet packet = new Packet ((int) ClientPackets.ChatMessage)) {
            packet.Write (message);
            SendTCPData (packet);
        }
    }

    public static void PlayerDataRequest () {
        using (Packet packet = new Packet ((int) ClientPackets.PlayerDataRequest)) {
            SendTCPData (packet);
        }
    }

    public static void ItemPickupDataRequest () {
        using (Packet packet = new Packet ((int) ClientPackets.ItemPickupDataRequest)) {
            SendTCPData (packet);
        }
    }

    public static void ContainerDataRequest () {
        using (Packet packet = new Packet ((int) ClientPackets.ContainerDataRequest)) {
            SendTCPData (packet);
        }
    }

    public static void ItemDropped (ItemPickup pickup) {
        using (Packet packet = new Packet ((int) ClientPackets.ItemDropped)) {
            packet.Write (pickup.id);
            packet.Write (pickup.item.id);
            SendTCPData (packet);
        }
    }

    public static void ItemPickedUp (ItemPickup pickup) {
        using (Packet packet = new Packet ((int) ClientPackets.ItemPickedUp)) {
            packet.Write (pickup.id);
            SendTCPData (packet);
        }
    }

    public static void ContainerDeposit (Container container, Item item) {
        using (Packet packet = new Packet ((int) ClientPackets.ContainerDeposit)) {
            packet.Write (container.id);
            packet.Write (item.id);
            SendTCPData (packet);
        }
    }

    public static void ContainerWithdraw (Container container, Item item) {
        using (Packet packet = new Packet ((int) ClientPackets.ContainerWithdraw)) {
            packet.Write (container.id);
            packet.Write (item.id);
            SendTCPData (packet);
        }
    }
    #endregion
}