using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Firebase.Database;
using UnityEngine;
using Newtonsoft.Json;

namespace GameServer {
    public static class Database {
        static DatabaseReference reference;
        static string databaseURL = "https://ceres-fcf64-default-rtdb.asia-southeast1.firebasedatabase.app/";
        static JsonSerializerSettings options;

        public static void Start () {
            reference = FirebaseDatabase.GetInstance (FirebaseSetup.app, databaseURL).RootReference;

            options = new JsonSerializerSettings { };
            options.Converters.Add (new VectorJsonConverter ());
        }

        public static async Task<ClientDatabaseData> GetUser (string userID) {
            try {
                DataSnapshot snapshot = await reference.Child ($"users/{userID}").GetValueAsync ();

                string json = snapshot.GetRawJsonValue ();
                Console.Log (json, "blue");

                ClientDatabaseData data = new ClientDatabaseData ();
                if (json != null)
                    data = JsonConvert.DeserializeObject<ClientDatabaseData> (json, options);

                return data;
            } catch (Exception exception) {
                Console.LogError (exception);
                return new ClientDatabaseData ();
            }
        }

        public static void WriteUser (string userID, ClientDatabaseData data) {
            try {
                string json = JsonConvert.SerializeObject (data, options);
                Console.Log (json, "blue");

                DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference;
                reference.Child ($"users/{userID}").SetRawJsonValueAsync (json);
            } catch (Exception exception) {
                Console.LogError (exception);
            }
        }
    }
}