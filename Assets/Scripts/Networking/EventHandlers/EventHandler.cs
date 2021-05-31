using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public static class EventHandler {
    private delegate void PacketHandler (Packet packet);
    private static Dictionary<int, PacketHandler> packetHandlers = new Dictionary<int, PacketHandler> () { 
        { (int) ServerPackets.ConnectedTCP, ConnectedTCP }, 
        { (int) ServerPackets.ConnectedUDP, ConnectedUDP }, 
        { (int) ServerPackets.VersionAccepted, VersionAccepted }, 
        { (int) ServerPackets.VersionDenied, VersionDenied }, 
        { (int) ServerPackets.LoginAccepted, LoginAccepted }, 
        { (int) ServerPackets.LoginDenied, LoginDenied },
        { (int) ServerPackets.LogoutSuccessful, LogoutSuccessful },
        { (int) ServerPackets.PlayerData, PlayerData },
        { (int) ServerPackets.OtherPlayerLoggedIn, OtherPlayerLoggedIn },
        { (int) ServerPackets.OtherPlayerLoggedOut, OtherPlayerLoggedOut },
        { (int) ServerPackets.PlayerPosition, PlayerPosition },
        { (int) ServerPackets.OtherPlayerMoved, OtherPlayerMoved },
        { (int) ServerPackets.ChatMessage, ChatMessage },
        
    };

    public static void HandlePacket (Packet packet) {
        int packetID = packet.ReadInt ();
        packetHandlers[packetID] (packet);
    }

    private static void ConnectedTCP (Packet packet) {
        int clientID = packet.ReadInt ();

        Client.id = clientID;
        Debug.Log ($"[Client] TCP connected. ClientID: {clientID}");

        Client.UDP.Connect (Client.TCP.GetPort());
    }

    private static void ConnectedUDP (Packet packet) {
        Debug.Log ($"[Client] UDP connected");
        PacketSender.VersionCheck();
    }

    private static void VersionAccepted (Packet packet) {
        Debug.Log($"[Client] Version accepted");
        MainMenuUI.ShowMainMenuPanel ();
    }

    private static void VersionDenied (Packet packet) {
        Debug.Log($"[Client] Game outdated");
        GameManager.Quit();
    }

    private static void LoginAccepted(Packet packet) {
        Client.Login();
        GameManager.LoadScene("World");
        Debug.Log($"[Client] Logged in");
    }

    private static void PlayerData(Packet packet) {
        Vector3 position = packet.ReadVector();
        GameManager.SpawnPlayer(position, Quaternion.identity);

        int otherClients = packet.ReadInt();
        for (int i = 0; i < otherClients; i++)
        {
            int otherClientID = packet.ReadInt();
            Vector3 otherClientPosition = packet.ReadVector();
            Quaternion otherClientRotation = packet.ReadQuaternion();
            GameManager.SpawnPlayer(otherClientPosition, otherClientRotation,  otherClientID);
        }
    }

    private static void LoginDenied(Packet packet) {
        Debug.Log($"[Client] Login failed");
        // TODO: Show error message to user
        MainMenuUI.ShowCredentialsPanel();
    }

    private static void LogoutSuccessful(Packet packet) {
        Debug.Log($"[Client] Logged out");
        GameManager.Logout();
        GameManager.LoadScene("Main Menu");
    }

    private static void OtherPlayerLoggedIn(Packet packet) {
        if (!Client.loggedIn) return;

        int clientID = packet.ReadInt();
        Vector3 position = packet.ReadVector();

        GameManager.SpawnPlayer(position, Quaternion.identity, clientID);
        Debug.Log($"[Client] Other client connected. ID: {clientID}");
    }

    private static void OtherPlayerLoggedOut(Packet packet) {
        if (!Client.loggedIn) return;

        int clientID = packet.ReadInt();

        GameManager.DestroyPlayer(clientID);
        Debug.Log($"[Client] Other client disconnected. ID: {clientID}");
    }

    private static void PlayerPosition(Packet packet) {
        Vector3 position = packet.ReadVector();

        GameManager.player.transform.position = position;
    }

    private static void OtherPlayerMoved(Packet packet) {
        if (!Client.loggedIn) return;

        int clientID = packet.ReadInt();
        Vector3 destination = packet.ReadVector();

        GameManager.GetPlayer(clientID).SetDestination(destination);
    }

    private static void ChatMessage(Packet packet) {
        if (!Client.loggedIn) return;

        string message = packet.ReadString();
        Chat.AddMessage(message);
    }
}