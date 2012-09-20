using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;

namespace BTBD.GameScreens
{
    class LossScreen : MenuScreen
    {
        private Texture2D lossGraphic;
        private Vector2 lossPosition;
        public LossScreen()
            : base("You Lose")
        {
            MenuItem goToMain = new MenuItem("Return to title screen");
            MenuItem quit = new MenuItem("Quit");
            goToMain.Selected += ReturnToMain;
            quit.Selected += QuitSelected;

            goToMain.Position = new Vector2(Game1.WIDTH / 2, Game1.HEIGHT / 2);
            quit.Position = new Vector2(Game1.WIDTH / 2, Game1.HEIGHT / 2 + 50);

            MenuItems.Add(goToMain);
            MenuItems.Add(quit);
        }

        public override void LoadContent()
        {
            //lossGraphic = ScreenManager.Game.Content.Load<Texture2D>("Sprites/Loss");
        }

        void QuitSelected(object sender, EventArgs e)
        {
            ScreenManager.Game.Exit();
        }

        void ReturnToMain(object sender, EventArgs e)
        {
            ScreenManager.QuitToScreen(new MainMenuScreen());
        }

        public override void Draw(GameTime gameTime)
        {
            GraphicsDevice device = ScreenManager.GraphicsDevice;
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            SpriteFont font = ScreenManager.SpriteFont;
            Texture2D menuBackground = ScreenManager.Game.Content.Load<Texture2D>("Sprites/MenuBG");
            spriteBatch.Begin();
            spriteBatch.Draw(menuBackground, new Vector2(Game1.WIDTH / 2 - menuBackground.Width / 2, 0), Color.White);

            for (int i = 0; i < MenuItems.Count; ++i)
            {
                bool isSelected = IsActive && (i == selected);
                MenuItems[i].Draw(this, isSelected, gameTime);
            }
            float transitionOffset = (float)Math.Pow(TransitionPosition, 2);
            Vector2 titlePosition = new Vector2(device.Viewport.Width / 2, 100);
            Vector2 titleOrigin = font.MeasureString("GAME OVER") / 2;
            Color titleColor = new Color(192, 192, 192) * TransitionAlpha;
            float titleScale = 2f;

            titlePosition.Y -= transitionOffset * 100;

            spriteBatch.DrawString(font, "GAME OVER", titlePosition, Color.Red, 0,
                                   titleOrigin, titleScale, SpriteEffects.None, 0);
            spriteBatch.End();
        }
    }
}
