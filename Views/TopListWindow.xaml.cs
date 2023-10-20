using CoinView.Models;
using CoinView.Services;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
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
		private readonly string apiUrl = "https://api.coincap.io/v2/assets";
		private readonly string filePath = "data.json";
		private CurrencyRoot currencyRoot = new CurrencyRoot();
		private int index = 0;

		public TopListWindow()
		{
			InitializeComponent();
			UpdateCurrencyData();
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
			var topListWindow = new TopListWindow();
			topListWindow.Left = this.Left;
			topListWindow.Top = this.Top;
			topListWindow.Show();
			Close();
		}

		private void btnShowSource_Click(object sender, RoutedEventArgs e)
		{
			Process.Start(new ProcessStartInfo
			{
				FileName = "cmd",
				Arguments = $"/c start {currencyRoot.Data[index].Explorer}"
			});
		}

		private void btnForward_Click(object sender, RoutedEventArgs e)
		{
			index += 1;
			if (index > currencyRoot.Data.Count - 1)
			{
				index = currencyRoot.Data.Count - 1;
			}
			UpdateCurrencyData();
		}

		private void btnBackward_Click(object sender, RoutedEventArgs e)
		{
			index -= 1;
			if (index < 0)
			{
				index = 0;
			}
			UpdateCurrencyData();
		}

		private void btnRefresh_Click(object sender, RoutedEventArgs e)
		{
			UpdateCurrencyData();
		}

		private async void UpdateCurrencyData()
		{
			ApiService apiService = new ApiService();
			await apiService.GetCrpytoDataAsync(apiUrl, filePath);
			currencyRoot = apiService.GetDeserializedData(filePath);

			lbCurrencyName.Content = currencyRoot.Data[index].Name;
			lbCurrencySymbol.Content = currencyRoot.Data[index].Symbol;
			lbCurrencyPrice.Content = $"${currencyRoot.Data[index].PriceUsd}";
			lbCurrencySupply.Content = $"${currencyRoot.Data[index].Supply.ToString("0.00")}";
			lbCurrencyMaxSupply.Content = $"${currencyRoot.Data[index].MaxSupply.ToString("0.00")}";

			if (currencyRoot.Data[index].ChangePercent24Hr > 0)
			{
				lbCurrencyChangePercent.Foreground = new SolidColorBrush(Colors.Green);
				lbCurrencyChangePercent.Content = $"↑ {currencyRoot.Data[index].ChangePercent24Hr}%";
			}
			else
			{
				lbCurrencyChangePercent.Foreground = new SolidColorBrush(Colors.Red);
				lbCurrencyChangePercent.Content = $"↓ {currencyRoot.Data[index].ChangePercent24Hr}%";
			}

			lbCurrencyVwap24Hr.Content = $"${currencyRoot.Data[index].Vwap24Hr}";
			lbDateTime.Content = $"Інформацію оновлено станом на: {currencyRoot.DateTime}";
		}
	}
}
