using System;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

public static partial class Client {
    public static class UDP {
        private static UdpClient socket;
        private static IPEndPoint endPoint;

        public static void Connect (int localPort) {
            Debug.Log ("[Client] Connecting UDP");

            endPoint = new IPEndPoint (IPAddress.Parse (Constants.serverIP), Constants.port);
            socket = new UdpClient (localPort);

            socket.Connect (endPoint);
            socket.BeginReceive (ReceiveCallback, null);

            using (Packet packet = new Packet ()) {
                SendData (packet);
            }
        }

        private static void ReceiveCallback (IAsyncResult result) {
            try {
                byte[] data = socket.EndReceive (result, ref endPoint);
                socket.BeginReceive (ReceiveCallback, null);

                if (data.Length < 4) {
                    Client.Disconnect ();
                    return;
                }

                HandleData (data);
            } catch {
                Client.Disconnect ();
            }
        }

        public static void SendData (Packet packet) {
            try {
                packet.InsertInt (id);

                if (socket != null) {
                    socket.BeginSend (packet.ToArray (), packet.Length (), null, null);
                }
            } catch (Exception exception) {
                Debug.LogError ($"[Client] Error sending UDP: {exception}");
            }
        }

        private static void HandleData (byte[] data) {
            using (Packet packet = new Packet (data)) {
                int packetLength = packet.ReadInt ();
                data = packet.ReadBytes (packetLength);
            }

            ThreadManager.ExecuteOnMainThread (() => {
                using (Packet packet = new Packet (data)) {
                    EventHandler.HandlePacket (packet);
                }
            });
        }

        public static void Disconnect () {
            if (socket != null) {
                socket.Close ();
            }
        }
    }
}