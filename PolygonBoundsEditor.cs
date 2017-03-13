using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Shapes;
using StopTheBoats.GameObjects;
using StopTheBoats.Graphics;
using StopTheBoats.Scenes;
using StopTheBoats.Templates;
using System.Collections.Generic;

namespace StopTheBoats
{
    public class PolygonBoundsEditor : GameAssetScene
    {
        private SpriteTemplate current;
        private List<Vector2> points;

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
                this.points = new List<Vector2>(this.current.Bounds.Points);
            }
        }

        public override void SetUp()
        {
            StopTheBoatsHelper.LoadGameAssets(this.Assets);
            this.cursor = this.Assets.Sprites.GetOrAdd("editor_cursor", (key) =>
            {
                var sprite = this.Assets.Sprites.Load("editor_cursor");
                sprite.Origin = Vector2.Zero;
                return sprite;
            });

            this.Current = this.Assets.Sprites["patrol_boat"];
        }

        public override void Update(GameTime gameTime)
        {
        }

        public override void Draw(Renderer renderer)
        {
            this.Camera.Clear(Color.DarkSlateBlue);
            var mouse = Mouse.GetState().Position;
            this.cursor.DrawSprite(renderer, new Vector2(mouse.X, mouse.Y), Color.White, 0, Vector2.One, SpriteEffects.None);
            if (this.Current != null)
            {
                var position = new Vector2(this.Camera.Viewport.Width / 2, this.Camera.Viewport.Height / 2);
                this.Current.DrawSprite(renderer, position, Color.White, 0, Vector2.One, SpriteEffects.None);

                renderer.Render.DrawPoint(position + this.Current.Origin, Color.White, size: 3);
                renderer.Render.DrawPolygon(position, this.points.ToArray(), Color.Yellow);
            }
            //renderer.Render.DrawPoint(new Vector2(mouse.X, mouse.Y), Color.White);
            //renderer.Render.DrawCircle(new Vector2(mouse.X, mouse.Y), 9, 16, Color.Gray);
        }
    }
}
