using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public static class EventHandler {
    delegate void PacketHandler (Packet packet);
    static Dictionary<int, PacketHandler> packetHandlers = new Dictionary<int, PacketHandler> () { {
        (int) ServerPackets.ConnectedTCP, ConnectedTCP }, {
        (int) ServerPackets.VersionAccepted,
        VersionAccepted
        }, {
        (int) ServerPackets.VersionDenied,
        VersionDenied
        }, {
        (int) ServerPackets.LoginAccepted,
        LoginAccepted
        }, {
        (int) ServerPackets.LoginDenied,
        LoginDenied
        }, {
        (int) ServerPackets.LogoutSuccessful,
        LogoutSuccessful
        }, {
        (int) ServerPackets.PlayerData,
        PlayerData
        }, {
        (int) ServerPackets.OtherPlayerLoggedIn,
        OtherPlayerLoggedIn
        }, {
        (int) ServerPackets.OtherPlayerLoggedOut,
        OtherPlayerLoggedOut
        }, {
        (int) ServerPackets.PlayerPosition,
        PlayerPosition
        }, {
        (int) ServerPackets.OtherPlayerMoved,
        OtherPlayerMoved
        }, {
        (int) ServerPackets.ChatMessage,
        ChatMessage
        }, {
        (int) ServerPackets.ItemPickupData,
        ItemPickupData
        }, {
        (int) ServerPackets.BankData,
        BankData
        }, {
        (int) ServerPackets.ItemDropped,
        ItemDropped
        }, {
        (int) ServerPackets.ItemPickedUp,
        ItemPickedUp
        },

    };

    public static void HandlePacket (Packet packet) {
        int packetID = packet.ReadInt ();
        Console.Log (Enum.GetName (typeof (ServerPackets), packetID), "green");
        packetHandlers[packetID] (packet);
    }

    static void ConnectedTCP (Packet packet) {
        int clientID = packet.ReadInt ();

        Client.id = clientID;
        Console.Log ($"TCP connected. ClientID: {clientID}");

        PacketSender.VersionCheck ();
    }

    static void VersionAccepted (Packet packet) {
        Console.Log ($"Version accepted");
        MainMenuUI.ShowMainMenuPanel ();
    }

    static void VersionDenied (Packet packet) {
        Console.Log ($"Game outdated");
        GameManager.Quit ();
    }

    static void LoginAccepted (Packet packet) {
        Client.Login ();
        StateManager.LoadScene ("World");
        Console.Log ($"Logged in");
    }

    static void PlayerData (Packet packet) {
        Vector3 position = packet.ReadVector ();
        Player player = (Player) GameManager.SpawnPlayer (position, Quaternion.identity);

        int otherClients = packet.ReadInt ();
        for (int i = 0; i < otherClients; i++) {
            int otherClientID = packet.ReadInt ();
            Vector3 otherClientPosition = packet.ReadVector ();
            Quaternion otherClientRotation = packet.ReadQuaternion ();
            GameManager.SpawnPlayer (otherClientPosition, otherClientRotation, otherClientID);
        }

        Inventory inventory = packet.ReadInventory ();
        player.inventory = inventory;
    }

    static void LoginDenied (Packet packet) {
        Console.Log ($"Login failed");
        // TODO: Show error message to user
        MainMenuUI.ShowCredentialsPanel ();
    }

    static void LogoutSuccessful (Packet packet) {
        Console.Log ($"Logged out");
        GameManager.Logout ();
        StateManager.LoadScene ("Main Menu");
    }

    static void OtherPlayerLoggedIn (Packet packet) {
        if (!Client.loggedIn) return;

        int clientID = packet.ReadInt ();
        Vector3 position = packet.ReadVector ();

        GameManager.SpawnPlayer (position, Quaternion.identity, clientID);
        Console.Log ($"Other client connected. ID: {clientID}");
    }

    static void OtherPlayerLoggedOut (Packet packet) {
        if (!Client.loggedIn) return;

        int clientID = packet.ReadInt ();

        GameManager.DestroyPlayer (clientID);
        Console.Log ($"Other client disconnected. ID: {clientID}");
    }

    static void PlayerPosition (Packet packet) {
        Vector3 position = packet.ReadVector ();

        GameManager.player.transform.position = position;
    }

    static void OtherPlayerMoved (Packet packet) {
        if (!Client.loggedIn) return;

        int clientID = packet.ReadInt ();
        Vector3 destination = packet.ReadVector ();

        GameManager.GetPlayer (clientID).SetDestination (destination);
    }

    static void ChatMessage (Packet packet) {
        string message = packet.ReadString ();
        Chat.AddMessage (message);
    }

    static void ItemPickupData (Packet packet) {
        int itemPickupCount = packet.ReadInt ();

        for (int i = 0; i < itemPickupCount; i++) {
            int pickupID = packet.ReadInt ();
            Vector3 position = packet.ReadVector ();
            int itemID = packet.ReadInt ();
            int count = packet.ReadInt ();

            ItemDatabase.SpawnItemPickup (itemID, count, position, pickupID);
        }
    }

    static void BankData (Packet packet) {
        int bankCount = packet.ReadInt ();

        for (int i = 0; i < bankCount; i++) {
            int bankID = packet.ReadInt ();
            Inventory inventory = packet.ReadInventory ();
            Bank.Get (bankID).SetItems (inventory.GetSortedItems ());
        }
    }

    static void ItemDropped (Packet packet) {
        int pickupID = packet.ReadInt ();
        Vector3 position = packet.ReadVector ();
        int itemID = packet.ReadInt ();

        ItemDatabase.SpawnItemPickup (itemID, 1, position, pickupID);
    }

    static void ItemPickedUp (Packet packet) {
        int pickupID = packet.ReadInt ();
        int count = packet.ReadInt ();

        ItemPickup.Get (pickupID).TakeItem (count);
    }
}