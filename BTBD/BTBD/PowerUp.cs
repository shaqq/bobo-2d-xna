#region File Description
//-----------------------------------------------------------------------------
// Gem.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;

namespace BTBD
{
    /// <summary>
    /// A valuable item the player can collect.
    /// </summary>
    class PowerUp
    {
        private Texture2D texture;
        private Vector2 position;
        //private SoundEffect collectedSound;
        public string Type
        {
            get {return type;}
        }
        private string type;

        // The gem is animated from a base position along the Y axis.
        private Vector2 basePosition;
        private float bounce;

        public Level Level
        {
            get { return level; }
        }
        Level level;

        /// <summary>
        /// Gets the current position of this gem in world space.
        /// </summary>
        public Vector2 Position
        {
            get
            {
                if (type == "boots")
                {
                    return basePosition + new Vector2(bounce, 0f);
                }
                else
                {
                    return basePosition + new Vector2(0f, bounce);
                }
            }
        }

        /// <summary>
        /// Gets a circle which bounds this gem in world space.
        /// </summary>
        public Circle BoundingCircle
        {
            get
            {
                return new Circle(Position, Tile.width / 3.0f);
            }
        }

        /// <summary>
        /// Constructs a new gem.
        /// </summary>
        public PowerUp(Level level, Vector2 position, string type)
        {
            this.level = level;
            this.basePosition = position;
            this.type = type;

            LoadContent();
        }

        /// <summary>
        /// Loads the gem texture and collected sound.
        /// </summary>
        public void LoadContent()
        {
            texture = Level.Content.Load<Texture2D>("Sprites/Powerups/" + type);
            position = new Vector2(texture.Width / 2.0f, texture.Height / 2.0f);
            //collectedSound = Level.Content.Load<SoundEffect>("Sounds/GemCollected");
        }

        /// <summary>
        /// Bounces up and down in the air to entice players to collect them.
        /// </summary>
        public void Update(GameTime gameTime)
        {
            // Bounce control constants
            const float BounceHeight = 0.18f;
            const float BounceRate = 3.0f;
            const float BounceSync = -0.75f;
            double t;
            // Bounce along a sine curve over time.
            // Include the X coordinate so that neighboring gems bounce in a nice wave pattern.
            if (type == "boots")
            {
                t = gameTime.TotalGameTime.TotalSeconds * BounceRate + Position.Y * BounceSync;
            }
            else
            {
                t = gameTime.TotalGameTime.TotalSeconds * BounceRate + Position.X * BounceSync;
            }
            bounce = (float)Math.Sin(t) * BounceHeight * texture.Height;
        }

        /// <summary>
        /// Called when this gem has been collected by a player and removed from the level.
        /// </summary>
        /// <param name="collectedBy">
        /// The player who collected this gem. Although currently not used, this parameter would be
        /// useful for creating special powerup gems. For example, a gem could make the player invincible.
        /// </param>
        public void OnCollected(Bobo collectedBy)
        {
            //collectedSound.Play();
        }

        /// <summary>
        /// Draws a gem in the appropriate color.
        /// </summary>
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, Position, null, Color.White, 0.0f, position, 1.0f, SpriteEffects.None, 0.0f);
        }
    }
}
