using System;
using System.Globalization;
using System.Net.NetworkInformation;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace CoinView.Views
{
    /// <summary>
    /// Логика взаимодействия для SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        private ResourceDictionary languageDictionary;
        private ResourceDictionary themeDictionary;

        public SettingsWindow()
        {
            InitializeComponent();
            LoadDictionaries();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            string userLanguage = AppLanguage.Default.Language;
            string userTheme = AppTheme.Default.Theme;
            UpdateLanguage(userLanguage);
            UpdateTheme(userTheme);
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
        #endregion

        private void SetLanguage(string culture)
        {
            var cultureInfo = new CultureInfo(culture);
            Thread.CurrentThread.CurrentCulture = cultureInfo;
            Thread.CurrentThread.CurrentUICulture = cultureInfo;

            // Збереження обраної мови у налаштуваннях
            AppLanguage.Default.Language = culture;
            AppLanguage.Default.Save();

            UpdateLanguage(culture);
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

        private void SetTheme(string theme) 
        {
            AppTheme.Default.Theme = theme;
            AppTheme.Default.Save();

            UpdateTheme(theme);
        }

        private void UpdateTheme(string theme)
        {
            themeDictionary.Source = new Uri($"..\\Resources\\Themes\\{theme}.xaml", UriKind.Relative);

            // Оновити колекцію тем
            Resources.MergedDictionaries[0] = themeDictionary;
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

        private void btnEnglish_Click(object sender, RoutedEventArgs e)
        {
            SetLanguage("en");
        }

        private void btnUkrainian_Click(object sender, RoutedEventArgs e)
        {
            SetLanguage("uk");
        }

        private void btnGermany_Click(object sender, RoutedEventArgs e)
        {
            SetLanguage("de");
        }

        private void btnWhiteTheme_Click(object sender, RoutedEventArgs e)
        {
            SetTheme("WhiteTheme");
        }

        private void btnBlackTheme_Click(object sender, RoutedEventArgs e)
        {
            SetTheme("BlackTheme");
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
