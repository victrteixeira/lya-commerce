using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Commerce.Core.Validations;

namespace Commerce.Core.Entities
{
    public class Product
    {
        public Product(int id, string name, string description, decimal price, string category, string? manufacturer = null, string? subcategory = null) { 
            Id = id;
            Name = name;
            Description = description;
            Price = price;
            Category = category;
            SubCategory = subcategory;
            Manufacturer = manufacturer;
            Validate();
        }

        public int Id { get; private set; }
        public string Name { get; }
        public string Description { get; }
        public decimal Price { get; }
        public string? Manufacturer { get; }
        public string Category { get; }
        public string? SubCategory { get; }

        public bool Validate() => new ProductValidator().Validate(this).IsValid;
    }
}
