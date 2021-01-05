using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.Graphics;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace Doodle
{
    class Hud
    {
        private static float FinalTextSize = 125;
        private static float NewScoreSize = 75; 

        private static float btnsPadding = 100f;
        private static float textPadding = 150f;

        private static float paddingHorizontal = 100f;
        private static float paddingVertical = 50f;

        private Context context;
        private bool isPauseButtonFlipped;

        private bool isGameOver, isNewScore;
        public float HudHeight { get; set; }
        public Canvas canvas { get; set; }
        public CanvasButton BtnPause { get; set; }
        public CanvasButton BtnHome { get; set; }
        public CanvasButton BtnPlay { get; set; }

        public Hud(Context context , Canvas canvas , float height)
        {
            this.context = context;
            this.canvas = canvas;
            HudHeight = height;

            isPauseButtonFlipped = false;
            isGameOver = false;
            isNewScore = false;

            Bitmap iconPause = BitmapFactory.DecodeResource(context.Resources, Resource.Drawable.ic_pause);
            iconPause = Operations.GetResizedBitmap(iconPause, 0.34f);
            Vector locationPause = new Vector(canvas.Width - iconPause.Width - paddingHorizontal, paddingVertical);
            Bitmap iconHome = BitmapFactory.DecodeResource(context.Resources, Resource.Drawable.ic_home);
            iconHome = Operations.GetResizedBitmap(iconHome, 0.34f);
            Vector locationHome = new Vector(canvas.Width / 2 - iconHome.Width - btnsPadding, canvas.Height / 2 + canvas.Height / 5);
            Bitmap iconPlay = BitmapFactory.DecodeResource(context.Resources, Resource.Drawable.ic_play);
            iconPlay = Operations.GetResizedBitmap(iconPlay, 0.34f);
            Vector locationPlay = new Vector(canvas.Width / 2 + iconPlay.Width - btnsPadding, canvas.Height / 2 + canvas.Height / 5);

            BtnPause = new CanvasButton(iconPause , locationPause);
            BtnHome = new CanvasButton(iconHome, locationHome);
            BtnPlay = new CanvasButton(iconPlay , locationPlay);

        }

        public void DrawHUD(int score)
        {
            DrawText("" + score , HudHeight, Color.White, paddingHorizontal, HudHeight + paddingVertical , false);
            BtnPause.Draw(canvas);
            if (isGameOver)
                DrawFinal(score);
        }

        public void DrawPause()
        {
            DrawText("Pause" , HudHeight, Color.Yellow , paddingHorizontal, 2 * HudHeight + paddingVertical , false);
        }

        private void DrawFinal(int score)
        {
            DrawText("Game\nOver!", FinalTextSize , Color.Red, canvas.Width / 2 , canvas.Height / 2 , true);
            BtnHome.Draw(canvas);
            BtnPlay.Draw(canvas);
            if (isNewScore)
                DrawRecord(score);
        }

        private void DrawRecord(int score)
        {
            DrawText("New High Score!", NewScoreSize , Color.Yellow  , canvas.Width / 2 , canvas.Height / 2 + textPadding , true);
            DrawText("" + score , NewScoreSize , Color.Green, canvas.Width / 2 , canvas.Height / 2 + 2 * textPadding, true);

        }
        private void DrawText(string str , float size , Color color , float x , float y , bool center)
        {

            Paint text = new Paint();
            text.TextSize = size;
            text.Color = color;
            text.SetTypeface(context.Resources.GetFont(Resource.Font.arcade));
            if (center)         
               text.TextAlign = Paint.Align.Center;

            Paint paintGlow = new Paint();
            paintGlow.Set(text);
            paintGlow.SetMaskFilter(new BlurMaskFilter(35, BlurMaskFilter.Blur.Normal));

            canvas.DrawText(str , x, y, text);
            canvas.DrawText(str , x, y, paintGlow);
        }
        
        public void FlipPauseButton()
        {
            if (isGameOver)
                return;

            if (!isPauseButtonFlipped)
                BtnPause.Icon = BtnPlay.Icon;
            else
                BtnPause.Icon = Operations.GetResizedBitmap(BitmapFactory.DecodeResource(context.Resources, Resource.Drawable.ic_pause), 0.34f);
            isPauseButtonFlipped = !isPauseButtonFlipped;
        }

        public void NotifyGameOver()
        {
            isGameOver = true;
        }

        public void NotifyNewScore()
        {
            isNewScore = true;
        }
    }
}