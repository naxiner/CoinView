using System;
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


            Currency.Data data = new Currency.Data();
        }
    }
}
