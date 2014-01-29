using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using TestNetWork_Server.serialize;
using ClassesForSerialize;
using TestNetWork_Server.NetWorkClass;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace TestNetWork_Server
{

    public delegate void AddLogs(String message);
    public delegate void AddState(List<ClientInformation> clients);

    public partial class Form1 : Form
    {
        Server server;

        object locker = new object();

        public Form1()
        {
            InitializeComponent();

            System.Windows.Forms.Control.CheckForIllegalCrossThreadCalls = false;

            this.textBox1.Text = "";

            AddLogs logs = new AddLogs(addNewLogs);
            AddLogs logsClient = new AddLogs(addNewClient);
            AddState addState = new AddState(addNewPlayerState);

            server = new Server(logsClient, logs, addState);

            this.textBox1.ScrollBars = ScrollBars.Vertical;
            this.textBox2.ScrollBars = ScrollBars.Vertical;
            this.textBox3.ScrollBars = ScrollBars.Vertical;

            IPHostEntry ipHostA = Dns.Resolve(Dns.GetHostName());
            IPAddress ipAddress = ipHostA.AddressList[0];

            this.label_ip.Text = ipAddress.ToString();

        }

        private void button_start_Click(object sender, EventArgs e)
        {
            server.startGame();
        }
        
        public void addNewClient(String message)
        {
            String[] str = new String[this.textBox1.Lines.Length + 1];

            for (int i = 0; i < this.textBox1.Lines.Length; i++)
            {
                str[i] = this.textBox1.Lines[i];
            }

            str[this.textBox1.Lines.Length] = message;

            this.textBox1.Lines = str;                
        }

        public void addNewLogs(String message)
        {
            //lock (locker)
            //{
            //    String[] str = new String[this.textBox2.Lines.Length + 1];

            //    for (int i = 0; i < this.textBox2.Lines.Length; i++)
            //    {
            //        str[i+1] = this.textBox2.Lines[i];
            //    }

            //    str[0] = message;

            //    this.textBox2.Lines = str;
            //}
        }

        public void addNewPlayerState(List<ClientInformation> clients)
        {
            //lock (locker)
            //{
            //    String[] str = new String[clients.Count];

            //    for (int i = 0; i < clients.Count; i++)
            //    {
            //        str[i] = "Player " + clients [i].PlayerName + " [ X = " + clients[i].InfoPull.Pull.Position.X
            //            + " Y = " + clients[i].InfoPull.Pull.Position.Y + " ] " + "SpeedH = " + clients[i].InfoPull.Pull.Speed
            //            + " SpeedV = " + clients[i].InfoPull.Pull.VerticalSpeed;

            //    }

            //    this.textBox3.Lines = str;
            //}
        }
    }
}
