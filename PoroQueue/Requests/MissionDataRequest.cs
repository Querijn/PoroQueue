using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoroQueue
{
    class MissionDataRequest
    {
        public class PlayerInventory
        {
            public List<object> champions { get; set; }
            public List<int> icons { get; set; }
            public List<object> skins { get; set; }
            public List<object> wardSkins { get; set; }
        }

        public int level { get; set; }
        public bool loyaltyEnabled { get; set; }
        public PlayerInventory playerInventory { get; set; }

        public static async Task<MissionDataRequest> Get()
        {
            return await JSONRequest.Get<MissionDataRequest>(LeagueOfLegends.APIDomain + "/lol-missions/v1/data");
        }
    }
}
