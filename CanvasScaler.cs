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
        enum dim 
        {
            width ,
            height
        }

        public static float ScallerWidth;
        public static float ScallerHeight;
        public static float ScallerSqaure;

        public static void InitCanvasScale(Canvas canvas)
        {
            ScallerWidth = canvas.Width;
            ScallerHeight = canvas.Height;
            ScallerSqaure = ScallerWidth * ScallerHeight;
        }

        public static float GetFloat(float f , string dim)
        {
            //if(dim == dim.height)
            //    return f * ScallerWidth;
            //if (dim == "height")
            //    return f * ScallerHeight;
            return 0;
        }


    }
}