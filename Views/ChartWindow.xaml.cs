using CoinView.Models;
using CoinView.Services;
using CoinView.Utils;
using LiveCharts;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace CoinView.Views
{
    /// <summary>
    /// Логика взаимодействия для ChartWindow.xaml
    /// </summary>
    public partial class ChartWindow : Window
    {
        private CurrencyRoot currencyRoot = new CurrencyRoot();
        private List<CurrencyHistory> currencyHistory = new List<CurrencyHistory>();
        private int currentIndex;
        private int previousWindow;
        public SeriesCollection SeriesCollection { get; set; }
        public string[] Labels { get; set; }
        public Func<double, string> YFormatter { get; set; }

        public ChartWindow(int index, int previousWindow)
        {
            currentIndex = index;
            this.previousWindow = previousWindow;
            SeriesCollection = new SeriesCollection();
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

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            UpdateCurrencyData();
            UpdateCurrencyChart();
        }

        private void btnReturn_Click(object sender, RoutedEventArgs e)
        {
            switch (previousWindow)
            {
                case 1:
                    var homeWindow = new HomeWindow();
                    homeWindow.Left = this.Left;
                    homeWindow.Top = this.Top;
                    homeWindow.Show();
                    Close();
                    break;
                case 2:
                    var topListWindow = new TopListWindow(currentIndex);
                    topListWindow.Left = this.Left;
                    topListWindow.Top = this.Top;
                    topListWindow.Show();
                    Close();
                    break;
                default:
                    break;
            }
        }
        #endregion

        #region CHART
        private async void UpdateCurrencyData()
        {
            ApiService apiService = new ApiService();
            await apiService.GetCrpytoDataAsync(Constants.ApiUrl, Constants.FilePathData);
            currencyRoot = apiService.GetDeserializedData(Constants.FilePathData);
            UpdateCurrencyChart();

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
        #endregion
    }
}
