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
using AsyncClientLib;

namespace Soxkets
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        AsyncServer server = new AsyncServer();

        AsyncClient client = new AsyncClient();

        ServerPage serverPage = new ServerPage();
        ClientPage clientPage = new ClientPage();

        bool isMaximized;
        string currentInterface;

        public MainWindow()
        {
            InitializeComponent();
            isMaximized = false;

            Main.Content = clientPage;
            currentInterface = "client";

            serverPage.startButtonIsOn = false;
        }

        //INTERFACE TOGGLE
        private void InterfaceToggle_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (currentInterface == "server")
            {
                Main.Content = clientPage;
                currentInterface = "client";
            }
            else if (currentInterface == "client")
            {
                Main.Content = serverPage;
                currentInterface = "server";
            }
        }

        //CLOSE BUTTON
        private void CloseButton_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (serverPage.startButtonIsOn)
            {
                var Result = MessageBox.Show("Your server is running, are you sure you want to exit?", "Soxkets", MessageBoxButton.YesNo, MessageBoxImage.Exclamation);
                if (Result == MessageBoxResult.Yes)
                {
                    client.Disconnect();
                    server.Stop();
                    Close();
                }
            }
            else
            {
                client.Disconnect();
                server.Stop();
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

        //MINIMIZE BUTTON
        private void MinimizeBBG_MouseEnter(object sender, MouseEventArgs e)
        {
            MinimizeBBG.Fill = new SolidColorBrush(Color.FromArgb(255, 80, 80, 80));
        }

        private void MinimizeBBG_MouseLeave(object sender, MouseEventArgs e)
        {
            MinimizeBBG.Fill = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
        }

        //MAXIMIZE BUTTON
        private void MaximizeBBG_MouseEnter(object sender, MouseEventArgs e)
        {
            MaximizeBBG.Fill = new SolidColorBrush(Color.FromArgb(255, 80, 80, 80));
        }

        private void MaximizeBBG_MouseLeave(object sender, MouseEventArgs e)
        {
            MaximizeBBG.Fill = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
        }

        private void MinimizeButton_MouseUp(object sender, MouseButtonEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void MaximizeButton_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (!isMaximized)
            {
                WindowState = WindowState.Maximized;
                isMaximized = true;
            }
            else
            {
                WindowState = WindowState.Normal;
                isMaximized = false;
            }
        }

        //DRAGGABLE SPACE
        private void Rectangle_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                DragMove();
            }
            catch (Exception)
            {
            }
        }
    }
}
