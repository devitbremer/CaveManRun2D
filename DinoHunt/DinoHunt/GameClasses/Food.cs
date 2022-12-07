using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;

namespace DinoHunt.GameClasses
{
    public class Food
    {
        private Texture2D texture;
        private Vector2 origin;
        private SoundEffect collectedSound;

        public readonly int PointValue = 30;
        public readonly Color Color = Color.White;


        private Vector2 basePosition;
        private float bounce;

        public Level Level{get; private set;}

        public Vector2 Position{ get { return basePosition + new Vector2(0.0f, bounce); } }

        public FoodLimit BoundingCircle{get{return new FoodLimit(Position, Tile.Width / 3.0f);}}

        //Class constructor
        public Food(Level Level, Vector2 position)
        {
            this.Level = Level;
            this.basePosition = position;

            LoadContent();
        }

        public void LoadContent()
        {
            texture = Level.Content.Load<Texture2D>("Food");
            origin = new Vector2(texture.Width / 2.0f, texture.Height / 2.0f);
            collectedSound = Level.Content.Load<SoundEffect>("SoundFX/collectFood");
        }

        public void Update(GameTime gameTime)
        {
            // Bounce control constants
            const float BounceHeight = 0.18f;
            const float BounceRate = 4.0f;
            const float BounceSync = -0.53f;
          
            double t = gameTime.TotalGameTime.TotalSeconds * BounceRate + Position.X * BounceSync;
            bounce = (float)Math.Sin(t) * BounceHeight * texture.Height;
        }


        public void OnCollected(Player collectedBy)
        {
            collectedSound.Play();
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, Position, null, Color, 0.0f, origin, 1.0f, SpriteEffects.None, 0.0f);
        }
    }
}
