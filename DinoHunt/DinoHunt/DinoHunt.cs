
using DinoHunt.Models;
using DinoHunt.Sprites;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using System;
using System.Collections.Generic;

namespace DinoHunt
{
    public class DinoHunt : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        //Player
        private Player _player;


        public DinoHunt()
        {
            _graphics = new GraphicsDeviceManager(this);
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

            //var texture = Content.Load<Texture2D>("Sprites/Player/idle");
            //Sets up animations
            var animations = new Dictionary<string, Animation>()
            {
                { "idle", new Animation(Content.Load<Texture2D>("Player/idle"), 7 ) },
                { "walkLeft", new Animation(Content.Load<Texture2D>("Player/walkLeft"), 7 ) },
                { "walkRight", new Animation(Content.Load<Texture2D>("Player/walkRight"), 7 ) },
                { "jumpLeft", new Animation(Content.Load<Texture2D>("Player/jumpLeft"), 7 ) },
                { "jumpRight", new Animation(Content.Load<Texture2D>("Player/jumpRight"), 7 ) },


            };

            //Sets up player
            _player = new Player(animations);
            _player.Position = new Vector2(200, 200);
            _player.Input = new Inputs() {Up = Keys.W, Down = Keys.S, Left = Keys.A, Right = Keys.D };


        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            _player.Update(gameTime, _player);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            _spriteBatch.Begin();

            _player.Draw(_spriteBatch);

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}