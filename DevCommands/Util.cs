using System;
using System.Collections;
using System.Collections.Generic;
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
