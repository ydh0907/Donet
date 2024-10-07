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
        bool isRunning = false;
        object locker = new object();

        public Form1()
        {
            InitializeComponent();
        }

        private void Msg()
        {
            textBox2.AppendText($"{Environment.NewLine}>> {readData}");
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

            isRunning = true;
            Thread ctThread = new Thread(GetMessage);
            ctThread.Start();
        }

        private void GetMessage()
        {
            byte[] buffer = new byte[1024];
            string message = string.Empty;
            try
            {
                while (isRunning)
                {
                    int numBytesRead;
                    while (stream.DataAvailable)
                    {
                        numBytesRead = stream.Read(buffer, 0, buffer.Length);
                        message = Encoding.UTF8.GetString(buffer, 0, numBytesRead);
                        lock (locker)
                        {
                            readData = message;
                            Msg();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                isRunning = false;
                message = ex.Message;
            }
        }

        private void send_Click(object sender, System.EventArgs e)
        {
            byte[] outStream = Encoding.UTF8.GetBytes($"{textBox3.Text}$");
            stream.Write(outStream, 0, outStream.Length);
            stream.Flush();
            textBox3.Clear();
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            isRunning = false;
            stream.Close();
        }
    }
}
