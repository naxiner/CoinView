using CoinView.Services;
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
		public MainWindow()
		{
			InitializeComponent();
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			ApiService apiService = new ApiService();
			_ = apiService.GetCrpytoDataAsync(apiUrl, filePath);
		}
	}
}
