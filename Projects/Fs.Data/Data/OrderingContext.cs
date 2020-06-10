using System;
using Microsoft.EntityFrameworkCore;
using AngularPOC.Data.Models;

namespace AngularPOC.Data
{
    public class OrderingContext : DbContext
    {
        public DbSet<Order> Orders { get; set; }
        public DbSet<Report> Reports { get; set; }

        public OrderingContext(DbContextOptions<OrderingContext> options) : base(options)
        {
            ChangeTracker.QueryTrackingBehavior = Microsoft.EntityFrameworkCore.QueryTrackingBehavior.NoTracking;
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Order>().Property(s => s.AddedDate).HasDefaultValueSql("GETDATE()");
            builder.Entity<Order>().Property(s => s.ModifiedDate).HasDefaultValueSql("GETDATE()");
            //builder.Entity<Order>().Property(s => s.AddedDate).ValueGeneratedOnAdd();
            //builder.Entity<Order>().Property(s => s.ModifiedDate).ValueGeneratedOnAddOrUpdate();

            builder.Entity<Report>().Property(s => s.AddedDate).HasDefaultValueSql("GETDATE()");
            builder.Entity<Report>().Property(s => s.ModifiedDate).HasDefaultValueSql("GETDATE()");
            //builder.Entity<Report>().Property(s => s.AddedDate).ValueGeneratedOnAdd();
            //builder.Entity<Report>().Property(s => s.ModifiedDate).ValueGeneratedOnAddOrUpdate();
        }
    }
}
