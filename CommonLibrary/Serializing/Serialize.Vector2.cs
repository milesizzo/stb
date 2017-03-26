using Microsoft.Xna.Framework;

namespace CommonLibrary.Serializing
{
    public static partial class CommonSerialize
    {
        public static void Write(ISerializer context, Vector2 vector)
        {
            context.Write("x", vector.X);
            context.Write("y", vector.Y);
        }

        public static void Read(IDeserializer context, out Vector2 vector)
        {
            var x = context.Read<float>("x");
            var y = context.Read<float>("y");
            vector = new Vector2(x, y);
        }
    }
}
