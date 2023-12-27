using System;
using System.Globalization;
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
        public SettingsWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            string userLanguage = Settings.Default.Language;
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

        private void btnMenuSettings_Click(object sender, RoutedEventArgs e)
        {
            var settingsWindow = new SettingsWindow();
            settingsWindow.Left = this.Left;
            settingsWindow.Top = this.Top;
            settingsWindow.Show();
            Close();
        }
        #endregion

        private void SetLanguage(string culture)
        {
            var cultureInfo = new CultureInfo(culture);
            Thread.CurrentThread.CurrentCulture = cultureInfo;
            Thread.CurrentThread.CurrentUICulture = cultureInfo;

            // Збереження обраної мови у налаштуваннях
            Settings.Default.Language = culture;
            Settings.Default.Save();

            UpdateLanguage(culture);
        }

        private void UpdateLanguage(string culture)
        {
            ResourceDictionary dict = new ResourceDictionary();

            dict.Source = new Uri($"..\\Resources\\{culture}\\Strings.{culture}.xaml", UriKind.Relative);

            // Очищення минулих словників
            this.Resources.MergedDictionaries.Clear();
            this.Resources.MergedDictionaries.Add(dict);
            
            DataContext = null;
            DataContext = this;
        }

        private void btnEnglish_Click(object sender, RoutedEventArgs e)
        {
            SetLanguage("en-EN");
        }

        private void btnUkrainian_Click(object sender, RoutedEventArgs e)
        {
            SetLanguage("ua-UA");
        }
    }
}
