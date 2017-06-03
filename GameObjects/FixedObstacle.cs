using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Dynamics;
using GameEngine.GameObjects;
using GameEngine.Templates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StopTheBoats.GameObjects
{
    public class FixedObstacle : SpriteObject
    {
        public FixedObstacle(IGameContext context, World world, ISpriteTemplate sprite) : base(context, world, sprite)
        {
            this.Body.BodyType = BodyType.Static;
            this.Fixture.Restitution = 0.001f;
        }
    }
}
