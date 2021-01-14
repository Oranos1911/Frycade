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
            if (value >= toSource)
                return toTarget;
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

        public static Paint PaintGlow(Paint paint , int amount)
        {
            Paint paintGlow = new Paint();
            paintGlow.Set(paint);
            paintGlow.SetMaskFilter(new BlurMaskFilter(amount , BlurMaskFilter.Blur.Normal));

            return paintGlow;
        }
        public static Paint PaintRandomColor()
        {
            Random rand = new Random();

            Color[] colors =
            {
                Color.Rgb(255 , 0 , 0) ,
                Color.Rgb(0 , 255 , 0) ,
                Color.Rgb(255 , 255 , 0) ,
                Color.Rgb(255, 0, 255) ,
                Color.Rgb(0 , 255 , 255)
            };

            return PaintColor(colors[rand.Next(0 , colors.Length)]);
        }
    }
}