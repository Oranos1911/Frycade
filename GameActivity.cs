using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Hardware;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Graphics;
using Java.Lang;
using Java.Util;

namespace Doodle
{
    [Activity(Label = "GameActivity" , ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class GameActivity : Activity , ISensorEventListener
    {
        Boardgame board;
        ISharedPreferences sp;
        FirebaseData fb;
        SensorManager sensorManager;
        Sensor accelerometer;
        bool isSensorAccurated;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Window.AddFlags(WindowManagerFlags.KeepScreenOn);        
            InitSensor();

            sp = GetSharedPreferences(Constants.SHARED_PREFERENCES_NAME, FileCreationMode.Private);
            fb = new FirebaseData();

            board = new Boardgame(this);
            SetContentView(board);
            board.t.Start();
        }

        protected override void OnResume()
        {
            base.OnResume();
            if (board != null)
            {
                board.resume();
            }
        }

        protected override void OnStart()
        {
            base.OnStart();
        }
        protected override void OnDestroy()
        {
            base.OnDestroy();
        }
        protected override void OnStop()
        {
            base.OnStop();
        }
        protected override void OnPause()
        {
            base.OnPause();
            if(board != null)
                board.pause();
        }
        public override void Finish()
        {
            base.Finish();
            board.threadRunning = false;
            while (true)
            {
                try
                {

                }
                catch (InterruptedException e)
                {
                    Toast.MakeText(this,"Some problem happened",ToastLength.Long).Show();
                }
                break;
            }
        }

        public void OnAccuracyChanged(Sensor sensor, [GeneratedEnum] SensorStatus accuracy)
        {
            if (sensor == accelerometer)
            {
                if (accuracy == SensorStatus.AccuracyHigh)
                    isSensorAccurated = true;
                else
                    isSensorAccurated = false;
            }
        }

        public void OnSensorChanged(SensorEvent e)
        {
            if (e.Sensor.Type.Equals(SensorType.Accelerometer) && isSensorAccurated)
            {
                board.NotifySensorChanged(e);
            }
        }


        public void InitSensor()
        {
            sensorManager = (SensorManager)GetSystemService(Context.SensorService);
            accelerometer = sensorManager.GetDefaultSensor(SensorType.Accelerometer);
            sensorManager.RegisterListener(this, accelerometer, SensorDelay.Game);
            isSensorAccurated = false;
        }


        /*
         Function gets the current player's score and updates to firbase and SharedPreferences if it's higher than the last higher score
         params:
            score - player's last score
         returns : 
            True if a new high score has been achived ; False - otherwise
        */
        public bool UpdateScore(int score)
        {
            int lastScore = sp.GetInt(Constants.LAST_HIGH_SCORE_KEY, 0);

            if(score > lastScore)
            {
                // Update to SharedPreferences
                var editor = sp.Edit();
                editor.PutInt(Constants.LAST_HIGH_SCORE_KEY, score);
                editor.Commit();
                // Update to Firebase
                var docref = fb.GetPlayerRef(sp.GetString(Constants.SELECT_NICKNAME_KEY, ""));
                IDictionary<string, Java.Lang.Object> dict = new Dictionary<string, Java.Lang.Object>();
                dict.Add("score", score);
                docref.Update(dict);
                return true;
            }

            return false;
        }
    }
}