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
    class Mass
    {
        // Analytical properties
        public static Vector gravityForce = new Vector(0, 1.35f);
        public static float dt = 1f;
        public static float collisionRange = 450f;
        public Vector r { get; set; } // Location
        public Vector v { get; set; } // Velocity
        public Vector a { get; set; } // Acceleration

        // Dimensional properties
        public Bitmap Sprite { get; set; }
        public Paint paint { get; set; }
        public Mass(Vector location, Vector velocity, Vector acceleration, Bitmap sprite , Paint paint , bool centerBitmap)
        {
            this.Sprite = sprite;
            this.paint = paint;

            if (!centerBitmap)
                r = new Vector(location);
            else
                r = new Vector(location + -1 * new Vector(sprite.Width / 2, sprite.Height / 2));
            
            v = new Vector(velocity);
            a = new Vector(acceleration);
        }

        public Mass(Vector location, Vector velocity, Vector acceleration, Bitmap sprite)
        {
            this.Sprite = sprite;
            this.paint = null;

            r = new Vector(location);
            v = new Vector(velocity);
            a = new Vector(acceleration);
        }
        public virtual void Run(Boardgame game, Canvas canvas)
        {   
            Update(game, canvas);
            Draw(canvas);
        }

        protected virtual void PreMechanics(Boardgame game ,  Canvas canvas) {; }
        protected virtual void PostMechanics(Boardgame game, Canvas canvas) {; }

        public virtual bool IsInteractedAbove(Mass mass)
        {
            bool axisY = r.y + Sprite.Height > mass.r.y && r.y + Sprite.Height < mass.r.y + mass.Sprite.Height;
            bool axisX1 = r.x > mass.r.x && r.x < mass.r.x + mass.Sprite.Width;
            bool axisX2 = r.x + Sprite.Width > mass.r.x && r.x + Sprite.Width < mass.r.x + mass.Sprite.Width;

            if (axisY && (axisX1 || axisX2) && v.y > 0)
                return true;
            return false;
        }

        public virtual bool IsInteractedBellow(Mass mass)
        {
            bool axisY = r.y < mass.r.y + mass.Sprite.Height && r.y > mass.r.y;
            bool axisX1 = r.x > mass.r.x && r.x < mass.r.x + mass.Sprite.Width;
            bool axisX2 = r.x + Sprite.Width > mass.r.x && r.x + Sprite.Width < mass.r.x + mass.Sprite.Width;

            if (axisY && (axisX1 || axisX2))
                return true;
            return false;
        }

        private void Update(Boardgame game, Canvas canvas)
        {
            PreMechanics(game, canvas);
            v = v + a * dt;
            r = r + v * dt;
            PostMechanics(game, canvas);
        }

        private void Draw(Canvas canvas)
        {
            if (Sprite != null)
                canvas.DrawBitmap(Sprite, r.x  , r.y , paint);
            //  For Debug only :
            /*
              Paint paint1 = new Paint();
              paint1.Color = Color.Red;
              canvas.DrawCircle(r.x, r.y, 10 , paint1);
            */
        }

    }
}