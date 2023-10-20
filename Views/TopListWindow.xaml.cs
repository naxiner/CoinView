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
using System.Windows.Shapes;

namespace CoinView.Views
{
	/// <summary>
	/// Логіка для взаємодії з TopListWindow.xaml
	/// </summary>
	public partial class TopListWindow : Window
	{
		public TopListWindow()
		{
			InitializeComponent();
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

		}

		private void btnClose_Click(object sender, RoutedEventArgs e)
		{

		}

		private void btnMenu_Click(object sender, RoutedEventArgs e)
		{

		}
	}
}
