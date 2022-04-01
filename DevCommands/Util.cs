using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace DevCommands
{
    public static class Util
    {
        public static List<Util.ConnectionData> Connections = new List<Util.ConnectionData>();
        public static IEnumerator WhitelistHandler2(ZRpc rpc)
        {
            rpc.Invoke("WhitelistHandler", new object[] {
        });
            yield return new WaitForSeconds(1);
        }
        public class ConnectionData
        {
            public ZRpc rpc;
        }
        public static IEnumerator AddIdToWhitelist_Server(long sender, ZPackage pkg)
        {
            DevCommands.whitelist = File.ReadAllLines(DevCommands.whitelistPath).ToList();
            Jotunn.Logger.LogInfo(sender);
            ZNetPeer peer = ZNet.instance.GetPeer(sender);
            List<string> idsToAdd = new List<string>();
            for (int i = 0; i < pkg.ReadInt(); i++)
            {
                idsToAdd.Add(pkg.ReadString());
                yield return new WaitForSeconds(1f);
            }
            DevCommands.whitelist.AddRange(idsToAdd);
            File.WriteAllLines(DevCommands.whitelistPath, DevCommands.whitelist);
            ZPackage resPkg = new ZPackage();
            resPkg.Write(1);
            resPkg.Write($"Usuario {sender} añadido correctamente a la whitelist.");
            DevCommands.AddIdToWhitelist_RPC.SendPackage(peer.m_uid, resPkg);
        }
        public static IEnumerator AddIdToWhitelist_Client(long sender, ZPackage pkg)
        {
            yield return null;
            int msgCount = pkg.ReadInt();
            for (int i = 0; i < msgCount; i++)
            {
                Console.instance.Print(pkg.ReadString());
                yield return new WaitForSeconds(0.5f);
            }
        }
        public static void Broadcast(string text, string username = ModInfo.Title)
        {
            ZRoutedRpc.instance.InvokeRoutedRPC(ZRoutedRpc.Everybody, "ChatMessage", new object[]
            {
                new Vector3(0,100,0),
                2,
                username,
                text
            });
        }
        public static void RoutedBroadcast(long peer, string text, string username = ModInfo.Title)
        {
            ZRoutedRpc.instance.InvokeRoutedRPC(peer, "ChatMessage", new object[]
            {
                new Vector3(0,100,0),
                2,
                username,
                text
            });
        }
#nullable enable
        internal static string? readLocalSteamID() =>
            Type.GetType("Steamworks.SteamUser, assembly_steamworks")?.GetMethod("GetSteamID")!
                .Invoke(null, Array.Empty<object>()).ToString();
#nullable disable
    }
}
