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

        private static Dictionary<int, Client> clients = new Dictionary<int, Client> ();

        private static TcpListener tcpListener;
        private static UdpClient udpListener;
        public static bool active { get; private set; } = false;

        public static void Start () {
            active = true;
            maxPlayers = Constants.maxPlayers;
            port = Constants.port;

            Debug.Log ($"[Server] Starting. Port: {port}");

            tcpListener = new TcpListener (IPAddress.Any, port);
            tcpListener.Server.SetSocketOption (SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, 1);
            tcpListener.Server.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.DontLinger, false);
            tcpListener.Server.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            tcpListener.Server.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, 5000);
            tcpListener.Server.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendTimeout, 5000);

            tcpListener.Start ();
            tcpListener.BeginAcceptTcpClient (TCPConnectCallback, null);

            udpListener = new UdpClient (port);
            udpListener.BeginReceive (UDPReceiveCallback, null);
            FirebaseSetup.Start ();

            Debug.Log ("[Server] Started");
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
                if (!clients.ContainsKey(i)) continue;

                Client otherClient = clients[i];
                if (otherClient.loggedIn && otherClient != client)
                    result.Add (otherClient);
            }

            return result.ToArray ();
        }

        private static void TCPConnectCallback (IAsyncResult result) {
            TcpClient client = tcpListener.EndAcceptTcpClient (result);
            tcpListener.BeginAcceptTcpClient (TCPConnectCallback, null);
            Debug.Log ($"[Server] Incoming connection ({client.Client.RemoteEndPoint})");

            for (int id = 0; id < maxPlayers; id++) {
                if (!clients.ContainsKey (id)) {
                    Debug.Log ($"[{id}] Creating new client");
                    Client newClient = new Client (id);
                    newClient.tcp.Connect (client);

                    clients.Add (id, newClient);
                    return;
                }
            }

            Debug.Log ($"[Server] Failed to connect: server full ({client.Client.RemoteEndPoint})");
        }

        private static void UDPReceiveCallback (IAsyncResult result) {
            try {
                IPEndPoint endPoint = new IPEndPoint (IPAddress.Any, 0);
                byte[] data = udpListener.EndReceive (result, ref endPoint);
                udpListener.BeginReceive (UDPReceiveCallback, null);

                if (data.Length < 4) {
                    return;
                }

                using (Packet packet = new Packet (data)) {
                    int clientID = packet.ReadInt ();
                    if (clientID < 0 || maxPlayers <= clientID) {
                        return;
                    }

                    Client client = GetClient (clientID);
                    if (client.udp.endPoint == null) {
                        client.udp.Connect (endPoint);
                        return;
                    }

                    if (client.udp.endPoint.ToString () == endPoint.ToString ()) {
                        client.udp.HandleData (packet);
                    }
                }
            } catch (Exception exception) {
                Debug.Log ($"[Server] Error receivng UDP: {exception}");
            }
        }

        public static void SendUDPData (IPEndPoint endpoint, Packet packet) {
            try {
                if (endpoint != null) {
                    udpListener.BeginSend (packet.ToArray (), packet.Length (), endpoint, null, null);
                }
            } catch (Exception exception) {
                Debug.Log ($"[Server] Error sending UDP: {exception}");
            }
        }

        public static void Disconnect (Client client) {
            client.Disconnect ();
            clients.Remove (client.id);
        }
    }
}