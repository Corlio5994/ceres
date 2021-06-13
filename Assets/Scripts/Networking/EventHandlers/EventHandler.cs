using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public static class EventHandler {
    private delegate void PacketHandler (Packet packet);
    private static Dictionary<int, PacketHandler> packetHandlers = new Dictionary<int, PacketHandler> () { {
            (int) ServerPackets.ConnectedTCP, ConnectedTCP }, {
            (int) ServerPackets.VersionAccepted, VersionAccepted }, {
            (int) ServerPackets.VersionDenied, VersionDenied }, {
            (int) ServerPackets.LoginAccepted, LoginAccepted }, {
            (int) ServerPackets.LoginDenied, LoginDenied }, {
            (int) ServerPackets.LogoutSuccessful, LogoutSuccessful }, {
            (int) ServerPackets.PlayerData, PlayerData }, {
            (int) ServerPackets.OtherPlayerLoggedIn, OtherPlayerLoggedIn }, {
            (int) ServerPackets.OtherPlayerLoggedOut, OtherPlayerLoggedOut }, {
            (int) ServerPackets.PlayerPosition, PlayerPosition }, {
            (int) ServerPackets.OtherPlayerMoved, OtherPlayerMoved }, {
            (int) ServerPackets.ChatMessage, ChatMessage }, {
            (int) ServerPackets.ItemPickupData, ItemPickupData }, {
            (int) ServerPackets.ContainerData, ContainerData }, {
            (int) ServerPackets.ItemDropped, ItemDropped }, {
            (int) ServerPackets.ItemPickedUp, ItemPickedUp }, {
            (int) ServerPackets.ContainerDeposit, ContainerDeposit }, {
            (int) ServerPackets.ContainerWithdraw, ContainerWithdraw },

    };

    public static void HandlePacket (Packet packet) {
        int packetID = packet.ReadInt ();
        packetHandlers[packetID] (packet);
    }

    private static void ConnectedTCP (Packet packet) {
        int clientID = packet.ReadInt ();

        Client.id = clientID;
        Console.Log ($"TCP connected. ClientID: {clientID}");

        PacketSender.VersionCheck ();
    }

    private static void VersionAccepted (Packet packet) {
        Console.Log($"Version accepted");
        MainMenuUI.ShowMainMenuPanel ();
    }

    private static void VersionDenied (Packet packet) {
        Console.Log($"Game outdated");
        GameManager.Quit();
    }

    private static void LoginAccepted(Packet packet) {
        Client.Login();
        GameManager.LoadScene("World");
        Console.Log($"Logged in");
    }

    private static void PlayerData (Packet packet) {
        Vector3 position = packet.ReadVector ();
        GameManager.SpawnPlayer (position, Quaternion.identity);

        int otherClients = packet.ReadInt ();
        for (int i = 0; i < otherClients; i++) {
            int otherClientID = packet.ReadInt ();
            Vector3 otherClientPosition = packet.ReadVector ();
            Quaternion otherClientRotation = packet.ReadQuaternion ();
            GameManager.SpawnPlayer (otherClientPosition, otherClientRotation, otherClientID);
        }
    }

    private static void LoginDenied(Packet packet) {
        Console.Log($"Login failed");
        // TODO: Show error message to user
        MainMenuUI.ShowCredentialsPanel ();
    }

    private static void LogoutSuccessful(Packet packet) {
        Console.Log($"Logged out");
        GameManager.Logout();
        GameManager.LoadScene("Main Menu");
    }

    private static void OtherPlayerLoggedIn (Packet packet) {
        if (!Client.loggedIn) return;

        int clientID = packet.ReadInt ();
        Vector3 position = packet.ReadVector ();

        GameManager.SpawnPlayer(position, Quaternion.identity, clientID);
        Console.Log($"Other client connected. ID: {clientID}");
    }

    private static void OtherPlayerLoggedOut (Packet packet) {
        if (!Client.loggedIn) return;

        int clientID = packet.ReadInt ();

        GameManager.DestroyPlayer(clientID);
        Console.Log($"Other client disconnected. ID: {clientID}");
    }

    private static void PlayerPosition (Packet packet) {
        Vector3 position = packet.ReadVector ();

        GameManager.player.transform.position = position;
    }

    private static void OtherPlayerMoved (Packet packet) {
        if (!Client.loggedIn) return;

        int clientID = packet.ReadInt ();
        Vector3 destination = packet.ReadVector ();

        GameManager.GetPlayer (clientID).SetDestination (destination);
    }

    private static void ChatMessage (Packet packet) {
        string message = packet.ReadString ();
        Chat.AddMessage (message);
    }

    private static void ItemPickupData (Packet packet) {
        int itemPickupCount = packet.ReadInt ();

        for (int i = 0; i < itemPickupCount; i++) {
            int itemID = packet.ReadInt();
            int count = packet.ReadInt();
            Vector3 position = packet.ReadVector();

            ItemDatabase.SpawnItemPickup(itemID, count, position);
        }
    }

    private static void ContainerData (Packet packet) {
        int containerCount = packet.ReadInt ();

        for (int i = 0; i < containerCount; i++) {
            int containerID = packet.ReadInt();
            int containerItemsCount = packet.ReadInt();
            Vector3 position = packet.ReadVector();
            Quaternion rotation = packet.ReadQuaternion();

            Container container = Container.Get(containerID);
            if (container == null) 
                container = ItemDatabase.SpawnContainer(position, rotation);
            for (int j = 0; j < containerItemsCount; j++) {
                int itemID = packet.ReadInt();
                int count = packet.ReadInt();

                container.inventory.AddItem(ItemDatabase.GetItem(itemID, count));
            }
            
        }
    }

    private static void ItemDropped (Packet packet) {
        int pickupID = packet.ReadInt();
        Vector3 position = packet.ReadVector();
        int itemID = packet.ReadInt ();

        ItemDatabase.SpawnItemPickup(itemID, 1, position, pickupID);
    }

    private static void ItemPickedUp (Packet packet) {
        int itemPickupID = packet.ReadInt ();
        int count = packet.ReadInt ();

        ItemPickup.Get (itemPickupID).TakeItem (count);
    }

    private static void ContainerDeposit (Packet packet) {
        int containerID = packet.ReadInt ();
        int itemID = packet.ReadInt ();

        Container container = Container.Get(containerID);
        container.Deposit (ItemDatabase.GetItem (itemID));
        if (ContainerUI.container == container) {
            ContainerUI.Show(container, false);
        }
    }

    private static void ContainerWithdraw (Packet packet) {
        int containerID = packet.ReadInt ();
        int itemID = packet.ReadInt ();

        Container container = Container.Get(containerID);
        container.Withdraw (itemID);
        if (ContainerUI.container == container) {
            ContainerUI.Show(container, false);
        }
    }
}