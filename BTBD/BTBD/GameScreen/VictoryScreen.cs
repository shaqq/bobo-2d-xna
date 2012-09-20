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
    class VictoryScreen : MenuScreen
    {
        private Texture2D trophy;
        private Vector2 trophyPosition;
        public VictoryScreen()
            : base("VICTORY!")
        {
            MenuItem goToMain = new MenuItem("Return to title screen");
            MenuItem quit = new MenuItem("Quit");

            goToMain.Selected += ReturnToMain;
            quit.Selected += QuitSelected;

            goToMain.Position = new Vector2(Game1.WIDTH / 2 - 50, 600);
            quit.Position = new Vector2(Game1.WIDTH / 2 - 50, 650);
            trophyPosition = new Vector2(Game1.WIDTH / 2 - 50, 200);

            MenuItems.Add(goToMain);
            MenuItems.Add(quit);
        }

        public override void LoadContent()
        {
            trophy = ScreenManager.Game.Content.Load<Texture2D>("Sprites/Victory");
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
            // add other effects here, look in Draw in MainMenuScreen for hints
            GraphicsDevice device = ScreenManager.GraphicsDevice;
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            SpriteFont font = ScreenManager.SpriteFont;
            spriteBatch.Begin();
            for (int i = 0; i < MenuItems.Count; ++i)
            {
                bool isSelected = IsActive && (i == selected);
                MenuItems[i].Draw(this, isSelected, gameTime);
            }
            float transitionOffset = (float)Math.Pow(TransitionPosition, 2);
            Vector2 titlePosition = new Vector2(device.Viewport.Width / 2, 80);
            Vector2 titleOrigin = font.MeasureString("VICTORY!") / 2;
            Color titleColor = new Color(192, 192, 192) * TransitionAlpha;
            float titleScale = 1.25f;

            titlePosition.Y -= transitionOffset * 100;

            spriteBatch.DrawString(font, "VICTORY!", titlePosition, Color.Blue, 0,
                                   titleOrigin, titleScale, SpriteEffects.None, 0);
            spriteBatch.Draw(trophy, trophyPosition, Color.White);
            spriteBatch.End();
        }
    }
}
