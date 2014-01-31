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
using Microsoft.Xna.Framework;

namespace TestNetWork_Server.NetWorkClass
{
    class Server
    {
        //List<ClientInformation> Clients;
        //List<BulletInfo> Bullets;

        GameController gameController;

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
        AddState addState;

        public Server(AddLogs addClient, AddLogs addLogs, AddState addState)
        {
            //Clients = new List<ClientInformation>();
            //Bullets = new List<BulletInfo>();

            gameController = new GameController();

            port = 5000;
            portIncrement = 1;

            timeTick = 20;

            ClientsLock = new object();

            this.addClient = addClient;
            this.addLogs = addLogs;
            this.addState = addState;

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

            startGameThread();

            while (true)
            {
                Socket handler = listener.Accept();

                byte[] bytes = new byte[1500];

                handler.Receive(bytes);

                IPAddress clientIPAddress = ((IPEndPoint)handler.RemoteEndPoint).Address;

                var obj = (StartPacket)SerializeB.deserialize(bytes);

                int privatePort = this.port + this.portIncrement;
                portIncrement++;

                this.addClient("Player [" + obj.username + "]" + " ip: [" + clientIPAddress.ToString() + ":" + privatePort + "]");

                ClientInformation newClient = new ClientInformation(obj.username, clientIPAddress,
                                                    privatePort, handler);

                newClient.DualPull.Pull = new PlayerInfo();
                newClient.DualPull.Pull.HP = 100;

                lock (this.ClientsLock)
                {
                    this.gameController.Clients.Add(newClient);
                }

                startGame(newClient);
            }
        }

        /// <summary>
        /// Функция рассылает всем клиентам сообщение о начале игры
        /// </summary>
        public void startGame()
        {
            //reciveFromPlayersThread.Start();

            StartPacket startPacket = new StartPacket();

            startPacket.username = "Go";

            // обходим всех клиентов и каждому посылаем сообщение
            lock (this.ClientsLock)
            {
                foreach (ClientInformation client in gameController.Clients)
                {
                    startPacket.port = client.Port;

                    byte[] bytes = SerializeB.serialize(startPacket);

                    client.PlayerHandler.Send(bytes);
                }
            }

            //sendToPlayersThread.Start();
        }

        /// <summary>
        /// Функция рассылает конкретному клиенту сообщение о начале игры
        /// </summary>
        public void startGame(ClientInformation client)
        {
            Thread.Sleep(1500);

            StartPacket startPacket = new StartPacket();

            startPacket.username = "Go";

            startPacket.port = client.Port;

            byte[] bytes = SerializeB.serialize(startPacket);

            client.PlayerHandler.Send(bytes);

        }

        /// <summary>
        /// Запуск потоков для отправи и приема сообщений от клиентов
        /// </summary>
        private void startGameThread()
        {
            reciveFromPlayersThread.Start();
            sendToPlayersThread.Start();
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
                byte[] bytes = new byte[150000];

                EndPoint clientEndPoint = new IPEndPoint(IPAddress.Any, 0);
                listener.ReceiveFrom(bytes, ref clientEndPoint);

                IPAddress clientIPAddress = ((IPEndPoint)clientEndPoint).Address;
                //int clientPort = ((IPEndPoint)clientEndPoint).Port;

                var obj = (PacketToServer)SerializeB.deserialize(bytes);

                int clientPort = (int)obj.Port;

                addLogs("resive player[" + clientIPAddress.ToString() + ":" + clientPort + "]");

                lock (this.ClientsLock)
                {
                    int i = 0;

                    while (i < gameController.Clients.Count) 
                    {
                        if (gameController.Clients[i].IpAddress.ToString() == clientIPAddress.ToString() && gameController.Clients[i].Port == clientPort)
                            break;
                        i++; 
                    }

                    if (i != gameController.Clients.Count)
                    {
                        gameController.Clients[i].DualPull.Pull = obj.PlayerInfo;

                        if (obj.BulletInfo != null)
                        {
                            gameController.Bullets.Add(obj.BulletInfo);
                        }
                    }
                    else 
                    {
                        MessageBox.Show("не найдет адресат в учетных записях!!!");
                    }
                }

                addState(gameController.Clients);

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
                    PacketFromServer toClient = new PacketFromServer();

                    // обновляем положение патронов и убираем вышедшые за пределы
                    // Расчет урона патронов и их удаление
                    gameController.GameStateUpdate();

                    // Изымаем текущие позиции игроков, Удаляет игроков из списка с hp = 0
                    toClient.PlayersInfo = gameController.GetPlayerInfo();
                    toClient.BulletInfo = gameController.Bullets;

                    // серелизуес пакет в байты
                    byte[] bytes = SerializeB.serialize(toClient);

                    // посылаем результат клиентам по UDP
                    SendToPlayers(bytes);
                }
            }
        }

        ///// <summary>
        ///// Обновляем положение патронов и убираем вышедшые за пределы
        ///// </summary>
        //private void BulletUpdate()
        //{
        //    foreach (BulletInfo bullet in Bullets)
        //    {
        //        bullet.move();
        //    }

        //    List<BulletInfo> newBullets = new List<BulletInfo>();

        //    foreach (BulletInfo bullet in Bullets)
        //    {
        //        if (bullet.Position.X < 800 && bullet.Position.X > -2)
        //        {
        //            newBullets.Add(bullet);
        //        }
        //    }

        //    Bullets = newBullets;        
        //}

        ///// <summary>
        ///// Расчет урона патронов и их удаление
        ///// </summary>
        //private void DamageCalculation()
        //{
        //    for (int i = 0; i < Bullets.Count; i++)
        //    {
        //        Rectangle obj = new Rectangle((int)Bullets[i].Position.X, (int)Bullets[i].Position.Y, 1, 1);

        //        for (int j = 0; j < Clients.Count; j++)
        //        {
        //            if (obj.Intersects(Clients[j].DualPull.Pull.Rectangle) == true)
        //            {
        //                if (Clients[j].DualPull.Pull.teamNumber != Bullets[i].teamNumber)
        //                {
        //                    if (Clients[j].DualPull.Pull.HP > 0)
        //                    {
        //                        Clients[j].DualPull.Pull.HP -= Bullets[i].Damage;
        //                        Bullets.RemoveAt(i);
        //                        i--;
        //                        break;
        //                    }
        //                }
        //            }
        //        }
        //    }
        //}

        ///// <summary>
        ///// Изымаем текущие позиции игроков, Удаляет игроков из списка с hp = 0
        ///// </summary>
        //private List<PlayerInfo> GetPlayerInfo()
        //{ 
        //    List<PlayerInfo> playerInfoList = new List<PlayerInfo>();

        //    foreach(ClientInformation client in Clients)
        //    {
        //        if(client.DualPull.Pull.HP > 0)
        //        {
        //            playerInfoList.Add(client.DualPull.ClearPull());
        //        }
        //    }

        //    return playerInfoList;
        //}

        /// <summary>
        /// Посылаем результат клиентам по UDP
        /// </summary>
        /// <param name="bytes"> Байты для посылки </param>
        private void SendToPlayers(byte[] bytes)
        {
            foreach (ClientInformation client in gameController.Clients)
            {
                DnsEndPoint remoteEP = new DnsEndPoint(client.IpAddress.ToString(), client.Port);

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
                socketUDP.Send(bytes);
            }
        }
    }
}
