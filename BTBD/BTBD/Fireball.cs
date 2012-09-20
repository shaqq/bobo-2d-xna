using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using BTBD.ScreenManager;
using BTBD.GameScreens;


namespace BTBD
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    class Fireball: GameScreen
    {
        public Fireball(GameScreen screen ,Level level, string fireballname)
        {
            this.level = level;
            LoadContent(screen, fireballname);
        }

        public Level Level
        {
            get { return level; }
        }
        Level level;


        public int Damage
        {
            get { return damage; }
        }
        private int damage;

        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }
        Vector2 position;

        Texture2D sprite;
        private SpriteEffects flip = SpriteEffects.None;
        public bool isDead;
        Vector2 mSpeed;
        Vector2 mDirection;
        private Rectangle localBounds;
        /// <summary>
        /// Gets a rectangle which bounds this enemy in world space.
        /// </summary>
        public Rectangle BoundingRectangle
        {
            get
            {
                int left = (int)Math.Round(Position.X - Origin.X) + localBounds.X;
                int top = (int)Math.Round(Position.Y - Origin.Y) + localBounds.Y;

                return new Rectangle(left, top, localBounds.Width, localBounds.Height);
            }
        }
        
        /// <summary>
        /// Gets a texture origin at the bottom center of each frame.
        /// </summary>
        public Vector2 Origin
        {
            get { return new Vector2(sprite.Width / 2.0f, sprite.Height); }
        }
        public void Fire(GameScreen screen, Vector2 thePosition, Vector2 theSpeed, Vector2 theDirection, string fireballname)
        {
            Position = thePosition;
            mSpeed = theSpeed;
            mDirection = theDirection;
            LoadContent(screen, fireballname);
        }

        public void Die()
        { isDead = true; }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>

        public void loadContent(GameScreen screen)
        { this.LoadContent(); }

        public void LoadContent(GameScreen screen, string fireballname)
        {
            // Calculate bounds within texture size.
            /*int width = (int)(sprite.Width * 0.35);
            int left = (sprite.Width - width) / 2;
            int height = (int)(sprite.Height * 0.7);
            int top = sprite.Height - height;
            localBounds = new Rectangle(0, 0, sprite.Width, sprite.Height);*/
            //------------------------end
            if (fireballname == "enemyfire")
            {
                sprite = Level.Content.Load<Texture2D>("Sprites/Enemy/enemyfire");
            }
            else
            {
                if (mDirection.X < 0)
                    sprite = Level.Content.Load<Texture2D>("Sprites/Bobo/fireball");
                else
                    sprite = Level.Content.Load<Texture2D>("Sprites/Bobo/fireball2");
            }

            localBounds = new Rectangle(0, 0, sprite.Width, sprite.Height);
        }

        /// <summary>
        /// Go right if it's going right, else go left
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public void Update(GameTime gameTime)
        {
            int tileX = (int)Math.Floor(Position.X / Tile.width);
            int tileY = (int)Math.Floor(Position.Y / Tile.height);
            if (Level.GetCollision(tileX, tileY) == TileCollision.Impassable ||
                 Level.GetCollision(tileX, tileY) == TileCollision.Platform)
            {
                this.Die();
            }
            else
                Position += mDirection * mSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            Vector2 offset = new Vector2(sprite.Width / 2, sprite.Height / 2);

            spriteBatch.Draw(sprite, Position, Color.White);
        }
    }
}
