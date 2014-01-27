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

namespace TestNetWork_Server
{

    public delegate void AddLogs(String message);
    //public delegate void AddClient(SomeText obj);

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

            server = new Server(logsClient, logs);

            this.textBox1.ScrollBars = ScrollBars.Vertical;
            this.textBox2.ScrollBars = ScrollBars.Vertical;
        }

        private void button_start_Click(object sender, EventArgs e)
        {

            server.startGame();

            //server = new Server();

            //IPHostEntry ipHostA = Dns.Resolve(Dns.GetHostName());
            //IPAddress ipAddress = ipHostA.AddressList[0];
            //IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 5000);
            ////Console.WriteLine(ipAddress);
            //this.textBox1.Text += " [ip: " + ipAddress + "]";


            //Socket listener = new Socket(AddressFamily.InterNetwork,
            //    SocketType.Dgram, ProtocolType.Udp);


            //listener.Bind(localEndPoint);
            ////listener.Listen(10);

            ////Socket handler = listener.Accept();

            //while (true)
            //{
            //    byte[] bytes = new byte[1500];
            //    listener.Receive(bytes);

            //    //handler.Send(bytes);

            //    var obj = (List<SomeText>)SerializeB.deserialize(bytes);
            //    MessageBox.Show("new list");
            //    for (int i = 0; i < obj.Count; i++)
            //    {
            //        this.textBox1.Text += " message[ " + obj[i].year + " " + obj[i].message + " " + obj[i].pi + "]";

            //        MessageBox.Show(" message[ " + obj[i].year + " " + obj[i].message + " " + obj[i].pi + "]");
            //    }
            //}

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
            lock (locker)
            {
                String[] str = new String[this.textBox2.Lines.Length + 1];

                for (int i = 0; i < this.textBox2.Lines.Length; i++)
                {
                    str[i+1] = this.textBox2.Lines[i];
                }

                str[0] = message;

                this.textBox2.Lines = str;
            }
        }

    }
}
