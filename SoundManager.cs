using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.Media;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace Doodle
{
    class SoundManager
    {
        private static string[] SoundKeys = {"fell", "jump" , "ramp" , "hawk" , "cat" ,  "punch" , "shot" , "jumpshot"};
        private static int musicCount = 5;

        private Dictionary<string , MediaPlayer> mps;
        public static bool muteSounds = false;
        public static bool muteMusic = false;
        private string currentMusicKey;
        public SoundManager(Context context)
        {
            mps = new Dictionary<string, MediaPlayer>();
            currentMusicKey = "music" + new Random().Next(0, musicCount);

            foreach(string key in SoundKeys)
            {
                mps.Add(key , new MediaPlayer());
                mps[key].Reset();
                mps[key] = MediaPlayer.Create(context, context.Resources.GetIdentifier(key, "raw", context.PackageName));
            }
            for(int i = 0; i < musicCount; i++)
            {
                string key = "music" + i;
                mps.Add(key , new MediaPlayer());
                mps[key].Reset();
                mps[key] = MediaPlayer.Create(context, context.Resources.GetIdentifier(key, "raw", context.PackageName));
                mps[key].Looping = true;
            }
        }

        public void PlaySound(string key)
        {
            if (!mps.ContainsKey(key) || muteSounds)
                return;
            mps[key].Start();             
        }

        public void StopSound(string key)
        {
            if (!mps.ContainsKey(key))
                return;
            mps[key].Pause();
        }

        public void PlayMusic()
        {
            if (muteMusic)
                return;
            mps[currentMusicKey].Start();
        }

        public void PauseMusic()
        {
            if (muteMusic)
                return;
            mps[currentMusicKey].Pause();
        }

        public static void SetSounds(bool isMute)
        {
            muteSounds = isMute;
        }
        public static void SetMusic(bool isMute)
        {
            muteMusic = isMute;
        }
    }
}