using System;
using Common;

class ClientHandler : ProtocolHandler
{
    public ClientHandler(NetPeer peer) : base(peer)
    {
    }

    public override String rfAckResult(byte result, string msg)
    {
        throw new NotImplementedException();
    }

    public override String rfAckTest(byte result, string msg)
    {
        System.Console.WriteLine("received ack packet1:" + result);
        System.Console.WriteLine("received ack packet2:" + msg);
        return "";
    }

    public override String rfReqTest(string test0, float test1, double test2, byte test3, ushort test4, uint test5, ulong test6)
    {
        throw new NotImplementedException();
    }
}
