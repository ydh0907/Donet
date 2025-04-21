using System.Collections.Concurrent;

using Donet.Sessions;

namespace Server
{
    public static class ObjectPool
    {
        private static ConcurrentStack<Session> sessions = new ConcurrentStack<Session>();

        public static void Initialize()
        {
            for (int i = 0; i < 2048; i++)
            {
                sessions.Push(new Session());
            }
        }

        public static Session PopSession()
        {
            Session session = null;
            while (!sessions.TryPop(out session)) ;
            return session;
        }

        public static void PushSession(Session session)
        {
            using var local = session.Active.Lock();
            while (local.Value) ;

            sessions.Push(session);
        }
    }
}
