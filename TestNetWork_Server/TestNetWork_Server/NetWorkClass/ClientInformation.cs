using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace TestNetWork_Server.NetWorkClass
{
    public class ClientInformation
    {
        public String PlayerName { get; set; }
        public IPAddress IpAddress { get; set; }
        public int Port { get; set; }
        public Socket PlayerHandler { get; set; }

        public DualPull DualPull { get; set; }

        public ClientInformation()
        {
            DualPull = new DualPull();
            DualPull.Pull.HP = 100000;
        }
        public ClientInformation(String PlayerName, IPAddress IpAddress, int Port, Socket PlayerHandler)
        {
            this.PlayerName = PlayerName;
            this.IpAddress = IpAddress;
            this.Port = Port;
            this.PlayerHandler = PlayerHandler;

            DualPull = new DualPull();
        }


    }
}
