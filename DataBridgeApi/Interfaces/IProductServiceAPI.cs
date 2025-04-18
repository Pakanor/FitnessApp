using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitnessApp.Interfaces
{
    public interface IProductServiceAPI
    {
         Task<dynamic> GetProductFromApiBarcode(string barcode);
        Task<dynamic> GetProductFromApiName(string productName);
        public class ApiSearchResponse;


    }
}
