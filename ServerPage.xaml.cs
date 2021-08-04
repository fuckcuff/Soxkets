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

        string appdataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        string servernamePath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"/.soxkets/servername.txt";
        string todayMessageLog = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"/.soxkets/logs/" + DateTime.Today.ToShortDateString().Replace(' ', '_').Replace('/', '-') + "_messageLog.txt";
        
        string serverName;
        public bool startButtonIsOn;

        private bool AutoScroll = true;

        public ServerPage() //! Add kick client option
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

            WriteToLogs($"({e.SenderEndPoint.ToString().Trim('\0', '\x00')}) {e.Message.ToString().Trim('\0', '\x00')}");
            server.SendToAll(e.Message, e.SenderEndPoint);
        }

        // New client event
        private void HandleClientConnected(object sender, ClientConnectedEventArgs e)
        {
            // Add new client to ClientStack StackPanel
            TextBlock newClient = new TextBlock();

            newClient.PreviewMouseRightButtonUp += new MouseButtonEventHandler(NewClient_PreviewRightMouseUp);

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

            WriteToLogs($"{e.NewClientUsername.Trim('\0', '\x00')} ({e.NewClientIP.Trim('\0', '\x00')}) connected");
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
            MakeSoxketsDir();
            MakeLogsDir();
            startButtonIsOn = false;

            // Load username
            if (File.Exists(servernamePath))
            {
                serverName = File.ReadAllText(servernamePath);
                UsernameTextbox.Text = serverName;
            }

            // Load Profile Picture
            if (File.Exists(appdataPath + @"/.soxkets/server_pfp.png"))
            {
                BitmapImage pfp = new BitmapImage();

                using (var stream = File.OpenRead(appdataPath + "/.soxkets/server_pfp.png"))
                {
                    pfp.BeginInit();
                    pfp.CacheOption = BitmapCacheOption.OnLoad;
                    pfp.StreamSource = stream;
                    pfp.EndInit();
                }

                PfpImg.Source = pfp;
                PfpImg.Stretch = Stretch.UniformToFill;
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

                    MakeSoxketsDir();
                    MakeLogsDir();

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
        private void Chatbox_KeyDown(object sender, KeyEventArgs e) //! Fix: Every message has profile picture included, TextBlock wont Wrap
        {
            if (e.Key == Key.Enter)
            {
                if (startButtonIsOn)
                {
                    try
                    {
                        if (Chatbox.Text.Trim() != "")
                        {
                            // Create profile picture 
                            BitmapImage msgPfpImg = new BitmapImage();
                            if (File.Exists(appdataPath + @"/.soxkets/server_pfp.png"))
                            {
                                using (var stream = File.OpenRead(appdataPath + "/.soxkets/server_pfp.png"))
                                {
                                    msgPfpImg.BeginInit();
                                    msgPfpImg.CacheOption = BitmapCacheOption.OnLoad;
                                    msgPfpImg.StreamSource = stream;
                                    msgPfpImg.EndInit();
                                }
                            }
                            else
                            {
                                msgPfpImg.BeginInit();
                                msgPfpImg.UriSource = new Uri(@"/Soxkets;component/res/defaultpfp.png", UriKind.Relative);
                                msgPfpImg.EndInit();
                            }
                            Image msgPfp = new Image();
                            msgPfp.Source = msgPfpImg;
                            msgPfp.MaxHeight = 32;
                            msgPfp.MaxWidth = 32;
                            msgPfp.MinHeight = 32;
                            msgPfp.MinWidth = 32;


                            // Create message profile picture frame
                            BitmapImage msgPfpFrameImg = new BitmapImage();
                            msgPfpFrameImg.BeginInit();
                            msgPfpFrameImg.UriSource = new Uri(@"/Soxkets;component/res/msgframe.png", UriKind.Relative);
                            msgPfpFrameImg.EndInit();
                            Image msgPfpFrame = new Image();
                            msgPfpFrame.Source = msgPfpFrameImg;
                            msgPfpFrame.MaxHeight = 46;
                            msgPfpFrame.MaxWidth = 46;
                            msgPfpFrame.MinHeight = 46;
                            msgPfpFrame.MinWidth = 46;
                            msgPfpFrame.HorizontalAlignment = HorizontalAlignment.Center;
                            msgPfpFrame.VerticalAlignment = VerticalAlignment.Center;

                            // Create Grid for Pfp and Pfp Frame
                            Grid pfpGrid = new Grid();
                            pfpGrid.HorizontalAlignment = HorizontalAlignment.Center;
                            pfpGrid.VerticalAlignment = VerticalAlignment.Top;
                            pfpGrid.MaxHeight = 34;
                            pfpGrid.MaxWidth = 34;
                            pfpGrid.Children.Add(msgPfp);
                            pfpGrid.Children.Add(msgPfpFrame);

                            // Create message TextBlock
                            if (serverName == null || string.IsNullOrEmpty(serverName))
                            {
                                serverName = "Server";
                            }
                            TextBlock messageTB = new TextBlock();
                            messageTB.Text = $"{serverName.Trim()}: " + Chatbox.Text.Trim();
                            messageTB.Foreground = Brushes.Gray;
                            messageTB.TextWrapping = TextWrapping.Wrap;
                            messageTB.FontSize = 14;
                            messageTB.VerticalAlignment = VerticalAlignment.Center;
                            messageTB.Margin = new Thickness(5, 0, 0, 0);
                            messageTB.Effect = new DropShadowEffect
                            {
                                ShadowDepth = 1,
                                Direction = 330,
                                Color = Colors.Black,
                                Opacity = 0.7,
                                BlurRadius = 2
                            };

                            // NEW MESSAGE (Pfp + Message)
                            DockPanel newMessage = new DockPanel();
                            newMessage.Name = "Local";

                            newMessage.Margin = new Thickness(5);
                            newMessage.HorizontalAlignment = HorizontalAlignment.Left;

                            if (Messages.Children.Count <= 0) // First message ever
                            {
                                newMessage.Children.Add(pfpGrid);
                                DockPanel.SetDock(newMessage.Children[newMessage.Children.Count - 1], Dock.Left);

                                /*
                                TextBlock servernameTB = new TextBlock();
                                servernameTB.Text = serverName;
                                servernameTB.Foreground = Brushes.White;
                                servernameTB.TextWrapping = TextWrapping.Wrap;
                                servernameTB.FontSize = 14;
                                servernameTB.VerticalAlignment = VerticalAlignment.Center;
                                servernameTB.Margin = new Thickness(5, 0, 0, 0);
                                servernameTB.Effect = new DropShadowEffect
                                {
                                    ShadowDepth = 1,
                                    Direction = 330,
                                    Color = Colors.Black,
                                    Opacity = 0.7,
                                    BlurRadius = 2
                                };

                                newMessage.Children.Add(servernameTB);
                                DockPanel.SetDock(newMessage.Children[newMessage.Children.Count - 1], Dock.Top);
                                */

                                newMessage.Children.Add(messageTB);
                                DockPanel.SetDock(newMessage.Children[newMessage.Children.Count - 1], Dock.Top);

                                Messages.Children.Add(newMessage);
                            }
                            else
                            {
                                if (Messages.Children[Messages.Children.Count - 1] is DockPanel msg)
                                {
                                    if (msg.Name == "Local")
                                    {
                                        msg.Children.Add(messageTB); // CANT add new TextBlock to already existing StackPanel msg
                                        DockPanel.SetDock(msg.Children[msg.Children.Count - 1], Dock.Top);
                                    }
                                    else
                                    {
                                        Debug.WriteLine("Previous message isnt local, adding new message (DockPanel) to Messages list");

                                        newMessage.Children.Add(pfpGrid);
                                        DockPanel.SetDock(newMessage.Children[newMessage.Children.Count - 1], Dock.Left);

                                        newMessage.Children.Add(messageTB); //message wont wrap here
                                        DockPanel.SetDock(newMessage.Children[newMessage.Children.Count - 1], Dock.Top);

                                        Messages.Children.Add(newMessage);
                                    }
                                }
                                else
                                {
                                    Debug.WriteLine("Previous message isnt a DockPanel, adding new message (DockPanel) to Messages list");

                                    newMessage.Children.Add(pfpGrid);
                                    DockPanel.SetDock(newMessage.Children[newMessage.Children.Count - 1], Dock.Left);

                                    /*
                                    TextBlock servernameTB = new TextBlock();
                                    servernameTB.Text = serverName;
                                    servernameTB.Foreground = Brushes.White;
                                    servernameTB.TextWrapping = TextWrapping.Wrap;
                                    servernameTB.FontSize = 14;
                                    servernameTB.VerticalAlignment = VerticalAlignment.Center;
                                    servernameTB.Margin = new Thickness(5, 0, 0, 0);
                                    servernameTB.Effect = new DropShadowEffect
                                    {
                                        ShadowDepth = 1,
                                        Direction = 330,
                                        Color = Colors.Black,
                                        Opacity = 0.7,
                                        BlurRadius = 2
                                    };

                                    newMessage.Children.Add(servernameTB);
                                    DockPanel.SetDock(newMessage.Children[newMessage.Children.Count - 1], Dock.Top);
                                    */

                                    newMessage.Children.Add(messageTB);
                                    DockPanel.SetDock(newMessage.Children[newMessage.Children.Count - 1], Dock.Top);

                                    Messages.Children.Add(newMessage);
                                }
                            }

                            WriteToLogs(messageTB.Text.Trim('\0', '\x00'));
                            server.SendToAll(messageTB.Text);
                            Chatbox.Text = "";
                        }
                    }
                    catch (Exception exc)
                    {
                        System.Diagnostics.Debug.WriteLine("Your message was not sent due to an exception\n");
                        System.Diagnostics.Debug.WriteLine(exc);
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

                MakeSoxketsDir();
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

        // Create directories
        private void MakeSoxketsDir()
        {
            if (!(Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"/.soxkets")))
            {
                Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"/.soxkets");
            }
        }
        private void MakeLogsDir()
        {
            MakeSoxketsDir();
            if (!(Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"/.soxkets/logs")))
            {
                Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"/.soxkets/logs");
            }
            if (!(File.Exists(todayMessageLog)))
            {
                Debug.WriteLine($"Attempting to create {todayMessageLog}");
                File.Create(todayMessageLog);
            }
        }
        private void WriteToLogs(string text)
        {
            MakeLogsDir();
            File.AppendAllText(todayMessageLog, $"({DateTime.Now}) Server Page: \"{text.Trim('\0', '\x00')}\"\n");
        }

        // Profile picture
        private void PfpImg_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                // Create OpenFileDialog
                Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

                // Set filter for file extension and default file extension
                //dlg.DefaultExt = ".png";
                dlg.Filter = "Images | *.jpg; *.jpeg; *.png; *.gif";

                // Display OpenFileDialog by calling ShowDialog method
                Nullable<bool> result = dlg.ShowDialog();

                // Get the selected file name and display in a TextBox
                if (result == true)
                {
                    // Open document
                    try
                    {
                        MakeSoxketsDir();

                        System.Diagnostics.Debug.WriteLine($"Copying {dlg.FileName} to {appdataPath + @"/.soxkets/server_pfp.png"}");
                        File.Copy(dlg.FileName, appdataPath + @"/.soxkets/server_pfp.png", true);

                        BitmapImage importedImage = new BitmapImage();

                        using (var stream = File.OpenRead(appdataPath + "/.soxkets/server_pfp.png"))
                        {
                            importedImage.BeginInit();
                            importedImage.CacheOption = BitmapCacheOption.OnLoad;
                            importedImage.StreamSource = stream;
                            importedImage.EndInit();
                        }

                        PfpImg.Source = importedImage;
                        PfpImg.Stretch = Stretch.UniformToFill;
                    }
                    catch
                    {
                        MessageBox.Show($"Could not convert selected file to image", "Soxkets", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else if (e.ChangedButton == MouseButton.Right)
            {
                ContextMenu menu = (ContextMenu)Resources["contextMenu"];
                menu.IsOpen = true;
            }
        }
        private void ResetPfp_Click(object sender, RoutedEventArgs e)
        {
            BitmapImage emptyPfp = new BitmapImage();
            emptyPfp.BeginInit();
            emptyPfp.UriSource = new Uri(@"/Soxkets;component/res/defaultpfp.png", UriKind.Relative);
            emptyPfp.EndInit();

            if (File.Exists(appdataPath + @"/.soxkets/server_pfp.png"))
            {
                File.Delete(appdataPath + @"/.soxkets/server_pfp.png");
            }

            PfpImg.Source = emptyPfp;
        }
        private void Image_Drop(object sender, DragEventArgs e)
        {
            try
            {
                if (e.Data.GetDataPresent(DataFormats.FileDrop))
                {
                    // Note that you can have more than one file.
                    string[] file = (string[])e.Data.GetData(DataFormats.FileDrop);

                    // Assuming you have one file that you care about, pass it off to whatever
                    // handling code you have defined.
                    BitmapImage importedImage = new BitmapImage();

                    using (var stream = File.OpenRead(file[0]))
                    {
                        importedImage.BeginInit();
                        importedImage.CacheOption = BitmapCacheOption.OnLoad;
                        importedImage.StreamSource = stream;
                        importedImage.EndInit();
                    }

                    System.Diagnostics.Debug.WriteLine($"Copying {file[0]} to {appdataPath + @"/.soxkets/server_pfp.png"}");
                    File.Copy(file[0], appdataPath + @"/.soxkets/server_pfp.png", true);

                    PfpImg.Source = importedImage;
                }
            }
            catch
            {
                MessageBox.Show($"Could not convert selected file to image", "Soxkets", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void Image_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.Bitmap))
            {
                e.Effects = DragDropEffects.Copy;
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }
        }

        // Kick a client
        private void NewClient_PreviewRightMouseUp(object sender, MouseButtonEventArgs e)
        {
            ContextMenu menu = (ContextMenu)Resources["manageUserMenu"];
            menu.IsOpen = true;
        }
        private void KickButton_Click(object sender, RoutedEventArgs e) // Not implemented
        {
            // client.Close();
            MessageBox.Show("This feature has not been implemented yet", "Soxkets");
        }
    }
}
