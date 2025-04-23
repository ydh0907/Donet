using System.Collections.Concurrent;

using Donet.Sessions;

namespace Server.Pool
{
    public static class SessionPool
    {
        private static ConcurrentStack<Session> sessions = null;

        public static void Initialize(int count)
        {
            sessions = new ConcurrentStack<Session>();
            for (int i = 0; i < count; i++)
                sessions.Push(new Session());
        }

        public static void Dispose()
        {
            sessions.Clear();
            sessions = null;
        }

        public static Session PopSession()
        {
            if (sessions == null)
                throw new ObjectDisposedException(nameof(SessionPool));

            Session session = null;
            if (!sessions.TryPop(out session))
                session = new Session();
            return session;
        }

        public static void PushSession(Session session)
        {
            if (sessions == null)
                throw new ObjectDisposedException(nameof(SessionPool));

            using (var sessionActive = session.Active.Lock())
            {
                using (var sessionClosing = session.Closing.Lock())
                {
                    if (sessionActive.Value && !sessionClosing.Value)
                        session.Close();
                }
            }

            sessions.Push(session);
        }
    }
}
