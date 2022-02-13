using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

#nullable enable

namespace MicaForEveryone.Win32
{
    public class Dispatcher
    {
        private ConcurrentQueue<Action> _queue = new();

        public void Enqueue(Action action)
        {
            _queue.Enqueue(action);
        }

        public void Invoke()
        {
            while (_queue.Count > 0)
            {
                if (_queue.TryDequeue(out var action))
                {
                    action.Invoke();
                }
            }
        }
    }
}