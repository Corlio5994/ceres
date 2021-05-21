using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Firebase.Database;
using UnityEngine;

namespace GameServer {
    public static class Database {
        private static DatabaseReference reference;
        private static string databaseURL = "https://ceres-fcf64-default-rtdb.asia-southeast1.firebasedatabase.app/";

        public static void Start () {
            reference = FirebaseDatabase.GetInstance (FirebaseSetup.app, databaseURL).RootReference;
        }

        public static async Task<ClientDatabaseData> GetUser (string userID) {
            try {
                DataSnapshot snapshot = await reference.Child ($"users/{userID}").GetValueAsync ();

                IDictionary dictUser = (IDictionary) snapshot.Value;
                IDictionary position = (IDictionary) dictUser["position"];
                ClientDatabaseData data = new ClientDatabaseData {
                    position = new Vector3 (Convert.ToInt32 (position["x"]), Convert.ToInt32 (position["y"]), Convert.ToInt32 (position["z"]))
                };

                return data;
            } catch (Exception exception) {
                Debug.LogError (exception);
                return new ClientDatabaseData ();
            }
        }

        public static void WriteUser (Client client) {
            try {
                ThreadManager.ExecuteOnMainThread (async () => {
                    Dictionary<string, object> updates = new Dictionary<string, object> ();

                    Vector3 position = client.player.transform.position;
                    updates["position/x"] = position.x;
                    updates["position/y"] = position.y;
                    updates["position/z"] = position.z;

                    MonoBehaviour.Destroy(client.player);
                    client.player = null;
                    reference.Child ($"users/{client.user.UserId}").UpdateChildrenAsync (updates);
                });
            } catch (Exception exception) {
                Debug.LogError (exception);
            }
        }
    }
}