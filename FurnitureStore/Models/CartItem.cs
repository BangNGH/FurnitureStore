namespace FurnitureStore.Models
{
    public class CartItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string CategoryName { get; set; }
        public decimal Price { get; set; }
        public string Image { get; set; }
        public int Quatity { get; set; }
        public decimal Money
        {
            get
            {
                return Quatity * Price;
            }
        }

    }
}