using System;
using System.IO;
using Common;

namespace ProtocolGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                System.Console.WriteLine("invalid parameter count.");
                return;
            }

            if (args[0] == "help")
            {
                System.Console.WriteLine("parameter1: code type (ex: cs, cpp, csweb)");
                System.Console.WriteLine("parameter2: protocol xml file name (ex: D:\\Protocol.xml)");
                System.Console.WriteLine("parameter3: generate directory (ex: D:\\ProtocolDirectory)");
                return;
            }

            if (args.Length < 3)
            {
                System.Console.WriteLine("invalid parameter count.");
                return;
            }

            Common.ProtocolType type;
            if (args[0] == "cs")
                type = Common.ProtocolType.CS;
            else if (args[0] == "cpp")
                type = Common.ProtocolType.CPP;
            else if (args[0] == "csweb")
                type = Common.ProtocolType.CSWEB;
            else
            {
                System.Console.WriteLine("invalid parameter1. (ex: cs, cpp, csweb)");
                return;
            }

            if (!File.Exists(args[1]))
            {
                System.Console.WriteLine("invalid file path. " + args[1]);
                return;
            }

            if (!Directory.Exists(args[2]))
            {
                System.Console.WriteLine("invalid directory path. " + args[2]);
                return;
            }

            ProtocolManager protocolManager = new ProtocolManager((Int16)type, args[1], args[2]);
            if(!protocolManager.Execute())
            {
                System.Console.WriteLine("failed generate.");
                return;
            }

            System.Console.WriteLine("complete generate.");
        }
    }
}
