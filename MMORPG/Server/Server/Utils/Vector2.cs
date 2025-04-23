using Donet.Utils;

namespace Server.Utils
{
    public struct Vector2 : INetworkSerializable
    {
        public static Vector2 zero = new Vector2(0, 0);
        public static Vector2 one = new Vector2(1, 1);
        public static Vector2 up = new Vector2(0, 1);
        public static Vector2 down = new Vector2(0, -1);
        public static Vector2 right = new Vector2(1, 0);
        public static Vector2 left = new Vector2(-1, 0);

        float x, y;

        public Vector2()
        {
            x = 0;
            y = 0;
        }

        public Vector2(float x, float y)
        {
            this.x = x;
            this.y = y;
        }

        public static Vector2 operator +(Vector2 left, Vector2 right) => new Vector2(left.x + right.x, left.y + right.y);
        public static Vector2 operator -(Vector2 left, Vector2 right) => new Vector2(left.x - right.x, left.y - right.y);
        public static Vector2 operator *(Vector2 left, float right) => new Vector2(left.x * right, left.y * right);
        public static Vector2 operator /(Vector2 left, float right) => new Vector2(left.x / right, left.y / right);

        public float magnitude => MathF.Sqrt(x * x + y * y);
        public Vector2 normalized => this / magnitude;
        public void Normalize()
        {
            x /= magnitude;
            y /= magnitude;
        }

        public void Serialize(Serializer serializer)
        {
            serializer.Serialize(ref x);
            serializer.Serialize(ref y);
        }
    }
}
