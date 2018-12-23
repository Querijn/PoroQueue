using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoroQueue
{
    class LobbyMatchmakingEvent
    {
        public class Data
        {
            public int counter { get; set; }
            public string phaseName { get; set; }
            public int timer { get; set; }
        }

        public Data data { get; set; }
        public string eventType { get; set; }
        public string uri { get; set; }
    }
}
