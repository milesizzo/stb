using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Shapes;
using StopTheBoats.GameObjects;
using StopTheBoats.Graphics;
using StopTheBoats.Scenes;
using StopTheBoats.Templates;
using MonoGame.Extended;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Common;

namespace StopTheBoats
{
    public class PolygonBoundsEditor : GameAssetScene
    {
        private SpriteTemplate current;
        private List<Vector2> points;
        private Vector2 position;
        private int currentPoint = -1;

        private SpriteTemplate cursor;

        public PolygonBoundsEditor(GraphicsDevice graphics, GameAssetStore assets) : base(graphics, assets)
        {
            this.Camera.Zoom = 2;
        }

        public SpriteTemplate Current
        {
            get { return this.current; }
            set
            {
                this.current = value;
                var shape = this.current.Shape as PolygonShape;
                if (shape == null)
                {
                    throw new InvalidOperationException("Expected a polygon shape");
                }
                this.points = new List<Vector2>(shape.Vertices);
            }
        }

        public override void SetUp()
        {
            StopTheBoatsHelper.LoadGameAssets(this.Assets);
            StopTheBoatsHelper.LoadFonts(this.Assets);

            this.cursor = this.Assets.Sprites.GetOrAdd("editor_cursor", (key) =>
            {
                var sprite = this.Assets.Sprites.Load("editor_cursor");
                sprite.Origin = Vector2.Zero;
                return sprite;
            });

            this.Current = this.Assets.Sprites["rock1"];
        }

        public bool FindPointAt(Vector2 relative, out int closest)
        {
            //var relative = worldPosition - this.position;
            var closestDist = float.MaxValue;
            closest = -1;
            for (var i = 0; i < this.points.Count; i++)
            {
                var point = this.points[i];
                var distance = Vector2.Distance(relative, point);
                if (distance < 2f)
                {
                    closest = i;
                    return true;
                }
                if (distance < closestDist)
                {
                    closest = i;
                    closestDist = distance;
                }
            }
            return false;
        }

        public override void Update(GameTime gameTime)
        {
            this.position = new Vector2(this.Camera.Viewport.Width / 2, this.Camera.Viewport.Height / 2);

            var mouse = Mouse.GetState();
            var keyboard = Keyboard.GetState();

            var mouseScreen = new Vector2(mouse.X, mouse.Y);
            var mouseWorld = this.Camera.ScreenToWorld(mouseScreen);

            if (keyboard.IsKeyDown(Keys.Escape))
            {
                this.SceneEnded = true;
            }
            if (keyboard.IsKeyDown(Keys.OemTilde))
            {
                this.Current.Shape = new PolygonShape(new Vertices(this.points), 1f);
            }
            if (mouse.LeftButton == ButtonState.Pressed)
            {
                var relative = mouseWorld - this.position;
                if (this.currentPoint == -1)
                {
                    int closestIndex;
                    if (this.FindPointAt(relative, out closestIndex))
                    {
                        this.currentPoint = closestIndex;
                    }
                    else
                    {
                        if (closestIndex > -1)
                        {
                            this.currentPoint = closestIndex + 1;
                            this.points.Insert(this.currentPoint, relative);
                        }
                    }
                }
                else
                {
                    this.points[this.currentPoint] = relative;
                }
            }
            else
            {
                this.currentPoint = -1;
            }
            if (mouse.RightButton == ButtonState.Pressed && this.currentPoint == -1 && this.points.Count > 3)
            {
                int closestIndex;
                if (this.FindPointAt(mouseWorld - this.position, out closestIndex))
                {
                    this.points.RemoveAt(closestIndex);
                }
            }
        }

        public override void Draw(Renderer renderer)
        {
            this.Camera.Clear(Color.DarkSlateBlue);
            var mouse = Mouse.GetState().Position;
            if (this.Current != null)
            {
                this.Current.DrawSprite(renderer, this.position, Color.White, 0, Vector2.One, SpriteEffects.None);

                renderer.Render.DrawPoint(this.position, Color.White, size: 3);
                renderer.Render.DrawPolygon(this.position, this.points.ToArray(), Color.Black);
                foreach (var point in this.points)
                {
                    renderer.Render.DrawPoint(this.position + point, Color.Yellow);
                }
            }
            //renderer.Render.DrawPoint(new Vector2(mouse.X, mouse.Y), Color.White);
            //renderer.Render.DrawCircle(new Vector2(mouse.X, mouse.Y), 9, 16, Color.Gray);

            //this.cursor.DrawSprite(renderer, this.Camera.WorldToScreen(mouse.X, mouse.Y), Color.White, 0, Vector2.One, SpriteEffects.None);
            var mouseScreen = new Vector2(mouse.X, mouse.Y);
            var mouseWorld = this.Camera.ScreenToWorld(mouseScreen);
            renderer.DrawString(this.Assets.Fonts["envy16"], string.Format("Mouse (screen): {0}", mouseScreen), this.Camera.ScreenToWorld(new Vector2(8, 8)), Color.White);
            renderer.DrawString(this.Assets.Fonts["envy16"], string.Format(" Mouse (world): {0}", mouseWorld), this.Camera.ScreenToWorld(new Vector2(8, 64)), Color.White);

            this.cursor.DrawSprite(renderer, this.Camera.ScreenToWorld(mouse.X, mouse.Y), Color.White, 0, Vector2.One, SpriteEffects.None);
        }
    }
}
