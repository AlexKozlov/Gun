using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using TestNetWork_Server.serialize;
using ClassesForSerialize;
using System.Windows.Forms;
using System.Threading;

namespace TestNetWork_Server.NetWorkClass
{
    class Server
    {
        List<ClientInformation> Clients;

        /// <summary>
        /// стандартный порт для TCP а также начало портов для UDP портов
        /// </summary>
        int port;
        /// <summary>
        /// смещение относительно стартового порта для портов UDP
        /// </summary>
        int portIncrement;

        /// <summary>
        /// Мьютекс для доступа к списку учетных записей
        /// </summary>
        Object ClientsLock;

        Thread connectionPlayersThread;
        Thread reciveFromPlayersThread;
        Thread sendToPlayersThread;

        /// <summary>
        /// время между двумя отправками сообщений игрокам
        /// </summary>
        int timeTick;

        AddLogs addClient;
        AddLogs addLogs;

        public Server(AddLogs addClient, AddLogs addLogs)
        {
            Clients = new List<ClientInformation>();

            port = 5000;
            portIncrement = 1;

            timeTick = 3000;

            ClientsLock = new object();

            this.addClient = addClient;
            this.addLogs = addLogs;


            connectionPlayersThread = new Thread(connectionPlayers);
            reciveFromPlayersThread = new Thread(reciveFromPlayers);
            sendToPlayersThread = new Thread(sendToPlayers);

            connectionPlayersThread.Start();            
        }
        
        /// <summary>
        /// Функция для потока, отслеживает новые подключения игроков, создает для них учетные записи
        /// </summary>
        private void connectionPlayers()
        {
            IPHostEntry ipHostA = Dns.Resolve(Dns.GetHostName());
            IPAddress ipAddress = ipHostA.AddressList[0];

            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, this.port);

            Socket listener = new Socket(AddressFamily.InterNetwork,
                SocketType.Stream, ProtocolType.Tcp);

            listener.Bind(localEndPoint);
            listener.Listen(10);

            while (true)
            {
                Socket handler = listener.Accept();

                byte[] bytes = new byte[1500];

                handler.Receive(bytes);

                IPAddress clientIPAddress = ((IPEndPoint)handler.RemoteEndPoint).Address;

                var obj = (SomeText)SerializeB.deserialize(bytes);

                int privatePort = this.port + this.portIncrement;
                portIncrement++;

                this.addClient("Player [" + obj.message + "]" + " ip: [" + clientIPAddress.ToString() + ":" + privatePort + "]");

                ClientInformation newClient = new ClientInformation(obj.message, clientIPAddress,
                                                    privatePort, handler);
                newClient.Pull.setPull(new SomeText(32,"lolka",3.14));                

                lock (this.ClientsLock)
                {
                    this.Clients.Add(newClient);
                }
            }
        }

        /// <summary>
        /// Функция принимает от клиетнов сообщения с обновленной информацией о их местонахождении и
        /// сохраняет ее в текущий пулл
        /// </summary>
        private void reciveFromPlayers()
        {
            IPHostEntry ipHostA = Dns.Resolve(Dns.GetHostName());
            IPAddress ipAddress = ipHostA.AddressList[0];
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 5000);

            Socket listener = new Socket(AddressFamily.InterNetwork,
                SocketType.Dgram, ProtocolType.Udp);

            listener.Bind(localEndPoint);

            while (true)
            {
                byte[] bytes = new byte[1500];

                EndPoint clientEndPoint = new IPEndPoint(IPAddress.Any, 0);
                listener.ReceiveFrom(bytes, ref clientEndPoint);

                IPAddress clientIPAddress = ((IPEndPoint)clientEndPoint).Address;
                //int clientPort = ((IPEndPoint)clientEndPoint).Port;
                
                var obj = (SomeText)SerializeB.deserialize(bytes);

                int clientPort = (int)obj.pi;

                addLogs("resive player[" + clientIPAddress.ToString() + ":" + clientPort + "] new message: " + obj.year);

                lock (this.ClientsLock)
                {
                    int i = 0;

                    while ( i < Clients.Count) 
                    {
                        if(Clients[i].IpAddress.ToString() == clientIPAddress.ToString() && Clients[i].Port == clientPort)
                            break;
                        i++; 
                    }

                    if (i != Clients.Count)
                    {
                        Clients[i].Pull.setPull(obj);
                    }
                    else 
                    {
                        MessageBox.Show("не найдет адресат в учетных записях!!!");
                    }
                }

            }
        }

        /// <summary>
        /// Функция рассылает информацию о игре всем клиентам каждые n секунд
        /// </summary>
        private void sendToPlayers()
        {
            while (true)
            {
                Thread.Sleep(timeTick);                

                lock (this.ClientsLock)
                {
                    string str = "Send: [";                    

                    List<SomeText> someTextList = new List<SomeText>();

                    foreach (ClientInformation client in Clients)
                    {
                        str += "$";

                        SomeText obj = client.Pull.getPull();

                        str += obj.message + "_";

                        someTextList.Add(obj);
                    }

                    str += "] ";

                    byte[] bytes = SerializeB.serialize(someTextList);
                    
                    foreach (ClientInformation client in Clients)
                    {

                        DnsEndPoint remoteEP = new DnsEndPoint(client.IpAddress.ToString(), client.Port);
                        //MessageBox.Show(client.IpAddress.ToString());
                        Socket socketUDP = new Socket(AddressFamily.InterNetwork,
                            SocketType.Dgram, ProtocolType.Udp);

                        try
                        {
                            socketUDP.Connect(remoteEP);
                        }
                        catch (Exception exp)
                        {
                            MessageBox.Show(exp.ToString());
                        }
                        str += " plr[" + client.IpAddress.ToString() + ":" + client.Port + "] ";
                        socketUDP.Send(bytes);
                    }

                    addLogs(str);
                }
            }
        }

        /// <summary>
        /// Функция рассылает всем клиентам сообщение о начале игры
        /// </summary>
        public void startGame()
        {
            reciveFromPlayersThread.Start();

            SomeText gogogo = new SomeText();

            gogogo.message = "Go";
            gogogo.pi = 3.14;


            // обходим всех клиентов и каждому посылаем сообщение
            lock (this.ClientsLock)
            {
                foreach (ClientInformation client in Clients)
                {
                    gogogo.year = client.Port;

                    byte[] bytes = SerializeB.serialize(gogogo);

                    client.PlayerHandler.Send(bytes);
                }
            }

            sendToPlayersThread.Start();
        }
    }
}
