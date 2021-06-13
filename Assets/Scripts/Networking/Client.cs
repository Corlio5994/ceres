using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

public static partial class Client {
    public static int id = -1;
    public static bool connected { get; private set; } = false;
    public static bool loggedIn { get; private set; } = false;

    public static bool Connect () {
        if (!connected) {
            TCP.Connect ();
            connected = true;
            return true;
        } else {
            return false;
        }
    }

    public static void Login() {
        loggedIn = true;
    }

    public static void Logout () {
        if (loggedIn) {
            loggedIn = false;
            PacketSender.Logout ();
        }
    }

    public static void Disconnect () {
        if (connected) {
            connected = false;
            id = -1;

            TCP.Disconnect ();
            // TODO: Show a disconnect message
            Application.Quit ();
        }
    }
}