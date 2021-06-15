using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

namespace GameServer {
    public static class Server {
        public static int maxPlayers { get; private set; }
        public static int port { get; private set; }
        public const string version = "1.0.0";
        public static bool active { get; private set; } = false;

        static Dictionary<int, Client> clients = new Dictionary<int, Client> ();
        static TcpListener tcpListener;

        public static void Start () {
            active = true;
            maxPlayers = Constants.maxPlayers;
            port = Constants.port;

            Console.Log ($"Server starting. Port: {port}");

            tcpListener = new TcpListener (IPAddress.Any, port);
            tcpListener.Server.SetSocketOption (SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, 1);
            tcpListener.Server.SetSocketOption (SocketOptionLevel.Socket, SocketOptionName.DontLinger, false);
            tcpListener.Server.SetSocketOption (SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            tcpListener.Server.SetSocketOption (SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, 5000);
            tcpListener.Server.SetSocketOption (SocketOptionLevel.Socket, SocketOptionName.SendTimeout, 5000);

            tcpListener.Start ();
            tcpListener.BeginAcceptTcpClient (TCPConnectCallback, null);

            FirebaseSetup.Start ();

            Console.Log ("Server started");
        }

        public static void Stop () {
            tcpListener.Stop ();
        }

        public static Client GetClient (int clientID) {
            if (clients.ContainsKey (clientID))
                return clients[clientID];
            return null;
        }

        public static Client[] GetOtherClients (Client client = null) {
            List<Client> result = new List<Client> ();

            for (int i = 0; i < maxPlayers; i++) {
                if (!clients.ContainsKey (i)) continue;

                Client otherClient = clients[i];
                if (otherClient.loggedIn && otherClient != client)
                    result.Add (otherClient);
            }

            return result.ToArray ();
        }

        public static void DisconnectClient (Client client) {
            ThreadManager.ExecuteOnMainThread (() => {
                client.Disconnect ();
                clients.Remove (client.id);
            });
        }

        static void TCPConnectCallback (IAsyncResult result) {
            TcpClient client = tcpListener.EndAcceptTcpClient (result);
            tcpListener.BeginAcceptTcpClient (TCPConnectCallback, null);
            Console.Log ($"Incoming connection ({client.Client.RemoteEndPoint.ToString()})");

            for (int id = 0; id < maxPlayers; id++) {
                if (!clients.ContainsKey (id)) {
                    Console.Log ($"[{id}] Creating new client");
                    Client newClient = new Client (id);
                    newClient.tcp.Connect (client);

                    clients.Add (id, newClient);
                    return;
                }
            }

            Console.Log ($"Failed to connect: server full ({client.Client.RemoteEndPoint})");
        }
    }
}