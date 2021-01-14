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
        // Canvas Variables (To be scalled)
        private static float paddingHorizontal = 100;
        private static float paddingVertical = 50;
        private static float hudHeight = 125;
        private static float StreakTextSize = 125;
        private static float FinalTextSize = 125;
        private static float NewScoreTextSize = 75;
        private static float btnsPadding = 100;
        private static float textPadding = 150;

        private static int BlurAmount = 35;
        private static Color InGameScoreTextColor = Color.Rgb(255, 255, 255);
        private static Color FinalTextColor = Color.Rgb(255, 255, 0);
        private static Color NewScoreTextColor = Color.Rgb(0 , 255 , 255);
        private static Color NewScoreTextNumberColor = Color.Rgb(0, 255, 0);
        private static Color PauseTextColor = Color.Rgb(255 ,255, 0);
        private static Color RampStreakColor = Color.Rgb(0, 255, 255);
        private static Color MonsterStreakColor = Color.Rgb(0, 255, 0);

        private Context context;
        private bool isPauseButtonFlipped;
        private bool isGameOver, isNewScore;
        public Canvas canvas { get; set; }
        public CanvasButton BtnPause { get; set; }
        public CanvasButton BtnHome { get; set; }
        public CanvasButton BtnPlay { get; set; }

        public static void InitCanvasVariables()
        {
            paddingHorizontal = CanvasScaler.GetFloat(paddingHorizontal , CanvasScaler.Dim.width);
            paddingVertical = CanvasScaler.GetFloat(paddingVertical, CanvasScaler.Dim.height);
            hudHeight = CanvasScaler.GetFloat(hudHeight, CanvasScaler.Dim.height);
            StreakTextSize = CanvasScaler.GetFloat(StreakTextSize, CanvasScaler.Dim.width);
            FinalTextSize = CanvasScaler.GetFloat(FinalTextSize, CanvasScaler.Dim.width);
            NewScoreTextSize = CanvasScaler.GetFloat(NewScoreTextSize, CanvasScaler.Dim.width);
            btnsPadding = CanvasScaler.GetFloat(btnsPadding, CanvasScaler.Dim.width);
            textPadding = CanvasScaler.GetFloat(textPadding, CanvasScaler.Dim.width);
        }

        public Hud(Context context , Canvas canvas)
        {
            this.context = context;
            this.canvas = canvas;

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

        public void DrawHUD(Boardgame game)
        {
            DrawText("" + game.GetCurrentScore() , hudHeight, InGameScoreTextColor, paddingHorizontal, hudHeight + paddingVertical , false);
            BtnPause.Draw(canvas);
            if (game.timer.RampStreak > 1)
                DrawRampStreak(game.timer.RampStreak);
            if (game.timer.MonsterStreak > 1)
                DrawMonsterStreak(game.timer.MonsterStreak);
            if (isGameOver)
                DrawFinal(game.GetCurrentScore());

        }

        public void DrawPause()
        {
            DrawText("Pause", hudHeight, PauseTextColor, paddingHorizontal, 2 * hudHeight + paddingVertical, false);
        }

        private void DrawRampStreak(int streak)
        {

            DrawText(String.Format("{0}X", streak), StreakTextSize , RampStreakColor, paddingHorizontal , 3 * hudHeight + paddingVertical, false);
        }
        private void DrawMonsterStreak(int streak)
        {
            DrawText(String.Format("{0}X", streak), StreakTextSize, MonsterStreakColor, 5 * paddingHorizontal, 3 * hudHeight + paddingVertical, false);
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
            DrawText("New High Score!", NewScoreTextSize , NewScoreTextColor, canvas.Width / 2 , canvas.Height / 2 + textPadding , true);
            DrawText("" + score , NewScoreTextSize , NewScoreTextNumberColor, canvas.Width / 2 , canvas.Height / 2 + 2 * textPadding, true);

        }
        private void DrawText(string str , float size , Color color , float x , float y , bool center)
        {

            Paint text = new Paint();
            if (center)
               text.TextAlign = Paint.Align.Center;
           
            text.TextSize = size;
            text.Color = color;
            text.SetTypeface(context.Resources.GetFont(Resource.Font.arcade));         

           // Paint paintGlow = new Paint();
           //  paintGlow.Set(text);
           //  paintGlow.SetMaskFilter(new BlurMaskFilter(BlurAmount , BlurMaskFilter.Blur.Normal));


            canvas.DrawText(str , x, y, text);
            canvas.DrawText(str , x, y, Operations.PaintGlow(text , BlurAmount));
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