using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

public static partial class Client {
    public static int id = -1;
    public static bool connected { get; private set; } = false;

    public static void Connect() {
        TCP.Connect();
        connected = true;
    }

    public static void Disconnect () {
        if (connected) {
            connected = false;
            id = -1;

            TCP.Disconnect ();
            UDP.Disconnect ();
            Debug.Log($"[Client] Disconnected");
            // TODO: Show a disconnect message and quit the game
        }
    }
}