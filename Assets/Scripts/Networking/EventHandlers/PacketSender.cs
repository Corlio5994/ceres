using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PacketSender {
    private static void SendTCPData (Packet packet) {
        packet.WriteLength ();
        Client.TCP.SendData (packet);
    }

    #region Packets
    public static void VersionCheck() {
        using (Packet packet = new Packet ((int) ClientPackets.VersionCheck)) {
            packet.Write(Constants.version);
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
            SendTCPData(packet);
        }
    }

    public static void PlayerMoved(Vector3 destination, bool queued = false) {
        using(Packet packet = new Packet((int) ClientPackets.PlayerMoved)) {
            packet.Write(destination);
            SendTCPData(packet);
        }
    }

    public static void ChatMessage(string message) {
        using(Packet packet = new Packet((int) ClientPackets.ChatMessage)) {
            packet.Write(message);
            SendTCPData(packet);
        }
    }

    public static void PlayerDataRequest() {
        using(Packet packet = new Packet((int) ClientPackets.PlayerDataRequest)) {
            SendTCPData(packet);
        }
    }
    #endregion
}