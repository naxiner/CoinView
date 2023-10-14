using CoinView.Models;
using CoinView.Services;
using CoinView.Views;
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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CoinView
{
	public partial class MainWindow : Window
	{
		private readonly string apiUrl = "https://api.coincap.io/v2/assets";
		private readonly string filePath = "data.json";
		private CurrencyRoot currencyRoot = new CurrencyRoot();

		public MainWindow()
		{
			InitializeComponent();
			StartApplication();
		}

		private async void StartApplication()
		{
			ApiService apiService = new ApiService();
			await apiService.GetCrpytoDataAsync(apiUrl, filePath);
			currencyRoot.Data = apiService.GetDeserializedData(filePath);
			var homeWindow = new HomeWindow();
			homeWindow.Left = this.Left;
			homeWindow.Top = this.Top;
			homeWindow.Show();
			Close();
		}
	}
}
