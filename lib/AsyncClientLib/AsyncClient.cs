using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Diagnostics;
using System.IO;

namespace AsyncClientLib
{
    public class AsyncClient
    {
        TcpClient mServer;

        IPAddress ip;
        ushort port;
        string username;

        public bool IsConnected { get; set; }

        public byte[] recievedMessageBuff = new byte[64];


        public void SendMessage(string username, string message)
        {
            if (mServer.Connected)
            {
                byte[] messageBuff = new byte[64];
                messageBuff = Encoding.ASCII.GetBytes($"{username}: {message}");

                try
                {
                    var stream = mServer.GetStream();
                    stream.Write(messageBuff, 0, messageBuff.Length);
                }
                catch (Exception e)
                {
                    Debug.WriteLine("\nAsyncClient Exception> SendMessage method: \n");
                    Debug.WriteLine(e.ToString());
                }
            }
            else
            {
                Debug.WriteLine("Message was not sent because you were not connected to a server");
            }
        }
        public async Task Connect(IPAddress ipAddr = null, ushort portNum = 0, string username = null)
        {
            if (ipAddr == null)
            {
                ip = IPAddress.Loopback;
            }
            else
            {
                ip = ipAddr;
            }
            if (portNum == 0)
            {
                port = 555;
            }
            else
            {
                port = portNum;
            }
            if (username == null)
            {
                this.username = "soxketsUser";
            }
            else
            {
                this.username = username;
            }

            mServer = new TcpClient();

            Debug.WriteLine($"AsyncClient> Connecting to {ip}:{port}...");

            try
            {
                await mServer.ConnectAsync(ip, port);

                // Send username
                byte[] usernameBuff = new byte[64];
                usernameBuff = Encoding.ASCII.GetBytes(this.username);
                mServer.GetStream().Write(usernameBuff, 0, usernameBuff.Length);

                IsConnected = true;
                Debug.WriteLine($"AsyncClient> Connected to {ip}:{port}!");
            }
            catch (Exception e)
            {
                Array.Clear(recievedMessageBuff, 0, recievedMessageBuff.Length);
                IsConnected = false;
                Debug.WriteLine("\nAsyncClient Exception> Connect method: \n");
                Debug.WriteLine(e.ToString());
            }
        }
        public async Task ReadMessage()
        {
            int numRet = await mServer.GetStream().ReadAsync(recievedMessageBuff, 0, recievedMessageBuff.Length);

            if (numRet <= 0)
            {
                Array.Clear(recievedMessageBuff, 0, recievedMessageBuff.Length);
                Debug.WriteLine($"AsyncClient> Server closed\n");
                Disconnect();
            }

            string message = Encoding.ASCII.GetString(recievedMessageBuff, 0, recievedMessageBuff.Length);

            Debug.WriteLine($"AsyncClient> {message}\n");
        }
        public void Disconnect()
        {
            if (mServer != null)
            {
                try
                {
                    IsConnected = false;
                    Array.Clear(recievedMessageBuff, 0, recievedMessageBuff.Length);
                    mServer.Close();
                    mServer.Dispose();
                }
                catch (Exception e)
                {
                    IsConnected = false;
                    Array.Clear(recievedMessageBuff, 0, recievedMessageBuff.Length);
                    Debug.WriteLine("\nAsyncClient Exception> Disconnect method: \n");
                    Debug.WriteLine(e.ToString());
                }
            }
        }
    }
}
