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
        public Mass Mass { get; set; }
        public bool IsDead { get; set; }

        public Monster() : base()
        {
            IsDead = false;
        }
        public void Kill()
        {
            IsDead = true;
            Mass.paint = Operations.PaintColor(Color.Red);
            Mass.v = new Vector(0 , Player.jumpVelocity);
            Mass.a = Mass.GravityForce;
        }

    }


    class Cat : Monster
    {
        public static Color ObjectColor = Color.Rgb(255, 255, 0);
        public Cat() : base() { }

        public override void Init(Boardgame game, Canvas canvas, Plane plane)
        {
            if(IsActive)
            {
                Mass = new Mass(
                    Vector.zeroVector, Vector.zeroVector, 
                    Vector.zeroVector, 
                    game.MonsterCatSprite , 
                    Operations.PaintColor(ObjectColor) , 
                    false
                    );
            }

            IsDead = false;
        }

        public override void Run(Boardgame game, Canvas canvas, Plane plane)
        {
            if(IsActive)
            {
                if (!IsDead)
                    Mass.r = plane.r + new Vector(plane.Sprite.Width / 2 - Mass.Sprite.Width / 2, -1 * Mass.Sprite.Height);
                Mass.Run(game, canvas);

                if (Mass.r.y > canvas.Height)
                    Dismiss(game, canvas, plane);
            }
           
        }
    }

    class Bird : Monster
    {
        // Canvas Variables (To be scalled)
        public static float Velocity = 9.5f;

        public static void InitCanvasVariables()
        {
            Velocity = CanvasScaler.GetFloat(Velocity, CanvasScaler.Dim.width);
        }
        public Bird() : base() { }

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
                    
                
                Mass = new Mass(
                    new Vector(initialLocation ,0) , 
                    direction * new Vector(Velocity , 0) , 
                    Vector.zeroVector, 
                    sprite ,
                    Operations.PaintRandomColor() ,
                    false
                    );
            }
        }

        public override void Run(Boardgame game, Canvas canvas, Plane plane)
        {
            if (IsActive)
            {
                Mass.Run(game, canvas);
                if (Mass.r.x > canvas.Width || Mass.r.x + Mass.Sprite.Width < 0)
                    Dismiss(game , canvas , plane);
                if (Mass.r.y - Mass.Sprite.Height > canvas.Height) 
                    Dismiss(game, canvas, plane);
            }
        }

        public void Drop(float offset)
        {
            Mass.r.y += offset;
        }

     }

    class Ramp : Accessorie
    {
        // Canvas Variables (To be scalled)
        public static float JumpVelocity = 110f;

        public static Color objectColor = Color.Rgb(0, 255, 255);
        public Mass mass;

        public static void InitCanvasVariables()
        {
            JumpVelocity = CanvasScaler.GetFloat(JumpVelocity, CanvasScaler.Dim.height);
        }
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
                    Operations.PaintColor(objectColor) , 
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
        // Canvas Variables (To be scalled)
        public static float ocillateVelocity = 7;
        public static float ocillateRadius = 300;

        public static Color objectColor = Color.Rgb(255, 0, 255);

        public float InitialX { get; set; }

        public static void InitCanvasVariables()
        {
            ocillateVelocity = CanvasScaler.GetFloat(ocillateVelocity, CanvasScaler.Dim.width);
            ocillateRadius = CanvasScaler.GetFloat(ocillateRadius, CanvasScaler.Dim.width);
        }

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

        public static float RampSpawnProb = 0.067f;
        public static float OcilatorSpawnMaxProb = 0.37f;      
        public static float MonsterCatSpawnMaxProb = 0.32f;
        public static float MonsterBirdSpawnMaxProb = 0.222f;

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
            ramp.IsActive = new Random().NextDouble() <= RampSpawnProb;
            Ocilator.IsActive =  new Random().NextDouble() <= game.GetScoreProbability(OcilatorSpawnMaxProb);        
            monsterCat.IsActive = new Random().NextDouble() <= game.GetScoreProbability(MonsterCatSpawnMaxProb) && game.player.IsProtected() && !ramp.IsActive;
            monsterBird.IsActive = new Random().NextDouble() <= game.GetScoreProbability(MonsterBirdSpawnMaxProb) &&  game.player.IsProtected();

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