using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Windows.Forms;

namespace SimpleDirectoryServer
{
    class Framer
    {
        public static string nextFrame(Socket sock, string delimiter)
        {
            byte[] nextByte = new byte[1];
            string token="", buffer;
            while (sock.Receive(nextByte, 1, SocketFlags.None) > 0)
            {
                buffer = Encoding.ASCII.GetString(nextByte);
                if (buffer.Equals(delimiter))
                {
                    if (sock.Available > 0)
                    {
                        sock.Receive(nextByte, 1, SocketFlags.Peek);
                        if (Encoding.ASCII.GetString(nextByte).Equals(delimiter))
                            token += buffer;
                    }
                    return token;
                }
                token += buffer;
            }
            return token;
        }
       
    }
}
