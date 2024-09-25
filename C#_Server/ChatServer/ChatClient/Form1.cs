using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace ChatClient
{
    public partial class Form1 : Form
    {
        TcpClient clientSocket = new TcpClient();
        NetworkStream stream = default;
        string readData = null;

        public Form1()
        {
            InitializeComponent();
        }

        private void Msg()
        {
            textBox2.Text += $"{Environment.NewLine}>> {readData}";
        }

        private void connect_Click(object sender, System.EventArgs e)
        {
            readData = "Connect to Chat Server...";
            Msg();
            clientSocket.Connect("localhost", 3001);
            stream = clientSocket.GetStream();

            byte[] outStream = Encoding.UTF8.GetBytes($"{textBox1.Text}$");
            stream.Write(outStream, 0, outStream.Length);
            stream.Flush();

            Thread ctThread = new Thread(GetMessage);
            ctThread.Start();
        }

        private void GetMessage()
        {

        }

        private void send_Click(object sender, System.EventArgs e)
        {
            byte[] outStream = Encoding.UTF8.GetBytes($"{textBox3.Text}$");
            stream.Write(outStream, 0, outStream.Length);
            stream.Flush();
            textBox3.Clear();
        }
    }
}
