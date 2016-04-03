using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1
{
    public class MouseHandler
    {
        private static MouseHandler instance;

        private int[] mouseCoordinates;
        private ConcurrentQueue<int> mouseEvents;

        public static MouseHandler Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new MouseHandler();
                }
                return instance;
            }
        }
        private MouseHandler()
        {
            mouseCoordinates = new int[2];
            mouseEvents = new ConcurrentQueue<int>();
        }
        public void SetCoordinates(int[] coords)
        {
            mouseCoordinates[0] = coords[0];
            mouseCoordinates[1] = coords[1];
        }
        public int[] GetCoordinates()
        {
            return mouseCoordinates;
        }
        public void Queue(int evt)
        {
            mouseEvents.Enqueue(evt);

            lock (mouseEvents)
            {
                int overflow;
                while (mouseEvents.Count > 5 && mouseEvents.TryDequeue(out overflow)) ;
            }
        }
        public int? Dequeue()
        {
            if (mouseEvents.Count == 0)
            {
                return null;
            }
            int overflow;
            mouseEvents.TryDequeue(out overflow);
            return overflow;
        }
        public void Reset()
        {
            int clear; while (mouseEvents.TryDequeue(out clear)) ;
        }

    }
}