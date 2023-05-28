using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Commerce.Core.Validations;
using FluentValidation;

namespace Commerce.Core.Entities
{
    public class Product : Base
    {
        protected Product()
        {
        }
        
        public Product(string name, string description, decimal price, string category, string? manufacturer = null, string? subcategory = null) {
            Name = name;
            Description = description;
            Price = price;
            Category = category;
            SubCategory = subcategory;
            Manufacturer = manufacturer;
            Validate();
        }
        
        public string Name { get; }
        public string Description { get; }
        public decimal Price { get; }
        public string? Manufacturer { get; }
        public string Category { get; }
        public string? SubCategory { get; }

        public void Validate() => new ProductValidator().ValidateAndThrow(this);
        //TODO generalize Validate method to all classes
    }
}
