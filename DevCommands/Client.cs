using HarmonyLib;

namespace DevCommands
{
    [HarmonyPatch]
    public class Client
    {
        public static long steamId = long.Parse(Util.readLocalSteamID());
        public static void RPC_EventAddIdToWhitelist(long sender, ZPackage pkg)
        {
            int msgCount = pkg.ReadInt();
            for (int i = 0; i < msgCount; i++)
            {
                Console.instance.Print(pkg.ReadString());
            }
        }

        public static void RPC_RequestAddIdToWhitelist(long sender, ZPackage pkg) { }

        public static void RPC_BadRequestMsg(long sender, ZPackage pkg)
        {
            if (sender == long.Parse(Util.readLocalSteamID()) && pkg != null && pkg.Size() > 0)
            {
                // Confirm our Server is sending the RPC
                var msg = pkg.ReadString(); // Get Our Msg
                if (msg != "") // Make sure it isn't empty
                    Chat.instance.AddString("Server", "<color=\"red\">" + msg + "</color>",
                        Talker.Type.Normal); // Add to chat with red color because it's an error
            }
        }

        public static void RPC_LogToChat(long sender, ZPackage pkg)
        {
            if (sender == steamId && pkg != null && pkg.Size() > 0)
            {
                // Confirm our Server is sending the RPC
                var msg = pkg.ReadString(); // Get Our Msg
                if (msg != "") // Make sure it isn't empty
                    Chat.instance.AddString("Server", "<color=\"green\">" + msg + "</color>",
                        Talker.Type.Normal); // Add to chat with red color because it's an error
            }
        }

        public static void RPC_LogToConsole(long sender, ZPackage pkg)
        {
            if (sender == steamId && pkg != null && pkg.Size() > 0)
            {
                // Confirm our Server is sending the RPC
                var msgAmt = pkg.ReadInt();
                for (int i = 0; i < msgAmt; i++)
                {
                    string msg = pkg.ReadString();
                    if (msg != "") Console.instance.Print(msg);
                }
            }
        }
    }
}
