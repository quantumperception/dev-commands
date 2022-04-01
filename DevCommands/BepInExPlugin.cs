using BepInEx;
using Jotunn.Entities;
using Jotunn.Managers;
using HarmonyLib;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;


namespace DevCommands
{
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    [BepInDependency(Jotunn.Main.ModGuid)]
    internal class DevCommands : BaseUnityPlugin
    {
        public const string PluginGUID = "Valkyrie.DevCommands";
        public const string PluginName = "DevCommands";
        public const string PluginVersion = "1.0.0";
        public static Harmony harm = new Harmony("DevCommands");
        public static CustomLocalization Localization = LocalizationManager.Instance.GetLocalization();
        public static string pluginsFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        public static string bepinexFolder = Path.Combine(Path.GetFullPath(Path.Combine(pluginsFolder, @"..\")));
        public static string rootFolder = Path.Combine(Path.GetFullPath(Path.Combine(bepinexFolder, @"..\")));
        public static string configFolder = Path.Combine(Path.GetFullPath(Path.Combine(bepinexFolder, "config")));
        public static string whitelistPath = Path.Combine(rootFolder, "Saved", "permittedlist.txt");
        public static List<string> whitelist = new List<string>();

        public static CustomRPC AddIdToWhitelist_RPC;

        private void Awake()
        {
            CommandManager.Instance.AddConsoleCommand(new AddToWhitelistCommand());
            AddIdToWhitelist_RPC = NetworkManager.Instance.AddRPC("AddIdToWhitelist", Util.AddIdToWhitelist_Server, Util.AddIdToWhitelist_Client);
        }

        public class AddToWhitelistCommand : ConsoleCommand
        {
            public override string Name => "whitelist";

            public override string Help => "like spawn but BETTER";

            public override void Run(string[] args)
            {
                if (args.Length != 2 || args[0] != "add")
                {
                    Console.instance.Print("Sintaxis incorrecta.\nUso: whitelist add ID64");
                    return;
                }

                if (args[1].Length != 17 || !args[1].All(char.IsDigit))
                {
                    Console.instance.Print("El ID64 debe contener 17 números.");
                    return;
                }

                ZPackage pkg = new ZPackage();
                pkg.Write(args.Length - 1);
                pkg.Write(args[1]);
                string steamID = Util.readLocalSteamID();
                AddIdToWhitelist_RPC.SendPackage(long.Parse(steamID), pkg);
                return;
            }

            public override List<string> CommandOptionList()
            {
                return ZNetScene.instance?.GetPrefabNames();
            }
        }
    }
}