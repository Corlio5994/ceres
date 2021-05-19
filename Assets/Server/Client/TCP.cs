using System;
using System.Net.Sockets;
using UnityEngine;

namespace GameServer {
    public partial class Client {
        public TCP tcp;

        public class TCP {
            public TcpClient socket;
            private NetworkStream stream;
            private Packet receivedData;
            private byte[] receiveBuffer;
            public bool connected { get; private set; } = false;

            private Client client;

            public TCP (Client client) {
                this.client = client;
            }

            public void Connect (TcpClient socket) {
                connected = true;

                this.socket = socket;
                socket.ReceiveBufferSize = Client.dataBufferSize;
                socket.SendBufferSize = Client.dataBufferSize;

                stream = socket.GetStream ();

                receivedData = new Packet ();
                receiveBuffer = new byte[dataBufferSize];

                stream.BeginRead (receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);

                Debug.Log ($"[{client.id}] Sucessfully connected TCP");
                PacketSender.ConnectedTCP (client);
            }

            public void SendData (Packet packet) {
                try {
                    if (socket != null) {
                        stream.BeginWrite (packet.ToArray (), 0, packet.Length (), null, null);
                    }
                } catch (Exception exception) {
                    Debug.Log ($"[{client.id}] Error sending TCP: {exception}");
                }
            }

            private void ReceiveCallback (IAsyncResult result) {
                try {
                    int byteLength = stream.EndRead (result);
                    if (byteLength <= 0) {
                        Server.Disconnect (client);
                        return;
                    }

                    byte[] data = new byte[byteLength];
                    Array.Copy (receiveBuffer, data, byteLength);

                    receivedData.Reset (HandleData (data));
                    stream.BeginRead (receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);
                } catch (Exception exception) {
                    Debug.Log ($"[{client.id}] Error receiving TCP: {exception}");
                    Server.Disconnect (client);
                }
            }

            private bool HandleData (byte[] data) {
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

            public void Disconnect () {
                socket.Close ();
                stream = null;
                receivedData = null;
                receiveBuffer = null;
            }
        }
    }
}