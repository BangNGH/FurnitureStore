namespace FurnitureStore.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("feedback")]
    public partial class feedback
    {
        public int FeedbackID { get; set; }

        [Required]
        [StringLength(128)]
        public string userId { get; set; }

        public int product_id { get; set; }

        public int? rate { get; set; }

        public string Message { get; set; }

        public virtual Product Product { get; set; }
    }
}
