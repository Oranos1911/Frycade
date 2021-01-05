using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Gms.Tasks;
using Android.Views;
using Android.Widget;
using Firebase.Firestore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.Support.V7.App;

namespace Doodle
{
    [Activity(Label = "LeaderActivity" , ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class LeaderActivity : AppCompatActivity , View.IOnClickListener , IOnSuccessListener , IOnFailureListener
    {
        FirebaseData fr;
        List<PlayerData> lstLeaders;
        LeaderAdpater leaderAdapter;
        RecyclerView rvLeaders;
        Button btnClose;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_leader);
            fr = new FirebaseData();
            InitLeaderboard();
            leaderAdapter = new LeaderAdpater(this, lstLeaders);
            InitViews();
        }

        private void InitLeaderboard()
        {
            lstLeaders = new List<PlayerData>();

            fr.GetLeaders()
                .AddOnSuccessListener(this)
                .AddOnFailureListener(this);
        }

        public void OnSuccess(Java.Lang.Object result)
        {
            var snapshot = (QuerySnapshot) result;
            var documents = snapshot.Documents;
            foreach (DocumentSnapshot snap in documents)
               lstLeaders.Add(new PlayerData(snap.GetString("name"), (double) snap.GetDouble("score")));
            leaderAdapter.NotifyDataSetChanged();
        }

        public void OnFailure(Java.Lang.Exception e)
        {
            Toast.MakeText(this, e.ToString() , ToastLength.Long).Show();
        }

        public void Dispose() => throw new NotImplementedException();

        private void InitViews()
        {
            btnClose = FindViewById<Button>(Resource.Id.btn_close);
            btnClose.SetOnClickListener(this);
            rvLeaders = FindViewById<RecyclerView>(Resource.Id.rv_leaders);
            rvLeaders.SetAdapter(leaderAdapter);
            rvLeaders.SetLayoutManager(new LinearLayoutManager(this , LinearLayoutManager.Vertical , false));
        }

        public void OnClick(View v)
        {
            if(v == btnClose)
                Finish();
        }
    }
}