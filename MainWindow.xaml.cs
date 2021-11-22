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
using System.Windows.Shell;

namespace Soxkets
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // Pages
        ServerPage serverPage = new ServerPage();
        ClientPage clientPage = new ClientPage();

        string currentInterface;

        // Main
        public MainWindow() //! Add profile button for profile picture option
        {
            InitializeComponent();
            LoadSettings();
        }

        // Interface toggle button
        private void InterfaceToggle_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (currentInterface == "server")
            {
                Main.Content = null;
                Main.Content = clientPage;
                currentInterface = "client";
            }
            else if (currentInterface == "client")
            {
                Main.Content = null;
                Main.Content = serverPage;
                currentInterface = "server";
            }
        }

        // Close button
        private void CloseButton_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (serverPage.server.KeepAccepting == true)
            {
                var Result = MessageBox.Show("Your server is running, are you sure you want to exit?", "Soxkets", MessageBoxButton.YesNo, MessageBoxImage.Exclamation);
                if (Result == MessageBoxResult.Yes)
                {
                    clientPage.server.Disconnect();
                    serverPage.server.Stop();
                    Close();
                }
            }
            else
            {
                clientPage.server.Disconnect();
                serverPage.server.Stop();
                Close();
            }
        }
        private void CloseBBG_MouseEnter(object sender, MouseEventArgs e)
        {
            CloseBBG.Fill = new SolidColorBrush(Color.FromArgb(255, 80, 80, 80));
        }
        private void CloseBBG_MouseLeave(object sender, MouseEventArgs e)
        {
            CloseBBG.Fill = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
        }

        // Minimize button
        private void MinimizeBBG_MouseEnter(object sender, MouseEventArgs e)
        {
            MinimizeBBG.Fill = new SolidColorBrush(Color.FromArgb(255, 80, 80, 80));
        }
        private void MinimizeBBG_MouseLeave(object sender, MouseEventArgs e)
        {
            MinimizeBBG.Fill = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
        }
        private void MinimizeButton_MouseUp(object sender, MouseButtonEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        // Maximize button
        private void MaximizeBBG_MouseEnter(object sender, MouseEventArgs e)
        {
            MaximizeBBG.Fill = new SolidColorBrush(Color.FromArgb(255, 80, 80, 80));
        }
        private void MaximizeBBG_MouseLeave(object sender, MouseEventArgs e)
        {
            MaximizeBBG.Fill = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
        }
        protected void MaximizeButton_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (WindowState == WindowState.Maximized)
            {
                WindowChrome.GetWindowChrome(this).ResizeBorderThickness = new Thickness(10);
                WindowState = WindowState.Normal;
            }
            else
            {
                WindowChrome.GetWindowChrome(this).ResizeBorderThickness = new Thickness(0);
                WindowState = WindowState.Maximized;
            }
        }

        // Window tab, dragging & maximizing
        private void Rectangle_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (e.ChangedButton == MouseButton.Left && e.ClickCount > 1)
                {
                    MaximizeButton_MouseUp(sender, e);
                }
                else
                {
                    if (WindowState == WindowState.Maximized && e.LeftButton == MouseButtonState.Pressed)
                    {
                        WindowState = WindowState.Normal;
                        WindowChrome.GetWindowChrome(this).ResizeBorderThickness = new Thickness(10);
                        Left = Mouse.GetPosition(this).X - 200;
                        Top = Mouse.GetPosition(this).Y - 30;
                    }

                    DragMove();

                    if (WindowState == WindowState.Maximized)
                    {
                        WindowChrome.GetWindowChrome(this).ResizeBorderThickness = new Thickness(0);
                    }
                }

                
            }
            catch (Exception exc)
            {
                System.Diagnostics.Debug.WriteLine(exc.ToString());
            }
        }

        // Load settings
        private void LoadSettings()
        {
            WindowChrome.SetWindowChrome(this, new WindowChrome());

            WindowChrome.GetWindowChrome(this).CornerRadius = new CornerRadius(25);
            WindowChrome.GetWindowChrome(this).GlassFrameThickness = new Thickness(1);
            WindowChrome.GetWindowChrome(this).UseAeroCaptionButtons = false;
            WindowChrome.GetWindowChrome(this).ResizeBorderThickness = new Thickness(10);
            WindowChrome.GetWindowChrome(this).CaptionHeight = 0;

            WindowState = WindowState.Normal;
            MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;
            Main.Content = clientPage;
            currentInterface = "client";
            serverPage.startButtonIsOn = false;
        }
    }
}
