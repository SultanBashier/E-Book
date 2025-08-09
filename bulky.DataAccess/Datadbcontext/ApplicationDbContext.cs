
using bulkyBook.Models;
using BulkyBook.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace bulkyBook.DataAccess.Datadbcontext
{
    //  ApplicationDbContext represents the EF Core database context,
    // including Identity support and custom app entities.
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        //  Define database tables (DbSets) for each entity model
        public DbSet<Category> categories { get; set; }
        public DbSet<Product> products { get; set; }
        public DbSet<ProductImage> productImages { get; set; }
        public DbSet<ApllicationUser> apllicationUsers { get; set; }
        public DbSet<Company> companies { get; set; }
        public DbSet<ShoppingCart> shoppingCarts { get; set; }
        public DbSet<OrderDetail> orderDetails { get; set; }
        public DbSet<OrderHeader> orderHeaders { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            base.OnModelCreating(modelBuilder);
            //  Seeding initial Category data into the database
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Programming", Displayorder = 1 }
            
                
                );

            // Seeding initial Product data into the database
            modelBuilder.Entity<Product>().HasData(
                new Product
                {
                    Id = 1,
                    Title = "Learning C++",
                    Author = "Michael N",
                    Description = "This book is a simplified and comprehensive guide to learning the basics of the C++ programming language from scratch to an intermediate level. It is ideal for beginners who have never programmed before, as well as those with a basic background who want to develop their skills.",
                    ISBN = "SWD9999001",
                    ListPrice = 99,
                    Price = 20,
                    Price50 = 15,
                    Price100 = 15,
                    CategoryId = 1,
                   

                },
                new Product
                {
                    Id = 2,
                    Title = "Machine Learning",
                    Author = "ANDROW",
                    Description = "A simplified book that explains the basics of artificial intelligence and its applications in everyday life, such as machine learning and computer vision. Suitable for beginners and anyone who wants to understand how machines think and learn. ",
                    ISBN = "CAW777777701",
                    ListPrice = 99,
                    Price = 22,
                    Price50 = 20,
                    Price100 = 15,
                    CategoryId = 1,
                   

                },
                new Product
                {
                    Id = 3,
                    Title = "Vanish in the Sunset",
                    Author = "Julian Button",
                    Description = "Praesent vitae sodales libero. Praesent molestie orci augue, vitae euismod velit sollicitudin ac. Praesent vestibulum facilisis nibh ut ultricies.\r\n\r\nNunc malesuada viverra ipsum sit amet tincidunt. ",
                    ISBN = "RITO5555501",
                    ListPrice = 55,
                    Price = 50,
                    Price50 = 40,
                    Price100 = 35,
                    CategoryId = 1,
                    

                },
                new Product
                {
                    Id = 4,
                    Title = "Cotton Candy",
                    Author = "Abby Muscles",
                    Description = "Praesent vitae sodales libero. Praesent molestie orci augue, vitae euismod velit sollicitudin ac. Praesent vestibulum facilisis nibh ut ultricies.\r\n\r\nNunc malesuada viverra ipsum sit amet tincidunt. ",
                    ISBN = "WS3333333301",
                    ListPrice = 70,
                    Price = 65,
                    Price50 = 60,
                    Price100 = 55,
                    CategoryId = 1,
                    

                },
                new Product
                {
                    Id = 5,
                    Title = "Rock in the Ocean",
                    Author = "Ron Parker",
                    Description = "Praesent vitae sodales libero. Praesent molestie orci augue, vitae euismod velit sollicitudin ac. Praesent vestibulum facilisis nibh ut ultricies.\r\n\r\nNunc malesuada viverra ipsum sit amet tincidunt. ",
                    ISBN = "SOTJ1111111101",
                    ListPrice = 30,
                    Price = 27,
                    Price50 = 25,
                    Price100 = 20,
                    CategoryId = 1,
                   


                },
                new Product
                {
                    Id = 6,
                    Title = "Leaves and Wonders",
                    Author = "Laura Phantom",
                    Description = "Praesent vitae sodales libero. Praesent molestie orci augue, vitae euismod velit sollicitudin ac. Praesent vestibulum facilisis nibh ut ultricies.\r\n\r\nNunc malesuada viverra ipsum sit amet tincidunt. ",
                    ISBN = "FOT000000001",
                    ListPrice = 25,
                    Price = 23,
                    Price50 = 22,
                    Price100 = 20,
                    CategoryId = 1
                    ,
                  
                }
                );

        }
    }
}
