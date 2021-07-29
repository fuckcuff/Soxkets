using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using AsyncServerLib;
using System.Globalization;

namespace AsyncServerLib
{
    public class ClientConnectedEventArgs : EventArgs
    {
        public string NewClientIP { get; set; }
        public string NewClientUsername { get; set; }

        public ClientConnectedEventArgs(string newClientIP, string newClientUsername)
        {
            NewClientIP = newClientIP;
            NewClientUsername = newClientUsername;
        }
    }

    public class ClientDisconnectedEventArgs : EventArgs
    {
        public string IP { get; set; }

        public ClientDisconnectedEventArgs(string ip)
        {
            IP = ip;
        }
    }

    public class ClientMessageEventArgs : EventArgs
    {
        public string Sender { get; set; }
        public string Message { get; set; }
        public IPEndPoint SenderEndPoint { get; set; }

        public ClientMessageEventArgs(string message, string sender)
        {
            Sender = sender;
            Message = message;
            SenderEndPoint = CreateIPEndPoint(sender);
        }
        public static IPEndPoint CreateIPEndPoint(string endPoint)
        {
            string[] ep = endPoint.Split(':');
            if (ep.Length != 2) throw new FormatException("Invalid endpoint format");
            IPAddress ip;
            if (!IPAddress.TryParse(ep[0], out ip))
            {
                throw new FormatException("Invalid ip-adress");
            }
            int port;
            if (!int.TryParse(ep[1], NumberStyles.None, NumberFormatInfo.CurrentInfo, out port))
            {
                throw new FormatException("Invalid port");
            }
            return new IPEndPoint(ip, port);
        }
    }
}
