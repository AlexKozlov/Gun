using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace TestNetWork_Server.NetWorkClass
{
    class ClientInformation
    {
        public String PlayerName { get; set; }
        public IPAddress IpAddress { get; set; }
        public int Port { get; set; }
        public Socket PlayerHandler { get; set; }

        public DualPull Pull { get; set; }

        public ClientInformation()
        {
            Pull = new DualPull();
        }
        public ClientInformation(String PlayerName, IPAddress IpAddress, int Port, Socket PlayerHandler)
        {
            this.PlayerName = PlayerName;
            this.IpAddress = IpAddress;
            this.Port = Port;
            this.PlayerHandler = PlayerHandler;

            Pull = new DualPull();
        }


    }
}
