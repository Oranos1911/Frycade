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
    static class CanvasScaler
    {
        public enum Dim 
        {
            width,
            height
        }

        private static float ScallerWidth;
        private static float ScallerHeight;

        private static int ConstantScallerWidth = 1440;
        private static int ConstantScallerHeight = 2890;

        public static void InitCanvasScaler(Canvas canvas)
        {
            ScallerWidth = canvas.Width;
            ScallerHeight = canvas.Height;
        }

        public static float GetCanvasWidth()
        {
            return ScallerWidth;
        }

        public static float GetCanvasHeight()
        {
            return ScallerHeight;
        }

        public static float GetPresent(float p , Dim dim)
        {
            if (dim == Dim.width)
                return GetCanvasWidth() * (p / 100);
            if (dim == Dim.height)
                return GetCanvasHeight() * (p / 100);
            return 0;
        }

        public static float GetFloat(float f , Dim dim)
        {
            if (dim == Dim.width)
                return f / ConstantScallerWidth * ScallerWidth;
            if (dim == Dim.height)
                return f / ConstantScallerHeight * ScallerHeight;
            return 0;
        }

        public static Vector GetVector(Vector v)
        {
            return new Vector(GetFloat(v.x, Dim.width), GetFloat(v.y, Dim.height));
        }

       
    }
}