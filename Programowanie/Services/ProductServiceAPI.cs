using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using Newtonsoft.Json;
using FitnessApp.Models;

namespace FitnessApp.Services
{
    public class ProductServiceAPI
    {
        private readonly HttpClient _client;

        // Konstruktor, który tworzy jeden HttpClient dla całej klasy
        public ProductServiceAPI()
        {
            _client = new HttpClient();
        }

        // Metoda do pobrania produktu z API na podstawie kodu kreskowego
        public async Task<dynamic> GetProductFromApiBarcode(string barcode)
        {
            try
            {
                // URL API, do którego wysyłamy zapytanie
                string url = $"https://world.openfoodfacts.org/api/v0/product/{barcode}.json";

                // Wykonanie zapytania i pobranie odpowiedzi
                var response = await _client.GetStringAsync(url);

                // Zdeserializowanie odpowiedzi JSON do obiektu dynamicznego
                dynamic apiResponse = JsonConvert.DeserializeObject(response);

                // Sprawdzenie, czy odpowiedź zawiera dane o produkcie
                if (apiResponse != null && apiResponse.product != null)
                {
                    return apiResponse.product; // Zwrócenie danych produktu
                }
                else
                {
                    Console.WriteLine("Brak danych produktu.");
                    return null; // Brak danych, zwróć null
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Błąd: {ex.Message}"); // Obsługa błędów
                return null; // Zwrócenie null w przypadku błędu
            }
        }

        public async Task<dynamic> GetProductFromApiName(string productName)
        {
            try
            {
                string url = $"https://world.openfoodfacts.org/cgi/search.pl?search_terms={Uri.EscapeDataString(productName)}&search_simple=1&action=process&json=1";


                var response = await _client.GetStringAsync(url);


                var apiResponse = JsonConvert.DeserializeObject<ApiSearchResponse>(response);

                if (apiResponse?.Products != null && apiResponse.Products.Any())

                {


                    return apiResponse.Products; // Zwracamy całą listę produktów
                }
                else
                {
                    MessageBox.Show("Brak produktów w odpowiedzi API!", "Błąd");
                    return new List<Product>(); // Zwracamy pustą listę zamiast null
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd pobierania danych z API:\n{ex.Message}", "Błąd");
                return null;
            }
        
    }
        public class ApiResponse
        {
            [JsonProperty("product")]
            public Product Product { get; set; }
        }

        public class ApiSearchResponse
        {
            [JsonProperty("products")]
            public List<Product> Products { get; set; }
        }


    }
}
