using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
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
    /// Логика взаимодействия для NoConnectionWindow.xaml
    /// </summary>
    public partial class NoConnectionWindow : Window
    {
        private ResourceDictionary languageDictionary;
        private ResourceDictionary themeDictionary;

        public NoConnectionWindow()
        {
            InitializeComponent();
            LoadDictionaries();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            string userLanguage = AppLanguage.Default.Language;
            UpdateLanguage(userLanguage);
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        #region BUTTONS
        private void btnHide_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnReconnect_Click(object sender, RoutedEventArgs e)
        {
            if (IsInternetAvailable())
            {
                // відмалювати HomeWindow
                var homeWindow = new HomeWindow();
                homeWindow.Left = this.Left;
                homeWindow.Top = this.Top;
                homeWindow.Show();
                Close();
            }
            else
            {
                ShowPopup();
            }
        }
        #endregion

        private void ShowPopup()
        {
            lbPopupText.Visibility = Visibility;
            Task.Delay(1200).ContinueWith(t => this.Dispatcher.Invoke(() => lbPopupText.Visibility = Visibility.Collapsed));
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
