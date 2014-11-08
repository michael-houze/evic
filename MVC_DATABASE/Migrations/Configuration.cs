namespace MVC_DATABASE.Migrations
{
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using MVC_DATABASE.Models;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<MVC_DATABASE.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(MVC_DATABASE.Models.ApplicationDbContext context)
        {
        if (!context.Roles.Any())
        {
            var roleStore = new ApplicationRoleStore(context);
            var roleManager = new ApplicationRoleManager(roleStore);
            var role = new ApplicationRole{
                Name = "Administrator"
            };
            var role2 = new ApplicationRole
            {
                Name = "Employee"
            };
            var role3 = new ApplicationRole
            {
                Name = "Vendor"
            };
            roleManager.Create(role);
            roleManager.Create(role2);
            roleManager.Create(role3);
        }

        if (!context.Users.Any())
        {
            var userStore = new ApplicationUserStore(context);
            var userManager = new ApplicationUserManager(userStore);

            var user = new ApplicationUser {
                Email = "admin@example.com",
                UserName = "admin@example.com",
                EmailConfirmed = true
            };
            userManager.Create(user, "Admin@123456");
            userManager.AddToRole(user.Id, "Administrator");
        }
    }
        }
    
}
