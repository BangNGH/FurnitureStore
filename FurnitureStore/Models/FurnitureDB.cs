using System.Data.Entity;

namespace FurnitureStore.Models
{
    public partial class FurnitureDB : DbContext
    {
        public FurnitureDB()
            : base("name=FurnitureDB")
        {
        }

        public virtual DbSet<ContactReceive> ContactReceives { get; set; }
        public virtual DbSet<feedback> feedbacks { get; set; }
        public virtual DbSet<InvoiceDetail> InvoiceDetails { get; set; }
        public virtual DbSet<Invoice> Invoices { get; set; }
        public virtual DbSet<ProductCategory> ProductCategories { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<AspNetUser> AspNetUsers { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<InvoiceDetail>()
                .Property(e => e.price)
                .HasPrecision(10, 2);

            modelBuilder.Entity<InvoiceDetail>()
                .Property(e => e.shipping_cost)
                .HasPrecision(10, 2);

            modelBuilder.Entity<ProductCategory>()
                .HasMany(e => e.Products)
                .WithRequired(e => e.ProductCategory)
                .HasForeignKey(e => e.category_id)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Product>()
                .Property(e => e.price)
                .HasPrecision(10, 2);

            modelBuilder.Entity<Product>()
                .HasMany(e => e.feedbacks)
                .WithRequired(e => e.Product)
                .HasForeignKey(e => e.product_id)
                .WillCascadeOnDelete(false);
        }
    }
}
