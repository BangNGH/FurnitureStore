namespace FurnitureStore.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Product
    {
        public int id { get; set; }

        [Required]
        [StringLength(255)]
        public string name { get; set; }

        [StringLength(255)]
        public string manufacturer { get; set; }

        public string description { get; set; }

        public decimal price { get; set; }

        [StringLength(100)]
        public string Image { get; set; }

        public int category_id { get; set; }

        public virtual ProductCategory ProductCategory { get; set; }
    }
}
