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

namespace Doodle
{
    static class Constants
    {
        public static int MAX_NAME_LENGTH = 13;
        public static int MIN_NAME_LENGTH = 3;

        public static string SHARED_PREFERENCES_NAME = "Settings";
        public static string SELECT_CHARACTER_KEY = "character";
        public static string SELECT_NICKNAME_KEY = "nickname";
        public static string MUTE_SOUNDS_KEY = "muteSounds";
        public static string MUTE_MUSIC_KEY = "muteMusic";
        public static string LAST_HIGH_SCORE_KEY = "lastScore";
    }
}