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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Net;
using System.Net.Sockets;
using AsyncClientLib;
using System.Windows.Media.Effects;
using System.IO;

namespace Soxkets
{
    /// <summary>
    /// Interaction logic for ClientPage.xaml
    /// </summary>
    public partial class ClientPage : Page
    {
        string usernamePath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"/soxkets/username.txt";
        string appdataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

        SolidColorBrush ErrorColor = new SolidColorBrush(Color.FromArgb(255, 255, 80, 80));

        public AsyncClient server = new AsyncClient();

        private bool AutoScroll = true;

        IPAddress ip = null;
        ushort port = 0;

        string userName;

        public ClientPage()
        {
            InitializeComponent();
            LoadSettings();


            // chatbox always starts with empty space at the top, pls fix (_scrollViewer is causing it)
        }

        // Load settings
        private void LoadSettings()
        {
            server.IsConnected = false;

            // Load username
            if (File.Exists(usernamePath))
            {
                userName = File.ReadAllText(usernamePath);
                ClientUsernameTextbox.Text = userName;
            }
        }

        // Connect button
        private async void ConnectButton_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (!server.IsConnected)
            {
                try
                {
                    ConnectButton.Foreground = Brushes.White;
                    await server.Connect(ip, port, userName);
                    if (server.IsConnected)
                    {
                        ConnectButton.ToolTip = "Disconnects from the connected server";
                        ConnectButton.Content = "DISCONNECT";
                        ConnectButton.Foreground = Brushes.White;
                        server.IsConnected = true;
                        while (server.IsConnected)
                        {
                            Array.Clear(server.recievedMessageBuff, 0, server.recievedMessageBuff.Length);
                            await server.ReadMessage();
                            string receivedMessage = Encoding.ASCII.GetString(server.recievedMessageBuff, 0, server.recievedMessageBuff.Length);

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

                            message.Text = receivedMessage.Trim();
                            if (server.IsConnected)
                            {
                                Messages.Children.Add(message);
                            }
                        }
                        LocalDisconnectMessage();
                        ConnectButton.ToolTip = "Attempts to connect to the specified IP and port";
                        ConnectButton.Content = "CONNECT";
                    }
                    else
                    {
                        ConnectButton.Foreground = ErrorColor;
                        ConnectButton.ToolTip = "Connection to specified server failed";
                    }
                }
                catch (Exception excep)
                {
                    System.Diagnostics.Debug.WriteLine("\nClientPage Exception> Connect button \n");
                    System.Diagnostics.Debug.WriteLine(excep.ToString());
                    ConnectButton.ToolTip = "Attempts to connect to the specified IP and port";
                    ConnectButton.Content = "CONNECT";
                    server.IsConnected = false;
                }
            }
            else
            {
                LocalDisconnectMessage();
                server.Disconnect();
                ConnectButton.ToolTip = "Attempts to connect to the specified IP and port";
                ConnectButton.Content = "CONNECT";
                server.IsConnected = false;
            }
        }

        // Chatbox
        private void Chatbox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (server.IsConnected == true)
                {
                    try
                    {
                        if (Chatbox.Text.Trim() != "")
                        {
                            if (userName == null || string.IsNullOrEmpty(userName))
                            {
                                userName = "You";
                            }
                            TextBlock message = new TextBlock();
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

                            message.Text = Chatbox.Text.Trim();

                            if (userName == "You")
                            {
                                server.SendMessage("Client", message.Text);
                            }
                            else
                            {
                                server.SendMessage(userName, message.Text);
                            }

                            message.Text = $"{userName.Trim()}: " + Chatbox.Text.Trim();
                            Messages.Children.Add(message);

                            Chatbox.Text = "";
                        }
                    }
                    catch (Exception)
                    {
                        System.Diagnostics.Debug.WriteLine("Your message was not sent due to an exception");
                        TextBlock message = new TextBlock();
                        message.Text = "Your message was not sent due to an error";
                        message.Foreground = ErrorColor;
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
                        System.Diagnostics.Debug.WriteLine("You are not connected to a server");
                        TextBlock message = new TextBlock();
                        message.Text = "You are not connected to a server";
                        message.Foreground = ErrorColor;
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
        private void ClientIPTextbox_MouseEnter(object sender, MouseEventArgs e)
        {
            if (ClientIPTextbox.Text == "IP")
            {
                ClientIPTextbox.Text = "";
                ClientIPTextbox.TextAlignment = TextAlignment.Left;
                ClientIPTextbox.Foreground = Brushes.White;
            }
        }
        private void ClientIPTextbox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (ClientIPTextbox.Text == "")
            {
                ClientIPTextbox.Text = "IP";
                ClientIPTextbox.TextAlignment = TextAlignment.Center;
                ClientIPTextbox.Foreground = Brushes.DarkGray;
            }
            else if (IPAddress.TryParse(ClientIPTextbox.Text, out IPAddress result))
            {
                ClientIPTextbox.BorderBrush = new SolidColorBrush(Color.FromRgb(150, 150, 150));
                ClientIPTextbox.ToolTip = null;
                ip = result;
            }
            else
            {
                ClientIPTextbox.BorderBrush = Brushes.Red;
                ClientIPTextbox.Text = "";
                ClientIPTextbox.ToolTip = "Invalid IP: Enter a valid IP Address";
            }
        }
        private void ClientIPTextbox_MouseLeave(object sender, MouseEventArgs e)
        {
            if (ClientIPTextbox.Text == "" && !ClientIPTextbox.IsFocused)
            {
                ClientIPTextbox.Text = "IP";
                ClientIPTextbox.TextAlignment = TextAlignment.Center;
                ClientIPTextbox.Foreground = Brushes.DarkGray;
            }
        }

        // Port box
        private void ClientPortTextbox_MouseEnter(object sender, MouseEventArgs e)
        {
            if (ClientPortTextbox.Text == "Port")
            {
                ClientPortTextbox.Text = "";
                ClientPortTextbox.TextAlignment = TextAlignment.Left;
                ClientPortTextbox.Foreground = Brushes.White;
            }
        }
        private void ClientPortTextbox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (ClientPortTextbox.Text == "")
            {
                ClientPortTextbox.Text = "Port";
                ClientPortTextbox.TextAlignment = TextAlignment.Center;
                ClientPortTextbox.Foreground = Brushes.DarkGray;
            }
            else if (ushort.TryParse(ClientPortTextbox.Text, out ushort result))
            {
                ClientPortTextbox.BorderBrush = new SolidColorBrush(Color.FromRgb(150, 150, 150));
                ClientPortTextbox.ToolTip = null;
                port = result;
            }
            else
            {
                ClientPortTextbox.BorderBrush = Brushes.Red;
                ClientPortTextbox.Text = "";
                ClientPortTextbox.ToolTip = "Invalid port: Enter a number greater than 0 and smaller than 65535";
            }
        }
        private void ClientPortTextbox_MouseLeave(object sender, MouseEventArgs e)
        {
            if (ClientPortTextbox.Text == "" && !ClientPortTextbox.IsFocused)
            {
                ClientPortTextbox.Text = "Port";
                ClientPortTextbox.TextAlignment = TextAlignment.Center;
                ClientPortTextbox.Foreground = Brushes.DarkGray;
            }
        }

        // Username box
        private void ClientUsernameTextbox_MouseEnter(object sender, MouseEventArgs e)
        {
            if (ClientUsernameTextbox.Text == "Username")
            {
                ClientUsernameTextbox.Text = "";
                ClientUsernameTextbox.TextAlignment = TextAlignment.Left;
                ClientUsernameTextbox.Foreground = Brushes.White;
            }
        }
        private void ClientUsernameTextbox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (ClientUsernameTextbox.Text == "")
            {
                ClientUsernameTextbox.Text = "Username";
                ClientUsernameTextbox.TextAlignment = TextAlignment.Center;
                ClientUsernameTextbox.Foreground = Brushes.DarkGray;
                ClientUsernameTextbox.ToolTip = null;
            }
            else
            {
                ClientUsernameTextbox.Foreground = Brushes.DarkGray;
                ClientUsernameTextbox.TextAlignment = TextAlignment.Center;
                userName = ClientUsernameTextbox.Text.Trim();
                
                if (!Directory.Exists(appdataPath + @"/soxkets"))
                {
                    Directory.CreateDirectory(appdataPath + @"/soxkets");
                }
                File.WriteAllText(usernamePath, userName);
            }
        }
        private void ClientUsernameTextbox_MouseLeave(object sender, MouseEventArgs e)
        {
            if (ClientUsernameTextbox.Text == "" && !ClientUsernameTextbox.IsFocused)
            {
                ClientUsernameTextbox.Text = "Username";
                ClientUsernameTextbox.TextAlignment = TextAlignment.Center;
                ClientUsernameTextbox.Foreground = Brushes.DarkGray;
            }
            else
            {
                ClientUsernameTextbox.Foreground = Brushes.White;
            }
        }
        private void ClientUsernameTextbox_GotFocus(object sender, RoutedEventArgs e)
        {
            ClientUsernameTextbox.Foreground = Brushes.White;
        }

        // Client side message
        private void LocalDisconnectMessage()
        {
            TextBlock disconnectMessage = new TextBlock();
            disconnectMessage.Text = "Disconnected";
            disconnectMessage.Foreground = ErrorColor;
            disconnectMessage.TextWrapping = TextWrapping.Wrap;
            disconnectMessage.FontSize = 14;
            disconnectMessage.Effect = new DropShadowEffect
            {
                ShadowDepth = 1,
                Direction = 330,
                Color = Colors.Black,
                Opacity = 0.7,
                BlurRadius = 2
            };
            Messages.Children.Add(disconnectMessage);
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
