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
    static class CharacterHelper
    {
        public static string[] Names = {"Aharon", "Sanjay", "Bell" , "Hector" , "Vjay"};
        public static float[] Scales = { 0.17f, 0.17f, 0.17f, 0.17f, 0.17f };
        public static Paint[] Paints = {
            Operations.PaintColor(Color.Rgb(255 , 0 , 76)) ,
            Operations.PaintColor(Color.Rgb(241 , 79 , 0)) ,
            Operations.PaintColor(Color.Rgb(0 , 255 , 255)) ,
            Operations.PaintColor(Color.Rgb(213 , 189 , 80)) ,
            Operations.PaintColor(Color.Rgb(213 , 189 , 80))
        };
        public static int Length = Names.Length;

        public static int Current = 0;
        public static List<Character> GetList(Context context)
        {

            List<Character> list = new List<Character>();
            for (int i = 0; i < Length; i++)
            {
                int id = context.Resources.GetIdentifier("character" + i, "drawable", context.PackageName);

                Bitmap sprite = BitmapFactory.DecodeResource(context.Resources , id);
                string name = Names[i];

                list.Add(new Character(sprite, name));
            }

            return list;
        }

        public static void SetCurrent(int index)
        {
            Current = index;
        }

        public static Bitmap GetCurrentPlayerBitmap(Context context)
        {
            int id = context.Resources.GetIdentifier("player" + Current, "drawable", context.PackageName);
            return Operations.GetResizedBitmap(BitmapFactory.DecodeResource(context.Resources, id) , Scales[Current]);
        }

        public static Paint GetCurrentPlayerPaint()
        {
            return Paints[Current]; 
        }

    }
}