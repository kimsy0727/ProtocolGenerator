using Common;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace TestCSClient
{
    class Program
    {
        static void ThreadFunc(NetPeer peer, ProtocolHandler handler)
        {
            while(true)
            {
                byte[] receivedBuffer = new byte[4100];
                peer.GetSocket().Receive(receivedBuffer);
                _PACKET_STRUCT receivedPacket = new _PACKET_STRUCT();
                receivedPacket.Deserialize(ref receivedBuffer);
                if (!handler.Process(receivedPacket.buffer))
                {
                    System.Console.WriteLine("failed process packet.");
                }
            }
        }

        static void Main(string[] args)
        {
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, System.Net.Sockets.ProtocolType.Tcp);
            IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 4500);
            socket.Connect(localEndPoint);

            NetPeer peer = new NetPeer(socket);
            ProtocolHandler handler = new ClientHandler(peer);
            // create recv thread
            Thread threadFunc = new Thread(() => ThreadFunc(peer, handler));
            threadFunc.Start();

            while(true)
            {
                string line = System.Console.ReadLine();
                if(line == "send")
                {
                    if(!handler.sfReqTest("test", 0.23f, 0.45, 23, 500, 30000, 200000000))
                    {
                        System.Console.WriteLine("failed send packet.");
                    }
                }
            }
        }
    }
}
