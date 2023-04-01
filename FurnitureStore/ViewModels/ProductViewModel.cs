using FurnitureStore.Models;
using System.Collections.Generic;

namespace FurnitureStore.ViewModels
{
    public class ProductViewModel
    {
        public IEnumerable<Product> Products { get; set; }
        public IEnumerable<ProductCategory> Categories { get; set; }
        public IEnumerable<CartItem> CartItems { get; set; }

    }
}