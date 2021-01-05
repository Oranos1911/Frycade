using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace Doodle
{
    class Character
    {
        public Bitmap Sprite { get; set; }
        public string Name { get; set; }

        public Character(Bitmap sprite , string name)
        {
            this.Sprite = sprite;
            this.Name = name;
        }


    }
}