using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using BTBD.ScreenManager;

namespace BTBD.GameScreens
{
    class MenuItem
    {
        string text;
        float selectionFade;
        Vector2 position;

        public string Text
        {
            get { return text; }
            set { text = value; }
        }

        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }



        public event EventHandler Selected;

        protected internal virtual void OnSelectEntry()
        {
            if (Selected != null)
            {
                Selected(this, new EventArgs());
            }
        }

        public MenuItem(string text)
        {
            this.text = text;
        }

        public virtual int GetWidth(MenuScreen screen)
        {
            return screen.ScreenManager.SpriteFont.LineSpacing;
        }

        public virtual int GetHeight(MenuScreen screen)
        {
            return (int)screen.ScreenManager.SpriteFont.MeasureString(Text).X;   
        }

        public virtual void Update(MenuScreen screen, bool isSelected, GameTime gameTime)
        {
            float fadeSpeed = (float)gameTime.ElapsedGameTime.TotalSeconds * 4;

            if (isSelected)
            {
                selectionFade = Math.Min(selectionFade + fadeSpeed, 1);
            }
            else
            {
                selectionFade = Math.Max(selectionFade - fadeSpeed, 0);
            }
        }

        public virtual void Draw(MenuScreen screen, bool isSelected, GameTime gameTime)
        {
            Color color = isSelected ? Color.Red : Color.Black;

            // Pulsate the size of the selected menu entry.
            double time = gameTime.TotalGameTime.TotalSeconds;

            float pulsate = (float)Math.Sin(time * 6) + 1;

            float scale = 1 + pulsate * 0.05f * selectionFade;

            // Modify the alpha to fade text out during transitions.
            color *= screen.TransitionAlpha;

            // Draw text, centered on the middle of each line.
            
            ScreenManager.ScreenManager screenManager = screen.ScreenManager;
            SpriteBatch spriteBatch = screenManager.SpriteBatch;
            SpriteFont font = screenManager.SpriteFont;

            Vector2 origin = new Vector2(0, font.LineSpacing / 2);

            spriteBatch.DrawString(font, text, position, color, 0,
                                   origin, scale, SpriteEffects.None, 0);
        }
    }
}
