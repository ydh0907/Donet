using System;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;

namespace ReceiveData
{
    public partial class Form1 : Form
    {
        bool runFlags = true;

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Thread thread = new Thread(Work);
            thread.Start();
        }

        private void Work()
        {
            while (runFlags)
            {
                Thread.Sleep(1);
                foreach (var proc in Process.GetProcesses())
                {
                    if (proc.ProcessName.ToString() == "SendData")
                    {
                        runFlags = false;
                        if (InvokeRequired)
                            Invoke(new MethodInvoker(() => label2.Text = SendData.Form1.sendString));
                    }
                }
            }
        }
    }
}
