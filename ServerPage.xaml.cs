using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using AsyncServerLib;
using System.Net;
using System.Net.Sockets;
using System.Diagnostics;
using System.IO;
using System.Collections;

namespace Soxkets
{
    /// <summary>
    /// Server interface
    /// </summary>
    public partial class ServerPage : Page
    {
        Hashtable clients = new Hashtable();

        public AsyncServer server = new AsyncServer();
        IPAddress ip = null;
        ushort port = 0;

        string servernamePath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"/soxkets/servername.txt";
        string serverName;
        public bool startButtonIsOn;

        private bool AutoScroll = true;

        public ServerPage()
        {
            InitializeComponent();
            LoadSettings();
            LoadEvents();
        }

        // Client message
        private void HandleClientMessage(object sender, ClientMessageEventArgs e)
        {
            TextBlock message = new TextBlock();
            message.Foreground = Brushes.White;
            message.TextWrapping = TextWrapping.Wrap;
            message.FontSize = 14;
            message.Effect = new DropShadowEffect
            {
                ShadowDepth = 1,
                Direction = 330,
                Color = Colors.Black,
                Opacity = 0.7,
                BlurRadius = 2
            };

            message.Text = e.Message;
            Messages.Children.Add(message);

            server.SendToAll(e.Message, e.SenderEndPoint);
        }

        // New client event
        private void HandleClientConnected(object sender, ClientConnectedEventArgs e)
        {
            // Add new client to ClientStack StackPanel
            TextBlock newClient = new TextBlock();
            newClient.Text = e.NewClientUsername;
            newClient.ToolTip = e.NewClientIP;
            newClient.Foreground = Brushes.White;
            newClient.HorizontalAlignment = HorizontalAlignment.Center;
            newClient.Width = 195;
            newClient.TextAlignment = TextAlignment.Center;
            newClient.Margin = new Thickness(2.5);

            clients.Add(new Client(e.NewClientUsername, e.NewClientIP).IP, new Client(e.NewClientUsername, e.NewClientIP));
            Debug.WriteLine($"ServerPage> clients.Count = {clients.Count}");

            Debug.WriteLine("\n");
            Debug.WriteLine($"ServerPage> New client: {e.NewClientUsername}");
            Debug.WriteLine("\n");
            Debug.WriteLine($"ServerPage> New client IP: {e.NewClientIP}");
            Debug.WriteLine("\n");

            ClientStack.Children.Add(newClient);

            ConnectedTitle.Text = "Connected" + $" ({clients.Count})";
        }

        // Client disconnected event
        private void HandleClientDisconnected(object sender, ClientDisconnectedEventArgs e)
        {
            try
            {
                for (int i = 0; i < clients.Count; i++)
                {
                    if ((string)ClientStack.Children.OfType<TextBlock>().ElementAt(i).ToolTip == e.IP)
                    {
                        ClientStack.Children.RemoveAt(i);
                        Debug.WriteLine($"ServerPage> {e.IP} removed from ClientStack.Children");
                        i++;
                    }
                }

                clients.Remove(e.IP);
                Debug.WriteLine($"ServerPage> clients.Count = {clients.Count}");

                if (clients.Count > 0)
                {
                    ConnectedTitle.Text = "Connected" + $" ({clients.Count})";
                }
                else
                {
                    ConnectedTitle.Text = "Connected";
                }
            }
            catch (Exception excep)
            {
                Debug.WriteLine("\n");
                Debug.WriteLine($"ServerPage Exception> HandleClientDisconnected method");
                Debug.WriteLine("\n");
                Debug.WriteLine(excep.ToString());
            }
        }

        // Set-up
        private void LoadSettings()
        {
            startButtonIsOn = false;

            // Load username
            if (File.Exists(servernamePath))
            {
                serverName = File.ReadAllText(servernamePath);
                UsernameTextbox.Text = serverName;
            }
        }
        private void LoadEvents()
        {
            server.RaiseClientConnectedEvent += HandleClientConnected;
            server.RaiseClientDisconnectedEvent += HandleClientDisconnected;
            server.RaiseClientMessageEvent += HandleClientMessage;
        }

        // Start button
        private void StartButton_MouseUp(object sender, RoutedEventArgs e)
        {
            if (!startButtonIsOn)
            {
                try
                {
                    server.StartListening(ip, port);
                    StartButton.ToolTip = "Stops accepting connections and drops all existing ones";
                    StartButton.Content = "STOP";
                    startButtonIsOn = true;
                }
                catch (Exception excep)
                {
                    Debug.WriteLine("\nException in StartPage from StartButton: \n");
                    Debug.WriteLine(excep.ToString());

                    StartButton.ToolTip = "Starts accepting connections on the specified IP and port";
                    StartButton.Content = "START";
                    startButtonIsOn = false;
                }
            }
            else
            {
                server.Stop();
                StartButton.ToolTip = "Starts accepting connections on the specified IP and port";
                StartButton.Content = "START";
                startButtonIsOn = false;

                if (clients.Count > 0)
                {
                    ConnectedTitle.Text = "Connected" + $" ({clients.Count})";
                }
                else
                {
                    ConnectedTitle.Text = "Connected";
                }
            }
        }

        // Chatbox
        private void Chatbox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (startButtonIsOn)
                {
                    try
                    {
                        if (Chatbox.Text.Trim() != "")
                        {
                            if (serverName == null || string.IsNullOrEmpty(serverName))
                            {
                                serverName = "Server";
                            }
                            TextBlock message = new TextBlock();
                            message.Text = $"{serverName.Trim()}: " + Chatbox.Text.Trim();
                            message.Foreground = Brushes.Gray;
                            message.TextWrapping = TextWrapping.Wrap;
                            message.FontSize = 14;
                            message.Effect = new DropShadowEffect
                            {
                                ShadowDepth = 1,
                                Direction = 330,
                                Color = Colors.Black,
                                Opacity = 0.7,
                                BlurRadius = 2
                            };
                            server.SendToAll(message.Text);
                            Messages.Children.Add(message);
                            Chatbox.Text = "";
                        }
                    }
                    catch (Exception)
                    {
                        System.Diagnostics.Debug.WriteLine("Your message was not sent due to an exception");
                        TextBlock message = new TextBlock();
                        message.Text = "Your message was not sent due to an error";
                        message.Foreground = new SolidColorBrush(Color.FromArgb(255, 255, 80, 80));
                        message.TextWrapping = TextWrapping.Wrap;
                        message.FontSize = 14;
                        message.Effect = new DropShadowEffect
                        {
                            ShadowDepth = 1,
                            Direction = 330,
                            Color = Colors.Black,
                            Opacity = 0.7,
                            BlurRadius = 2
                        };
                        Messages.Children.Add(message);
                    }
                }
                else
                {
                    if (Chatbox.Text.Trim() != "")
                    {
                        System.Diagnostics.Debug.WriteLine("Your message was not sent because the server has not started");
                        TextBlock message = new TextBlock();
                        message.Text = "Your message was not sent because the server has not started";
                        message.Foreground = new SolidColorBrush(Color.FromArgb(255, 255, 80, 80));
                        message.TextWrapping = TextWrapping.Wrap;
                        message.FontSize = 14;
                        message.Effect = new DropShadowEffect
                        {
                            ShadowDepth = 1,
                            Direction = 330,
                            Color = Colors.Black,
                            Opacity = 0.7,
                            BlurRadius = 2
                        };
                        Messages.Children.Add(message);
                    }
                }
            }
        }

        // IP box
        private void IPTextbox_MouseEnter(object sender, MouseEventArgs e)
        {
            if (IPTextbox.Text == "IP")
            {
                IPTextbox.Text = "";
                IPTextbox.TextAlignment = TextAlignment.Left;
                IPTextbox.Foreground = Brushes.White;
            }
        }
        private void IPTextbox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (IPTextbox.Text == "")
            {
                IPTextbox.Text = "IP";
                IPTextbox.TextAlignment = TextAlignment.Center;
                IPTextbox.Foreground = Brushes.DarkGray;
                PortTextbox.ToolTip = null;
            }
            else if (IPAddress.TryParse(IPTextbox.Text, out IPAddress result))
            {
                IPTextbox.BorderBrush = new SolidColorBrush(Color.FromRgb(150, 150, 150));
                IPTextbox.ToolTip = null;
                ip = result;
            }
            else
            {
                IPTextbox.BorderBrush = Brushes.Red;
                IPTextbox.Text = "";
                IPTextbox.ToolTip = "Invalid IP: Enter a valid IP Address";
            }
        }
        private void IPTextbox_MouseLeave(object sender, MouseEventArgs e)
        {
            if (IPTextbox.Text == "" && !IPTextbox.IsFocused)
            {
                IPTextbox.Text = "IP";
                IPTextbox.TextAlignment = TextAlignment.Center;
                IPTextbox.Foreground = Brushes.DarkGray;
            }
        }

        // Port box
        private void PortTextbox_MouseEnter(object sender, MouseEventArgs e)
        {
            if (PortTextbox.Text == "Port")
            {
                PortTextbox.Text = "";
                PortTextbox.TextAlignment = TextAlignment.Left;
                PortTextbox.Foreground = Brushes.White;
            }
        }
        private void PortTextbox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (PortTextbox.Text == "")
            {
                PortTextbox.Text = "Port";
                PortTextbox.TextAlignment = TextAlignment.Center;
                PortTextbox.Foreground = Brushes.DarkGray;
                PortTextbox.ToolTip = null;
            }
            else if (ushort.TryParse(PortTextbox.Text, out ushort result))
            {
                PortTextbox.BorderBrush = new SolidColorBrush(Color.FromRgb(150, 150, 150));
                PortTextbox.ToolTip = null;
                port = result;
            }
            else
            {
                PortTextbox.BorderBrush = Brushes.Red;
                PortTextbox.Text = "";
                PortTextbox.ToolTip = "Invalid port: Enter a number greater than 0 and smaller than 65535";
            }
        }
        private void PortTextbox_MouseLeave(object sender, MouseEventArgs e)
        {
            if (PortTextbox.Text == "" && !PortTextbox.IsFocused)
            {
                PortTextbox.Text = "Port";
                PortTextbox.TextAlignment = TextAlignment.Center;
                PortTextbox.Foreground = Brushes.DarkGray;
            }
        }

        // Username box
        private void UsernameTextbox_MouseEnter(object sender, MouseEventArgs e)
        {
            if (UsernameTextbox.Text == "Username")
            {
                UsernameTextbox.Text = "";
                UsernameTextbox.TextAlignment = TextAlignment.Left;
                UsernameTextbox.Foreground = Brushes.White;
            }
        }
        private void UsernameTextbox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (UsernameTextbox.Text == "")
            {
                UsernameTextbox.Text = "Username";
                UsernameTextbox.TextAlignment = TextAlignment.Center;
                UsernameTextbox.Foreground = Brushes.DarkGray;
                UsernameTextbox.ToolTip = null;
            }
            else
            {
                UsernameTextbox.Foreground = Brushes.DarkGray;
                UsernameTextbox.TextAlignment = TextAlignment.Center;
                serverName = UsernameTextbox.Text.Trim();

                string appdataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

                if (!Directory.Exists(appdataPath + @"/soxkets"))
                {
                    Directory.CreateDirectory(appdataPath + @"/soxkets");
                }
                File.WriteAllText(servernamePath, serverName);
            }
        }
        private void UsernameTextbox_MouseLeave(object sender, MouseEventArgs e)
        {
            if (UsernameTextbox.Text == "" && !UsernameTextbox.IsFocused)
            {
                UsernameTextbox.Text = "Username";
                UsernameTextbox.TextAlignment = TextAlignment.Center;
                UsernameTextbox.Foreground = Brushes.DarkGray;
            }
            else
            {
                UsernameTextbox.Foreground = Brushes.White;
            }
        }
        private void UsernameTextbox_GotFocus(object sender, RoutedEventArgs e)
        {
            UsernameTextbox.Foreground = Brushes.White;
        }

        // Auto scroll
        private void _scrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            // User scroll event : set or unset auto-scroll mode
            if (e.ExtentHeightChange == 0)
            {   // Content unchanged : user scroll event
                if (_scrollViewer.VerticalOffset == _scrollViewer.ScrollableHeight)
                {   // Scroll bar is in bottom
                    // Set auto-scroll mode
                    AutoScroll = true;
                }
                else
                {   // Scroll bar isn't in bottom
                    // Unset auto-scroll mode
                    AutoScroll = false;
                }
            }

            // Content scroll event : auto-scroll eventually
            if (AutoScroll && e.ExtentHeightChange != 0)
            {   // Content changed and auto-scroll mode set
                // Autoscroll
                _scrollViewer.ScrollToVerticalOffset(_scrollViewer.ExtentHeight);
            }
        }
    }
}
