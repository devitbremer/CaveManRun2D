using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DinoHunt.GameClasses
{
    public class Animation
    {

        public Texture2D Texture { get; set; }
        public float FrameTime { get; set; }
        public bool IsLooping{ get; set; }


        //Works for squres only.
        public int FrameCount{ get { return Texture.Width / FrameHeight; } }

        //Works for squres only. 
        public int FrameWidth{ get { return Texture.Height; } }


        public int FrameHeight{ get { return Texture.Height; } }

     
        public Animation(Texture2D texture, float frameTime, bool isLooping)
        {
            this.Texture = texture;
            this.FrameTime = frameTime;
            this.IsLooping = isLooping;
        }
    }
}
