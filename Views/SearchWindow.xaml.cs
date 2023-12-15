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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace CoinView.Views
{
    /// <summary>
    /// Логика взаимодействия для SearchWindow.xaml
    /// </summary>
    public partial class SearchWindow : Window
    {
        public SearchWindow()
        {
            InitializeComponent();
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void btnHide_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnMenu_Click(object sender, RoutedEventArgs e)
        {
            DoubleAnimation animation = new DoubleAnimation();

            if (MenuPanel.Width == 0)
            {
                animation.To = 200;
            }
            else
            {
                animation.To = 0;
            }

            animation.Duration = TimeSpan.FromSeconds(0.2);
            MenuPanel.BeginAnimation(Grid.WidthProperty, animation);
        }

        private void btnMenuHome_Click(object sender, RoutedEventArgs e)
        {
            var homeWindow = new HomeWindow();
            homeWindow.Left = this.Left;
            homeWindow.Top = this.Top;
            homeWindow.Show();
            Close();
        }

        private void btnMenuTop100_Click(object sender, RoutedEventArgs e)
        {
            var topListWindow = new TopListWindow(0);
            topListWindow.Left = this.Left;
            topListWindow.Top = this.Top;
            topListWindow.Show();
            Close();
        }

        private void btnMenuSearch_Click(object sender, RoutedEventArgs e)
        {
            var searchWindow = new SearchWindow();
            searchWindow.Left = this.Left;
            searchWindow.Top = this.Top;
            searchWindow.Show();
            Close();
        }
    }
}
