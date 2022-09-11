using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamJSONAccount.Core
{
    internal class SteamLvlJsonClass
    {
        public class Response
        {
            public int player_level { get; set; }
        }

        public class Root
        {
            public Response response { get; set; }
        }
    }
}
