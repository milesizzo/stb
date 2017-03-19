using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GameEngine.Templates;

namespace GameEngine.Graphics
{
    public class Sprite
    {
        public const int InfiniteCycles = -1;
        private readonly SpriteTemplate template;
        private float animFrame;
        private int animCyclesTotal;
        private int animCyclesCount;

        public Sprite(SpriteTemplate template, int animCycles = InfiniteCycles)
        {
            this.template = template;
            this.animFrame = 0;
            this.animCyclesTotal = animCycles;
            this.animCyclesCount = 0;
        }

        public void Draw(Renderer renderer, Vector2 position, Color colour, float rotation, Vector2 scale, SpriteEffects effects)
        {
            if (!this.IsComplete)
            {
                this.template.DrawSprite(renderer, (int)Math.Floor(this.animFrame), position, colour, rotation, scale, effects);
            }
        }

        public void Update(GameTime gameTime)
        {
            if (this.template != null && this.template.NumberOfFrames > 1)
            {
                this.animFrame += (float)gameTime.ElapsedGameTime.TotalSeconds * this.template.FPS;
                while (this.animFrame > this.template.NumberOfFrames)
                {
                    this.animFrame -= this.template.NumberOfFrames;
                    this.animCyclesCount++;
                }
            }
        }

        public int TotalCycles
        {
            get { return this.animCyclesTotal; }
            set { this.animCyclesTotal = value; }
        }

        public SpriteTemplate Template
        {
            get { return this.template; }
        }

        public bool IsComplete
        {
            get
            {
                if (this.animCyclesTotal == InfiniteCycles) return false;
                if (this.animCyclesCount < this.animCyclesTotal) return false;
                return true;
            }
        }
    }
}
