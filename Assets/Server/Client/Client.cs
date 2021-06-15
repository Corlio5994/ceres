using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace GameServer {
    public class ClientDatabaseData {
        public Vector3 position { get; set; } = Vector3.zero;
        public List<ItemData> items { get; set; } = new List<ItemData> ();
        public List<BankData> banks { get; set; } = new List<BankData> ();
    }

    public class ItemData {
        public int count { get; set; }
        public int id { get; set; }
    }

    public class BankData {
        public int id { get; set; }
        public List<ItemData> items { get; set; } = new List<ItemData> ();
    }

    public partial class Client {
        public readonly int id;
        private const int dataBufferSize = 4096;
        public bool loggedIn { get; private set; } = false;
        public Firebase.Auth.FirebaseUser user { get; private set; }

        public Player player;
        public Dictionary<int, Inventory> banks = new Dictionary<int, Inventory> ();

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
            player = GameManager.SpawnPlayer (data.position, Quaternion.identity, id);
            loggedIn = true;

            // Load the inventory
            foreach (ItemData itemData in data.items) {
                player.AddItem (ItemDatabase.GetItem (itemData.id, itemData.count));
            }

            // Load the Banks
            foreach (BankData bankData in data.banks) {
                var newBank = new Inventory (999);

                foreach (ItemData itemData in bankData.items) {
                    Item item = ItemDatabase.GetItem (itemData.id, itemData.count);
                    newBank.AddItem (item);
                }

                banks.Add (bankData.id, newBank);
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

        // TODO: Move these into a more suitable place
        // Eventually Client should only control the networking of the user, all logic
        // should be somewhere else
        public void BankDeposit (int bankID, int itemID, int count) {
            if (!banks.ContainsKey (bankID)) {
                banks.Add (bankID, new Inventory (999));
            }

            Item item = ItemDatabase.GetItem (itemID, count);

            banks[bankID].AddItem (item);
            player.RemoveItem (itemID, count);
        }

        public void BankWithdraw (int bankID, int itemID, int count) {
            if (!banks.ContainsKey (bankID)) {
                banks.Add (bankID, new Inventory (999));
            }

            Item item = ItemDatabase.GetItem (itemID, count);

            banks[bankID].RemoveItem (itemID, count);
            player.AddItem (item);
        }

        public void Disconnect () {
            Logout ();

            tcp.Disconnect ();
            Console.Log ($"[{id}] Disconnected");
        }

        private ClientDatabaseData GetWriteableData () {
            ClientDatabaseData data = new ClientDatabaseData ();

            // Save position
            data.position = player.transform.position;

            // Save the Inventory
            foreach (Item item in player.inventory.GetSortedItems ()) {
                data.items.Add (new ItemData { id = item.id, count = item.count });
            }

            // Save the Banks
            foreach (KeyValuePair<int, Inventory> bank in banks) {
                var bankData = new BankData { id = bank.Key };

                foreach (Item item in bank.Value.GetSortedItems ()) {
                    bankData.items.Add (new ItemData { id = item.id, count = item.count });
                }

                data.banks.Add (bankData);
            }

            return data;
        }
    }
}