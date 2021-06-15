using System;
using System.Net.Sockets;
using UnityEngine;

namespace GameServer {
    public partial class Client {
        public TCP tcp;

        public class TCP {
            public TcpClient socket;
            public bool connected { get; private set; } = false;
            NetworkStream stream;
            Packet receivedData;
            byte[] receiveBuffer;
            Client client;

            public TCP (Client client) {
                this.client = client;
            }

            public void Connect (TcpClient socket) {
                connected = true;

                this.socket = socket;
                socket.ReceiveBufferSize = Constants.dataBufferSize;
                socket.SendBufferSize = Constants.dataBufferSize;

                stream = socket.GetStream ();

                receivedData = new Packet ();
                receiveBuffer = new byte[Constants.dataBufferSize];

                stream.BeginRead (receiveBuffer, 0, Constants.dataBufferSize, ReceiveCallback, null);

                Console.Log ($"[{client.id}] Sucessfully connected TCP");
                PacketSender.ConnectedTCP (client);
            }

            public void Disconnect () {
                socket.Close ();
                stream = null;
                receivedData = null;
                receiveBuffer = null;
            }

            public void SendData (Packet packet) {
                try {
                    if (socket != null) {
                        stream.BeginWrite (packet.ToArray (), 0, packet.Length (), null, null);
                    }
                } catch (Exception exception) {
                    Console.LogError ($"[{client.id}] Error sending TCP: {exception}");
                }
            }

            void ReceiveCallback (IAsyncResult result) {
                try {
                    int byteLength = stream.EndRead (result);
                    if (byteLength <= 0) {
                        Server.DisconnectClient (client);
                        return;
                    }

                    byte[] data = new byte[byteLength];
                    Array.Copy (receiveBuffer, data, byteLength);

                    receivedData.Reset (HandleData (data));
                    stream.BeginRead (receiveBuffer, 0, Constants.dataBufferSize, ReceiveCallback, null);
                } catch (Exception exception) {
                    Console.LogError ($"[{client.id}] Error receiving TCP: {exception}");
                    Server.DisconnectClient (client);
                }
            }

            bool HandleData (byte[] data) {
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
                            EventHandler.HandlePacket (client, packet);
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
}