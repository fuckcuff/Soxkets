using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Diagnostics;
using System.IO;

namespace AsyncServerLib
{
    public class AsyncServer
    {
        TcpListener listener;
        IPAddress ip;
        ushort port;

        public EventHandler<ClientConnectedEventArgs> RaiseClientConnectedEvent;
        public EventHandler<ClientDisconnectedEventArgs> RaiseClientDisconnectedEvent;
        public EventHandler<ClientMessageEventArgs> RaiseClientMessageEvent;

        public List<TcpClient> clients = new List<TcpClient>();

        public bool KeepAccepting { get; set; }

        public char[] receivedBuff = new char[64];

        async public void StartListening(IPAddress ipAddr = null, ushort portNum = 0)
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

            listener = new TcpListener(ip, port);

            try
            {
                listener.Start();

                Debug.WriteLine($"AsyncServer> Listening on {ip}:{port} for incoming connections...\n");

                KeepAccepting = true;

                while (KeepAccepting)
                {
                    var newClient = await listener.AcceptTcpClientAsync();

                    clients.Add(newClient);

                    // New client event
                    ClientConnectedEventArgs eaClientConnected;
                    eaClientConnected = new ClientConnectedEventArgs(newClient.Client.RemoteEndPoint.ToString(), await GetUsername(newClient));
                    OnRaiseClientConnectedEvent(eaClientConnected);

                    Debug.WriteLine($"AsyncServer> {newClient.Client.RemoteEndPoint} connected!\n");

                    ReadClientMessage(newClient); // this activates for each new client that connects
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("\nAsyncServer Exception> StartListening method: \n");
                
                Debug.WriteLine(e.ToString());
            }
        }

        public void Stop()
        {
            try
            {
                if (listener != null)
                {
                    listener.Stop();
                    KeepAccepting = false; // why was this commented out
                }
                foreach (TcpClient client in clients)
                {
                    ClientDisconnectedEventArgs eaClientDisconnected;
                    eaClientDisconnected = new ClientDisconnectedEventArgs(client.Client.RemoteEndPoint.ToString());
                    OnRaiseClientDisconnectedEvent(eaClientDisconnected);

                    client.Close();
                }
                clients.Clear();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
            }
        }

        public async Task<string> GetUsername(TcpClient client)
        {
            NetworkStream stream = null;
            StreamReader reader = null;

            try
            {
                stream = client.GetStream();
                reader = new StreamReader(stream);

                int numRet = await reader.ReadAsync(receivedBuff, 0, receivedBuff.Length); // waits for new message

                if (numRet <= 0)
                {
                    if (clients.Contains(client))
                    {
                        clients.Remove(client);
                    }
                    Debug.WriteLine($"AsyncServer> Failed to get {client.Client.RemoteEndPoint}'s username\n");
                    return "error";
                }

                string receivedUsername = new string(receivedBuff);
                Debug.WriteLine($"AsyncServer> Received {client.Client.RemoteEndPoint}'s username: {receivedUsername}");
                Debug.WriteLine("\n");

                return receivedUsername;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
                return "error";
            }
        }

        public async void ReadClientMessage(TcpClient client)
        {
            NetworkStream stream = null;
            StreamReader reader = null;

            try
            {
                stream = client.GetStream();
                reader = new StreamReader(stream);

                while (KeepAccepting)
                {
                    int numRet = await reader.ReadAsync(receivedBuff, 0, receivedBuff.Length); // waits for new message

                    if (numRet <= 0)
                    {
                        if (clients.Contains(client))
                        {
                            clients.Remove(client);
                        }

                        OnRaiseClientDisconnectedEvent(new ClientDisconnectedEventArgs(client.Client.RemoteEndPoint.ToString()));

                        Debug.WriteLine($"AsyncServer> {client.Client.RemoteEndPoint} disconnected\n");
                        Debug.WriteLine($"AsyncServer> Client count: {clients.Count}\n");
                        break;
                    }

                    string message = new string(receivedBuff);

                    OnRaiseClientMessageEvent(new ClientMessageEventArgs(message, client.Client.RemoteEndPoint.ToString()));

                    Debug.WriteLine($"AsyncServer> {message}\n");

                    Array.Clear(receivedBuff, 0, receivedBuff.Length);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
            }
        }

        public void SendToAll(string message, IPEndPoint senderIP = null)
        {
            byte[] messageBuff = new byte[64];
            messageBuff = Encoding.ASCII.GetBytes(message);
            
            if (senderIP == null)
            {
                foreach (TcpClient client in clients)
                {
                    NetworkStream stream = client.GetStream();

                    stream.Write(messageBuff, 0, messageBuff.Length);
                }
            }
            else
            {
                Debug.WriteLine($"AsyncServer> Message from {senderIP}");
                foreach (TcpClient client in clients)
                {
                    if (client.Client.RemoteEndPoint.ToString() == senderIP.ToString())
                    {
                        Debug.WriteLine($"AsyncServer> Skipping {senderIP} because they are the sender");
                    }
                    else
                    {
                        NetworkStream stream = client.GetStream();

                        stream.Write(messageBuff, 0, messageBuff.Length);

                        Debug.WriteLine($"AsyncServer> Sending message from {senderIP} to {client.Client.RemoteEndPoint}");
                    }
                }
            }
            Array.Clear(messageBuff, 0, messageBuff.Length);

        }

        // Events
        protected virtual void OnRaiseClientMessageEvent(ClientMessageEventArgs e)
        {
            EventHandler<ClientMessageEventArgs> handler = RaiseClientMessageEvent;

            if (handler != null)
            {
                handler(this, e);
            }
        }
        protected virtual void OnRaiseClientConnectedEvent(ClientConnectedEventArgs e)
        {
            EventHandler<ClientConnectedEventArgs> handler = RaiseClientConnectedEvent;

            if (handler != null)
            {
                handler(this, e);
            }
        }
        protected virtual void OnRaiseClientDisconnectedEvent(ClientDisconnectedEventArgs e)
        {
            EventHandler<ClientDisconnectedEventArgs> handler = RaiseClientDisconnectedEvent;

            if (handler != null)
            {
                handler(this, e);
            }
        }
    }
}
