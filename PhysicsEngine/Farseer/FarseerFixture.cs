using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhysicsEngine.Farseer
{
    public class FarseerFixture : IFixture
    {
        private readonly Fixture fixture;
        private readonly PhysicsShape shape;
        private IBody body;

        public FarseerFixture(IBody body, Fixture fixture, PhysicsShape shape)
        {
            this.fixture = fixture;
            this.shape = shape;
            this.body = body;
        }

        public Fixture Internal { get { return this.fixture; } }

        public PhysicsShape Shape { get { return this.shape; } }

        public void SetOffset(Vector2 offset)
        {
        }

        public IBody Body
        {
            get { return this.body; }
            set
            {
                this.body = value;
                /*var asFarseer = value as FarseerBody;
                if (asFarseer == null)
                {
                    throw new InvalidOperationException("Parent for FarseerFixture must be FarseerBody");
                }
                this.body = asFarseer;*/
            }
        }
    }
}
