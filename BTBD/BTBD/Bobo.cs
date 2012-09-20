using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using BTBD.ScreenManager;
using BTBD.GameScreens;

namespace BTBD
{
    class Bobo : GameScreen
    {
        // Animations
        private Animation idleAnimation;
        private Animation runAnimation;
        private Animation jumpAnimation;
        private Animation dieAnimation;
        private Animation fireAnimation;
        private SpriteEffects flip = SpriteEffects.None;
        private AnimationPlayer sprite;
        private KeyboardState previousKeyboardState;
        
        public Level Level
        {
            get { return level; }
        }
        Level level;
        

        public bool IsAlive
        {
            get { return isAlive; }
        }
        bool isAlive;

        // Physics state
        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }
        Vector2 position;

        private float previousBottom;

        public Vector2 Velocity
        {
            get { return velocity; }
            set { velocity = value; }
        }
        Vector2 velocity;

        // Constants for controlling horizontal movement
        private const float MoveAcceleration = 13000.0f;
        private const float MaxMoveSpeed = 1750.0f;
        private const float GroundDragFactor = 0.48f;
        private const float AirDragFactor = 0.58f;

        // Constants for controlling vertical movement
        private const float MaxJumpTime = 0.9f;
        private const float JumpLaunchVelocity = -3500.0f;
        private const float GravityAcceleration = 2000.0f;
        private const float MaxFallSpeed = 550.0f;
        private const float JumpControlPower = 0.14f;
        private int numberOfJumps = 0;
        private int extraJumps = 0;

        public bool ScrollON{
            get { return scrollON; }
            set { scrollON = value; }
        }
        bool scrollON;


        public float NumFire
        {
            get { return numFireBalls; }
            set { numFireBalls = value; }
        }

        private float numFireBalls = 5f;
        /// <summary>
        /// Gets whether or not the player's feet are on the ground.
        /// </summary>
        public bool IsOnGround
        {
            get { return isOnGround; }
        }
        bool isOnGround;

        /// <summary>
        /// Current user movement input.
        /// </summary>
        private float movement;


        // Jumping state
        private bool isJumping;
        private bool wasJumping;
        private float jumpTime;
        private bool isFiring;

        public bool WasStunned
        {
            get { return wasStunned; }
        }
        private bool wasStunned; 

        List<Fireball> mFireballs = new List<Fireball>();
        public List<Fireball> fireballs
        {
            get { return mFireballs; }
        }

        private Rectangle localBounds;
        /// <summary>
        /// Gets a rectangle which bounds this player in world space.
        /// </summary>
        public Rectangle BoundingRectangle
        {
            get
            {
                int left = (int)Math.Round(Position.X - sprite.Origin.X) + localBounds.X;
                int top = (int)Math.Round(Position.Y - sprite.Origin.Y) + localBounds.Y;

                return new Rectangle(left, top, localBounds.Width, localBounds.Height);
            }
        }

        /// <summary>
        /// Constructors a new player.
        /// </summary>
        public Bobo(GameScreen screen, Level level, Vector2 position)
            
        {
            this.level = level;

            LoadContent(screen);

            Reset(position);
        }

        /// <summary>
        /// Loads the player sprite sheet and sounds.
        /// </summary>
        public void LoadContent(GameScreen screen)
        {
            // Load animated textures.
            //spriteBatch = new SpriteBatch(screen.ScreenManager.GraphicsDevice);

            idleAnimation = new Animation(screen.ScreenManager.Game.Content.Load<Texture2D>("Sprites/Bobo/idle"), 0.1f, true);
            runAnimation = new Animation(screen.ScreenManager.Game.Content.Load<Texture2D>("Sprites/Bobo/running"), 0.1f, true);
            jumpAnimation = new Animation(screen.ScreenManager.Game.Content.Load<Texture2D>("Sprites/Bobo/jumping"), 0.1f, false);
            dieAnimation = new Animation(screen.ScreenManager.Game.Content.Load<Texture2D>("Sprites/Bobo/dead"), 0.1f, false);
            fireAnimation = new Animation(screen.ScreenManager.Game.Content.Load<Texture2D>("Sprites/Bobo/firing"), 0.1f, false);

            // Calculate bounds within texture size.            
            int width = (int)(idleAnimation.FrameWidth * 1);
            int left = (idleAnimation.FrameWidth - width) / 2;
            int height = (int)(idleAnimation.FrameWidth * 1);
            int top = idleAnimation.FrameHeight - height;
            localBounds = new Rectangle(left, top, width, height);

            // Load Fireballs
            foreach (Fireball f in mFireballs)
                f.loadContent(screen);

        }

        /// <summary>
        /// Resets the player to life.
        /// </summary>
        /// <param name="position">The position to come to life at.</param>
        public void Reset(Vector2 position)
        {
            Position = position;
            Velocity = Vector2.Zero;
            isAlive = true;
            sprite.PlayAnimation(idleAnimation);
        }

        /// <summary>
        /// Handles input, performs physics, and animates the player sprite.
        /// </summary>
        /// <remarks>
        /// We pass in all of the input states so that our game is only polling the hardware
        /// once per frame. We also pass the game's orientation because when using the accelerometer,
        /// we need to reverse our motion when the orientation is in the LandscapeRight orientation.
        /// </remarks>
        public void Update(GameScreen screen, GameTime gameTime, KeyboardState keyboardState, List<Enemy> enemies)
        {
            keyboardState = Keyboard.GetState();
            GetInput(keyboardState);

            ApplyPhysics(gameTime);


            if (IsAlive && IsOnGround)
            {
                if (Math.Abs(Velocity.X) - 0.02f > 0)
                {
                    sprite.PlayAnimation(runAnimation);
                }
                else
                {
                    sprite.PlayAnimation(idleAnimation);
                }
            }

            // Clear input.
            movement = 0.0f;
            isJumping = false;
            if (isOnGround)
                numberOfJumps = 0;

            wasStunned = false;

            //Firing component for Bobo
            UpdateFireball(screen, gameTime, keyboardState);
            DoStun(screen, enemies); 

            previousKeyboardState = keyboardState;
            
        }

        private void DoStun(GameScreen screen, List<Enemy> Enemies)
        {
            foreach (Enemy E in Enemies){
                if (this.BoundingRectangle.Intersects(E.BoundingRectangle) && E.IsAlive)
                {
                    sprite.PlayAnimation(dieAnimation);
                    //this.velocity = new Vector2(-200, 8000);
                    wasStunned = true; 
                }
            }
        }

        private void UpdateFireball(GameScreen screen, GameTime gameTime, KeyboardState keyboardState)
        {
            foreach (Fireball f in mFireballs)
                f.Update(gameTime);

            if (keyboardState.IsKeyDown(Keys.Space) == true && previousKeyboardState.IsKeyDown(Keys.Space) == false)
            {
                isFiring = true;
                ShootFireball(screen);
            }
        }

        private void ShootFireball(GameScreen screen)
        {
            if (isFiring && (numFireBalls > 0))
            {
                sprite.PlayAnimation(fireAnimation);
                Fireball f = new Fireball(screen,Level, "");
                //f.loadContent();
                if (flip == SpriteEffects.FlipHorizontally)
                    f.Fire(screen, new Vector2(Position.X + 10, Position.Y - 25),
                        new Vector2(200, 200), new Vector2(1, 0), "");
                else
                    f.Fire(screen, new Vector2(Position.X - 25, Position.Y - 25),
                        new Vector2(200, 200), new Vector2(-1, 0), "");
                mFireballs.Add(f);
                numFireBalls--;
            }
        }

        /// <summary>
        /// Gets player horizontal movement and jump commands from input.
        /// </summary>
        private void GetInput(KeyboardState keyboardState)
        {
            

            // Ignore small movements to prevent running in place.
            if (Math.Abs(movement) < 0.5f)
                movement = 0.0f;

            // If any digital horizontal movement input is found, override the analog movement.
            if (keyboardState.IsKeyDown(Keys.Left) ||
                keyboardState.IsKeyDown(Keys.A))
            {

                movement = -1.0f;
                scrollON = true;
            }
            else if (keyboardState.IsKeyDown(Keys.Right) ||
                     keyboardState.IsKeyDown(Keys.D))
            {
                movement = 1.0f;
                scrollON = true; 
            }

            // Check if the player wants to jump.
            isJumping =
                keyboardState.IsKeyDown(Keys.Up) ||
                keyboardState.IsKeyDown(Keys.W);

            if (isJumping == true)
            {
                scrollON = true; 
            }
            //Check if the player is firing.
            isFiring = keyboardState.IsKeyDown(Keys.Space);

        }

        /// <summary>
        /// Updates the player's velocity and position based on input, gravity, etc.
        /// </summary>
        public void ApplyPhysics(GameTime gameTime)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            Vector2 previousPosition = Position;

            // Base velocity is a combination of horizontal movement control and
            // acceleration downward due to gravity.
            velocity.X += movement * MoveAcceleration * elapsed;
            velocity.Y = MathHelper.Clamp(velocity.Y + GravityAcceleration * elapsed, -MaxFallSpeed, MaxFallSpeed);

            velocity.Y = DoJump(velocity.Y, gameTime);

            // Apply pseudo-drag horizontally.
            if (IsOnGround)
                velocity.X *= GroundDragFactor;
            else
                velocity.X *= AirDragFactor;

            // Prevent the player from running faster than his top speed.            
            velocity.X = MathHelper.Clamp(velocity.X, -MaxMoveSpeed, MaxMoveSpeed);

            // Apply velocity.
            Position += velocity * elapsed;
            Position = new Vector2((float)Math.Round(Position.X), (float)Math.Round(Position.Y));

            // If the player is now colliding with the level, separate them.
            HandleCollisions();

            // If the collision stopped us from moving, reset the velocity to zero.
            if (Position.X == previousPosition.X)
                velocity.X = 0;

            if (Position.Y == previousPosition.Y)
                velocity.Y = 0;
        }

        /// <summary>
        /// Calculates the Y velocity accounting for jumping and
        /// animates accordingly.
        /// </summary>
        /// <remarks>
        /// During the accent of a jump, the Y velocity is completely
        /// overridden by a power curve. During the decent, gravity takes
        /// over. The jump velocity is controlled by the jumpTime field
        /// which measures time into the accent of the current jump.
        /// </remarks>
        /// <param name="velocityY">
        /// The player's current velocity along the Y axis.
        /// </param>
        /// <returns>
        /// A new Y velocity if beginning or continuing a jump.
        /// Otherwise, the existing Y velocity.
        /// </returns>
        private float DoJump(float velocityY, GameTime gameTime)
        {
            // If the player wants to jump
            if (isJumping && previousKeyboardState.IsKeyDown(Keys.Up) == false && previousKeyboardState.IsKeyDown(Keys.W) == false)
            {
                // Begin or continue a jump
                if ((!wasJumping && IsOnGround) || jumpTime > 0.0f)
                {
                    jumpTime += (float)gameTime.ElapsedGameTime.TotalSeconds;

                    sprite.PlayAnimation(jumpAnimation);
                }

                // If we are in the ascent of the jump
                if (0.0f < jumpTime && jumpTime <= MaxJumpTime)
                {
                    // Fully override the vertical velocity with a power curve that gives players more control over the top of the jump
                    velocityY = JumpLaunchVelocity * (1.0f - (float)Math.Pow(jumpTime / MaxJumpTime, JumpControlPower));
                }
                else
                {
                    // Reached the apex of the jump
                    // And now adding the ability for Bobo to jump a lot
                    if (velocityY > -MaxFallSpeed * 0.5f && !wasJumping && numberOfJumps < extraJumps)
                    {
                        velocityY = JumpLaunchVelocity * (0.2f - (float)Math.Pow(jumpTime / MaxJumpTime, JumpControlPower));
                        jumpTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
                        numberOfJumps++;
                    }
                    else
                        jumpTime = 0.0f;
                }
            }
            else
            {
                // Continues not jumping or cancels a jump in progress
                jumpTime = 0.0f;
            }
            wasJumping = isJumping;

            return velocityY;
        }

        /// <summary>
        /// Detects and resolves all collisions between the player and his neighboring
        /// tiles. When a collision is detected, the player is pushed away along one
        /// axis to prevent overlapping. There is some special logic for the Y axis to
        /// handle platforms which behave differently depending on direction of movement.
        /// </summary>
        
        
        private void HandleCollisions()
        {
            
            // Get the player's bounding rectangle and find neighboring tiles.
            Rectangle bounds = BoundingRectangle;
            int leftTile = (int)Math.Floor((float)bounds.Left / Tile.width);
            int rightTile = (int)Math.Ceiling(((float)bounds.Right / Tile.width)) - 1;
            int topTile = (int)Math.Floor((float)bounds.Top / Tile.height);
            int bottomTile = (int)Math.Ceiling(((float)bounds.Bottom / Tile.height)) - 1;

            // Reset flag to search for ground collision.
            isOnGround = false;

            // For each potentially colliding tile,
            for (int y = topTile; y <= bottomTile; ++y)
            {
                for (int x = leftTile; x <= rightTile; ++x)
                {
                    // If this tile is collidable,
                    TileCollision collision = Level.GetCollision(x, y);
                    if (collision != TileCollision.Passable)
                    {
                        // Determine collision depth (with direction) and magnitude.
                        Rectangle tileBounds = Level.GetBounds(x, y);
                        Vector2 depth = RectangleExtensions.GetIntersectionDepth(bounds, tileBounds);
                        if (depth != Vector2.Zero)
                        {
                            float absDepthX = Math.Abs(depth.X);
                            float absDepthY = Math.Abs(depth.Y);

                            // Resolve the collision along the shallow axis.
                            if (absDepthY < absDepthX || collision == TileCollision.Platform)
                            {
                                // If we crossed the top of a tile, we are on the ground.
                                if (previousBottom <= tileBounds.Top)
                                    isOnGround = true;

                                // Ignore platforms, unless we are on the ground.
                                if (collision == TileCollision.Impassable || IsOnGround)
                                {
                                    // Resolve the collision along the Y axis.
                                    Position = new Vector2(Position.X, Position.Y + depth.Y);

                                    // Perform further collisions with the new bounds.
                                    bounds = BoundingRectangle;
                                }
                            }
                            else if (collision == TileCollision.Impassable) // Ignore platforms.
                            {
                                // Resolve the collision along the X axis.
                                Position = new Vector2(Position.X + depth.X, Position.Y);

                                // Perform further collisions with the new bounds.
                                bounds = BoundingRectangle;
                            }
                        }
                    }
                }
            }

            // Save the new bounds bottom.
            previousBottom = bounds.Bottom;
             
        }
        
        /// <summary>
        /// Called when the player has been killed.
        /// </summary>
        /// <param name="killedBy">
        /// The enemy who killed the player. This parameter is null if the player was
        /// not killed by an enemy (fell into a hole).
        /// </param>
        
        public void OnKilled(Enemy killedBy)
        {
            if (killedBy != null)
            { 
                if (killedBy.IsAlive)
                {
                    isAlive = false;
                    sprite.PlayAnimation(dieAnimation);
                    
                }   
            }
        }

        /// <summary>
        /// Called when this player reaches the level's exit.
        /// </summary>
        public void OnReachedExit()
        {
            
        }

        public void OnBoots()
        {
            extraJumps++;
        }

        /// <summary>
        /// Draws the animated player.
        /// </summary>
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            foreach (Fireball f in mFireballs)
                if (!f.isDead)
                    f.Draw(gameTime, spriteBatch);
            // Draw that sprite.
            sprite.Draw(gameTime, spriteBatch, Position, flip);

            // Flip the sprite to face the way we are moving.
            if (Velocity.X > 0)
                flip = SpriteEffects.FlipHorizontally;
            else if (Velocity.X < 0)
                flip = SpriteEffects.None;
        }


    }
    
}
