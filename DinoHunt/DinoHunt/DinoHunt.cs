
using DinoHunt.GameClasses;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.IO;

namespace DinoHunt
{
    public class DinoHunt : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        // Global content.
        private SpriteFont GameFont;
        private Texture2D WinScreen;
        private Texture2D LoseScreen;
        

        // Meta-Level game state.
        private int levelIndex = -1;
        private Level level;
        private bool ContinueKeyPressed;

        //Triggers color alert that time is ending.
        private static readonly TimeSpan WarningTime = TimeSpan.FromSeconds(30);

        private GamePadState gamePadState;
        private KeyboardState keyboardState;

        private const int numberOfLevels = 3;

        public DinoHunt()
        {
            _graphics = new GraphicsDeviceManager(this);
            _graphics.IsFullScreen = false;
            Content.RootDirectory = "Content";
            IsMouseVisible = false;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            GameFont = Content.Load<SpriteFont>("Fonts/RockFont");

            WinScreen = Content.Load<Texture2D>("Screens/Win");
            LoseScreen = Content.Load<Texture2D>("Screens/Lose");

            LoadNextLevel();

        }


        protected override void Update(GameTime gameTime)
        {

            HandleInput(gameTime);

            //Level has the core game functionality (Player / Tile / Food / Animation)
            level.Update(gameTime, keyboardState, gamePadState);


            base.Update(gameTime);
        }

        private void HandleInput(GameTime gameTime)
        {
            keyboardState = Keyboard.GetState();
            gamePadState = GamePad.GetState(PlayerIndex.One);

            bool goToNextLevel =  keyboardState.IsKeyDown(Keys.Space) || gamePadState.IsButtonDown(Buttons.A);

            if (!ContinueKeyPressed && goToNextLevel)
            {
                if (level.TimeLeft == TimeSpan.Zero)
                {
                    if (level.ReachedExit)
                        LoadNextLevel();
                    else
                        ReloadCurrentLevel();
                }
            }

            ContinueKeyPressed = goToNextLevel;


        }
        private void LoadNextLevel()
        {
            // move to the next Level
            levelIndex = (levelIndex + 1) % numberOfLevels;

            // Unloads the content for the current Level before loading the next one.
            if (level != null)
                level.Dispose();

            // Load the Level.
            string levelPath = string.Format("Content/Levels/{0}.txt", levelIndex);
            using (Stream fileStream = TitleContainer.OpenStream(levelPath))
                level = new Level(Services, fileStream, levelIndex);
        }

        private void ReloadCurrentLevel()
        {
            --levelIndex;
            LoadNextLevel();
        }
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            _spriteBatch.Begin();

            level.Draw(gameTime, _spriteBatch);

            DrawGameStatus();

            _spriteBatch.End();

            base.Draw(gameTime);
        }

        private void DrawGameStatus()
        {
            //Local Variables
            Rectangle titleArea = GraphicsDevice.Viewport.TitleSafeArea;
            Vector2 textLocation = new Vector2(titleArea.X+5, titleArea.Y+5);
            Vector2 screenCenter = new Vector2(_graphics.PreferredBackBufferWidth / 2, _graphics.PreferredBackBufferHeight / 2);


            //Sets up the timer
            string timerText = "TIME: " + level.TimeLeft.Minutes.ToString("00") + ":" + level.TimeLeft.Seconds.ToString("00");
            Color timerColor;


            if (level.TimeLeft > WarningTime || level.ReachedExit || (int)level.TimeLeft.TotalSeconds % 2 == 0)
            {
                timerColor = Color.White;
            }
            else
            {
                timerColor = Color.Red;
            }

            //Draws timer
            _spriteBatch.DrawString(GameFont, timerText, textLocation + new Vector2(1.0f, 1.0f), timerColor);


            // Draws points
            float timerHeight = GameFont.MeasureString(timerText).Y;

            _spriteBatch.DrawString(GameFont, "SCORE: " + level.PlayerPoints.ToString(), new Vector2(5f, timerHeight * 1.2f), Color.White);


            //Check game status to show message.
            Texture2D status = null;

            //Verifiy game timer
            if (level.TimeLeft == TimeSpan.Zero)
            {
                //Check if all items were collected and player is the exit point
                if (level.ReachedExit && level.food.Count == 0)
                {
                    status = WinScreen;
                }
                else
                {
                    status = LoseScreen;
                }
            }

            if (status != null)
            {
                // Draw status message.
                Vector2 statusSize = new Vector2(status.Width, status.Height);
                _spriteBatch.Draw(status, screenCenter - statusSize / 2, Color.White);
            }
        }
    }
}