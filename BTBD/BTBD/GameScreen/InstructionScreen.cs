using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BTBD.ScreenManager;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace BTBD.GameScreens
{
    class InstructionScreen : MenuScreen
    {
        private string instructionsString = 

"    1. Use Left, Right to navigate BOBO\n" +
"    2. Up for JUMP\n" +
"    3. BOOTS give you extra jumps!!\n" +
"    4. Use space to shoot fireballs\n" +
"    5. Fireballs are LIMITED!!\n" +
"    6. Pick up AMMO along the way to defeat enemies.\n" +
"    6. Kill monsters to score points!\n" +
"    7. You can Pause your game using P key!\n" +
"    8. Climb quickly!!\n\n";

        private Vector2 instructionsPosition = new Vector2(130, 200);

        public InstructionScreen() : base("Instructions") {
            MenuItem returnItem = new MenuItem("Return to main menu");
            returnItem.Selected += ReturnSelected;
            returnItem.Position = new Vector2(Game1.WIDTH / 3, Game1.HEIGHT  - Game1.HEIGHT / 4);
            MenuItems.Add(returnItem);
        }

        void ReturnSelected(object sender, EventArgs e)
        {
            this.Exit();
            ScreenManager.AddScreen(new MainMenuScreen());
        }

        public override void Draw(GameTime gameTime)
        {
            GraphicsDevice device = ScreenManager.GraphicsDevice;
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            SpriteFont font = ScreenManager.SpriteFont;
            spriteBatch.Begin();
            MenuItems[0].Draw(this, true, gameTime);
            float transitionOffset = (float)Math.Pow(TransitionPosition, 2);
            Vector2 titlePosition = new Vector2(device.Viewport.Width / 2, 130);
            Vector2 titleOrigin = font.MeasureString("Instructions") / 2;
            Color titleColor = new Color(192, 192, 192) * TransitionAlpha;
            float titleScale = 1.25f;

            titlePosition.Y -= transitionOffset * 100;

            spriteBatch.DrawString(font, "Instructions", titlePosition, Color.White, 0,
                                   titleOrigin, titleScale, SpriteEffects.None, 0);
            spriteBatch.DrawString(font, instructionsString, instructionsPosition, Color.White);

            spriteBatch.End();
        }
    }
}
