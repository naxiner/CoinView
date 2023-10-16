using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using CoinView.Models;
using Newtonsoft.Json;

namespace CoinView.Services
{
    class ApiService
    {
        private readonly HttpClient _httpClient;
		private CurrencyRoot currencyRoot = new CurrencyRoot();

		public ApiService()
        {
            _httpClient = new HttpClient();
		}

        public async Task GetCrpytoDataAsync(string apiUrl, string filePath)
        {
            try
            {
                var response = await _httpClient.GetAsync(apiUrl);
                response.EnsureSuccessStatusCode();
                var jsonData = await response.Content.ReadAsStringAsync();

                if (!string.IsNullOrEmpty(jsonData))
                {
                    if (!File.Exists(filePath))
                    {
                        File.Create(filePath).Close();
                    }
                    File.WriteAllText(filePath, jsonData);
                }
                else
                {
					MessageBox.Show($"Отримано порожній вміст відповіді, файл {filePath} не було заповнено.");
				}
			}
            catch (Exception ex)
            {
                MessageBox.Show($"$Відбулася помилка при спробі отримати дані: {ex.Message}");
            }
        }

        public CurrencyRoot GetDeserializedData(string filePath)
        {
            string data = File.ReadAllText(filePath);
            currencyRoot = JsonConvert.DeserializeObject<CurrencyRoot>(data);
            currencyRoot.DateTime = DateTime.Now;

			return currencyRoot;
        }
    }
}
