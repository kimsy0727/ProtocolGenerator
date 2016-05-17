using System;
using Common;

class ClientHandler : ProtocolHandler
{
    public ClientHandler(NetPeer peer) : base(peer)
    {
    }

    public override bool rfAckResult(byte result, string msg)
    {
        System.Console.WriteLine("\nparam1:" + result.ToString());
        System.Console.WriteLine("\nparam2:" + msg);
        return true;
    }

    public override bool rfAckTest(byte result, string msg)
    {
        System.Console.WriteLine("received ack packet1:"+ result);
        System.Console.WriteLine("received ack packet2:"+ msg);
        return true;
    }

    public override bool rfReqTest(string test0, float test1, double test2, byte test3, ushort test4, uint test5, ulong test6)
    {
        System.Console.WriteLine("\nparam1:" + test0);
        System.Console.WriteLine("\nparam2:" + test1);
        System.Console.WriteLine("\nparam3:" + test2);
        System.Console.WriteLine("\nparam4:" + test3);
        System.Console.WriteLine("\nparam5:" + test4);
        System.Console.WriteLine("\nparam6:" + test5);
        System.Console.WriteLine("\nparam7:" + test6);
        System.Console.WriteLine("\n\n");

        sfAckTest(0, "Success");

        return true;
    }
}
