using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitnessApp.Models
{
    //Product for api ProductLogEntry for db
    public class Product
    {
        [JsonIgnore]
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

        [JsonProperty("carbohydrates")]
        public double Carbs { get; set; } 

        [JsonProperty("proteins")]
        public double Proteins { get; set; } 

        [JsonProperty("salt")]
        public double Salt { get; set; }
    }


    public class ProductLogEntry
    {
        public int Id { get; set; }

        public string ProductName { get; set; }
        public string Brands { get; set; }

        public double Grams { get; set; }

        public double Energy { get; set; }
        public double Fat { get; set; }
        public double Sugars { get; set; }
        public double Proteins { get; set; }
        public double Salt { get; set; }
        public string EnergyUnit { get; set; }

        public DateTime LoggedAt { get; set; } = DateTime.UtcNow;

        public int? UserId { get; set; }
    }


    //for future
    public class User
    {
        public int ID { get; set; }
    }
}
