using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace Doodle
{
    abstract class Accessorie 
    {
        public bool IsActive { get; set; }

        public Accessorie()
        {           
            IsActive = false;
        }

        public abstract void Init(Boardgame game, Canvas canvas, Plane plane);
        public abstract void Run(Boardgame game, Canvas canvas, Plane plane);
        public virtual void Dismiss(Boardgame game, Canvas canvas, Plane plane) 
        {
            IsActive = false;
        }


    }

    abstract class Monster : Accessorie 
    {
        public Mass mass;
        public bool IsDead { get; set; }

        public Monster() : base()
        {
            IsDead = false;
        }
        public void Kill()
        {
            IsDead = true;
            mass.paint = Operations.PaintColor(Color.Red);
            mass.v = new Vector(0 , Player.jumpVelocity);
            mass.a = Mass.gravityForce;
        }

    }


    class Cat : Monster
    {
        public Color objectColor = Color.Rgb(255, 255, 0);
        public Cat() : base() { }

        public override void Init(Boardgame game, Canvas canvas, Plane plane)
        {
            if(IsActive)
            {
                mass = new Mass(
                    Vector.zeroVector, Vector.zeroVector, 
                    Vector.zeroVector, 
                    game.MonsterCatSprite , 
                    Operations.PaintColor(objectColor) , 
                    false
                    );
            }
        }

        public override void Run(Boardgame game, Canvas canvas, Plane plane)
        {
            if(IsActive)
            {
                if (!IsDead)
                    mass.r = plane.r + new Vector(plane.Sprite.Width / 2 - mass.Sprite.Width / 2, -1 * mass.Sprite.Height);
                mass.Run(game, canvas);

                if (mass.r.y > canvas.Height)
                    Dismiss(game, canvas, plane);
            }
           
        }
    }

    class Bird : Monster
    {
        public static bool IsInstanceRunning = false;
        public static float Velocity = 7.5f;

        public Bird() : base() 
        {
            IsInstanceRunning = false;
        }

        public override void Init(Boardgame game, Canvas canvas, Plane plane)
        {
            if (IsActive)
            {
                game.soundManager.PlaySound("hawk");

                Random rand = new Random();
                float initialLocation;
                int direction = (int) Math.Pow(-1, rand.Next(1, 3));
                Bitmap sprite;
                if (direction < 0)
                {
                    sprite = game.MonsterBirdSprite;
                    initialLocation = canvas.Width;
                }
                else
                {
                    sprite = Operations.FlipHorizontalBitmap(game.MonsterBirdSprite);
                    initialLocation = 0;
                }
                    
                
                mass = new Mass(
                    new Vector(initialLocation ,0) , 
                    direction * new Vector(Velocity , 0) , 
                    Vector.zeroVector, sprite ,
                    Operations.PaintRandomColor() ,
                    false
                    );
                IsInstanceRunning = true;
            }
        }

        public override void Run(Boardgame game, Canvas canvas, Plane plane)
        {
            if (IsActive)
            {
                mass.Run(game, canvas);
                if (mass.r.x > canvas.Width || mass.r.x + mass.Sprite.Width < 0)
                    Dismiss(game , canvas , plane);
                if (mass.r.y - mass.Sprite.Height > canvas.Height) 
                    Dismiss(game, canvas, plane);
            }
        }

        public override void Dismiss(Boardgame game, Canvas canvas, Plane plane)
        {
            base.Dismiss(game, canvas, plane);
            IsInstanceRunning = false;
        }

        public void Drop(float offset)
        {
            mass.r.y += offset;
        }

     }

    class Ramp : Accessorie
    {
        public static Paint objectColor = Operations.PaintColor(Color.Rgb(0, 255, 255));
        public Mass mass;

        public static float jumpVelocity = 110f;

        public Ramp() : base() { }

        public override void Init(Boardgame game, Canvas canvas, Plane plane)
        {
            if (IsActive)
            {
                mass = new Mass(
                    Vector.zeroVector , 
                    Vector.zeroVector, 
                    Vector.zeroVector, 
                    game.RampSprite , 
                    objectColor , 
                    false
                    );
            }
        }

        public override void Run(Boardgame game, Canvas canvas, Plane plane)
        {
            if (IsActive)
            {
                mass.r = plane.r + new Vector(plane.Sprite.Width / 2 , -1 * mass.Sprite.Height / 2);
                mass.Run(game, canvas);
            }
        }

    }
    class Ocilator : Accessorie
    {
        public static Color objectColor = Color.Rgb(255, 0, 255);
        public static float ocillateVelocity = 7;
        public static float ocillateRadius = 300;
        public float InitialX { get; set; }

        public Ocilator() : base() { }
        public override void Init(Boardgame game , Canvas canvas , Plane plane)
        {
            if(IsActive)
            {
                InitialX = plane.r.x;
                plane.v.x = ocillateVelocity;
                plane.paint = Operations.PaintColor(objectColor); 
            }

        }

        public override void Run(Boardgame game, Canvas canvas, Plane plane)
        {
            if (IsActive)
            {
                if (Math.Abs(plane.r.x - InitialX) > ocillateRadius)
                {
                    if (plane.v.x > 0)
                        plane.r.x = InitialX + ocillateRadius;
                    else
                        plane.r.x = InitialX - ocillateRadius;
                    plane.v.x *= -1;
                }
            }
            else
                Dismiss(game, canvas, plane);
            
        }

        public override void Dismiss(Boardgame game, Canvas canvas, Plane plane)
        {
            base.Dismiss(game, canvas, plane);
            plane.ResetColor();
            plane.v.x = 0;
        }
    }

    class Plane : Mass
    {

        public static Paint DefaultPaint = Operations.PaintColor(Color.OrangeRed);

        public static float OcilatorSpawnProb = 0.09f;
        public static float RampSpawnProb = 0.043f;
        public static float MonsterCatSpawnProb = 0.07f;
        public static float MonsterBirdSpawnProb = 0.03f;

        public Ocilator Ocilator { get; set; }
        public Ramp ramp { get; set; }
        public Cat monsterCat { get; set; }
        public Bird monsterBird { get; set; }


        public Plane(Vector location , Vector velocity , Vector acceleration , Bitmap sprite) 
            : base(location , velocity , acceleration , sprite , DefaultPaint , true) 
        {

            Ocilator = new Ocilator();
            ramp = new Ramp();
            monsterCat = new Cat();
            monsterBird = new Bird();

            ResetColor();
        }

        public void ResetColor()
        {
            paint = DefaultPaint;
        }

        private void Restate(Boardgame game, Canvas canvas)
        {
            Ocilator.IsActive =  new Random().NextDouble() <= OcilatorSpawnProb;
            ramp.IsActive = new Random().NextDouble() <= OcilatorSpawnProb;
            monsterCat.IsActive = new Random().NextDouble() <= MonsterCatSpawnProb && !ramp.IsActive && game.player.IsProtected();
            monsterBird.IsActive = new Random().NextDouble() <= MonsterBirdSpawnProb && !Bird.IsInstanceRunning && game.player.IsProtected();

            Ocilator.Init(game , canvas , this);
            ramp.Init(game, canvas, this);
            monsterCat.Init(game, canvas, this);
            monsterBird.Init(game, canvas, this);
        }

        public void Relocate(Boardgame game ,  Canvas canvas)
        {
            Random rand = new Random();
            float x = rand.Next(Sprite.Width, canvas.Width - Sprite.Width);
            float y = (float) (rand.NextDouble() * Boardgame.PlanesPadding) - Boardgame.PlanesPadding;
            r = new Vector(x, y);
            Restate(game, canvas);
        }
        public void Drop(float offset)
        {
            r.y += offset;
            if (monsterBird.IsActive)
                monsterBird.Drop(offset);
        }

        protected override void PreMechanics(Boardgame game, Canvas canvas)
        {
            Ocilator.Run(game , canvas ,this);
            ramp.Run(game, canvas, this);
            monsterCat.Run(game, canvas, this);
            monsterBird.Run(game, canvas, this);
        }

        protected override void PostMechanics(Boardgame game, Canvas canvas)
        {
            // Check Right Corener collosion
            if(r.x + Sprite.Width > canvas.Width)
            {
                r.x = canvas.Width - Sprite.Width;
                v.x *= -1;
            }

            // Check Left Corener collosion
            if (r.x < 0)
            {
                r.x = 0;
                v.x *= -1;
            }
        }
    }
}