using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamJSONAccount.Core
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class Player
    {
        public string steamid { get; set; }
        public int communityvisibilitystate { get; set; }
        public int profilestate { get; set; }
        public string personaname { get; set; }
        public string profileurl { get; set; }
        public string avatar { get; set; }
        public string avatarmedium { get; set; }
        public string avatarfull { get; set; }
        public string avatarhash { get; set; }
        public int personastate { get; set; }
        public int precreated { get; set; }
        public int personastateflags { get; set; }
        public string loccountimaryclanid { get; set; }
        public string timrycode { get; set; }
    }

    public class Response
    {
        public List<Player> players { get; set; }
    }

    public class Root
    {
        public Response response { get; set; }
    }


}
