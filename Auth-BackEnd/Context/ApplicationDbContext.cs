using Auth_BackEnd.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Auth_BackEnd.Context
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {

        private readonly Action<ApplicationDbContext, ModelBuilder> _customizeModel;

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {

        }

        //used in test project
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, Action<ApplicationDbContext, ModelBuilder> customizeModel)
              : base(options)
        {
            // customizeModel must be the same for every instance in a given application.
            // Otherwise a custom IModelCacheKeyFactory implementation must be provided.
            _customizeModel = customizeModel;
        }

        private void SeedUsers(ModelBuilder builder)
        {
            ApplicationUser user = new ApplicationUser()
            {
                Id = "E09D0551-C919-4F3D-9BDD-6FF2F983CF94",
                UserName = "admintest",
                Email = "admintest@test.com",
                EmailConfirmed = true,
                NormalizedEmail = "ADMINTEST@TEST.COM",

            };

            PasswordHasher<ApplicationUser> passwordHasher = new PasswordHasher<ApplicationUser>();
            user.PasswordHash = passwordHasher.HashPassword(user, "Admin.123");
            builder.Entity<ApplicationUser>().HasData(user);

            user = new ApplicationUser()
            {
                Id = "403B0631-D0DC-4910-B408-EB448E7869AC",
                UserName = "userTest",
                Email = "usertest@test.com",
                EmailConfirmed = true,
                NormalizedEmail = "USERTEST@TEST.COM",
            };

            passwordHasher = new PasswordHasher<ApplicationUser>();
            user.PasswordHash = passwordHasher.HashPassword(user, "User.123");

            builder.Entity<ApplicationUser>().HasData(user);
        }

        private void SeedRoles(ModelBuilder builder)
        {
            var roleAdmin = new IdentityRole()
            {
                Id = "F7D5F796-9D38-46AD-818E-454C349E5072",
                Name = "Admin",
                NormalizedName = "Admin",

            };

            builder.Entity<IdentityRole>().HasData(roleAdmin);


            var roleUser = new IdentityRole()
            {
                Id = "6F82D50F-A009-412B-86D9-8BB438AAFFCB",
                Name = "User",
                NormalizedName = "User"
            };

            builder.Entity<IdentityRole>().HasData(roleUser);

        }
        private void SeedUserRoles(ModelBuilder builder)
        {
            builder.Entity<IdentityUserRole<string>>().HasData(
               new IdentityUserRole<string>()
               {
                   RoleId = "F7D5F796-9D38-46AD-818E-454C349E5072",
                   UserId = "E09D0551-C919-4F3D-9BDD-6FF2F983CF94"
               }
               );
            builder.Entity<IdentityUserRole<string>>().HasData(
               new IdentityUserRole<string>()
               {
                   RoleId = "6F82D50F-A009-412B-86D9-8BB438AAFFCB",
                   UserId = "403B0631-D0DC-4910-B408-EB448E7869AC"
               }
               );
        }

        private void SeedClaims(ModelBuilder builder)
        {
            builder.Entity<IdentityUserClaim<string>>().HasData(
                new IdentityUserClaim<string>()
                {
                    UserId = "E09D0551-C919-4F3D-9BDD-6FF2F983CF94",
                    Id = 1,
                    ClaimType = "IsAdmin",
                    ClaimValue = "1"
                });

            builder.Entity<IdentityUserClaim<string>>().HasData(
               new IdentityUserClaim<string>()
               {
                   UserId = "403B0631-D0DC-4910-B408-EB448E7869AC",
                   Id = 2,
                   ClaimType = "IsUser",
                   ClaimValue = "1"
               });

        }

      
        protected override void OnModelCreating(ModelBuilder builder)
        {
            SeedUsers(builder);
            SeedRoles(builder);
            SeedUserRoles(builder);
            SeedClaims(builder);


            base.OnModelCreating(builder);
        }


    }
}
