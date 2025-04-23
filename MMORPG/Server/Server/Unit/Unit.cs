using Server.Utils;

namespace Server.Unit
{
    public abstract class Unit
    {
        public Vector2 pos;
        public Vector2 dir;
        public float radius;

        public virtual void Update() { }
        public virtual void Dispose() { }
    }
}
