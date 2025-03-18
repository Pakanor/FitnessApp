using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Programowanie.Services
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


                // Zdeserializowanie odpowiedzi JSON do obiektu dynamicznego
                dynamic apiResponse = JsonConvert.DeserializeObject(response);

                if (apiResponse != null && apiResponse.products != null && apiResponse.products.Count > 0)
                {
                    dynamic mostPopularProduct = null;
                    List<dynamic> popularProducts = new List<dynamic>();

                    foreach (var product in apiResponse.products)
                    {
                        if (mostPopularProduct == null || product.popularity_key > mostPopularProduct.popularity_key)
                        {
                            mostPopularProduct = product;
                        }
                    }
                    return mostPopularProduct;
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


    }
}
