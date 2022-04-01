using HarmonyLib;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DevCommands
{
    [HarmonyPatch]
    public class Server
    {
        public static void RPC_RequestAddIdToWhitelist(long sender, ZPackage pkg)
        {
            Jotunn.Logger.LogInfo($"Server: SenderID: ${sender}");
            if (pkg != null && pkg.Size() > 0)
            {
                ZNetPeer peer = ZNet.instance.GetPeer(sender);
                if (peer != null)
                {
                    DevCommands.whitelist = File.ReadAllLines(DevCommands.whitelistPath).ToList();
                    Jotunn.Logger.LogInfo(sender);
                    List<string> idsToAdd = new List<string>();
                    for (int i = 0; i < pkg.ReadInt(); i++)
                    {
                        idsToAdd.Add(pkg.ReadString());
                    }
                    DevCommands.whitelist.AddRange(idsToAdd);
                    File.WriteAllLines(DevCommands.whitelistPath, DevCommands.whitelist);
                    ZPackage resPkg = new ZPackage();
                    resPkg.Write(idsToAdd.Count);
                    resPkg.Write($"Usuario {sender} añadido a la whitelist.");
                    ZRoutedRpc.instance.InvokeRoutedRPC(sender, "LogToConsole", resPkg);
                }
                else
                {
                    ZPackage resPkg = new ZPackage(); // Create a new ZPackage.
                    resPkg.Write(1);
                    resPkg.Write("Peer was null, Server Patch"); // Tell them what's going on.
                    ZRoutedRpc.instance.InvokeRoutedRPC(sender, "BadRequestMsg", resPkg); // Send the error message.
                }
            }
        }
        public static void RPC_EventAddIdToWhitelist(long sender, ZPackage pkg) { }
    }
}
