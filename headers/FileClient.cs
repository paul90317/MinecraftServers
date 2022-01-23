using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;
using System.Net;

namespace MinecraftServers.headers
{
    class FileClient
    {

        public string[] GetList()
        {
            byte[] data = System.Text.Encoding.Unicode.GetBytes("#");
            byte[] data_back = new byte[FileServer.SEND_SIZE];
            string sdata;
            Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPAddress ip = Dns.GetHostAddresses(host)[0];
            EndPoint ep = new IPEndPoint(ip, port);
            client.Connect(ep);
            client.Send(data);
            int len = client.Receive(data_back);
            sdata = System.Text.Encoding.Unicode.GetString(data_back, 0, len);
            client.Close();
            return sdata.Split('&');
        }
        public bool GetFile(string filename,string local_path)
        {
            try
            {
                byte[] send_buf;
                byte[] recv_buf = new byte[FileServer.SEND_SIZE];
                int len;
                Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                IPAddress ip = Dns.GetHostAddresses(host)[0];
                EndPoint ep = new IPEndPoint(ip, port);
                client.Connect(ep);
                string data = filename;
                send_buf = Encoding.Unicode.GetBytes(data);
                client.Send(send_buf);
                Stream s = useful.get_file_or_create(local_path).Create();
                using (BinaryWriter sw = new BinaryWriter(s))
                {
                    while (useful.IsConnected(client))
                    {
                        len = client.Receive(recv_buf);
                        sw.Write(recv_buf, 0, len);
                    }
                }
                client.Close();
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }
        string host;
        int port;
        public FileClient(string _host, int _port)
        {
            host = _host;
            port = _port;
        }
    }
}
