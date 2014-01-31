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

        CoolDown Respown { get; set; }

        public ClientInformation()
        {
            DualPull = new DualPull();
            DualPull.Pull.HP = 100;
        }

        public ClientInformation(String PlayerName, IPAddress IpAddress, int Port, Socket PlayerHandler)
        {
            this.PlayerName = PlayerName;
            this.IpAddress = IpAddress;
            this.Port = Port;
            this.PlayerHandler = PlayerHandler;

            DualPull = new DualPull();
        }

        public bool IsLive()
        {
            if (this.DualPull.Pull.HP > 0)
            {
                return true;
            }
            else
            {
                if (Respown == null)
                {
                    Respown = new CoolDown(100);
                    return false;
                }
                else 
                {
                    Respown.addTime();

                    if (Respown.isReload() == true)
                    {
                        Respown = null;

                        this.DualPull.Pull.HP = 100;

                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }            
        }
        public void Dead()
        {
            Respown = new CoolDown(100);
            this.DualPull.Pull.HP = 0;
        }
    }
}
