using System;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

namespace GameServer {
    public partial class Client {
        public UDP udp;

        public class UDP {
            public IPEndPoint endPoint;
            private Client client;

            public UDP (Client client) {
                this.client = client;
            }

            public void Connect (IPEndPoint endPoint) {
                this.endPoint = endPoint;
                Debug.Log ($"[{client.id}] Sucessfully connected UDP");
                PacketSender.ConnectedUDP (client);
            }

            public void SendData (Packet packet) {
                Server.SendUDPData (endPoint, packet);
            }

            public void HandleData (Packet packetData) {
                int length = packetData.ReadInt ();
                byte[] packetBytes = packetData.ReadBytes (length);

                ThreadManager.ExecuteOnMainThread (() => {
                    using (Packet packet = new Packet (packetBytes)) {
                        EventHandler.HandlePacket (client, packet);
                    }
                });
            }

            public void Disconnect () {
                endPoint = null;
            }
        }
    }
}