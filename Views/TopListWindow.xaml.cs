using CoinView.Models;
using CoinView.Services;
using CoinView.Utils;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace CoinView.Views
{
	/// <summary>
	/// Логіка для взаємодії з TopListWindow.xaml
	/// </summary>
	public partial class TopListWindow : Window
	{
		private CurrencyRoot currencyRoot = new CurrencyRoot();
		private int currentIndex = 0;

		public TopListWindow(int index)
		{
			InitializeComponent();
            UpdateCurrencyData();

			if (index >= 0 && index <= 99)
			{
				currentIndex = index;
			}
			else
			{
				currentIndex = 0;
			}
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

        private void btnMenuSettings_Click(object sender, RoutedEventArgs e)
        {
            var settingsWindow = new SettingsWindow();
            settingsWindow.Left = this.Left;
            settingsWindow.Top = this.Top;
            settingsWindow.Show();
            Close();
        }

        private void btnShowSource_Click(object sender, RoutedEventArgs e)
		{
			Process.Start(new ProcessStartInfo
			{
				FileName = "cmd",
				Arguments = $"/c start {currencyRoot.Data[currentIndex].Explorer}"
			});
		}

		private void btnShowChart_Click(object sender, RoutedEventArgs e)
		{
			var chartWindow = new ChartWindow(currentIndex, 2);
            chartWindow.Left = this.Left;
            chartWindow.Top = this.Top;
            chartWindow.Show();
            Close();
        }

		private void btnCopy_Click(object sender, RoutedEventArgs e)
		{
			Clipboard.SetText(CopyByIndex(currentIndex));
			ShowPopup();
		}
		
		private void btnBackward_Click(object sender, RoutedEventArgs e)
		{
            currentIndex -= 1;
			if (currentIndex < 0)
			{
                currentIndex = 0;
				return;
			}

			UpdateUI();
        }

		private void btnForward_Click(object sender, RoutedEventArgs e)
		{
            currentIndex += 1;
			if (currentIndex > currencyRoot.Data.Count - 1)
			{
                currentIndex = currencyRoot.Data.Count - 1;
                return;
            }

            UpdateUI();
        }

		private void btnRefresh_Click(object sender, RoutedEventArgs e)
		{
			UpdateCurrencyData();
		}
        #endregion

        private string CopyByIndex(int index)
		{
			string textToCopy =
				$"{currencyRoot.Data[index].Name} " +
				$"{currencyRoot.Data[index].Symbol} " +
				$"${currencyRoot.Data[index].PriceUsd} " +
				$"{currencyRoot.Data[index].ChangePercent24Hr}% " +
				$"${currencyRoot.Data[index].Supply:0.00} " +
				$"${currencyRoot.Data[index].MaxSupply:0.00} " +
				$"${currencyRoot.Data[index].Vwap24Hr} ";
			return textToCopy;
		}

		private async void UpdateCurrencyData()
		{
			ApiService apiService = new ApiService();
			await apiService.GetCrpytoDataAsync(Constants.ApiUrl, Constants.FilePathData);
			currencyRoot = apiService.GetDeserializedData(Constants.FilePathData);

			UpdateUI();
		}
        
		private void UpdateUI()
		{
            lbCurrencyName.Content = currencyRoot.Data[currentIndex].Name;
            lbCurrencySymbol.Content = currencyRoot.Data[currentIndex].Symbol;
            lbCurrencyPrice.Content = $"${currencyRoot.Data[currentIndex].PriceUsd}";
            lbCurrencySupply.Content = $"${currencyRoot.Data[currentIndex].Supply:0.00}";
            lbCurrencyMaxSupply.Content = $"${currencyRoot.Data[currentIndex].MaxSupply:0.00}";

            if (currencyRoot.Data[currentIndex].ChangePercent24Hr > 0)
            {
                lbCurrencyChangePercent.Foreground = new SolidColorBrush(Colors.Green);
                lbCurrencyChangePercent.Content = $"↑ {currencyRoot.Data[currentIndex].ChangePercent24Hr}%";
            }
            else
            {
                lbCurrencyChangePercent.Foreground = new SolidColorBrush(Colors.Red);
                lbCurrencyChangePercent.Content = $"↓ {currencyRoot.Data[currentIndex].ChangePercent24Hr}%";
            }

            lbCurrencyVwap24Hr.Content = $"${currencyRoot.Data[currentIndex].Vwap24Hr}";
            
			string updateText = (string)TryFindResource("DateTimeLable");
            lbDateTime.Content = $"{updateText} {currencyRoot.DateTime}";
        }

		private void ShowPopup()
        {
            lbPopupText.Visibility = Visibility;
            Task.Delay(1200).ContinueWith(t => this.Dispatcher.Invoke(() => lbPopupText.Visibility = Visibility.Collapsed));
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
    }
}
