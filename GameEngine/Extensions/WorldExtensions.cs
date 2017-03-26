using Microsoft.Xna.Framework;
using MonoGame.Extended;
using FarseerPhysics.Common;
using FarseerPhysics.Dynamics;
using GameEngine.Graphics;

namespace GameEngine.Extensions
{
    public static class WorldExtensions
    {
        public static void Draw(this World world, Renderer renderer)
        {
            foreach (var body in world.BodyList)
            {
                Transform transform;
                body.GetTransform(out transform);
                foreach (var fixture in body.FixtureList)
                {
                    renderer.World.DrawShape(fixture.Shape, transform, Color.White);
                }
                renderer.World.DrawPoint(body.Position, Color.Yellow, size: 3f);
            }
        }
    }
}
