using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Firebase.Auth;

namespace GameServer {
    public static class Authoriser {
        private static FirebaseAuth auth;

        public static void Start () {
            auth = FirebaseAuth.GetAuth (FirebaseSetup.app);
        }

        public static async Task<FirebaseUser> LoginUser (string email, string password) {
            try {
                Firebase.Auth.Credential credential = Firebase.Auth.EmailAuthProvider.GetCredential (email, password);
                FirebaseUser user = await auth.SignInWithCredentialAsync (credential);
                return user;
            } catch {
                return null;
            }
        }
    }
}