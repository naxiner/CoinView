﻿using CoinView.Models;
using CoinView.Services;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace CoinView.Views
{
	/// <summary>
	/// Логіка для взаємодії з HomeWindow.xaml
	/// </summary>
	public partial class HomeWindow : Window
	{
		private readonly string apiUrl = "https://api.coincap.io/v2/assets";
		private readonly string filePath = "data.json";
		private CurrencyRoot currencyRoot = new CurrencyRoot();

		List<Label> lbCurrencyNames = new List<Label>();
		List<Label> lbCurrencySymbols = new List<Label>();
		List<Label> lbCurrencyPrices = new List<Label>();
		List<Label> lbCurrencyChangePercents = new List<Label>();

		public HomeWindow()
		{
			InitializeComponent();
			FillInList();
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
			UpdateCurrencyData();
		}

		private void btnMenuTop100_Click(object sender, RoutedEventArgs e)
		{
			var topListWindow = new TopListWindow();
			topListWindow.Left = this.Left;
			topListWindow.Top = this.Top;
			topListWindow.Show();
			Close();
        }

		private void FillInList() 
		{
			lbCurrencyNames.Add(lbCurrencyName1);
			lbCurrencyNames.Add(lbCurrencyName2);
			lbCurrencyNames.Add(lbCurrencyName3);
			lbCurrencyNames.Add(lbCurrencyName4);
			lbCurrencyNames.Add(lbCurrencyName5);

			lbCurrencySymbols.Add(lbCurrencySymbol1);
			lbCurrencySymbols.Add(lbCurrencySymbol2);
			lbCurrencySymbols.Add(lbCurrencySymbol3);
			lbCurrencySymbols.Add(lbCurrencySymbol4);
			lbCurrencySymbols.Add(lbCurrencySymbol5);

			lbCurrencyPrices.Add(lbCurrencyPrice1);
			lbCurrencyPrices.Add(lbCurrencyPrice2);
			lbCurrencyPrices.Add(lbCurrencyPrice3);
			lbCurrencyPrices.Add(lbCurrencyPrice4);
			lbCurrencyPrices.Add(lbCurrencyPrice5);

			lbCurrencyChangePercents.Add(lbCurrencyChangePercent1);
			lbCurrencyChangePercents.Add(lbCurrencyChangePercent2);
			lbCurrencyChangePercents.Add(lbCurrencyChangePercent3);
			lbCurrencyChangePercents.Add(lbCurrencyChangePercent4);
			lbCurrencyChangePercents.Add(lbCurrencyChangePercent5);
		}

		private async void UpdateCurrencyData()
		{
			ApiService apiService = new ApiService();
			await apiService.GetCrpytoDataAsync(apiUrl, filePath);
			currencyRoot = apiService.GetDeserializedData(filePath);

			for (int i = 0; i < lbCurrencyNames.Count; i++)
			{
				lbCurrencyNames[i].Content = currencyRoot.Data[i].Name;
				lbCurrencySymbols[i].Content = currencyRoot.Data[i].Symbol;
				lbCurrencyPrices[i].Content = $"${currencyRoot.Data[i].PriceUsd}";

				if (currencyRoot.Data[i].ChangePercent24Hr > 0)
				{
					lbCurrencyChangePercents[i].Foreground = new SolidColorBrush(Colors.Green);
					lbCurrencyChangePercents[i].Content = $"↑ {currencyRoot.Data[i].ChangePercent24Hr}%";
				}
				else
				{
					lbCurrencyChangePercents[i].Foreground = new SolidColorBrush(Colors.Red);
					lbCurrencyChangePercents[i].Content = $"↓ {currencyRoot.Data[i].ChangePercent24Hr}%";
				}
			}

			lbDateTime.Content = $"Інформацію оновлено станом на: {currencyRoot.DateTime}";
		}

		private void btnCopy1_Click(object sender, RoutedEventArgs e)
		{
			Clipboard.SetText(CopyByIndex(0));
		}

		private void btnCopy2_Click(object sender, RoutedEventArgs e)
		{
			Clipboard.SetText(CopyByIndex(1));
		}

		private void btnCopy3_Click(object sender, RoutedEventArgs e)
		{
			Clipboard.SetText(CopyByIndex(2));
		}

		private void btnCopy4_Click(object sender, RoutedEventArgs e)
		{
			Clipboard.SetText(CopyByIndex(3));
		}

		private void btnCopy5_Click(object sender, RoutedEventArgs e)
		{
			Clipboard.SetText(CopyByIndex(4));
		}

		private string CopyByIndex(int index)
		{
			string textToCopy =
				$"{currencyRoot.Data[index].Name} " +
				$"{currencyRoot.Data[index].Symbol} " +
				$"${currencyRoot.Data[index].PriceUsd} " +
				$"{currencyRoot.Data[index].ChangePercent24Hr}%";
			return textToCopy;
		}

		private void btnRefresh_Click(object sender, RoutedEventArgs e)
		{
			UpdateCurrencyData();
		}
    }
}
