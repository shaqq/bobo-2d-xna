using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Input;
using BTBD.GameScreens;

namespace BTBD.ScreenManager
{
    public class ScreenManager : DrawableGameComponent
    {
        List<GameScreen> screens = new List<GameScreen>();
        //container to keep track of which screens have been updated
        List<GameScreen> screensToUpdate = new List<GameScreen>();

        SpriteBatch spriteBatch;
        SpriteFont spriteFont;
        InputState input = new InputState();

        bool isInitialized;

        public SpriteBatch SpriteBatch
        {
            get { return spriteBatch; }
        }

        public SpriteFont SpriteFont
        {
            get { return spriteFont; }
        }

        public ScreenManager(Game game)
            : base(game)
        {
        }

        public override void Initialize()
        {
            base.Initialize();
            isInitialized = true;
        }

        public override void Update(GameTime gameTime)
        {
            input.Update();

            bool otherScreenHasFocus = !Game.IsActive;
            bool coveredByOtherScreen = false;
            screensToUpdate.Clear();
            foreach (GameScreen screen in screens)
            {
                if (!screen.IsActive && !(screen is MainMenuScreen) && screen.bgSong != null)
                    MediaPlayer.Stop();
                screensToUpdate.Add(screen);
            }

            GameScreen s;
            //this is a weird way of doing this, but takes into account if a screen is needed to update twice
            while (screensToUpdate.Count > 0)
            {
                s = screensToUpdate[screensToUpdate.Count - 1];

                screensToUpdate.RemoveAt(screensToUpdate.Count - 1);
                s.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
                if (s.ScreenState == ScreenState.TransitionToOn || s.ScreenState == ScreenState.Active)
                {
                    if (!otherScreenHasFocus)
                    {
                        s.HandleInput(input);
                        otherScreenHasFocus = true;
                    }
                }
            }
        }

        public override void Draw(GameTime gameTime)
        {
            foreach (GameScreen screen in screens)
            {
                if (screen.ScreenState != ScreenState.Hidden)
                {
                    screen.Draw(gameTime);
                }
            }
        }

        public int GetNumberOfScreens()
        {
            return screens.Count;
        }

        public GameScreen[] GetScreens()
        {
            return screens.ToArray();
        }

        protected override void LoadContent()
        {
            ContentManager manager = Game.Content;

            spriteBatch = new SpriteBatch(GraphicsDevice);
            spriteFont = manager.Load<SpriteFont>("Sprites/Fonts/Hud");
            foreach (GameScreen screen in screens)
            {
                screen.LoadContent();
            }
        }

        protected override void UnloadContent()
        {
            foreach (GameScreen screen in screens)
            {
                screen.UnloadContent();
            }
        }


        public void QuitToScreen(GameScreen screen)
        {
            
            int length = screens.Count;
            for (int i = length - 1; i >= 0; --i)
            {
                if (screens[i].bgSong != null)
                    MediaPlayer.Stop();
                screens.Remove(screens[i]);
            }
            length = screensToUpdate.Count;
            UnloadContent();
            for (int i = length - 1; i >= 0; --i)
            {
                screensToUpdate.Remove(screensToUpdate[i]);
            }
            AddScreen(screen);

        }

        public void AddScreen(GameScreen screen)
        {
            screen.IsExiting = false;
            screen.ScreenManager = this;
            if (isInitialized)
            {
                screen.LoadContent();
            }
            screens.Add(screen);

        }

        public void RemoveScreen(GameScreen screen)
        {
            if (isInitialized)
            {
                screen.UnloadContent();
            }
            screens.Remove(screen);
            screensToUpdate.Remove(screen);
        }
    }
}
