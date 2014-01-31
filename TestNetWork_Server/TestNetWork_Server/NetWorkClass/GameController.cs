using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ClassesForSerialize;
using Microsoft.Xna.Framework;

namespace TestNetWork_Server.NetWorkClass
{
    class GameController
    {
        public List<ClientInformation> Clients { get; set; }
        public List<BulletInfo> Bullets { get; set; }

        public GameController()
        {
            Clients = new List<ClientInformation>();
            Bullets = new List<BulletInfo>();
        }

        /// <summary>
        /// Обновляем положения элементов игры 
        /// </summary>
        public void GameStateUpdate()
        {
            BulletUpdate();
            DamageCalculation();
        }

        /// <summary>
        /// Обновляем положение патронов и убираем вышедшые за пределы
        /// </summary>
        private void BulletUpdate()
        {
            foreach (BulletInfo bullet in Bullets)
            {
                bullet.move();
            }

            List<BulletInfo> newBullets = new List<BulletInfo>();

            foreach (BulletInfo bullet in Bullets)
            {
                if (bullet.Position.X < 800 && bullet.Position.X > -2)
                {
                    newBullets.Add(bullet);
                }
            }

            Bullets = newBullets;
        }

        /// <summary>
        /// Расчет урона патронов и их удаление
        /// </summary>
        private void DamageCalculation()
        {
            for (int i = 0; i < Bullets.Count; i++)
            {
                Rectangle obj = new Rectangle((int)Bullets[i].Position.X, (int)Bullets[i].Position.Y, 1, 1);

                for (int j = 0; j < Clients.Count; j++)
                {
                    if (obj.Intersects(Clients[j].DualPull.Pull.Rectangle) == true)
                    {
                        if (Clients[j].DualPull.Pull.teamNumber != Bullets[i].teamNumber)
                        {
                            if (Clients[j].DualPull.Pull.HP > 0)
                            {
                                Clients[j].DualPull.Pull.HP -= Bullets[i].Damage;
                                Bullets.RemoveAt(i);

                                if (Clients[j].DualPull.Pull.HP <= 0)
                                {
                                    Clients[j].Dead();
                                }

                                i--;
                                break;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Изымаем текущие позиции игроков, Удаляет игроков из списка с hp = 0
        /// </summary>
        public List<PlayerInfo> GetPlayerInfo()
        {
            List<PlayerInfo> playerInfoList = new List<PlayerInfo>();

            foreach (ClientInformation client in Clients)
            {
                if (client.IsLive() == true)
                {
                    playerInfoList.Add(client.DualPull.ClearPull());
                }
            }

            return playerInfoList;
        }
    }
}
