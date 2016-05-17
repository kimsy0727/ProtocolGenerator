using System;
using Common;
using System.Web.Mvc;

class ServerHandler : ProtocolHandler
{
    public ServerHandler(NetPeer peer) : base(peer)
    {
    }

    public override String rfAckResult(byte result, string msg)
    {
        throw new NotImplementedException();
    }

    public override String rfAckTest(byte result, string msg)
    {
        throw new NotImplementedException();
    }

    public override String rfReqTest(string test0, float test1, double test2, byte test3, ushort test4, uint test5, ulong test6)
    {
        System.Diagnostics.Debug.WriteLine("received req packet1:" + test0);
        System.Diagnostics.Debug.WriteLine("received req packet2:" + test1);
        System.Diagnostics.Debug.WriteLine("received req packet3:" + test2);
        System.Diagnostics.Debug.WriteLine("received req packet4:" + test3);
        System.Diagnostics.Debug.WriteLine("received req packet5:" + test4);
        System.Diagnostics.Debug.WriteLine("received req packet6:" + test5);
        System.Diagnostics.Debug.WriteLine("received req packet7:" + test6);

        return sfAckTest(0, test0 + test1.ToString() + test2.ToString() + test3.ToString() + test4.ToString() + test5.ToString() + test6.ToString());
    }
}
