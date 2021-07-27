using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Soxkets
{
    public class Client
    {
        public string Username { get; set; }
        public string IP { get; set; }

        public Client(string username, string ip)
        {
            Username = username;
            IP = ip;
        }
    }
}
