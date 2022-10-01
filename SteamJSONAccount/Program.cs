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
           try
            {
                Log.checkLogFile("loginusers.vdf checker log file");
                if (!Directory.Exists("vdf"))
                {
                    Directory.CreateDirectory("vdf");
                    Console.WriteLine("Created vdf folder, put loginusers.vdf file and start program");
                    Log.write("Created vdf folder, put loginusers.vdf file and start program", LogType.INFO, true);
                    Console.ReadLine();
                    return;
                }
                // checking api key
                if (File.Exists("config.ini"))
                {
                    apiKey = File.ReadLines("config.ini").Skip(1).First();
                    string cfgPath = File.ReadLines("config.ini").Skip(3).First();
                    string vdfFileName = File.ReadLines("config.ini").Skip(5).First();

                    if (cfgPath == "default" && vdfFileName == "default")
                        SteamAccountsPath = "vdf//loginusers.vdf";
                    if (cfgPath == "default" && vdfFileName != "default")
                        SteamAccountsPath = $"vdf//{vdfFileName}.vdf";
                    if (cfgPath != "default" && vdfFileName == "default")
                        SteamAccountsPath = $"{cfgPath}//loginusers.vdf";
                    if (cfgPath != "default" && vdfFileName != "default")
                        SteamAccountsPath = $"{cfgPath}//{vdfFileName}.vdf";

                }
                else
                {
                    FileStream fs = File.Create("config.ini");
                    fs.Close();
                    using (StreamWriter sw = new StreamWriter("config.ini"))
                    {
                        sw.WriteLine($"[ApiKey]\nApi key here");
                        sw.WriteLine($"[VdfFolderPath] # leave default to default path\ndefault");
                        sw.WriteLine($"[VdfFileName] # leave default to default name(loginusers)\ndefault");
                        sw.Close();
                        Console.WriteLine("Created file with config, fill it out and try again");
                        Log.write("Created file with config, fill it out and try again", LogType.INFO, true);
                        Console.ReadLine();
                        return;
                    }
                    
                }
                Log.write("Sending request to valve servers...", LogType.INFO, true);
                Console.WriteLine("Sending request to valve servers...");
                var updateTitleThread = new Thread(() => titleUpdate());
                updateTitleThread.Start();
                foreach (SteamAccount item in SteamAccountsUtility.GetAllAccounts())
                {
                    Log.write(item.ToString(), LogType.INFO, true);
                    Console.WriteLine(item);
                }
                Console.ForegroundColor = ConsoleColor.Magenta;
                Log.write("Done! Save file with accounts? Y/N: ", LogType.INFO, true);
                Console.Write("Done!\nSave file with accounts? Y/N: ");
                if (Console.ReadLine() == "Y")
                {
                    Log.write("User answer: Y", LogType.INFO, true);
                    Log.write("Creating and writing accounts to txt file", LogType.INFO, true);
                    File.WriteAllText($"{DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss")}.txt", $"----------- loginusers checker by fan9 | Accounts with level -----------\n{String.Join("\n", accounts.ToArray())}\n----------- Accounts without level -----------\n{String.Join("\n", nonLevelAccounts.ToArray())}");
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Log.write($"File with accounts has been saved as {DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss")}.txt", LogType.INFO, true);
                    Console.WriteLine($"File with accounts has been saved as {DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss")}.txt");
                }
                else
                {
                    Console.WriteLine("You select: Don't save file, ok\nDone!");
                    Log.write("User answer: N, or any answer. Skipping save process ", LogType.INFO, true);
                }

                updateTitleThread.Abort();
                Console.ReadLine();
            } catch (Exception e)
            {
                Log.write(e.ToString(), LogType.ERROR, true);
                Console.WriteLine("Program is crashed. Check latest.log file. If you have some questions/found bug, open issue on my github - https://github.com/FanyaOff/loginusers.vdf-checker");
            }
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
