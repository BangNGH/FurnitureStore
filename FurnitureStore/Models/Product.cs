namespace FurnitureStore.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public partial class Product
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Product()
        {
            InvoiceDetails = new HashSet<InvoiceDetail>();
        }


        public int id { get; set; }

        [Required]
        [StringLength(255)]
        [Display(Name = "Product")]
        public string name { get; set; }

        [StringLength(255)]
        [Display(Name = "Manufacturer")]
        public string manufacturer { get; set; }

        [Display(Name = "Description")]
        public string description { get; set; }
        [Display(Name = "Price")]

        public decimal price { get; set; }

        [StringLength(100)]
        public string Image { get; set; }

        public int category_id { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<InvoiceDetail> InvoiceDetails { get; set; }

        public virtual ProductCategory ProductCategory { get; set; }
    }
}
