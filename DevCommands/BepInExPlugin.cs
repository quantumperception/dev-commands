using BepInEx;
using HarmonyLib;
using Jotunn.Entities;
using Jotunn.Managers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        public static CustomLocalization Localization = LocalizationManager.Instance.GetLocalization();
        public static string pluginsFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        public static string bepinexFolder = Path.Combine(Path.GetFullPath(Path.Combine(pluginsFolder, @"..\")));
        public static string rootFolder = Path.Combine(Path.GetFullPath(Path.Combine(bepinexFolder, @"..\")));
        public static string configFolder = Path.Combine(Path.GetFullPath(Path.Combine(bepinexFolder, "config")));
        public static string whitelistPath = Path.Combine(rootFolder, "Saved", "permittedlist.txt");
        public static List<string> whitelist = new List<string>();

        private Harmony _harmony;
        public DevCommands(Harmony harmony)
        {
            _harmony = harmony;
        }

        private void Awake()
        {
            CommandManager.Instance.AddConsoleCommand(new AddToWhitelistCommand());
            _harmony = Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), PluginGUID);
        }
        private void OnDestroy()
        {
            _harmony?.UnpatchSelf();
        }
        /* Server */

        [HarmonyPatch(typeof(Game), "Start")]
        public static class GameStartPatch
        {
            private static void Prefix()
            {
                if (ZNet.instance.IsServer())
                {
                    ZRoutedRpc.instance.Register("Request_AddIdToWhitelist",
                        new Action<long, ZPackage>(Server.RPC_RequestAddIdToWhitelist)); // Our Server Handler
                    ZRoutedRpc.instance.Register("Event_AddIdToWhitelist",
                        new Action<long, ZPackage>(Server.RPC_EventAddIdToWhitelist)); // Our Mock Client Function
                }
            }
        }


        /* Client */
        [HarmonyPatch(typeof(Game), "Start")]
        public static class GameStartPatchClient
        {
            private static void Prefix()
            {
                if (!ZNet.instance.IsServer())
                {
                    ZRoutedRpc.instance.Register("Request_AddIdToWhitelist",
                        new Action<long, ZPackage>(Client.RPC_RequestAddIdToWhitelist)); // Our Mock Server Handler
                    ZRoutedRpc.instance.Register("Event_AddIdToWhitelist",
                        new Action<long, ZPackage>(Client.RPC_EventAddIdToWhitelist)); // Our Client Function
                    ZRoutedRpc.instance.Register("LogToChat", new Action<long, ZPackage>(Client.RPC_LogToChat));
                    ZRoutedRpc.instance.Register("LogToConsole", new Action<long, ZPackage>(Client.RPC_LogToConsole));
                    ZRoutedRpc.instance.Register("BadRequestMsg",
                        new Action<long, ZPackage>(Client.RPC_BadRequestMsg)); // Our Error Handler
                }
            }
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
                long steamID = long.Parse(Util.readLocalSteamID());
                ZRoutedRpc.instance.InvokeRoutedRPC(steamID, "Request_AddIdToWhitelist", pkg);
                return;
            }

            public override List<string> CommandOptionList()
            {
                return ZNetScene.instance?.GetPrefabNames();
            }
        }
    }
}