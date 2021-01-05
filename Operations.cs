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

    public static class ExtensionMethods
    {
        public static float Map(float value, float fromSource, float toSource, float fromTarget, float toTarget)
        {
            return (value - fromSource) / (toSource - fromSource) * (toTarget - fromTarget) + fromTarget;
        }
    }

    public static class Operations
    {
        public static Bitmap GetResizedBitmap(Bitmap bitmap, float scaleFactor)
        {
            int width = bitmap.Width;
            int height = bitmap.Height;
            Matrix matrix = new Matrix();
            matrix.PostScale(scaleFactor, scaleFactor);
            Bitmap resizedBitmap = Bitmap.CreateBitmap(bitmap, 0, 0, width, height, matrix, false);
            bitmap.Recycle();
            return resizedBitmap;
        }


        public static Bitmap FlipHorizontalBitmap(Bitmap bitmap)
        {
            Matrix matrix = new Matrix();
            matrix.PreScale(-1, 1);
            Bitmap dst = Bitmap.CreateBitmap(bitmap, 0, 0, bitmap.Width, bitmap.Height, matrix, false);
            return dst;
        }

        public static Paint PaintColor(Color color)
        {
            ColorFilter filter = new LightingColorFilter(color , 1);
            Paint paint = new Paint();
            paint.SetColorFilter(filter);
            return paint;
        }

        public static Paint PaintRandomColor()
        {
            Random rand = new Random();

            int red = 25 + rand.Next(0, 255);
            int green = 25 + rand.Next(0, 255);
            int blue = 25 + rand.Next(0, 255);

            return PaintColor(Color.Rgb(red, green, blue));
        }
    }
}