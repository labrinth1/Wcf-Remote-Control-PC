using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1
{
    public class StartClientStreamCS
    {
        private static StartClientStreamCS instance;
        private int startClient;

        public int StartClientStream
        {
            get { return startClient; }
            set {

                startClient = value;

                if (startClient == 0)
                {
                    ResetInstances();
                }
            }
        }

        public static StartClientStreamCS Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new StartClientStreamCS();
                }
                return instance;
            }
        }
        public static void ResetInstances()
        {
            KeyboardHandler.Instance.Reset();
            MouseHandler.Instance.Reset();
        }
    }
}