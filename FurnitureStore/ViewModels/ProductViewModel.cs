using FurnitureStore.Models;
using PagedList;
using System.Collections.Generic;

namespace FurnitureStore.ViewModels
{


    public class ProductViewModel
    {
        public IEnumerable<Product> Products { get; set; }
        public IEnumerable<ProductCategory> Categories { get; set; }
        public IEnumerable<CartItem> CartItems { get; set; }
        public IEnumerable<feedback> product_feedback { get; set; }
        public IEnumerable<ProductRevenue> RevenueList { get; set; }
        public IPagedList<Product> pagedList { get; set; }
    }
    public class ProductRevenue
    {
        public int ProductId { get; set; }
        public decimal Revenue { get; set; }
    }
}
