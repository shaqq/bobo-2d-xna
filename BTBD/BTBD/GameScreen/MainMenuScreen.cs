using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
//using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using System.IO;
using System.Xml.Serialization;

namespace BTBD.GameScreens
{
    class MainMenuScreen : MenuScreen
    {
        Texture2D logo;
        Vector2 logoPosition;

        public MainMenuScreen()
            : base("Bobo The Baby Dragon")
        {
            TransitionOffTime = TimeSpan.Zero;
            MenuItem play = new MenuItem("Play");
            MenuItem options = new MenuItem("Instructions");
            //MenuItem load = new MenuItem("Load");
            MenuItem quit = new MenuItem("Exit");

            play.Selected += PlayMenuItemSelected;
            options.Selected += InstructionsItemSelected;
            quit.Selected += OnCancel;
            //load.Selected += LoadGameSelected;
            
            play.Position = new Vector2(Game1.WIDTH / 2f - 20, 500);
            options.Position = new Vector2(Game1.WIDTH / 2f - 20, 540);
            //load.Position = new Vector2(Game1.WIDTH / 2f - 20, 580);
            quit.Position = new Vector2(Game1.WIDTH / 2f - 20, 580);
            logoPosition = new Vector2(Game1.WIDTH / 2, 0);
            MenuItems.Add(play);
            MenuItems.Add(options);
            //MenuItems.Add(load);
            MenuItems.Add(quit);
        }

        void InstructionsItemSelected(object sender, EventArgs e)
        {
            this.Exit();
            ScreenManager.AddScreen(new InstructionScreen());
        }

        void PlayMenuItemSelected(object sender, EventArgs e)
        {
            ScreenManager.AddScreen(new MainGameScreen());
        }

        public override void Draw(GameTime gameTime)
        {
            GraphicsDevice device = ScreenManager.GraphicsDevice;
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            SpriteFont font = ScreenManager.SpriteFont;
            Texture2D menuBackground = ScreenManager.Game.Content.Load<Texture2D>("Sprites/MenuBG");
            spriteBatch.Begin();
            spriteBatch.Draw(menuBackground, new Vector2(Game1.WIDTH / 2 - menuBackground.Width / 2, 0), Color.White);

            MenuItem item;
            for (int i = 0; i < MenuItems.Count; ++i)
            {
                item = MenuItems[i];
                bool isSelected = IsActive && (i == selected);
                item.Draw(this, isSelected, gameTime);
            }
            float transitionOffset = (float)Math.Pow(TransitionPosition, 2);

            // Draw the menu title centered on the screen
            Vector2 titlePosition = new Vector2(device.Viewport.Width / 2, 80);
            Vector2 titleOrigin = font.MeasureString(menuTitle) / 2;
            Color titleColor = new Color(255, 255, 255) * TransitionAlpha;
            float titleScale = 1.25f;

            titlePosition.Y -= transitionOffset * 100;

            spriteBatch.DrawString(font, menuTitle, titlePosition, titleColor, 0,
                                   titleOrigin, titleScale, SpriteEffects.None, 0);
            spriteBatch.Draw(logo, new Vector2(logoPosition.X - logo.Width / 2, 100), Color.White);
            spriteBatch.End();
        }


        public override void LoadContent()
        {
            base.LoadContent();
            logo = ScreenManager.Game.Content.Load<Texture2D>("Logo");
            bgSong = this.ScreenManager.Game.Content.Load<Song>("Music/Pixel");
            MediaPlayer.Play(bgSong);
        }
    }
}
