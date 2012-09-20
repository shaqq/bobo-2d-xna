using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.GamerServices;
using BTBD.ScreenManager;

namespace BTBD.GameScreens
{

    public class MainGameScreen : GameScreen
    {
        public override bool isPaused
        {
            get { return paused; }
            set { paused = value; }
        }
        private bool paused;


        public bool isVictory;
        bool songstart = false;
        SpriteBatch spriteBatch;
        GraphicsDevice graphicsDevice;
        KeyboardState keyboardState;

        private Level level;
        private Bobo bobo; 
        private SpriteFont hudFont;
        private Texture2D mHealthBar;

        public override void HandleInput(InputState input)
        {
            if (input.IsPause() && isPaused == false)
            {
                ScreenManager.AddScreen(new PauseScreen(this));
                isPaused = true;
            }

        }

        public void UnpauseEvent(object sender, EventArgs e)
        {
            isPaused = false;
        }

        public override void LoadContent()
        {
            
            this.spriteBatch = new SpriteBatch(this.ScreenManager.GraphicsDevice);
            graphicsDevice = this.ScreenManager.GraphicsDevice;
            //gamePanel.LoadContent(this);
            // Create a new SpriteBatch, which can be used to draw textures.

            //Load fonts
            hudFont = ScreenManager.Game.Content.Load<SpriteFont>("Sprites/Fonts/Hud");

            //Load the HealthBar
            mHealthBar = ScreenManager.Game.Content.Load<Texture2D>("Sprites/Bobo/HealthBar2");

            // TODO: use this.Content to load your game content here
            LoadLevel();

            bgSong = this.ScreenManager.Game.Content.Load<Song>("Music/Pep");
            MediaPlayer.IsRepeating = true; 

            bobo = new Bobo(this,level, new Vector2(200, 400));
            keyboardState = Keyboard.GetState();
        }

        private void LoadLevel()
        {
            // Unloads the content for the current level before loading the next one.
            if (level != null)
                level.Dispose();
            // Find the path of the next level.
            // Load the level.
            string levelPath = "Content/Levels/1.txt";
            using (Stream fileStream = TitleContainer.OpenStream(levelPath))
                level = new Level(this, ScreenManager.Game.Services, fileStream);
        }

        public override void Update(GameTime gameTime, bool hasFocus, bool isCovered)
        {
            if (isPaused == false)
            {
                level.Update(this, gameTime, keyboardState);
                //bobo.Update(this, gameTime, keyboardState);
                
                if (!songstart)
                {
                    MediaPlayer.Play(bgSong);
                    songstart = true;
                } 
            }

        }

        public override void Draw(GameTime gameTime)
        {
            

            level.Draw(gameTime, spriteBatch, this);

            spriteBatch.Begin();

            DrawHud();

            DrawHealthBar();

            spriteBatch.End();

            bobo.Draw(gameTime);

            base.Draw(gameTime);
        }

        private void DrawHealthBar()
        {
            
            spriteBatch.Draw(mHealthBar, new Vector2(2, 3), new Rectangle(0, 45, mHealthBar.Width, 44), Color.Gray, 0.0f, new Vector2(0, 0), 0.33f, SpriteEffects.None, 0);

            
            spriteBatch.Draw(mHealthBar, new Vector2(2,
                  3), new Rectangle(0, 45, (int)(mHealthBar.Width * ((double)level.Health / 500)), 44), Color.Red, 0.0f, new Vector2(0, 0), 0.33f, SpriteEffects.None, 0);
          

            spriteBatch.Draw(mHealthBar, new Vector2(2,
                     3), new Rectangle(0, 0, mHealthBar.Width, 44), Color.White, 0.0f, new Vector2(0.0f, 0.0f), 0.33f, SpriteEffects.None, 0);
        }

        private void DrawHud()
        {
            Rectangle titleSafeArea = graphicsDevice.Viewport.TitleSafeArea;
            Vector2 hudLocation = new Vector2(titleSafeArea.X, titleSafeArea.Y);
            Vector2 center = new Vector2(titleSafeArea.X + titleSafeArea.Width / 2.0f,
                                         titleSafeArea.Y + titleSafeArea.Height / 2.0f);

            //Draw health
            string healthString = "Health:";
            float healthHeight = hudFont.MeasureString(healthString).Y;

            DrawShadowedString(hudFont, "Health: " + level.Health.ToString(), hudLocation + new Vector2(0.0f, healthHeight * 0.8f), Color.Turquoise);

            // Draw score

            DrawShadowedString(hudFont, "SCORE: " + level.Score.ToString(), hudLocation + new Vector2(0.0f, healthHeight * 2.4f), Color.Gold);

            //draw number of fireballs
            DrawShadowedString(hudFont, "Fireballs: " + level.Player.NumFire.ToString(), hudLocation + new Vector2(0.0f, healthHeight * 1.6f), Color.DarkOrange);
        }

        private void DrawShadowedString(SpriteFont font, string value, Vector2 position, Color color)
        {
            spriteBatch.DrawString(font, value, position + new Vector2(1.0f, 1.0f), Color.Black);
            spriteBatch.DrawString(font, value, position, color);
        }

        public override void UnloadContent()
        {
            base.UnloadContent();
            ScreenManager.Game.Content.Unload();
        }
    }
}
