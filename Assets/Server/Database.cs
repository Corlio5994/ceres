using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Firebase.Database;
using UnityEngine;

namespace GameServer {
    public static class Database {
        private static DatabaseReference reference;
        private static string databaseURL = "https://ceres-fcf64-default-rtdb.asia-southeast1.firebasedatabase.app/";
        private static JsonSerializerOptions options;

        public static void Start () {
            reference = FirebaseDatabase.GetInstance (FirebaseSetup.app, databaseURL).RootReference;
            
            options = new JsonSerializerOptions { };
            options.Converters.Add(new VectorJsonConverter());
        }

        public static async Task<ClientDatabaseData> GetUser (string userID) {
            try {
                DataSnapshot snapshot = await reference.Child ($"users/{userID}").GetValueAsync ();

                ClientDatabaseData data = new ClientDatabaseData ();
                string json = snapshot.GetRawJsonValue ();
                // JsonUtility.FromJsonOverwrite(json, data);
                data = JsonSerializer.Deserialize<ClientDatabaseData>(json, options);

                Console.Log(json, "blue");

                return data;
            } catch (Exception exception) {
                Console.LogError (exception);
                return new ClientDatabaseData ();
            }
        }

        public static void WriteUser (string userID, ClientDatabaseData data) {
            try {
                // string json = JsonUtility.ToJson (data);
                string json = JsonSerializer.Serialize (data, options);
                Console.Log (json, "blue");

                DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference;
                reference.Child ($"users/{userID}").SetRawJsonValueAsync (json);
            } catch (Exception exception) {
                Console.LogError (exception);
            }
        }
    }
}