using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
namespace Client2
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        Socket server;
        bool ok = false;
        string input, stringData;
        NetworkStream ns;
        private void SendMsgBTN_Click(object sender, EventArgs e)
        {
            byte[] data = new byte[1024];
            int recv;
            if (!ok)
            {
                IPEndPoint ipep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 9050);
                server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                try
                {
                    server.Connect(ipep);
                }
                catch (SocketException ex)
                {
                    InfoTxt.Text += "Unable to connect to server.\n";
                    InfoTxt.Text += ex.ToString() + "\n";
                    return;
                }
                IPTxt.Text = "127.0.0.1";
                ns = new NetworkStream(server);
                if (ns.CanRead)
                {
                    recv = ns.Read(data, 0, data.Length);
                    stringData = Encoding.ASCII.GetString(data, 0, recv);
                    InfoTxt.Text += stringData + "\n";
                }
                else
                {
                    InfoTxt.Text += "Error: Can't read from this socket\n";
                    ns.Close();
                    server.Close();
                    return;
                }
                MsgTxt.ReadOnly = false;
                MsgTxt.Text = "Connecting to server: 127.0.0.1";
                SendMsgBTN.Text = "Send Message";
                ok = true;
            }
            input = MsgTxt.Text;
            if (ns.CanWrite)
            {
                ns.Write(Encoding.ASCII.GetBytes(input), 0, input.Length);
                ns.Flush();
            }
            recv = ns.Read(data, 0, data.Length);
            stringData = Encoding.ASCII.GetString(data, 0, recv);
            InfoTxt.Text += stringData + "\n";
            // Encode the data string into a byte array+
            if (MsgTxt.Text == "exit")
            {
                SendMsgBTN.Text = "Connect";
                MsgTxt.ReadOnly = true;
                InfoTxt.Text += "Disconnecting from server...\n";
                ns.Close();
                server.Shutdown(SocketShutdown.Both);
                server.Close();
                ok = false;
            }
            MsgTxt.Clear();
        }
    }
}