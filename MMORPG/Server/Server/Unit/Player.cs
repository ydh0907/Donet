using Donet.Sessions;
using Donet.Utils;

using Server.Utils;

namespace Server.Unit
{
    public class Player : Unit
    {
        public Session session { get; private set; } = null;

        public Atomic<bool> Active => session.Active;

        public Player()
        {
            pos = Vector2.zero;
            dir = Vector2.down;
            radius = 0.2f;
        }

        public void Initialize(Session session)
        {
            this.session = session;
            session.closed += HandleClose;
        }

        private void HandleClose(Session session)
        {
            Dispose();
        }

        public void Disconnect()
        {
            session.Close();
        }

        public override void Dispose()
        {
            session = null;
        }
    }
}
