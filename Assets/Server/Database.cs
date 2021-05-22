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

                ClientDatabaseData data = new ClientDatabaseData ();
                JsonUtility.FromJsonOverwrite(snapshot.GetRawJsonValue(), data);

                return data;
            } catch (Exception exception) {
                Debug.LogError (exception);
                return new ClientDatabaseData ();
            }
        }

        public static void WriteUser (string userID, ClientDatabaseData data) {
            try {
                Dictionary<string, object> updates = new Dictionary<string, object> ();
                string json = JsonUtility.ToJson (data);

                DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference;
                reference.Child ($"users/{userID}").SetRawJsonValueAsync (json);
            } catch (Exception exception) {
                Debug.LogError (exception);
            }
        }
    }
}