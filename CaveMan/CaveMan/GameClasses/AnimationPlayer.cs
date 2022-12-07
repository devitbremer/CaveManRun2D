using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DinoHunt.GameClasses
{
    struct AnimationPlayer
    {

        public Animation Animation { get; set; }

        public int FrameIndex { get; set; }

        private float time;

        public Vector2 Origin{ get { return new Vector2(Animation.FrameWidth / 2.0f, Animation.FrameHeight); } }


        public void PlayAnimation(Animation animation)
        {
            // If this animation is already running, do not restart it.
            if (Animation == animation)
                return;

            // Start the new animation.
            this.Animation = animation;
            this.FrameIndex = 0;
            this.time = 0.0f;
        }


        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, Vector2 position, SpriteEffects spriteEffects)
        {
            if (Animation == null)
                throw new NotSupportedException("No animation is currently playing.");

            // Process passing time.
            time += (float)gameTime.ElapsedGameTime.TotalSeconds;
            while (time > Animation.FrameTime)
            {
                time -= Animation.FrameTime;

                // Advance the frame index; looping or clamping as appropriate.
                if (Animation.IsLooping)
                {
                    FrameIndex = (FrameIndex + 1) % Animation.FrameCount;
                }
                else
                {
                    FrameIndex = Math.Min(FrameIndex + 1, Animation.FrameCount - 1);
                }
            }

            // Calculate the source rectangle of the current frame.
            Rectangle source = new Rectangle(FrameIndex * Animation.Texture.Height, 0, Animation.Texture.Height, Animation.Texture.Height);

            // Draw the current frame.
            spriteBatch.Draw(Animation.Texture, position, source, Color.White, 0.0f, Origin, 1.0f, spriteEffects, 0.0f);
        }
    }
}
