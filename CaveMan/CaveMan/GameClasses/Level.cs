using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;



namespace DinoHunt.GameClasses
{
    public class Level: IDisposable
    {
        // Physical structure of the Level.
        private Tile[,] tiles;
        private Texture2D[] staticLayers;
        private const int EntityLayer = 2;
        private Vector2 start;
        private Point exit = InvalidPosition;
        private static readonly Point InvalidPosition = new Point(-1, -1);


        //Level objects
        public Player Player { get; private set; }
        public List<Food> food = new List<Food>();

        //Level Song
        private Song BackgroundSong;


        //Level configuration
        private Random randomNumber = new Random(354668);

        public int PlayerPoints { get; private set; }

        public bool ReachedExit { get; private set; }

        public TimeSpan TimeLeft { get; private set; }

        private const int PointsPerSecond = 5;

        public int Width { get { return tiles.GetLength(0); } }
        public int Height { get { return tiles.GetLength(1); } }

        // Level content.        
        public ContentManager Content { get; private set; }


        //Level constructor
        public Level(IServiceProvider serviceProvider, Stream fileStream, int levelIndex)
        {

            Content = new ContentManager(serviceProvider, "Content");

            TimeLeft = TimeSpan.FromMinutes(1.0);

            GenerateTilesArray(fileStream);

            //Dynamically generates static background based on Level.
            staticLayers = new Texture2D[4];
            for (int i = 0; i < staticLayers.Length; ++i)
            {
                int segmentIndex = levelIndex;
                staticLayers[i] = Content.Load<Texture2D>("Backgrounds/Layer" + i + "_" + segmentIndex);
            }


            BackgroundSong = Content.Load<Song>("Music/theme-"+ levelIndex);
            
            MediaPlayer.Volume= 0.2f;
            MediaPlayer.IsRepeating= true;
            MediaPlayer.Play(BackgroundSong);
        }


        //Reads TXT file to build the Level tiles array.
        private void GenerateTilesArray(Stream fileStream)
        {

            int width;
            List<string> fileLines = new List<string>();
            using (StreamReader reader = new StreamReader(fileStream))
            {
                string fileLine = reader.ReadLine();
                width = fileLine.Length;
                while (fileLine != null)
                {
                    fileLines.Add(fileLine);
                    if (fileLine.Length != width)
                    {
                        throw new Exception(String.Format("Lines width dont match", fileLines.Count));
                    }
                        
                    fileLine = reader.ReadLine();
                }
            }

            // Allocate the tile array.
            tiles = new Tile[width, fileLines.Count];

            // Loop over every tile position,
            for (int y = 0; y < Height; ++y)
            {
                for (int x = 0; x < Width; ++x)
                {
                    // to load each tile.
                    char tileType = fileLines[y][x];
                    tiles[x, y] = LoadTexturesInTileArray(tileType, x, y);
                }
            }

            // Verify that the Level has a beginning and an end.
            if (Player == null)
                throw new NotSupportedException("Cant find a starting point");
            if (exit == InvalidPosition)
                throw new NotSupportedException("Cant find an exit point");

        }

        private Tile LoadTexturesInTileArray(char tileType, int x, int y)
        {
            switch (tileType)
            {
                // Blank space
                case '.':
                    return new Tile(null, TileCollision.Passable);


                case '1':
                    return LoadLevelStart(x, y);


                case 'X':
                    return LoadLevelEnd(x, y);


                case 'F':
                    return LoadFood(x, y);

                case '-':
                    return LoadPlatform("Platform2", TileCollision.Platform);


                case '~':
                    return LoadDecoration("Grass", 1, TileCollision.Passable);

                case ':':
                    return LoadDecoration("Flower", 0, TileCollision.Passable);

                case '#':
                    return LoadDecoration("BlockGround", 1, TileCollision.Impassable);

                default:
                    throw new NotSupportedException(String.Format("Review Level file: Unsupported tile type '{0}' at position {1}, {2}.", tileType, x, y));
            }
        }

        private Tile LoadPlatform(string name, TileCollision collision)
        {
            return new Tile(Content.Load<Texture2D>("Tiles/" + name), collision);
        }

        private Tile LoadDecoration(string baseName, int variationCount, TileCollision collision)
        {
            int index = randomNumber.Next(variationCount);
            return LoadPlatform(baseName + index, collision);
        }

        private Tile LoadLevelStart(int x, int y)
        {
            if (Player != null)
                throw new NotSupportedException("A Level may only have one starting point.");

            start = RectangleTools.GetBottomCenter(CreateBounds(x, y));
            Player = new Player(this, start);

            return new Tile(null, TileCollision.Passable);
        }

        private Tile LoadLevelEnd(int x, int y)
        {
            if (exit != InvalidPosition)
                throw new NotSupportedException("A Level may only have one exit.");

            exit = CreateBounds(x, y).Center;

            return LoadPlatform("Exit", TileCollision.Passable);
        }

        private Tile LoadFood(int x, int y)
        {
            Point position = CreateBounds(x, y).Center;
            food.Add(new Food(this, new Vector2(position.X, position.Y)));

            return new Tile(null, TileCollision.Passable);

        }

        public void Dispose()
        {
            Content.Unload();
        }

        public TileCollision GetCollision(int x, int y)
        {
            // Locks player on screen.
            if (x < 0 || x >= Width)
                return TileCollision.Impassable;
            if (y < 0 || y >= Height)
                return TileCollision.Impassable;


            //Get collision from tiles array.
            return tiles[x, y].Collision;
        }

        //Creates a rectangle on each tile to generate collisions.
        public Rectangle CreateBounds(int x, int y)
        {
            return new Rectangle(x * Tile.Width, y * Tile.Height, Tile.Width, Tile.Height);
        }

        public void Update(GameTime gameTime, KeyboardState keyboardState, GamePadState gamePadState)
        {

            if (TimeLeft == TimeSpan.Zero)
            {
                Player.ApplyPhysics(gameTime);
            }
            else if (ReachedExit && food.Count == 0)
            {
                // Animate the time being converted into points.
                int seconds = (int)Math.Round(gameTime.ElapsedGameTime.TotalSeconds * 100.0f);
                seconds = Math.Min(seconds, (int)Math.Ceiling(TimeLeft.TotalSeconds));
                TimeLeft -= TimeSpan.FromSeconds(seconds);
                PlayerPoints += seconds * PointsPerSecond;
            }
            else
            {
                TimeLeft -= gameTime.ElapsedGameTime;
                Player.Update(gameTime, keyboardState, gamePadState);
                UpdateFoodList(gameTime);


                if (Player.IsOnGround &&  Player.BoundingRectangle.Contains(exit) && food.Count == 0)
                {
                    OnExitReached();
                }
            }

            // Clamp the time remaining at zero.
            if (TimeLeft < TimeSpan.Zero)
                TimeLeft = TimeSpan.Zero;
        }


        private void UpdateFoodList(GameTime gameTime)
        {
            for (int i = 0; i < food.Count; ++i)
            {
                Food foodItem = food[i];

                foodItem.Update(gameTime);

                if (foodItem.BoundingCircle.Intersects(Player.BoundingRectangle))
                {
                    food.RemoveAt(i--);
                    OnFoodCollected(foodItem, Player);
                }
            }
        }

        private void OnFoodCollected(Food food, Player collectedBy)
        {
            PlayerPoints += food.PointValue;

            food.OnCollected(collectedBy);
        }

        private void OnExitReached()
        {
            Player.OnReachedExit();
            ReachedExit = true;
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            for (int i = 0; i <= EntityLayer; ++i)
            {
                spriteBatch.Draw(staticLayers[i], Vector2.Zero, Color.White);
            }

            

            foreach (Food foodItem in food)
            {
                foodItem.Draw(gameTime, spriteBatch);
            }
                

            

            for (int i = EntityLayer + 1; i < staticLayers.Length; ++i)
            {
                spriteBatch.Draw(staticLayers[i], Vector2.Zero, Color.White);
            }
                

            
            //DrawTiles
            for (int y = 0; y < Height; ++y)
            {
                for (int x = 0; x < Width; ++x)
                {
                    // If there is a visible tile in that position
                    Texture2D texture = tiles[x, y].Texture;
                    if (texture != null)
                    {
                        // Draw it in screen space.
                        Vector2 position = new Vector2(x, y) * Tile.Size;
                        spriteBatch.Draw(texture, position, Color.White);
                    }
                }
            }

            Player.Draw(gameTime, spriteBatch); 

            
        }



    }
}
