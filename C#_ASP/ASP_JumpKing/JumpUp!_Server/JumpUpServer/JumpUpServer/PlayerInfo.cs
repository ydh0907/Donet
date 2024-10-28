using System.Numerics;

namespace JumpUpServer
{
    public class PlayerInfo
    {
        public string? id { get; set; }
        public string? name { get; set; }
        public Vector2 position { get; set; }
        public Vector3 rotation { get; set; }
    }
}
