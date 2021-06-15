using System;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

public static partial class Client {
    public static class TCP {
        public static TcpClient socket;
        static NetworkStream stream;
        static Packet receivedData;
        static byte[] receiveBuffer;

        public static void Connect () {
            Console.Log ("Connecting TCP");

            socket = new TcpClient {
                ReceiveBufferSize = Constants.dataBufferSize,
                SendBufferSize = Constants.dataBufferSize
            };

            receiveBuffer = new byte[Constants.dataBufferSize];
            socket.BeginConnect (Constants.serverIP, Constants.port, ConnectCallback, socket);
        }

        

        public static void SendData (Packet packet) {
            try {
                if (socket != null) {
                    stream.BeginWrite (packet.ToArray (), 0, packet.Length (), null, null);
                }
            } catch (Exception exception) {
                Console.LogError ($"Error sending TCP: {exception}");
            }
        }

        public static void Disconnect () {
            socket.Close ();
        }

        static void ConnectCallback (IAsyncResult result) {
            socket.EndConnect (result);

            if (!socket.Connected) {
                return;
            }

            stream = socket.GetStream ();
            receivedData = new Packet ();

            stream.BeginRead (receiveBuffer, 0, Constants.dataBufferSize, ReceiveCallback, null);
            MainMenuUI.ShowMainMenuPanel ();
        }

        static void ReceiveCallback (IAsyncResult result) {
            try {
                int byteLength = stream.EndRead (result);
                if (byteLength <= 0) {
                    Client.Disconnect ();
                    return;
                }

                byte[] data = new byte[byteLength];
                Array.Copy (receiveBuffer, data, byteLength);

                receivedData.Reset (HandleData (data));
                stream.BeginRead (receiveBuffer, 0, Constants.dataBufferSize, ReceiveCallback, null);
            } catch {
                Client.Disconnect ();
            }
        }

        static bool HandleData (byte[] data) {
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
    }
}