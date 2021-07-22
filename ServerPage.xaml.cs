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

namespace Soxkets
{
    /// <summary>
    /// Server interface
    /// </summary>
    public partial class ServerPage : Page
    {
        AsyncServer server = new AsyncServer();

        IPAddress ip = null;
        ushort port = 0;

        string serverName;
        public bool startButtonIsOn;

        public ServerPage()
        {
            InitializeComponent();
            startButtonIsOn = false;
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
                    //InterfaceToggle.Source = new BitmapImage(new Uri(@"C:\Users\demented\Desktop\C#Programs\Soxkets\iconactivated_25x.png"));
                    startButtonIsOn = true;

                    /*
                    while (startButtonIsOn)
                    {
                        string receivedMessage = new string(server.recievedBuff, 0, server.recievedBuff.Length);

                        TextBlock message = new TextBlock();
                        message.Foreground = Brushes.White;
                        message.TextWrapping = TextWrapping.Wrap;
                        message.Effect = new DropShadowEffect
                        {
                            ShadowDepth = 1,
                            Direction = 330,
                            Color = Colors.Black,
                            Opacity = 0.7,
                            BlurRadius = 2
                        };

                        message.Text = receivedMessage.Trim();
                        Messages.Children.Add(message);
                    }
                    */
                }
                catch (Exception excep)
                {
                    Debug.WriteLine("\nException in StartPage from StartButton: \n");
                    Debug.WriteLine(excep.ToString());

                    StartButton.ToolTip = "Starts accepting connections on the specified IP and port";
                    StartButton.Content = "START";
                    //InterfaceToggle.Source = new BitmapImage(new Uri(@"C:\Users\demented\Desktop\C#Programs\Soxkets\icon_25x.png"));
                    startButtonIsOn = false;
                }
            }
            else
            {
                server.Stop();
                StartButton.ToolTip = "Starts accepting connections on the specified IP and port";
                StartButton.Content = "START";
                //InterfaceToggle.Source = new BitmapImage(new Uri(@"C:\Users\demented\Desktop\C#Programs\Soxkets\icon_25x.png"));
                startButtonIsOn = false;
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
                UsernameTextbox.TextAlignment = TextAlignment.Left;
                serverName = UsernameTextbox.Text.Trim();
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
        }
    }
}
