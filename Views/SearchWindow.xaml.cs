using CoinView.Models;
using CoinView.Services;
using CoinView.Utils;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace CoinView.Views
{
    /// <summary>
    /// Логика взаимодействия для SearchWindow.xaml
    /// </summary>
    public partial class SearchWindow : Window
    {
        private CurrencyRoot currencyRoot = new CurrencyRoot();

        public SearchWindow()
        {
            InitializeComponent();
            tbSearch.Focus();
            UpdateCurrencyData();
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }

            DoubleAnimation animation = new DoubleAnimation();
            animation.To = 0;
            animation.Duration = TimeSpan.FromSeconds(0.2);
            MenuPanel.BeginAnimation(Grid.WidthProperty, animation);
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

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            SearchBy();
        }
        private void tbSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                SearchBy();
            }
        }

        private int GetSearchingId(string searchText)
        {
            if (int.TryParse(searchText, out int rank) && rank >= 1 && rank <= 100)
            {
                // Шукати по рангу (1)
                var findByRank = currencyRoot.Data.FirstOrDefault(u => u.Rank == rank);
                if (findByRank != null)
                {
                    return findByRank.Rank;
                }
            }
            else
            {
                // Шукати по символу (BTC)
                var findBySymbol = currencyRoot.Data.FirstOrDefault(u => u.Symbol == searchText);
                if (findBySymbol != null)
                {
                    return findBySymbol.Rank;
                }

                // Шукати по символу (Bitcoin)
                var findByName = currencyRoot.Data.FirstOrDefault(u => u.Name == searchText);
                if (findByName != null)
                {
                    return findByName.Rank;
                }
            }

            // Не було знайдено нічого
            return -1;
        }

        private void SearchBy()
        {
            int id = GetSearchingId(tbSearch.Text);
            if (id == -1)
            {
                ShowPopup();
            }
            else
            {
                var topListWindow = new TopListWindow(id - 1);
                topListWindow.Left = this.Left;
                topListWindow.Top = this.Top;
                topListWindow.Show();
                Close();
            }
        }

        private async void UpdateCurrencyData()
        {
            ApiService apiService = new ApiService();
            await apiService.GetCrpytoDataAsync(Constants.ApiUrl, Constants.FilePathData);
            currencyRoot = apiService.GetDeserializedData(Constants.FilePathData);
        }

        private void ShowPopup()
        {
            lbPopupText.Visibility = Visibility;
            Task.Delay(1200).ContinueWith(t => this.Dispatcher.Invoke(() => lbPopupText.Visibility = Visibility.Collapsed));
        }
    }
}
