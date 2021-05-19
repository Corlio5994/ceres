using System;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

public static partial class Client {
    public static class TCP {
        public static TcpClient socket;
        private static NetworkStream stream;
        private static Packet receivedData;
        private static byte[] receiveBuffer;
        private const int dataBufferSize = 4096;

        public static void Connect () {
            Debug.Log ("[Client] Connecting TCP");

            socket = new TcpClient {
                ReceiveBufferSize = dataBufferSize,
                SendBufferSize = dataBufferSize
            };

            receiveBuffer = new byte[dataBufferSize];
            socket.BeginConnect (Constants.serverIP, Constants.port, ConnectCallback, socket);
        }

        private static void ConnectCallback (IAsyncResult result) {
            socket.EndConnect (result);

            if (!socket.Connected) {
                return;
            }

            stream = socket.GetStream ();
            receivedData = new Packet ();

            stream.BeginRead (receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);
            UI.ShowMainMenuPanel ();
        }

        public static void SendData (Packet packet) {
            try {
                if (socket != null) {
                    stream.BeginWrite (packet.ToArray (), 0, packet.Length (), null, null);
                }
            } catch (Exception exception) {
                Debug.LogError ($"[Client] Error sending TCP: {exception}");
            }
        }

        private static void ReceiveCallback (IAsyncResult result) {
            try {
                int byteLength = stream.EndRead (result);
                if (byteLength <= 0) {
                    Client.Disconnect ();
                    return;
                }

                byte[] data = new byte[byteLength];
                Array.Copy (receiveBuffer, data, byteLength);

                receivedData.Reset (HandleData (data));
                stream.BeginRead (receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);
            } catch {
                Client.Disconnect ();
            }
        }

        private static bool HandleData (byte[] data) {
            int packetLength = 0;

            receivedData.SetBytes (data);

            if (receivedData.UnreadLength () >= 4) {
                packetLength = receivedData.ReadInt ();
                if (packetLength <= 0) {
                    return true;
                }
            }

            while (packetLength > 0 && packetLength <= receivedData.UnreadLength ()) {
                byte[] packetBytes = receivedData.ReadBytes (packetLength);
                ThreadManager.ExecuteOnMainThread (() => {
                    using (Packet packet = new Packet (packetBytes)) {
                        EventHandler.HandlePacket (packet);
                    }
                });

                packetLength = 0;
                if (receivedData.UnreadLength () >= 4) {
                    packetLength = receivedData.ReadInt ();
                    if (packetLength <= 0) {
                        return true;
                    }
                }
            }

            if (packetLength <= 1) {
                return true;
            }

            return false;
        }

        public static int GetPort () {
            return ((IPEndPoint) socket.Client.LocalEndPoint).Port;
        }

        public static void Disconnect () {
            socket.Close ();
        }
    }
}