using bulkyBook.DataAccess.Datadbcontext;
using bulkyBook.Models;
using bulkyBook.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bulkyBook.DataAccess.DbInitializer
{
    public class DbInitializer : IDbInitializer
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _db;

        public DbInitializer(
            ApplicationDbContext applicationDbContext
            , UserManager<IdentityUser> userManager
            , RoleManager<IdentityRole> roleManager)
        {
            _db = applicationDbContext;
            _userManager = userManager;
            _roleManager = roleManager;
        }


        public void Initialize()
        {
            //Migartions if they are not applied
            try
            {
                if (_db.Database.GetPendingMigrations().Count() > 0)
                {
                    _db.Database.Migrate();
                }

            }
            catch (Exception ex)
            { }
            //Create roles if they are not created
            if (!_roleManager.RoleExistsAsync(SD.Role_Customer).GetAwaiter().GetResult())
            {
                _roleManager.CreateAsync(new IdentityRole(SD.Role_Customer)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(SD.Role_Company)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(SD.Role_Admin)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(SD.Role_Employee)).GetAwaiter().GetResult();


                //if roles not created ,they we will create admin user as well
                //Create the default admin user with full access
                _userManager.CreateAsync(new ApllicationUser
                {
                    UserName = "admin@yahoo.com",
                    Email = "admin@yahoo.com",
                    Name = "Sultan Abo bashier",
                    PhoneNumber = "00201111818357",
                    StreetAdrress = "qena-qena",
                    state = "IL",
                    Postalcode = "23422",
                    City = "qena"
                }, "Admin@123").GetAwaiter().GetResult();

                //  Assign the admin role to the new user
                ApllicationUser user = _db.apllicationUsers.FirstOrDefault(u => u.Email == "admin@yahoo.com");
                _userManager.AddToRoleAsync(user, SD.Role_Admin).GetAwaiter().GetResult();


            }
            return;
        }


    }
}
