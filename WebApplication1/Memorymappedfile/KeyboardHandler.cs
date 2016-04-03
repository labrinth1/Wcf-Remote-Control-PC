using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1
{
    public class KeyboardHandler
    {
        private static KeyboardHandler instance;
        private ConcurrentQueue<int> KeyBoardEvents;

        public static KeyboardHandler Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new KeyboardHandler();
                }
                return instance;
            }
        }
        private KeyboardHandler()
        {
            KeyBoardEvents = new ConcurrentQueue<int>();
        }
        public void Queue(int evt)
        {
            KeyBoardEvents.Enqueue(evt);

            lock (KeyBoardEvents)
            {
                int overflow;
                while (KeyBoardEvents.Count > 5 && KeyBoardEvents.TryDequeue(out overflow)) ;
            }
        }
        public int? Dequeue()
        {
            if (KeyBoardEvents.Count == 0)
            {
                return null;
            }
            int overflow;
            KeyBoardEvents.TryDequeue(out overflow);
            return overflow;
        }
        public void Reset()
        {
            int clear; while (KeyBoardEvents.TryDequeue(out clear));
        }


    }
}