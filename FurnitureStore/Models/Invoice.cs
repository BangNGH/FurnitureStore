namespace FurnitureStore.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Invoice
    {
        public int id { get; set; }

        public DateTime OrderDate { get; set; }

        public DateTime? DeliveryDate { get; set; }

        public bool isComplete { get; set; }

        public bool isPaid { get; set; }

        [Required]
        [StringLength(128)]
        public string customer_id { get; set; }
    }
}
