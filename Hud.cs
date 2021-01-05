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


        private static Color InGameScoreTextColor = Color.Rgb(255, 255, 255);
        private static Color FinalTextColor = Color.Rgb(255, 255, 0);
        private static Color NewScoreTextColor = Color.Rgb(0 , 255 , 255);
        private static Color NewScoreTextNumberColor = Color.Rgb(0, 255, 0);
        private static Color PauseTextColor = Color.Rgb(255 ,255, 0);


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
            Bitmap iconPlay = BitmapFactory.DecodeResource(context.Resources, Resource.Drawable.ic_play);
            iconPlay = Operations.GetResizedBitmap(iconPlay, 0.34f);
            Bitmap iconHome = BitmapFactory.DecodeResource(context.Resources, Resource.Drawable.ic_home);
            iconHome = Operations.GetResizedBitmap(iconHome, 0.34f);
            Vector locationPause = new Vector(canvas.Width - iconPause.Width - paddingHorizontal, paddingVertical);
            Vector locationHome = new Vector(canvas.Width / 2 - iconHome.Width - btnsPadding, 65 * canvas.Height / 100);
            Vector locationPlay = new Vector(canvas.Width / 2 + iconPlay.Width - btnsPadding, 65 * canvas.Height / 100);

            BtnPause = new CanvasButton(locationPause , iconPause , iconPlay);
            BtnHome = new CanvasButton(iconHome, locationHome);
            BtnPlay = new CanvasButton(iconPlay , locationPlay);

        }

        public void DrawHUD(int score)
        {
            DrawText("" + score , HudHeight, InGameScoreTextColor, paddingHorizontal, HudHeight + paddingVertical , false);
            BtnPause.Draw(canvas);
            if (isGameOver)
                DrawFinal(score);
        }

        public void DrawPause()
        {
            DrawText("Pause" , HudHeight, PauseTextColor , paddingHorizontal, 2 * HudHeight + paddingVertical , false);
        }

        private void DrawFinal(int score)
        {
            DrawText("Game\nOver!", FinalTextSize , FinalTextColor , canvas.Width / 2 , canvas.Height / 2 , true);        
            if (isNewScore)
                DrawRecord(score);
            BtnHome.Draw(canvas);
            BtnPlay.Draw(canvas);
        }

        private void DrawRecord(int score)
        {
            DrawText("New High Score!", NewScoreSize , NewScoreTextColor, canvas.Width / 2 , canvas.Height / 2 + textPadding , true);
            DrawText("" + score , NewScoreSize , NewScoreTextNumberColor, canvas.Width / 2 , canvas.Height / 2 + 2 * textPadding, true);

        }
        private void DrawText(string str , float size , Color color , float x , float y , bool center)
        {

            Paint text = new Paint();
            if (center)
               text.TextAlign = Paint.Align.Center;
           
            text.TextSize = size;
            text.Color = color;
            text.SetTypeface(context.Resources.GetFont(Resource.Font.arcade));         

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
            {
                BtnPause.SetState(1);
                isPauseButtonFlipped = true;
            }
            else
            {
                BtnPause.SetState(0);
                isPauseButtonFlipped = false;
            }
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