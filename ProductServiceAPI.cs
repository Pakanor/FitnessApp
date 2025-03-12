using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Programowanie
{ 
    public class ProductSeviceAPI
    {
        private readonly HttpClient _client;


        public ProductService()
        {
            _client = new HttpClient();
        }

        public async Task<dynamic> GetProductFromApi(string barcode)
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    string url = $"https://world.openfoodfacts.org/api/v0/product/{barcode}.json";

                    var response = await client.GetStringAsync(url);

                    // Zdeserializowanie odpowiedzi JSON do obiektu dynamicznego
                    dynamic apiResponse = JsonConvert.DeserializeObject(response);

                    if (apiResponse != null && apiResponse.product != null)
                    {
                        return apiResponse.product; // Zwracamy produkt
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
        }

    }



}