using Auth_BackEnd.Context;
using Auth_BackEnd.Controllers;
using Auth_BackEnd.DTOs;
using Auth_BackEnd.Model;
using Auth_Test.Helpers;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auth_Test.Back_End
{
    [TestClass]

    public class AccountControllerTest : BaseTest
    {
        private static ApplicationDbContext CreateContext(DbContextOptions<ApplicationDbContext> options)
        {
            return new ApplicationDbContext(options, (context, modelBuilder) =>
            {
                modelBuilder.Entity<ApplicationUser>().ToInMemoryQuery(() =>
                context.Users.Select(b => new ApplicationUser
                {
                    Id = "E09D0551-C919-4F3D-9BDD",
                    UserName = "admintest",
                    Email = "admintest@test.com",
                    EmailConfirmed = true,
                    NormalizedEmail = "ADMINTEST@TEST.COM",

                    PasswordHash= "d1e8a70b5ccab1dc2f56bbf7e99f064a660c08e361a3575"
                }));


            });




        }


        ApplicationUser userAdmin = new ApplicationUser
        {
            Id = "E09D0551-C919-4F3D-9BDD",
            UserName = "Admin",
            Email = "admintest@test.com",
            EmailConfirmed = true,
            NormalizedEmail = "ADMINTEST@TEST.COM",
            PasswordHash = "d1e8a70b5ccab1dc2f56bbf7e99f064a660c08e361a3575"

        };
    


        [TestMethod]
        public async Task LoginAdminTestOK()
        {
            var logger = new Mock<ILogger<AccountController>>();
            var mapper = new Mock<IMapper>();
            var configuration = Configuration;//  new Mock<IConfiguration>();
            //var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            //optionsBuilder.UseInMemoryDatabase<ApplicationDbContext>(databaseName: Guid.NewGuid().ToString());
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
               .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
               .Options;

            var appUser=new Mock<UserManager<ApplicationUser>> ();
            var signIn=new Mock<SignInManager<ApplicationUser>> ();


            var userAdminLogin = new UserDTO
            {
                Email= "admintest@test.com",
                Password="String.1"
            };
            using (var context = CreateContext(options))
            {
                context.Users.Add(userAdmin);

                context.SaveChanges();

                var homeController = new AccountController(appUser.Object, signIn.Object, configuration, mapper.Object, logger.Object);
                ActionResult<UserToken> countResult = await homeController.Login(userAdminLogin);


                var pp = countResult.Result;
             //   OkObjectResult contentResult = countResult as OkObjectResult;

                Assert.AreEqual(1, pp);
            }
        }

    }
}
