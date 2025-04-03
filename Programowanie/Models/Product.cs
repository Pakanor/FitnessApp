using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitnessApp.Models
{
    public class Product
    {
        public int Id { get; set; } 

        [JsonProperty("product_name")]
        public string ProductName { get; set; }

        [JsonProperty("brands")]
        public string Brands { get; set; }

        [JsonProperty("energy_value")]
        public string EnergyValue { get; set; }
        [JsonProperty("nutriments")]
        public Nutriments Nutriments { get; set; }

    }
    [Owned]
    public class Nutriments
    {

        [JsonProperty("energy")]
        public double Energy { get; set; }

        [JsonProperty("energy_unit")]
        public string EnergyUnit { get; set; }

        [JsonProperty("fat")]
        public double Fat { get; set; } 

        [JsonProperty("sugars")]
        public double Sugars { get; set; } 

        [JsonProperty("proteins")]
        public double Proteins { get; set; } 

        [JsonProperty("salt")]
        public double Salt { get; set; }
    }
}
