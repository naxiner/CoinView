using CoinView.Models;
using CoinView.Services;
using CoinView.Utils;
using System;
using System.Linq;
using System.Net.NetworkInformation;
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
        private ResourceDictionary languageDictionary;
        private ResourceDictionary themeDictionary;

        private CurrencyRoot currencyRoot = new CurrencyRoot();

        public SearchWindow()
        {
            InitializeComponent();
            tbSearch.Focus();
            UpdateCurrencyData();
            LoadDictionaries();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            string userLanguage = AppLanguage.Default.Language;
            UpdateLanguage(userLanguage);
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DoubleAnimation animation = new DoubleAnimation();
            animation.To = 0;
            animation.Duration = TimeSpan.FromSeconds(0.2);
            MenuPanel.BeginAnimation(Grid.WidthProperty, animation);
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        #region BUTTONS
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

        private void btnHide_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnMenuHome_Click(object sender, RoutedEventArgs e)
        {
            if (IsInternetAvailable())
            {
                var homeWindow = new HomeWindow();
                homeWindow.Left = this.Left;
                homeWindow.Top = this.Top;
                homeWindow.Show();
                Close();
            }
            else
            {
                OpenNoConnectionWindow();
            }
        }

        private void btnMenuTop100_Click(object sender, RoutedEventArgs e)
        {
            if (IsInternetAvailable())
            {
                var topListWindow = new TopListWindow(0);
                topListWindow.Left = this.Left;
                topListWindow.Top = this.Top;
                topListWindow.Show();
                Close();
            }
            else
            {
                OpenNoConnectionWindow();
            }
        }

        private void btnMenuSearch_Click(object sender, RoutedEventArgs e)
        {
            if (IsInternetAvailable())
            {
                var searchWindow = new SearchWindow();
                searchWindow.Left = this.Left;
                searchWindow.Top = this.Top;
                searchWindow.Show();
                Close();
            }
            else
            {
                OpenNoConnectionWindow();
            }
        }

        private void btnMenuSettings_Click(object sender, RoutedEventArgs e)
        {
            if (IsInternetAvailable())
            {
                var settingsWindow = new SettingsWindow();
                settingsWindow.Left = this.Left;
                settingsWindow.Top = this.Top;
                settingsWindow.Show();
                Close();
            }
            else
            {
                OpenNoConnectionWindow();
            }
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
        #endregion

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
                var findBySymbol = currencyRoot.Data.FirstOrDefault(u => string
                    .Equals(u.Symbol, searchText, StringComparison.OrdinalIgnoreCase));
                if (findBySymbol != null)
                {
                    return findBySymbol.Rank;
                }

                // Шукати по символу (Bitcoin)
                var findByName = currencyRoot.Data.FirstOrDefault(u => string
                    .Equals(u.Name, searchText, StringComparison.OrdinalIgnoreCase));
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
            if (!IsInternetAvailable())
            {
                OpenNoConnectionWindow();
            }
            else
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
        }

        private async void UpdateCurrencyData()
        {
            if (!IsInternetAvailable())
            {
                OpenNoConnectionWindow();
            }
            else
            {
                ApiService apiService = new ApiService();
                await apiService.GetCrpytoDataAsync(Constants.ApiUrl, Constants.FilePathData);
                currencyRoot = apiService.GetDeserializedData(Constants.FilePathData);
            }
        }

        private void ShowPopup()
        {
            lbPopupText.Visibility = Visibility;
            Task.Delay(1200).ContinueWith(t => this.Dispatcher.Invoke(() => lbPopupText.Visibility = Visibility.Collapsed));
        }

        private void OpenNoConnectionWindow()
        {
            var noConnectionWindow = new NoConnectionWindow();
            noConnectionWindow.Left = this.Left;
            noConnectionWindow.Top = this.Top;
            noConnectionWindow.Show();
            Close();
        }

        private void UpdateLanguage(string culture)
        {
            languageDictionary.Source = new Uri($"..\\Resources\\{culture}\\Strings.{culture}.xaml", UriKind.Relative);

            // Очищення минулих словників
            Resources.MergedDictionaries[0] = languageDictionary;

            DataContext = null;
            DataContext = this;
        }

        private void LoadDictionaries()
        {
            // Завантаження словника для мови
            languageDictionary = new ResourceDictionary();
            languageDictionary.Source = new Uri($"pack://application:,,,/CoinView;component/Resources/{AppLanguage.Default.Language}/Strings.{AppLanguage.Default.Language}.xaml");

            // Завантаження словника для теми
            themeDictionary = new ResourceDictionary();
            themeDictionary.Source = new Uri($"pack://application:,,,/CoinView;component/Resources/Themes/{AppTheme.Default.Theme}.xaml");

            // Додавання словників до колекції
            this.Resources.MergedDictionaries.Add(languageDictionary);
            this.Resources.MergedDictionaries.Add(themeDictionary);
        }

        private bool IsInternetAvailable()
        {
            using (var ping = new Ping())
            {
                try
                {
                    var reply = ping.Send("8.8.8.8", 1000); // Google DNS сервер
                    return reply.Status == IPStatus.Success;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }
    }
}
