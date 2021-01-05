﻿using Android.App;
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
    class Bullet : Mass
    {
        private static float BulletVelocity = 65f;
        private static float ToRange = 1350;
        private static float FromRange = 0;
        private static float FromAngle = (float) (3 * Math.PI / 4);
        private static float ToAngle = (float) Math.PI / 4;


        public bool IsActive { get; set; }
        public Bullet(Bitmap sprite) 
            : base(Vector.zeroVector , Vector.zeroVector , Vector.zeroVector , sprite)
        {
            IsActive = false;
        }

        public void Init(Player player , float offset)
        {
            IsActive = true;
            float angle = ExtensionMethods.Map(offset, FromRange , ToRange, FromAngle , ToAngle);

            paint = Operations.PaintRandomColor();
            r = player.r + new Vector(0 , player.Sprite.Height / 2);
            v = Vector.FromPolar(BulletVelocity , angle);
        }

        public override void Run(Boardgame game, Canvas canvas)
        {
            if(IsActive)
                base.Run(game, canvas);
        }

        protected override void PostMechanics(Boardgame game, Canvas canvas)
        {
            base.PostMechanics(game, canvas);
            bool axisY = r.y + Sprite.Height > canvas.Height || r.y < 0;
            bool axisX = r.x > canvas.Width || r.x + Sprite.Width < 0;
            if (axisX || axisY)
                IsActive = false;
        }

        public override bool IsInteractedBellow(Mass mass)
        {
            if (!IsActive)
                return false;
            return base.IsInteractedBellow(mass);
        }

    }
}