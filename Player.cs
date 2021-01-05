using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Hardware;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace Doodle
{
    class Player : Mass
    {

        public static float jumpVelocity = 50f;
        private static float protectVelocity = 45f;
        private static float limitVelocity = 250f;
        private static float sensorVelocity = 5f;
        private static float sensorAcceleration = 4.5f;
        private static float sensorFlipSensitivity = 2.5f;

        public bool IsInverted { get; set; }
        public bool IsDead { get;  set; }

        public Player(Vector location , Vector velocity , Vector acceleration , Bitmap sprite) 
            : base(location , velocity , acceleration , sprite , CharacterHelper.GetCurrentPlayerPaint() , true)
        {
            IsDead = false;
            IsInverted = false;
        }

 
        public void ReactToSensor(float value)
        {
            v.x = -1 * sensorVelocity * value;
            a.x = -1 * sensorAcceleration * value;
        }

        public void Kill()
        {
            IsDead = true;
            paint = Operations.PaintColor(Color.Red);
            v = new Vector(0, jumpVelocity);
        }

        public bool IsProtected()
        {
            if (Math.Abs(v.y) < protectVelocity)
                return true;
            return false;
        }
        protected override void PostMechanics(Boardgame game, Canvas canvas)
        {

            if (r.x > canvas.Width)
                r.x = 0;
         
            if (r.x < 0)          
                r.x = canvas.Width;
            
            if (Math.Abs(v.x) > limitVelocity)
            {
                v.x = Math.Sign(v.x) * limitVelocity;
            }

            if (v.x < -1 * sensorFlipSensitivity && !IsInverted)
            {
                Sprite = Operations.FlipHorizontalBitmap(Sprite);
                IsInverted = true;
            }
            if (v.x > sensorFlipSensitivity && IsInverted)
            {
                Sprite = Operations.FlipHorizontalBitmap(Sprite);
                IsInverted = false;
            }
        }

        public override bool IsInteractedAbove(Mass mass)
        {
            if (IsDead) return false;
            return base.IsInteractedAbove(mass);
        }

        public override bool IsInteractedBellow(Mass mass)
        {
            if (IsDead) return false;
            return base.IsInteractedBellow(mass);
        }

    }
}