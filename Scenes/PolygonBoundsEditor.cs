using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Common;
using GameEngine.Templates;
using GameEngine.Scenes;
using GameEngine.Graphics;
using GameEngine.Helpers;
using GameEngine.Extensions;
using GameEngine.Content;

namespace StopTheBoats.Scenes
{
    public class PolygonBoundsEditor : GameAssetScene
    {
        private Camera camera;
        private const float DefaultZoom = 2f;
        private List<Vector2> points;
        private Vector2 position;
        private int currentPoint = -1;
        private List<KeyValuePair<string, ISpriteTemplate>> sprites = new List<KeyValuePair<string, ISpriteTemplate>>();
        private int currentSpriteIndex = 0;

        private ISpriteTemplate cursor;

        public PolygonBoundsEditor(string name, GraphicsDevice graphics) : base(name, graphics)
        {
            this.camera = new Camera(graphics);
            this.Camera.Zoom = DefaultZoom;
        }

        private Camera Camera
        {
            get { return this.camera; }
        }

        public string CurrentName
        {
            get { return this.sprites[this.currentSpriteIndex].Key; }
        }

        public ISpriteTemplate Current
        {
            get { return this.sprites[this.currentSpriteIndex].Value; }
        }

        private void SetCurrent(int index)
        {
            while (index >= this.sprites.Count) index -= this.sprites.Count;
            while (index < 0) index += this.sprites.Count;
            this.currentSpriteIndex = index;
            var shape = this.Current.Shape as PolygonShape;
            if (shape == null)
            {
                throw new InvalidOperationException("Expected a polygon shape");
            }
            this.points = new List<Vector2>(shape.Vertices);
        }

        public override void SetUp()
        {
            base.SetUp();

            Store.Instance.LoadFromJson("Content\\StopTheBoats.json");
            Store.Instance.LoadFromJson("Content\\BoundsEditor.json");

            this.cursor = Store.Instance.Sprites<SpriteTemplate>("BoundsEditor", "editor_cursor");

            foreach (var kvp in Store.Instance["StopTheBoats"].Sprites.All)
            {
                if (kvp.Value.Shape is PolygonShape)
                {
                    this.sprites.Add(kvp);
                }
            }

            this.SetCurrent(0);
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
            if (KeyboardHelper.KeyPressed(Keys.OemTilde))
            {
                this.Current.Shape = new PolygonShape(new Vertices(this.points), 1f);
                foreach (var storeName in Store.Instance.Keys)
                {
                    Store.Instance.SaveToJson(storeName, $"{storeName}.json");
                }
            }

            if (keyboard.IsKeyDown(Keys.PageUp))
            {
                this.Camera.ZoomIn(gameTime.GetElapsedSeconds());
            }
            if (keyboard.IsKeyDown(Keys.PageDown))
            {
                this.Camera.ZoomOut(gameTime.GetElapsedSeconds());
            }
            if (keyboard.IsKeyDown(Keys.Home))
            {
                this.Camera.Zoom = DefaultZoom;
            }
            if (KeyboardHelper.KeyPressed(Keys.Left))
            {
                this.SetCurrent(this.currentSpriteIndex - 1);
            }
            if (KeyboardHelper.KeyPressed(Keys.Right))
            {
                this.SetCurrent(this.currentSpriteIndex + 1);
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
                        if (closestIndex > -1 && this.points.Count < FarseerPhysics.Settings.MaxPolygonVertices)
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

        public override void PreDraw(Renderer renderer)
        {
            base.PreDraw(renderer);
            renderer.World.Begin(blendState: BlendState.NonPremultiplied, transformMatrix: this.Camera.GetViewMatrix());
        }

        public override void Draw(Renderer renderer, GameTime gameTime)
        {
            this.Camera.Clear(Color.DarkSlateBlue);
            var mouse = Mouse.GetState().Position;
            if (this.Current != null)
            {
                this.Current.DrawSprite(renderer.World, this.position, Color.White, 0, Vector2.One, SpriteEffects.None);

                renderer.World.DrawPoint(this.position, Color.White, size: 3);
                renderer.World.DrawPolygon(this.position, this.points.ToArray(), Color.Black);
                foreach (var point in this.points)
                {
                    renderer.World.DrawPoint(this.position + point, Color.Yellow);
                }
            }

            var font = Store.Instance.Fonts("Base", "envy16");
            var mouseScreen = new Vector2(mouse.X, mouse.Y);
            var mouseWorld = this.Camera.ScreenToWorld(mouseScreen);
            renderer.Screen.DrawString(font, string.Format("Mouse (screen): {0}", mouseScreen), new Vector2(8, 8), Color.White);
            renderer.Screen.DrawString(font, string.Format(" Mouse (world): {0}", mouseWorld), new Vector2(8, 32), Color.White);

            this.cursor.DrawSprite(renderer.World, this.Camera.ScreenToWorld(mouse.X, mouse.Y), Color.White, 0, Vector2.One, SpriteEffects.None);
        }

        public override void PostDraw(Renderer renderer)
        {
            renderer.World.End();
            base.PostDraw(renderer);
        }
    }
}
