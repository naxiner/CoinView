using CoinView.Models;
using CoinView.Services;
using CoinView.Utils;
using LiveCharts;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
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
		private CurrencyRoot currencyRoot = new CurrencyRoot();
		private List<CurrencyHistory> currencyHistory = new List<CurrencyHistory>();
		private int currentIndex = 0;

		public SeriesCollection SeriesCollection { get; set; }
		public string[] Labels { get; set; }
		public Func<double, string> YFormatter { get; set; }

		public TopListWindow(int index)
		{
			InitializeComponent();
            UpdateCurrencyData();

			if (index >= 0 && index <= 100)
			{
				currentIndex = index;
			}
			else
			{
				currentIndex = 0;
			}

            SeriesCollection = new SeriesCollection();
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
			if (lvcChart.Visibility == Visibility.Hidden)
			{
				UpdateCurrencyChart();
                lvcChart.Visibility = Visibility.Visible;
            }
			else
			{
                lvcChart.Visibility = Visibility.Hidden;
			}
		}

		private void btnCopy_Click(object sender, RoutedEventArgs e)
		{
			Clipboard.SetText(CopyByIndex(currentIndex));
		}

		private void btnForward_Click(object sender, RoutedEventArgs e)
		{
            currentIndex += 1;
			if (currentIndex > currencyRoot.Data.Count - 1)
			{
                currentIndex = currencyRoot.Data.Count - 1;
                return;
            }
			
			UpdateCurrencyData();

			if (lvcChart.Visibility == Visibility.Visible)
			{
                UpdateCurrencyChart();
            }
		}

		private void btnBackward_Click(object sender, RoutedEventArgs e)
		{
            currentIndex -= 1;
			if (currentIndex < 0)
			{
                currentIndex = 0;
				return;
			}

			UpdateCurrencyData();

            if (lvcChart.Visibility == Visibility.Visible)
            {
                UpdateCurrencyChart();
            }
        }

		private void btnRefresh_Click(object sender, RoutedEventArgs e)
		{
			UpdateCurrencyData();
		}

		private async void UpdateCurrencyData()
		{
			ApiService apiService = new ApiService();
			await apiService.GetCrpytoDataAsync(Constants.ApiUrl, Constants.FilePathData);
			currencyRoot = apiService.GetDeserializedData(Constants.FilePathData);

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
			lbDateTime.Content = $"Інформацію оновлено станом на: {currencyRoot.DateTime}";
		}

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

		private async Task<List<CurrencyHistory>> UpdateCurrencyHistory()
		{
			string url = $"http://api.coincap.io/v2/assets/{currencyRoot.Data[currentIndex].Id}/history?interval=m1";
			ApiService apiService = new ApiService();
			await apiService.GetCrpytoDataAsync(url, Constants.FilePathHistory);
			currencyHistory = apiService.GetDeserializedHistory(Constants.FilePathHistory);

			DateTimeOffset dateEnd = DateTimeOffset.Now;
			DateTimeOffset dateStart = dateEnd.AddDays(-1);

			return currencyHistory
				.Where(x => x.Date.ToLocalTime() < dateEnd && x.Date.ToLocalTime() > dateStart)
				.ToList();
		}

        private async void UpdateCurrencyChart()
        {
            SeriesCollection.Clear();

            var chartValues = await UpdateCurrencyHistory();

            // Останню точку завжди додаємо
            if (chartValues.Count > 0)
            {
                List<decimal> filteredValues = new List<decimal>
                {
                    chartValues[0].PriceUsd // Додаємо першу точку
                };

                // Додаємо кожну десяту точку
                for (int i = 5; i < chartValues.Count; i += 10)
                {
                    filteredValues.Add(chartValues[i].PriceUsd);
                }

                SeriesCollection = new SeriesCollection
				{
					new LineSeries
					{
						Title = currencyRoot.Data[currentIndex].Id,
						Values = new ChartValues<decimal>(filteredValues),
						StrokeThickness = 3
					}
				};

                YFormatter = value => value.ToString("N2") + "$";

                // Створюємо масив міток для відображення на осі X
                Labels = chartValues.Select(x => x.Date.ToString("d")).ToArray();

                // Поновлюємо контекст даних графіку
                lvcChart.DataContext = null;
                lvcChart.DataContext = this;
            }
        }
    }
}
