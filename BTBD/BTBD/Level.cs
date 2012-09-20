using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using System.IO;
using Microsoft.Xna.Framework.Input;
using BTBD.ScreenManager;
using BTBD.GameScreens;
using Microsoft.Xna.Framework.GamerServices;

namespace BTBD
{
     class Level : GameScreen
    {
        private Tile[,] tiles;
        private Texture2D background;
        private GameScreen mainGameScreen;
        private Vector2 start;
        private bool isKilled = false;
        private bool lost = false;
        private bool isBeginning = true;
        private float cameraPosition;

        private Point exit = InvalidPosition;

        // Entities in the level.
        public Bobo Player
        {
            get { return player; }
        }
        Bobo player;
         

        private List<Enemy> enemies = new List<Enemy>();
        private List<PowerUp> powerups = new List<PowerUp>();

        private Random random = new Random(234543234);
        
        private static readonly Point InvalidPosition = new Point(-1, -1);

        //keeps score
        public int Score
        {
            get { return score; }
        }
        int score;

         public int Health
        {
            get { return health; }
            set { health = value; }
        }
        int health = 500;

        // Level content.
        public ContentManager Content
        {
            get { return content; }
        }
        ContentManager content;

        public bool ReachedExit
        {
            get { return reachedExit; }
        }
        bool reachedExit;

        #region Loading
        /// <summary>
        /// Constructs a new level.
        /// </summary>
        /// <param name="serviceProvider">
        /// The service provider that will be used to construct a ContentManager.
        /// </param>
        /// <param name="fileStream">
        /// A stream containing the tile data.
        /// </param>
        public Level(GameScreen screen, IServiceProvider serviceProvider, Stream fileStream)
        {
            // Create a new content manager to load content used just by this level.
            content = new ContentManager(serviceProvider, "Content");
            mainGameScreen = screen;
            background = Content.Load<Texture2D>("background/sunnysky");
            LoadTiles(fileStream);
        }
        /// <summary>
        /// Iterates over every tile in the structure file and loads its
        /// appearance and behavior. This method also validates that the
        /// file is well-formed with a player start point, exit, etc.
        /// </summary>
        /// <param name="fileStream">
        /// A stream containing the tile data.
        /// </param>
        private void LoadTiles(Stream fileStream)
        {
            // Load the level and ensure all of the lines are the same length.
            int width;
            List<string> lines = new List<string>();
            using (StreamReader reader = new StreamReader(fileStream))
            {
                string line = reader.ReadLine();
                width = line.Length;
                while (line != null)
                {
                    lines.Add(line);
                    if (line.Length != width)
                        
                        throw new Exception(String.Format("The length of line {0} is different from all preceeding lines.", lines.Count));
                    line = reader.ReadLine();
                }
            }

            // Allocate the tile grid.
            tiles = new Tile[width, lines.Count];

            // Loop over every tile position,
            for (int y = 0; y < Height; ++y)
            {
                for (int x = 0; x < Width; ++x)
                {
                    // to load each tile.
                    char tileType = lines[y][x];
                    tiles[x, y] = LoadTile(tileType, x, y);
                }

            }
            //if (exit == InvalidPosition)
            //    throw new NotSupportedException("A level must have an exit.");
        }

        private Tile LoadTile(char tileType, int x, int y)
        {
            switch (tileType)
            {
                // Blank space
                case '.':
                    return new Tile(null, TileCollision.Passable);

                // Floating platform
                case '-':
                    return LoadTile("Platform", TileCollision.Platform);

                // Impassable block
                case '#':
                    return LoadVarietyTile("BlockA", 7, TileCollision.Impassable);
                // Load enemys
                case 'Z':
                    return LoadEnemyTile(x, y, "zombie");
                case 'E':
                    return LoadEnemyTile(x, y, "enemy");
                //powerups
                case 'A':
                    return LoadPowerupTile(x, y, "ammo");
                case 'B':
                    return LoadPowerupTile(x, y, "boots");
                // Bobo start point
                case '*':
                    return LoadStartTile(x, y);
                //case 'W':
                //    return LoadExitTile(x, y);
                // Unknown tile type character
                default:
                    throw new NotSupportedException(String.Format("Unsupported tile type character '{0}' at position {1}, {2}.", tileType, x, y));
            }
        }

        /// <summary>
        /// Creates a new tile. The other tile loading methods typically chain to this
        /// method after performing their special logic.
        /// </summary>
        /// <param name="name">
        /// Path to a tile texture relative to the Content/Tiles directory.
        /// </param>
        /// <param name="collision">
        /// The tile collision type for the new tile.
        /// </param>
        /// <returns>The new tile.</returns>
        private Tile LoadTile(string name, TileCollision collision)
        {
            return new Tile(Content.Load<Texture2D>("Tiles/" + name), collision);
        }

        private Tile LoadVarietyTile(string name, int variation, TileCollision collision)
        {
            int index = random.Next(variation);
            // get index on tile to rect dictionary
            return LoadTile(name + index, collision);
        }
        /// <summary>
        /// Instantiates an enemy and puts him in the level.
        /// </summary>
        private Tile LoadEnemyTile(int x, int y, string spriteSet)
        {

            Vector2 position = RectangleExtensions.GetBottomCenter(GetBounds(x, y));
            enemies.Add(new Enemy(this, position, spriteSet));

            return new Tile(null, TileCollision.Passable);
        }

        /// <summary>
        /// Instantiates a gem and puts it in the level.
        /// </summary>
        private Tile LoadPowerupTile(int x, int y, string type)
        {
            Point position = GetBounds(x, y).Center;
            powerups.Add(new PowerUp(this, new Vector2(position.X, position.Y), type));

            return new Tile(null, TileCollision.Passable);
        }
        /// <summary>
        /// Instantiates a player, puts him in the level, and remembers where to put him when he is resurrected.
        /// </summary>
        private Tile LoadStartTile(int x, int y)
        {
            if (Player != null)
                throw new NotSupportedException("A level may only have one starting point.");

            start = RectangleExtensions.GetBottomCenter(GetBounds(x, y));
            player = new Bobo(mainGameScreen, this, start);

            return new Tile(null, TileCollision.Passable);
        }
        /// <summary>
        /// Remembers the location of the level's exit.
        /// </summary>
        //private Tile LoadExitTile(int x, int y)
        //{
        //    if (exit != InvalidPosition)
        //        throw new NotSupportedException("A level may only have one exit.");

        //    exit = GetBounds(x, y).Center;

        //    return LoadTile("wings", TileCollision.Passable);
        //}


        #endregion

        ///// <summary>
        ///// Called when the player reaches the level's exit.
        ///// </summary>
        //private void OnExitReached(GameScreen screen)
        //{
        //    Player.OnReachedExit();
        //    reachedExit = true;
        //    screen.ScreenManager.AddScreen(new VictoryScreen());
            
        //}

        #region Bound/Collisions


        /// <summary>
        /// Width of level measured in tiles.
        /// </summary>
        public int Width
        {
            get { return tiles.GetLength(0); }
        }

        /// <summary>
        /// Height of the level measured in tiles.
        /// </summary>
        public int Height
        {
            get { return tiles.GetLength(1); }
        }

        /// <summary>
        /// Gets the bounding rectangle of a tile in world space.
        /// </summary>        
        public Rectangle GetBounds(int x, int y)
        {
            return new Rectangle(x * Tile.width, y * Tile.height, Tile.width, Tile.height);
        }

        /// <summary>
        /// Gets the collision mode of the tile at a particular location.
        /// This method handles tiles outside of the levels boundries by making it
        /// impossible to escape past the left or right edges, but allowing things
        /// to jump beyond the top of the level and fall off the bottom.
        /// </summary>
        public TileCollision GetCollision(int x, int y)
        {
            // Prevent escaping past the level ends.
            if (x < 0 || x >= Width)
                return TileCollision.Impassable;
            // Allow jumping past the level top and falling through the bottom.
            if (y >= Height || y < 0)
            {
                return TileCollision.Passable;
            }
            
            return tiles[x, y].Collision;
        }

        #endregion

        public void Update(GameScreen screen, GameTime gameTime, KeyboardState keyboardState)
        {
            keyboardState = Keyboard.GetState();
            mainGameScreen = screen;
            if (isKilled == false)
            {
                Player.Update(screen, gameTime, keyboardState, enemies);
                UpdateEnemies(screen, gameTime, player.fireballs);
                UpdatePowers(gameTime);
            }
            if (Player.Position.Y > Height * Tile.height - cameraPosition)
            {
                if (!lost)
                    lost = Lose(null, screen);
            }
            if (ReachedExit)
            {
                // Animate the time being converted into points.
                int seconds = (int)Math.Round(gameTime.ElapsedGameTime.TotalSeconds * 100.0f) - 11;
                score += seconds * 2;
            }
            //// The player has reached the exit if they are standing on the ground and
            //// his bounding rectangle contains the center of the exit tile. They can only
            //// exit when they have collected all of the gems.
            //if (Player.IsAlive &&
            //    Player.IsOnGround &&
            //    Player.BoundingRectangle.Contains(exit))
            //{
            //    OnExitReached(screen);
            //}
        }

        /// <summary>
        /// Animates each gem and checks to allows the player to collect them.
        /// </summary>
        private void UpdatePowers(GameTime gameTime)
        {
            for (int i = 0; i < powerups.Count; ++i)
            {
                PowerUp pu = powerups[i];

                pu.Update(gameTime);

                if (pu.BoundingCircle.Intersects(Player.BoundingRectangle))
                {
                    if (pu.Type == "boots")
                    {
                        OnBootsCollected(Player);
                    }
                    else if (pu.Type == "ammo")
                    {
                        Player.NumFire += 5;
                    }
                    powerups.RemoveAt(i--);
                    
                    
                }
            }
        }

        


        private void OnBootsCollected(Bobo player)
        {
            //score += Gem.PointValue;

            player.OnBoots();
        }
        /// <summary>
        /// Called when the player is killed.
        /// </summary>
        /// <param name="killedBy">
        /// The enemy who killed the player. This is null if the player was not killed by an
        /// enemy, such as when a player falls into a hole.
        /// </param>
        private void OnPlayerKilled(Enemy killedBy)
        {
            Player.OnKilled(killedBy);
        }

        public void AddScore(int x)
        { score += x; }

        /// <summary>
        /// Animates each enemy and allow them to kill the player.
        /// </summary>
        private void UpdateEnemies(GameScreen screen, GameTime gameTime, List<Fireball> fireballs)
        {
            foreach (Enemy enemy in enemies)
            {
                enemy.Update(screen, gameTime, fireballs, player);

                // Touching an enemy instantly hurts the player
                if (enemy.BoundingRectangle.Intersects(Player.BoundingRectangle) && enemy.IsAlive)
                {
                    health -= 10;
                }
                if (health == 0)
                {
                    Lose(enemy, screen);
                }
            }
        }

        private bool Lose(Enemy enemy, GameScreen screen)
        {
            screen.isPaused = true;
            isKilled = true;
            OnPlayerKilled(enemy);
            screen.ScreenManager.AddScreen(new LossScreen());
            return true;
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, GameScreen screen)
        {
            spriteBatch.Begin();
            spriteBatch.Draw(background, Vector2.Zero, null, Color.White, 0.0f, Vector2.Zero, 1.5f, SpriteEffects.None, 0.0f);
            spriteBatch.End();


            float start = (float)(Height * Tile.height / 1.0365); //1x = 2
            Matrix cameraTransform = Matrix.CreateTranslation(0.0f, cameraPosition-start, 0f);
            if (!screen.isPaused)
            {

                if (isBeginning == true || gameTime.TotalGameTime.TotalSeconds < 11 || !player.ScrollON)
                {
                    //cameraTransform = Matrix.CreateTranslation(0.0f, -start, 0.0f);
                    isBeginning = false;

                }
                else
                {
                    ScrollCamera(spriteBatch.GraphicsDevice.Viewport, gameTime);
                    cameraTransform = Matrix.CreateTranslation(0.0f, cameraPosition - start, 0.0f);
                }
            }
            

                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, null, cameraTransform);
                DrawTiles(spriteBatch);
                foreach (PowerUp pu in powerups)
                    pu.Draw(gameTime, spriteBatch);

                Player.Draw(gameTime, spriteBatch);

                // Draw enemys
                foreach (Enemy enemy in enemies)
                {
                    if (enemy.IsAlive == true)
                        enemy.Draw(gameTime, spriteBatch, screen);
                }
                spriteBatch.End();
        }

        private void ScrollCamera(Viewport viewport, GameTime gameTime)
        {
            //const float ViewMargin = 0.35f;

            // Calculate the edges of the screen.
            //float marginWidth = viewport.Width * ViewMargin;
            //float marginLeft = cameraPosition + marginWidth;
            //float marginRight = cameraPosition + viewport.Width - marginWidth;

            // Calculate how far to scroll when the player is near the edges of the screen.
            float cameraMovement = 0.5f; //* (gameTime.TotalGameTime.Seconds - 11);

            //if (Player.Position.X < marginLeft)
            //    cameraMovement = Player.Position.X - marginLeft;
            //else if (Player.Position.X > marginRight)
            //    cameraMovement = Player.Position.X - marginRight;

            // Update the camera position, but prevent scrolling off the ends of the level.
            float maxCameraPosition = Tile.height * Height - viewport.Height;
            cameraPosition = MathHelper.Clamp(cameraPosition + cameraMovement, 0.0f, maxCameraPosition);
        }

        /// <summary>
        /// Draws each tile in the level.
        /// </summary>
        private void DrawTiles(SpriteBatch spriteBatch)
        {
            // Calculate the visible range of tiles. Added a line above and below to make it look cleaner
            int bottom = Height - spriteBatch.GraphicsDevice.Viewport.Height / Tile.height - (int)Math.Ceiling(cameraPosition / Tile.height) - 1;
            int top = bottom + spriteBatch.GraphicsDevice.Viewport.Height / Tile.height + 1;
            top = Math.Min(top, Height - 1);

            if (bottom < 0)
                bottom++;
            // For each tile position
            for (int y = bottom; y <= top; ++y)
            {
                for (int x = 0; x < Width; ++x)
                {
                    // If there is a visible tile in that position
                    Texture2D texture = tiles[x, y].Texture;
                    if (texture != null)
                    {
                        // Draw it in screen space.
                        Vector2 position = new Vector2(x, y) * Tile.Size;
                        spriteBatch.Draw(texture, position, Color.SteelBlue);
                    }
                }
            }
        }

        public void Dispose()
        {
            Content.Unload();
        }
    }

}
