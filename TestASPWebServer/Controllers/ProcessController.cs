using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Common;

namespace TestASPWebServer.Controllers
{
    public class ProcessController : Controller
    {
        // GET: Process
        public String Index()
        {
            String encryptedPacket = Request.Form["packet"];
            NetPeer peer = new NetPeer(this);
            ProtocolHandler handler = new ServerHandler(peer);
            if (encryptedPacket == null)
            {
                return handler.sfAckResult(1, "Error");
            }
            
            return handler.Process(encryptedPacket);
        }
    }
}