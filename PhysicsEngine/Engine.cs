using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace PhysicsEngine
{
    public interface IPhysicsEngine
    {
        void Update(GameTime gameTime);

        void Draw(SpriteBatch sb);

        void CreateBody(PhysicsShape shape, out IBody body);

        void CreateFixture(PhysicsShape shape, out IFixture fixture);

        void CreateFixture(IBody parent, PhysicsShape shape, out IFixture fixture);

        void RemoveBody(IBody body);

        void Reset();

        float WorldDamping { get; set; }
    }

    public abstract class PhysicsEngine<TBody, TFixture> : IPhysicsEngine where TBody : IBody where TFixture : IFixture
    {
        public void CreateBody(PhysicsShape shape, out IBody body)
        {
            TBody result;
            this.CreateBody(shape, out result);
            body = result;
        }

        public void RemoveBody(IBody body)
        {
            if (body is TBody)
            {
                this.RemoveBody((TBody)body);
            }
            else
            {
                throw new InvalidOperationException("Invalid body type");
            }
        }

        public void CreateFixture(PhysicsShape shape, out IFixture fixture)
        {
            TFixture result;
            this.CreateFixture(shape, out result);
            fixture = result;
        }

        public void CreateFixture(IBody parent, PhysicsShape shape, out IFixture fixture)
        {
            if (parent is TBody)
            {
                TFixture result;
                this.CreateFixture((TBody)parent, shape, out result);
                fixture = result;
            }
            else
            {
                throw new InvalidOperationException("Invalid body type");
            }
        }

        public abstract void Update(GameTime gameTime);

        public abstract void Draw(SpriteBatch sb);

        public abstract void CreateBody(PhysicsShape shape, out TBody body);

        public abstract void CreateFixture(PhysicsShape shape, out TFixture fixture);

        public abstract void CreateFixture(TBody parent, PhysicsShape shape, out TFixture fixture);

        public abstract void RemoveBody(TBody body);

        public abstract void Reset();

        public abstract float WorldDamping { get; set; }
    }
}
