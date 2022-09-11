using Gameloop.Vdf;
using Gameloop.Vdf.JsonConverter;
using Gameloop.Vdf.Linq;
using Newtonsoft.Json.Linq;
using SteamJSONAccount.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Net;
using System.Linq;

namespace SteamJSONAccount
{
    public class Program
    {

        public static string SteamAccountsPath = null;
        public static int globalValid = 0;
        public static int globalNotValid = 0;
        public static List<string> accounts = new List<string>();
        public static List<string> nonLevelAccounts = new List<string>();
        public static string apiKey = null;

        private static void Main(string[] args)
        {
            // checking api key
            if (File.Exists("config.ini"))
            {
                apiKey = File.ReadLines("config.ini").Skip(1).First();
                SteamAccountsPath = File.ReadLines("config.ini").Skip(3).First();
                if (SteamAccountsPath == "default")
                    SteamAccountsPath = "vdf//loginusers.vdf";
                else
                    SteamAccountsPath = $"{File.ReadLines("config.ini").Skip(3).First()}//loginusers.vdf";
            } 
            else
            {
                FileStream fs = File.Create("config.ini");
                fs.Close();
                StreamWriter sw = new StreamWriter("config.ini");
                sw.WriteLine($"[ApiKey]\nApi key here");
                sw.WriteLine($"[VdfFolderPath] # leave default to default path\ndefault");
                sw.Close();
                Console.WriteLine("Created file with config, fill it out and try again");
                Console.ReadLine();
                return;
            }

            Console.WriteLine("Sending request to valve servers...");
            var updateTitleThread = new Thread(() => titleUpdate());
            updateTitleThread.Start();
            foreach (SteamAccount item in SteamAccountsUtility.GetAllAccounts())
            {
                Console.WriteLine(item);
            }
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.Write("Done!\nSave file with accounts? Y/N: ");
            if (Console.ReadLine() == "Y")
            {
                File.WriteAllText($"{DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss")}.txt", $"----------- loginusers checker by fan9 | Accounts with level -----------\n{String.Join("\n", accounts.ToArray())}\n----------- Accounts without level -----------\n{String.Join("\n", nonLevelAccounts.ToArray())}");
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine($"File with account has been saved\nSaved as {DateTime.Now.ToString("yyyy - MM - dd - HH - mm - ss")}.txt");
            }
            else
                Console.WriteLine("You select: Don't save file, ok\nDone!");
            updateTitleThread.Abort();
            Console.ReadLine();
        }
        public static void titleUpdate()
        {
            while (true)
                Console.Title = $"loginusers.vdf checker | Have Lvl: {globalValid} | Don't have Lvl: {globalNotValid}";
        }
    }

    public class SteamAccountsUtility
    {
        public static readonly string AccountName = nameof(AccountName);

        public static readonly string PersonaName = nameof(PersonaName);

        public static readonly string MostRecent = nameof(MostRecent);



        public const byte MostRecentValue = 1;



        public static IEnumerable<SteamAccount> GetAllAccounts()
        {
            VProperty deserializedProperty = VdfConvert.Deserialize(File.ReadAllText(Program.SteamAccountsPath));
            foreach (JProperty property in deserializedProperty.ToJson().Value)
            {
                using (WebClient wc = new WebClient())
                {
                    yield return new SteamAccount
                    (
                        ulong.Parse(property.Name),
                        property.Value[AccountName].ToString(),
                        property.Value[PersonaName].ToString(),
                        wc.DownloadString($"http://api.steampowered.com/IPlayerService/GetSteamLevel/v1/?key={Program.apiKey}&steamid={ulong.Parse(property.Name)}")
                    );
                }
            }
        }
    }
}
