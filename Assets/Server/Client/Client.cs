using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace GameServer {
    public class ClientDatabaseData {
        public Vector3 position = Vector3.zero;
        public List<ItemData> items = new List<ItemData> ();
    }

    [Serializable]
    public class ItemData {
        public int id = -1;
        public int count = 0;
    }

    public partial class Client {
        public readonly int id;
        private const int dataBufferSize = 4096;
        public bool loggedIn { get; private set; } = false;
        public Firebase.Auth.FirebaseUser user { get; private set; }

        public Person player;

        public Client (int id) {
            this.id = id;
            tcp = new TCP (this);
        }

        public async Task<bool> Login (string email, string password) {
            if (loggedIn) return false;

            user = await Authoriser.LoginUser (email, password);
            if (user == null) return false;

            ClientDatabaseData data = await Database.GetUser (user.UserId);

            // Spawn the player
            player = (Person) GameManager.SpawnPlayer (data.position, Quaternion.identity, id);
            loggedIn = true;

            // Load the inventory
            foreach (ItemData itemData in data.items) {
                Item item = ItemDatabase.GetItem (itemData.id, itemData.count);
                player.AddItem (item);
            }

            Console.Log ($"[{id}] Logged in");
            return true;
        }

        public void Logout () {
            if (!loggedIn) return;

            Database.WriteUser (user.UserId, GetWriteableData ());
            GameManager.DestroyPlayer (id);
            player = null;

            PacketSender.LogoutSuccessful (this);
            PacketSender.OtherPlayerLoggedOut (this);

            Console.Log ($"[{id}] Logging out");
            loggedIn = false;
        }

        private ClientDatabaseData GetWriteableData () {
            ClientDatabaseData data = new ClientDatabaseData ();

            // Save position
            data.position = player.transform.position;

            // Save the Inventory
            foreach (Item item in player.inventory.GetSortedItems ()) {
                data.items.Add (new ItemData { id = item.id, count = item.count });
            }

            return data;
        }

        public void Disconnect () {
            Logout ();

            tcp.Disconnect ();
            Console.Log ($"[{id}] Disconnected");
        }
    }
}