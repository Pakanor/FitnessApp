using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using Newtonsoft.Json;
using FitnessApp.Models;
using FitnessApp.Interfaces;

namespace FitnessApp.Services
{
    public class ProductServiceAPI : IProductServiceAPI
    {
        private readonly HttpClient _client;

        public ProductServiceAPI()
        {
            _client = new HttpClient();
        }

        // API
        public async Task<dynamic> GetProductFromApiBarcode(string barcode)
        {
            try
            {
                string url = $"https://world.openfoodfacts.org/api/v0/product/{barcode}.json";

                var response = await _client.GetStringAsync(url);

                // deserialising Object
                dynamic apiResponse = JsonConvert.DeserializeObject(response);

                if (apiResponse != null && apiResponse.product != null)
                {
                    return apiResponse.product; 
                }
                else
                {
                    Console.WriteLine("Brak danych produktu.");
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Błąd: {ex.Message}"); 
                return null; 
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


                    return apiResponse.Products;
                }
                else
                {
                    MessageBox.Show("Brak produktów w odpowiedzi API!", "Błąd");
                    return new List<Product>();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd pobierania danych z API:\n{ex.Message}", "Błąd");
                return null;
            }
        
    }
       

        // for serializing
        public class ApiSearchResponse
        {
            [JsonProperty("products")]
            public List<Product> Products { get; set; }
        }


    }
}
