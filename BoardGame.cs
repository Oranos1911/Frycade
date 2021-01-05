using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Hardware;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;


namespace Doodle
{
    class Boardgame : SurfaceView
    {
        private static int hudHeight = 125;
        private static int scoreFactor = 10;

        public static float StageScrollLimit = 1000f;
        public static int PlanesCount = 10;
        public static float PlanesPadding = 45;

        private bool isFirstCall;
        public bool threadRunning = true;
        public bool isRunning = true;

        public Thread t;
        public ThreadStart ts;

        public Context context { get; set; }
        public SoundManager soundManager { get; set; }
        public Hud hud { get; set; }

        private bool isGameOver;
        private bool isGamePaused;
        private int score;
        public Player player { get; set; }
        public Plane[] planes { get; set; }

        public List<Bullet> bullets { get; set; }

        public Bitmap PlayerSprite { get; set; }
        public Bitmap PlaneSprite { get; set; }
        public Bitmap RampSprite { get; set; }
        public Bitmap MonsterCatSprite { get; set; }
        public Bitmap MonsterBirdSprite { get; set; }
        public Bitmap BulletSprite { get; set; }


        public Boardgame(Context context) : base(context)
        {

            isFirstCall = true;
            isRunning = true;
            threadRunning = true;
            ts = new ThreadStart(Run);
            t = new Thread(ts);

            this.context = context;
            this.soundManager = new SoundManager(context);
        }

        private void InitGame(Canvas canvas)
        {
            isGameOver = false;
            isGamePaused = false;
            score = 0;

            PlayerSprite = CharacterHelper.GetCurrentPlayerBitmap(context);
            BulletSprite = Operations.GetResizedBitmap(BitmapFactory.DecodeResource(context.Resources, Resource.Drawable.sp_bullet), 0.2f);
            PlaneSprite = Operations.GetResizedBitmap(BitmapFactory.DecodeResource(context.Resources, Resource.Drawable.sp_plane), 0.55f);
            RampSprite = Operations.GetResizedBitmap(BitmapFactory.DecodeResource(context.Resources, Resource.Drawable.sp_trampoline), 0.14f);
            MonsterCatSprite = Operations.GetResizedBitmap(BitmapFactory.DecodeResource(context.Resources, Resource.Drawable.sp_monster_cat), 0.08f);
            MonsterBirdSprite = Operations.GetResizedBitmap(BitmapFactory.DecodeResource(context.Resources, Resource.Drawable.sp_monster_bird), 0.07f);

            hud = new Hud(context, canvas, hudHeight);

            player = new Player(
                      new Vector(canvas.Width / 2, canvas.Height / 2),
                      Vector.zeroVector,
                      Mass.gravityForce,
                      PlayerSprite
                );

            bullets = new List<Bullet>();

            planes = new Plane[PlanesCount];

            for (int i = 0; i < planes.Length; i++)
            {
                if (i == 0) planes[i] = new Plane(
                     new Vector(canvas.Width / 2, canvas.Height / 2 + canvas.Height / 3),
                     new Vector(0, 0),
                     new Vector(0, 0),
                     PlaneSprite
                 );
                else
                {
                    float planeHeight = planes[i - 1].r.y - canvas.Height / planes.Length;
                    planes[i] = new Plane(
                    Vector.CreateRandom(PlaneSprite.Width, canvas.Width - PlaneSprite.Width, planeHeight, planeHeight),
                    Vector.zeroVector,
                    Vector.zeroVector,
                    PlaneSprite
                    );
                }

            }

            soundManager.PlayMusic();
        }

        public void Run()
        {
            while (threadRunning)
            {
                if (isRunning)
                {
                    if (!this.Holder.Surface.IsValid)
                        continue;
                    Canvas canvas = null;
                    try
                    {
                        canvas = this.Holder.LockCanvas();
                        canvas.DrawColor(Color.Transparent, PorterDuff.Mode.Clear);
                        RunGame(canvas);
                    }
                    catch (Exception e) 
                    {            
                    }

                    finally
                    {
                        if (canvas != null)
                        {
                            this.Holder.UnlockCanvasAndPost(canvas);
                        }
                    }

                }
            }

        }

        private void RunLogic(Canvas canvas)
        {
            if (player.r.y > canvas.Height)
            {
                FinishGame();
                return;
            }

            if (player.r.y < StageScrollLimit)
            {
                float offset = StageScrollLimit - player.r.y;
                player.r.y = StageScrollLimit;
                foreach (Plane plane in planes)
                {
                    plane.Drop(offset);
                    if (plane.r.y > canvas.Height)
                        plane.Relocate(this , canvas);
                }

                score += (int)(offset / scoreFactor);
            }


            foreach (Plane plane in planes)
            {
                if (plane.monsterCat.IsActive)
                {
                    if (bullets.Count > 0 && bullets.Last().IsInteractedBellow(plane.monsterCat.mass))
                    {
                        plane.monsterCat.Kill();
                        soundManager.PlaySound("cat");
                    }

                    if (player.IsInteractedAbove(plane.monsterCat.mass))
                    {
                        player.Jump();
                        plane.monsterCat.Kill();
                        soundManager.PlaySound("cat");
                    }
                    else if (player.IsInteractedBellow(plane.monsterCat.mass) && !plane.monsterCat.IsDead)
                    {
                        soundManager.PlaySound("punch");
                        player.Kill();
                    }
                }

                if (plane.monsterBird.IsActive)
                {
                    if (bullets.Count > 0 && bullets.Last().IsInteractedBellow(plane.monsterBird.mass))
                    {
                        plane.monsterBird.Kill();
                        soundManager.PlaySound("jumpshot");
                    }

                    if (player.IsInteractedAbove(plane.monsterBird.mass))
                    {
                        player.Jump();
                        plane.monsterBird.Kill();
                        soundManager.PlaySound("jumpshot");
                    }
                    else if (player.IsInteractedBellow(plane.monsterBird.mass) && !plane.monsterBird.IsDead)
                    {
                        soundManager.PlaySound("punch");
                        player.Kill();
                        break;
                    }
                }

                if (player.IsInteractedAbove(plane))
                {
                    if (plane.ramp.IsActive)
                    {
                        player.JumpRamp();
                        soundManager.PlaySound("ramp");
                    }
                    else
                    {
                        player.Jump();
                        soundManager.PlaySound("jump");
                    }
                }

                //   If the jump velocity is set to be higher than hitting velocity (now it is) -> No need to reset position
                //   Because Euler Integration Method is used
                //   r.y = plane.r.y - sprite.Height;
            }
        }

        public void RunGame(Canvas canvas)
        {

            if (isFirstCall)
            {
                InitGame(canvas);
                isFirstCall = false;
            }

            foreach (Plane plane in planes)
                plane.Run(this, canvas);
            
            foreach (Bullet bullet in bullets)
                bullet.Run(this, canvas);

            if (bullets.Count > 0 && !bullets.Last().IsActive)
                bullets.RemoveAt(bullets.Count - 1);

            if (!isGameOver)
            {
                player.Run(this, canvas);
                RunLogic(canvas);
            }

            hud.DrawHUD(score);

            if (isGamePaused)
            {
                hud.DrawPause();
                pause();
            }
        }

        public void NotifySensorChanged(SensorEvent e)
        {
            if (player == null) return;
            player.ReactToSensor(e.Values[0]);
        }

        public override bool OnTouchEvent(MotionEvent e)
        {

            Vector location = new Vector(e.GetX(), e.GetY());


            if (hud.BtnPause.IsPressed(location))
            {
                hud.FlipPauseButton();
                if (isRunning && !isGameOver)
                    isGamePaused = true;
                else
                {
                    isGamePaused = false;
                    resume();
                }
            }

            else if (hud.BtnHome.IsPressed(location))
            {
                destroy();
            }

            else if (hud.BtnPlay.IsPressed(location))
            {
                RestartGame();
            }

            else if(!isGameOver && !isGamePaused)
            {
                Bullet bullet = new Bullet(BulletSprite);
                bullet.Init(player, location.x);
                bullets.Add(bullet);
                soundManager.PlaySound("shot");
            }

            return base.OnTouchEvent(e);
        }

        public void startGame()
        {
            isRunning = true;
        }

        public void RestartGame()
        {
            isFirstCall = true;
        }

        public void FinishGame()
        {
            isGameOver = true;
            soundManager.PlaySound("fell");
            soundManager.PauseMusic();
            hud.NotifyGameOver();
            if (((GameActivity)context).UpdateScore(score))
            {
                hud.NotifyNewScore();
            }
        }

        public void resume()
        {
            if(!isFirstCall)
                soundManager.PlayMusic();
            isRunning = true;
        }

        public void pause()
        {
            soundManager.PauseMusic();
            isRunning = false;
        }

        public void destroy()
        {
            isRunning = false;
            ((GameActivity)context).Finish();
        }

    }
}