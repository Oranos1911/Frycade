using Android.App;
using Android.Content;
using Android.Graphics;
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
    static class CanvasConstants
    {
        public static float ScallerWidth;
        public static float ScallerHeight;
        public static float ScallerSqaure;

        public static void InitCanvasScale(Canvas canvas)
        {
            ScallerWidth = canvas.Width;
            ScallerHeight = canvas.Height;
            ScallerSqaure = ScallerWidth * ScallerHeight;
        }


    }
}