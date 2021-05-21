using System.Collections;
using System.Collections.Generic;
using Firebase;
using Firebase.Auth;
using UnityEngine;
using System.IO;

namespace GameServer {
    public static class FirebaseSetup {
        public static FirebaseApp app { get; private set; }

        public static void Start () {
            string path = Application.streamingAssetsPath + "/google-services-desktop.json";

            StreamReader reader = new StreamReader(path);
            string json = reader.ReadToEnd();
            app = FirebaseApp.Create (AppOptions.LoadFromJsonConfig(json), "Ceres-Server");
            reader.Close();
            Authoriser.Start ();
            Database.Start();
        }
    }
}