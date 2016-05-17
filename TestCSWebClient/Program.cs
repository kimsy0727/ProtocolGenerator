using Common;

namespace TestCSWebClient
{
    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                string line = System.Console.ReadLine();
                if (line == "send")
                {
                    NetPeer peer = new NetPeer();
                    ProtocolHandler handler = new ClientHandler(peer);
                    handler.sfReqTest("test123", 0.23f, 0.45, 23, 500, 30000, 20000000023);
                    handler.Process(peer.GetAckPacket());
                }
            }
        }
    }
}
