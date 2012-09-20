using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using BTBD.ScreenManager;
using BTBD.GameScreens;


using System.Collections.Generic;
//test commit
namespace BTBD
{
    enum FaceDirection
    {
        Left = -1,
        Right = 1,
    }
    class Enemy //: DrawableGameComponent
    {
        string enemy;
        Vector2 velocity;
        bool isJumping;
        bool wasJumping;
        private float jumpTime;
        private float waitFire;
        private float movement;
        private float previousBottom;
        private float waitJumpTime;
        private int Health;
        private bool isFiring;
        private const float MaxJumpTime = 0.35f;
        private const float MoveAcceleration = 13000.0f;
        private const float MaxMoveSpeed = 1750.0f;
        private const float GroundDragFactor = 0.48f;
        private const float AirDragFactor = 0.58f;
        private const float JumpLaunchVelocity = -3500.0f;
        private const float GravityAcceleration = 3400.0f;
        private const float MaxFallSpeed = 550.0f;
        private const float JumpControlPower = 0.14f;
        public Vector2 Position
        {
            set { position = value; }
            get { return position; }
        }
        Vector2 position;

        List<Fireball> mFireballs = new List<Fireball>();
        public List<Fireball> Fireballs
        {
            get { return mFireballs; }
        }

        public Level Level
        {
            get { return level; }
        }
        Level level;

        public bool IsOnGround
        {
            get { return isOnGround; }
        }
        bool isOnGround;

        public bool IsAlive
        {
            get { return isAlive; }
        }
        bool isAlive;

        private Rectangle localBounds;
        /// <summary>
        /// Gets a rectangle which bounds this enemy in world space.
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
        // Animations
        private Animation runAnimation;
        private Animation dieAnimation;
        private Animation idleAnimation;
        private AnimationPlayer sprite;
        /// <summary>
        /// The direction this enemy is facing and moving along the X axis.
        /// </summary>
        private FaceDirection direction = FaceDirection.Left;

        /// <summary>
        /// How long this enemy has been waiting before turning around.
        /// </summary>
        private float waitTime = -MaxWaitTime;

        /// <summary>
        /// How long to wait before turning around.
        /// </summary>
        private const float MaxWaitTime = 0.25f;

        /// <summary>
        /// The speed at which this enemy moves along the X axis.
        /// </summary>
        private const float MoveSpeed = 64.0f;

        public Enemy(Level level, Vector2 position, string enemy)
        //    : base(g)
        {
            this.level = level;
            this.enemy = enemy;
            if (enemy == "enemy")
                Health = 0;
            else
                Health = 1;
            LoadContent();
            Reset(position);
        }

        public void Reset(Vector2 position)
        {
            Position = position;
            isAlive = true;
            sprite.PlayAnimation(runAnimation);
        }

        public void LoadContent()
        {
            //spriteBatch = new SpriteBatch(Game.GraphicsDevice);
            // Load animations.
            runAnimation = new Animation(Level.Content.Load<Texture2D>("Sprites/Enemy/" + enemy), 0.1f, true);
            dieAnimation = new Animation(Level.Content.Load<Texture2D>("Sprites/Enemy/die_" + enemy), 0.5f, false);
            idleAnimation = new Animation(Level.Content.Load<Texture2D>("Sprites/Enemy/" + enemy + "-idle"), .1f, true);
            sprite.PlayAnimation(runAnimation);

            // Calculate bounds within texture size.
            int width = (int)(runAnimation.FrameWidth * 0.35);
            int left = (runAnimation.FrameWidth - width) / 2;
            int height = (int)(runAnimation.FrameHeight * 0.8);
            int top = runAnimation.FrameHeight - height;
            localBounds = new Rectangle(0, 0, runAnimation.FrameWidth, runAnimation.FrameHeight/2);
        }
        int flip = 1;
        private float waitDieTime = 0.5f;
        public void Update(GameScreen screen, GameTime gameTime, List<Fireball> fireballs, Bobo player)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (IsAlive)
            {   
                // Calculate tile position based on the side we are walking towards.
                float posX = Position.X + localBounds.Width / 2 * (int)direction;
                int tileX = (int)Math.Floor(posX / Tile.width) - (int)direction;
                int tileY = (int)Math.Floor(Position.Y / Tile.height);
                if (waitTime > 0)
                {
                    // Wait for some amount of time.
                    waitTime = Math.Max(0.0f, waitTime - (float)gameTime.ElapsedGameTime.TotalSeconds);
                    if (waitTime <= 0.0f)
                    {
                        // Then turn around.
                        direction = (FaceDirection)(-(int)direction);
                    }
                }
                else
                {
                    // If we are about to run into a wall or off a cliff, start waiting.
                    if (Level.GetCollision(tileX + (int)direction, tileY - 1) == TileCollision.Impassable ||
                        Level.GetCollision(tileX + (int)direction, tileY) == TileCollision.Passable)
                    {
                        waitTime = MaxWaitTime;
                    }
                    else
                    {
                        if (enemy == "enemy")
                        {
                            // Move in the current direction.
                            Vector2 velocity = new Vector2((int)direction * MoveSpeed * elapsed, 0.0f);
                            position = position + velocity;
                        }
                        else
                        {
                            waitTime = Math.Min(0.0f, waitTime - elapsed);
                            if (-1.3f < waitTime && waitTime < -1.0f)
                            {

                                if (flip == 1)
                                    direction = (FaceDirection)(-(int)direction);
                                flip = 0;
                            }
                            else
                            {
                                if (flip == 0)
                                {
                                    flip = 1;
                                    direction = (FaceDirection)(-(int)direction);
                                }

                                // Move in the current direction.
                                Vector2 velocity = new Vector2((int)direction * MoveSpeed * elapsed, 0.0f);
                                position = position + velocity;
                            }
                        }
                    }
                    foreach (Fireball f in fireballs)
                        if (this.BoundingRectangle.Intersects(f.BoundingRectangle) && !f.isDead)
                        {
                            OnEnemyKilled(f);
                            level.AddScore(100);
                            f.Die();
                        }
                }
                if(enemy == "enemy")
                    UpdateFireball(screen, gameTime, player);
            }
        }

        private void UpdateFireball(GameScreen screen, GameTime gameTime, Bobo player)
        {
            foreach (Fireball f in mFireballs)
            {
                f.Update(gameTime);
                if (player.BoundingRectangle.Intersects(f.BoundingRectangle) && !f.isDead)
                {
                    f.Die();
                    level.Health -= 10;
                }
            }
    
            if (waitFire > 0)
            {
                waitFire = Math.Max(0.0f, waitFire - (float)gameTime.ElapsedGameTime.TotalSeconds);
            }
            else
            {
               if ((((int)direction == 1 && Position.X < player.Position.X)
                   || (int)direction == -1 && Position.X > player.Position.X) 
                   && ((int)Math.Floor(Position.Y / Tile.height) == (int)Math.Floor(player.Position.Y / Tile.height)))
                {
                    isFiring = true;
                    ShootFireball(screen);
                }
               waitFire = 0.8f;
            }
        }

        private void ShootFireball(GameScreen screen)
        {
            if (isFiring)
            {
                sprite.PlayAnimation(runAnimation);
                Fireball f = new Fireball(screen, Level, "enemyfire");
                //f.loadContent();
                if ((int)direction == 1)
                    f.Fire(screen, new Vector2(Position.X + 10, Position.Y - 25),
                        new Vector2(200, 200), new Vector2(1, 0), "enemyfire");
                else
                    f.Fire(screen, new Vector2(Position.X - 25, Position.Y - 25),
                        new Vector2(200, 200), new Vector2(-1, 0), "enemyfire");
                mFireballs.Add(f);
            }
        }

        /// <summary>
        /// Called when the enemy is killed.
        /// </summary>
        /// <param name="killedBy">
        /// The fireball which touch the enemy. This is null if the enemy was not killed by a
        /// fireball
        /// </param>
        private void OnEnemyKilled(Fireball killedBy)
        {
            OnKilled(killedBy);
        }

        /// <summary>
        /// Called when the enemy has been killed.
        /// </summary>
        public void OnKilled(Fireball killedBy)
        {
            if (Health == 0)
            {
                isAlive = false;
                sprite.PlayAnimation(dieAnimation);
            }
            else
                Health--;
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, GameScreen screen)
        {
            // Stop running when the game is paused or before turning around.
            if (screen.isPaused)
            {
                sprite.PlayAnimation(idleAnimation);
            }
            else
            {
                sprite.PlayAnimation(runAnimation);
            }

            // Draw facing the way the enemy is moving.
            if (IsAlive)
            {
                SpriteEffects flip = direction > 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
                sprite.Draw(gameTime, spriteBatch, Position, flip);
                    foreach (Fireball f in mFireballs)
                        if (!f.isDead)
                            f.Draw(gameTime, spriteBatch);
            }
            else
            if (waitDieTime > 0)
            {
                // Wait for some amount of time.
                waitDieTime = Math.Max(0.0f, waitDieTime - (float)gameTime.ElapsedGameTime.TotalSeconds);
                SpriteEffects flip = direction > 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
                sprite.Draw(gameTime, spriteBatch, Position, flip);
            }
             
        }
    }
}
