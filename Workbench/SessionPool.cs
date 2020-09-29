using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Workbench
{
    public interface ISession : IDisposable
    {
        void Do();
    }

    public class Session : ISession
    {
        public void Do() { }

        public void Dispose()
        {
            Console.WriteLine("Disposing...");
        }
    }

    public class PoolSession : ISession
    {
        private readonly Session session;
        private readonly SessionPool pool;

        public PoolSession(Session session, SessionPool pool)
        {
            this.session = session;
            this.pool = pool;
        }
        public void Do() { session.Do(); }

        public void Dispose()
        {
            pool.Release(this);
            Console.WriteLine($"Release session [PObj]... {this.GetHashCode()}");
        }

        public void CloseSession()
        {
            session.Dispose();
        }
    }

    public class SessionPool
    {
        private ConcurrentQueue<PoolSession> _sessions;
        private Timer _timer;
        private int _timerIntervalMs = 10000;

        public SessionPool(int timerIntervalMs = 10000)
        {
            _sessions = new ConcurrentQueue<PoolSession>();
            _timerIntervalMs = timerIntervalMs;
            _timer = new Timer(OnTimerClick, null, _timerIntervalMs, _timerIntervalMs);
        }

        public ISession Get()
        {
            PoolSession session;
            if (!_sessions.TryDequeue(out session))
                session = Create();

            Console.WriteLine($"Get Session {session.GetHashCode()}");
            return session;
        }

        public void Release(PoolSession session)
        {
            Console.WriteLine($"Release session {session.GetHashCode()}");
            _timer.Change(_timerIntervalMs, _timerIntervalMs);
            _sessions.Enqueue(session);
        }

        private PoolSession Create()
        {
            return new PoolSession(new Session(), this);
        }

        private void OnTimerClick(object state)
        {
            if (_sessions.TryDequeue(out var s))
            {
                Console.WriteLine($"Disposing {s.GetHashCode()}");
                s.CloseSession();
            }
            Console.WriteLine($"Timer click {_sessions.Count} sessions...");
        }
    }

    public interface ISessionManager
    {
        ISession Create();
    }

    public class SessionManager : ISessionManager
    {
        private readonly SessionPool _pool;

        public SessionManager(SessionPool pool)
        {
            _pool = pool;
        }

        public ISession Create()
        {
            return _pool.Get();
        }
    }
}
