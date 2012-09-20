using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using BTBD.ScreenManager;
using BTBD.GameScreens;

/*********************************************************************************************************
BOBO the Baby Dragon - EECS 395 2D Project
by Shaker Islam, Ross Freiman, Li Jin, and Wenfan Li

 * We got help by using the XNA Platformer Starter Kit, mainly for physics, collisions, and a little game structure.
 * Some help from xnadevelopment.com for shooting projectiles
 * Rest of it completely by us!
 
 *...... It's actually pretty challenging 
 
 NOTE: I've decided not to re-factor this insane code because 1) It is way more than I'd like to put up with, and
 2) The game works, so it works. If I were to continue developing this, I would start from scratch.
 
 ENJOY!

**********************************************************************************************************/
namespace BTBD
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        ScreenManager.ScreenManager screenManager;
        public static int WIDTH = 800;
        public static int HEIGHT = 600;
        

        private KeyboardState keyboardState;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);

            //Setting the Game to look more vertical
            graphics.PreferredBackBufferHeight = HEIGHT;
            graphics.PreferredBackBufferWidth = WIDTH;

            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            screenManager = new ScreenManager.ScreenManager(this);
            Components.Add(screenManager);
            Components.Add(new GamerServicesComponent(this));
            screenManager.AddScreen(new MainMenuScreen());
            AddInitialScreens();
            base.Initialize();
        }

        private void AddInitialScreens()
        {
            // Activate the first screens.
            screenManager.AddScreen(new MainMenuScreen());
        }

        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            // TODO: Add your update logic here
            HandleInput();

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            //graphics.GraphicsDevice.Clear(Color.Black);
            //// TODO: Add your drawing code here
            //spriteBatch.Begin();
            //level.Draw(gameTime, spriteBatch);

            //DrawHud();

            //DrawHealthBar();

            //spriteBatch.End(); 

            base.Draw(gameTime);           
        }

        private void HandleInput()
        {
            // get all of our input states
            keyboardState = Keyboard.GetState();
        }      
    }

}

