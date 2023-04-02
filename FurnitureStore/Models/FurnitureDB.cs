using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace FurnitureStore.Models
{
    public partial class FurnitureDB : DbContext
    {
        public FurnitureDB()
            : base("name=FurnitureDB")
        {
        }

        public virtual DbSet<InvoiceDetail> InvoiceDetails { get; set; }
        public virtual DbSet<Invoice> Invoices { get; set; }
        public virtual DbSet<ProductCategory> ProductCategories { get; set; }
        public virtual DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<InvoiceDetail>()
                .Property(e => e.price)
                .HasPrecision(10, 2);

            modelBuilder.Entity<InvoiceDetail>()
                .Property(e => e.Total)
                .HasPrecision(10, 2);

            modelBuilder.Entity<Invoice>()
                .HasMany(e => e.InvoiceDetails)
                .WithRequired(e => e.Invoice)
                .HasForeignKey(e => e.invoice_id)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<ProductCategory>()
                .HasMany(e => e.Products)
                .WithRequired(e => e.ProductCategory)
                .HasForeignKey(e => e.category_id)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Product>()
                .Property(e => e.price)
                .HasPrecision(10, 2);

            modelBuilder.Entity<Product>()
                .HasMany(e => e.InvoiceDetails)
                .WithRequired(e => e.Product)
                .HasForeignKey(e => e.product_id)
                .WillCascadeOnDelete(false);
        }
    }
}
