using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Gms.Tasks;
using Firebase.Firestore;
using Firebase.Storage;
using Firebase.Auth;
using Firebase;
using Java.Util;
using Android.Media;
using System.Threading;

namespace Doodle
{
    class FirebaseData
    {
        private static int LEADER_BOARD_SIZE = 10;
        private static int MAX_PLAYERS = 10000;

        private FirebaseFirestore firestore;
        private FirebaseApp app;

        public FirebaseData()
        {

            app = FirebaseApp.InitializeApp(Application.Context);

            if (app is null)
            {
                FirebaseOptions options = new FirebaseOptions.Builder()
                .SetProjectId("fulcrum-7c537")
                .SetApplicationId("fulcrum-7c537")
                .SetApiKey("AIzaSyA8lo7k0EFPNR32-g4xdBnMkQnycn_v4G8")
                .SetDatabaseUrl("https://fulcrum-7c537.firebaseio.com")
                .SetStorageBucket("fulcrum-7c537.appspot.com")
                .Build();

                app = FirebaseApp.InitializeApp(Application.Context, options);
            }

            firestore = FirebaseFirestore.GetInstance(app);
        }
        public DocumentReference GetPlayerRef(string name) => firestore.Collection("Players").Document(HashFunction(name));
        public Task GetPlayer(string name) => firestore.Collection("Players").WhereEqualTo("name", name).Get();
        public Task GetAllPlayers() => firestore.Collection("Players").Get();
        public Task DeletePlayer(string name) => firestore.Collection("Players").Document(HashFunction(name)).Delete();

        public Task GetLeaders()
        {
            return firestore.Collection("Players")
                .WhereGreaterThan("score", 0)
                .OrderBy("score", Query.Direction.Descending)
                .Limit(LEADER_BOARD_SIZE).Get();
        }
        private string HashFunction(string str)
        {    
            int sum = 0;
            for (int i = 0; i < str.Length; i++)
            {
                sum *= 256;
                sum += str[i];
            }
            int result = sum % MAX_PLAYERS;
            return result.ToString("X");
        }
    }
}
