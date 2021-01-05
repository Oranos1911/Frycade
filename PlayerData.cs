using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Doodle
{
    class PlayerData
    {
        public string Name { get; set; }
        public string Score { get; set; }

        public PlayerData(string name , double score)
        {
            this.Name = name;
            this.Score = (int)score + "";
        }
    }
}