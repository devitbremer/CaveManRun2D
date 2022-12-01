using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DinoHunt.Models
{
    public class Animation
    {
        public int CurrentFrame { get; set; }

        public int FrameCount { get; private set; }

        public int FrameHeight { get { return Texture.Height; } }

        public float FrameSpeed { get; set; }

        public int FrameWidth { get { return Texture.Width / FrameCount; } }

        public bool IsLooping { get; set; }

        public Texture2D Texture { get; private set; }

        //Constructor receives texture and frame count.
        //Those parameters will be used by the properties to calculate each frame size and location.
        public Animation(Texture2D texture, int frameCount)
        {
            Texture = texture;

            FrameCount = frameCount;

            IsLooping = true;

            FrameSpeed = 0.08f;
        }
    }
}
