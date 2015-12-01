using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading;

namespace CitizenMP.Server.Resources
{
    class InternalCallRefHandler : ICallRefHandler
    {
        private ConcurrentDictionary<int, Delegate> m_callbacks = new ConcurrentDictionary<int, Delegate>();
        private int m_callbackId;

        public int AddCallback(Delegate deleg)
        {
            int index = Interlocked.Increment(ref m_callbackId);
            m_callbacks.TryAdd(index, deleg);

            return index;
        }

        public Delegate GetRef(int index)
        {
            Delegate deleg;

            if (m_callbacks.TryGetValue(index, out deleg))
            {
                return deleg;
            }

            return null;
        }

        public bool HasRef(int index, uint instance)
        {
            Delegate deleg;

            return (m_callbacks.TryGetValue(index, out deleg));
        }

        // static getter
        private static InternalCallRefHandler ms_instance;

        public static InternalCallRefHandler Get()
        {
            if (ms_instance == null)
            {
                ms_instance = new InternalCallRefHandler();
            }

            return ms_instance;
        }
    }
}
