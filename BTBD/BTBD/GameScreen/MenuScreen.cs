using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BTBD.ScreenManager;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Input;

namespace BTBD.GameScreens
{
    abstract class MenuScreen : GameScreen
    {
        protected List<MenuItem> menuItems = new List<MenuItem>();
        protected int selected = 0;
        protected string menuTitle;
        

        protected IList<MenuItem> MenuItems
        {
            get { return menuItems; }
        }

        public MenuScreen(string menuTitle)
        {
            this.menuTitle = menuTitle;

            TransitionOffTime = TimeSpan.FromSeconds(0.5);
            TransitionOnTime = TimeSpan.FromSeconds(0.5);
        }

        public override void HandleInput(InputState input)
        {
            if (input.IsMenuDown())
            {
                selected++;
                if (selected >= menuItems.Count)
                {
                    selected = 0;
                }
            }
            if (input.IsMenuUp())
            {
                selected--;
                if (selected < 0)
                {
                    selected = menuItems.Count - 1;
                }
            }
            if (input.IsMenuSelect())
            {
                OnSelectEntry(selected);
            }
            if (input.IsMenuCancel())
            {
                OnCancel();
            }
        }

        protected virtual void OnSelectEntry(int index)
        {
            menuItems[index].OnSelectEntry();
            if (this.bgSong != null)
                MediaPlayer.Stop();
        }

        protected virtual void OnCancel()
        {
            Exit();
            ScreenManager.Game.Exit();
        }


        protected void OnCancel(object sender, EventArgs e)
        {
            OnCancel();
        }

        public override void Update(GameTime gameTime, bool hasFocus, bool isCovered)
        {
            for (int i = 0; i < menuItems.Count; ++i)
            {
                bool isSelected = IsActive && (i == selected);
                menuItems[i].Update(this, isSelected, gameTime);
            }
                base.Update(gameTime, hasFocus, isCovered);
        }

        public override void Draw(GameTime gameTime)
        {
            GraphicsDevice device = ScreenManager.GraphicsDevice;
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            SpriteFont font = ScreenManager.SpriteFont;
            Texture2D menuBackground = ScreenManager.Game.Content.Load<Texture2D>("Sprites/MenuBG");
            spriteBatch.Begin();
            spriteBatch.Draw(menuBackground, new Vector2(Game1.WIDTH / 2 - menuBackground.Width / 2, 0), Color.White);

            for (int i = 0; i < menuItems.Count; ++i)
            {
                MenuItem item = menuItems[i];
                item.Position = new Vector2(Game1.WIDTH / 2 - 50, 200 + 50 * i);
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
            spriteBatch.End();
        }
    }
}
