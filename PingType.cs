using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSRSpeedMonitor
{
    public struct PingType
    {
        public int index;
        public string ip;
        public int delay;
        public bool success;
        public string msg;
    }
}
