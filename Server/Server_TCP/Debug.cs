using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server_TCP
{
    public static class Debug
    {
        public static void Log(string msg)
        {
            //UnityEngine.Debug.Log("[Server] " + msg);
             Console.WriteLine("[Server] " + msg);
        }
        public static void LogWarning(string msg)
        {
            //UnityEngine.Debug.LogWarning("[Server] " + msg);
             Console.WriteLine("[Server] [Waring] " + msg);
        }
    }
}
