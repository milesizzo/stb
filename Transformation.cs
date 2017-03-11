using Microsoft.Xna.Framework;

namespace StopTheBoats
{
    public class Transformation
    {
        public Vector2 Position = Vector2.Zero;
        public Vector2 Scale = Vector2.One;
        public float Rotation = 0;

        public static Transformation Compose(Transformation a, Transformation b)
        {
            var result = new Transformation();
            result.Position = a.TransformVector(b.Position);
            result.Rotation = a.Rotation + b.Rotation;
            result.Scale = a.Scale * b.Scale;
            return result;
        }

        public static Transformation Decompose(Transformation combined, Transformation single)
        {
            var result = new Transformation();
            result.Position = single.UntransformVector(combined.Position);
            result.Rotation = combined.Rotation - single.Rotation;
            result.Scale = combined.Scale / single.Scale;
            return result;
        }

        public Vector2 TransformVector(Vector2 point)
        {
            var result = Vector2.Transform(point, Matrix.CreateRotationZ(this.Rotation));
            result *= this.Scale;
            result += this.Position;
            return result;
        }

        public Vector2 UntransformVector(Vector2 point)
        {
            var result = Vector2.Transform(point, Matrix.CreateRotationZ(this.Rotation + MathHelper.Pi));
            result /= this.Scale;
            result -= this.Position;
            return result;
        }
    }
}
