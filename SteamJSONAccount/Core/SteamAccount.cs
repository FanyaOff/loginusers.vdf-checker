using System;
using Newtonsoft.Json;

namespace SteamJSONAccount.Core
{
    public class SteamAccount : ISteamAccount
    {
        public ulong Id { get; }

        public string Name { get; }

        public string Nickname { get; }

        public string Request { get; }

        private static int accCounter = 1;
        private static int nLvlCounter = 1;
        public int getSteamLvl(string json)
        {
            var steamLvl = JsonConvert.DeserializeObject<SteamLvlJsonClass.Root>(json);
            return steamLvl.response.player_level;
        }

        public SteamAccount(ulong id, string name, string nickName, string request)
        {
            Id = id;
            Name = name;
            Nickname = nickName;
            Request = getSteamLvl(request).ToString();
        }



        public override string ToString()
        {
            if (Request.StartsWith("0"))
            {
                int nLevel = nLvlCounter++;
                Program.nonLevelAccounts.Add($"{nLevel}) Link: https://steamcommunity.com/profiles/{Id} Login: {Name} Level: {Request} ");
                Program.globalNotValid = Program.globalNotValid + 1;
                Console.ForegroundColor = ConsoleColor.Red;
                return $"Id: {Id}, Steam Login: {Name}, Nickname: {Nickname}, Lvl: {Request}";
            }
            int pCounter = accCounter++;
            Program.accounts.Add($"{pCounter}) Link: https://steamcommunity.com/profiles/{Id} Login: {Name} Level: {Request} ");
            Program.globalValid = Program.globalValid + 1;
            Console.ForegroundColor = ConsoleColor.Green;
            return $"Id: {Id}, Steam Login: {Name}, Nickname: {Nickname}, Lvl: {Request}";
        }

        public bool Equals(SteamAccount target) => Id == target.Id;
    }
}
