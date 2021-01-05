﻿using Android.Graphics;
using System;

namespace Doodle
{
    class CanvasButton
    {
        public bool IsExists { get; set; }
        public Bitmap Icon { get; set; }
        public Vector Location { get; set; }

        public CanvasButton(Bitmap icon , Vector location)
        {
            this.IsExists = false;
            this.Icon = icon;
            this.Location = location;
        }

        public void Draw(Canvas canvas)
        {
            canvas.DrawBitmap(Icon, Location.x, Location.y, null);
            IsExists = true;
        }


        public bool IsPressed(Vector vector)
        {
            if (!IsExists) return false;

            bool axisX = vector.x > Location.x && vector.x < Location.x + Icon.Width;
            bool axisY = vector.y > Location.y && vector.y < Location.y + Icon.Height;
            if (axisX && axisY)
                return true;
            return false;
        }

    }

}