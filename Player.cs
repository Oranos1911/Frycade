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

        // Canvas Variables (To be scalled)
        public static float jumpVelocity = 50f;
        public static float protectVelocity = 45f;
        public static float limitVelocity = 250f;
        public static float sensorVelocity = 7.4f;
        public static float sensorAcceleration = 1.5f;
        public static float sensorFlipSensitivity = 2.5f;

        public bool IsInverted { get; set; }
        public bool IsDead { get;  set; }

        public static void InitCanvasVariables()
        {
            jumpVelocity = CanvasScaler.GetFloat(jumpVelocity, CanvasScaler.Dim.height);
            protectVelocity = CanvasScaler.GetFloat(protectVelocity, CanvasScaler.Dim.height);
            limitVelocity = CanvasScaler.GetFloat(limitVelocity, CanvasScaler.Dim.width);
            sensorVelocity = CanvasScaler.GetFloat(sensorVelocity, CanvasScaler.Dim.width);
            sensorAcceleration = CanvasScaler.GetFloat(sensorAcceleration, CanvasScaler.Dim.width);
            sensorFlipSensitivity = CanvasScaler.GetFloat(sensorFlipSensitivity, CanvasScaler.Dim.width);
        }
        public Player(Vector location , Vector velocity , Vector acceleration , Bitmap sprite) 
            : base(location , velocity , acceleration , sprite , CharacterHelper.GetCurrentPlayerPaint() , true)
        {
            IsDead = false;
            IsInverted = false;
        }

        public bool IsProtected()
        {
            if (Math.Abs(v.y) < protectVelocity)
                return true;
            return false;
        }

        public void ReactToSensor(float value)
        {
            v.x = -1 * sensorVelocity * value;
            a.x = -1 * sensorAcceleration * value;
        }

        public void Jump()
        {
            v.y = -1 * jumpVelocity;
        }

        public void JumpRamp()
        {
            v.y = -1 * Ramp.JumpVelocity;
        }
        public void Kill()
        {
            IsDead = true;
            paint = Operations.PaintColor(Color.Red);
            v = new Vector(0, jumpVelocity);
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