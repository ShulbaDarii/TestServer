using System;
using System.Net.Sockets;

namespace TestServer
{
    public class Info
    {
        public Socket ClientSocket { get; set; }
        public String RemoteEndPoint { get; set; }
        public override string ToString()
        {
            return RemoteEndPoint;
        }
    }
}