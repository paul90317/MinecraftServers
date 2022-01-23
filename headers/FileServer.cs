using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Windows.Forms;

namespace MinecraftServers.headers
{
    class FileServer
    {
        public static int SEND_SIZE = 1000000;
        DirectoryInfo Dir;
        public int port;
        public FileServer(DirectoryInfo dir,int _port)
        {
            Dir = dir;
            port = _port;
        }
        Socket tcpserver = null;
        void start()
        {
            tcpserver = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            EndPoint ep = new IPEndPoint(IPAddress.Any, port);
            tcpserver.Bind(ep);
            tcpserver.Listen(100);
            byte[] recv_buf = new byte[SEND_SIZE];
            byte[] send_buf;
            int len;
            while (true)
            {
                //接受一個 client;
                Socket client = tcpserver.Accept();
                len = client.Receive(recv_buf); 
                if (len == 0) break;
                string filename = Encoding.Unicode.GetString(recv_buf, 0, len);
                if (filename[0] == '#')
                {
                    string data = "";
                    FileInfo[] files = Dir.GetFiles();
                    for (int i = 0; i < files.Length; i++)
                    {
                        data += files[i].Name + "&";
                    }
                    send_buf = Encoding.Unicode.GetBytes(data);
                    client.Send(send_buf);
                    client.Close();
                    continue;
                }
                if (Dir.GetFiles(filename).Length == 0)
                {
                    client.Close();
                    continue;
                }
                using (BinaryReader br = new BinaryReader(File.Open(Dir.GetFiles(filename)[0].FullName, FileMode.Open)))
                {
                    while (true)
                    {
                        send_buf = br.ReadBytes(SEND_SIZE);
                        if (send_buf.Length == 0)
                        {
                            client.Close();
                            break;
                        }
                        else
                        {
                            client.Send(send_buf);
                        }
                    }
                }
            }
        }
        public void Start()
        {
            try
            {
                start();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        public void Stop()
        {
            if (tcpserver != null)
            {
                tcpserver.Close();
            }
        }
    }
}
