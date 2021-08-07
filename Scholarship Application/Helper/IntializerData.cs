using Microsoft.AspNetCore.Identity;
using Scholarship_Application.Data;
using Scholarship_Application.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Scholarship_Application.Helper
{
    public static class IntializerData
    {

        public static async Task Initialize(ApplicationDbContext context, SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {

            if (!roleManager.Roles.Any())
            {
                List<IdentityRole> roles = new List<IdentityRole>
                {
                    new IdentityRole { Name ="Admin"} ,
                    new IdentityRole { Name ="Student"} ,

                };
                foreach (var item in roles)
                {
                    await roleManager.CreateAsync(item);
                }
            }

            if (!userManager.Users.Any())
            {

                var user = new ApplicationUser
                {
                    Email = "pgalfred2014@hotmail.com",
                    UserName = "PeterGeorege",
                    EmailConfirmed = true
                };


                await userManager.CreateAsync(user, "Peter123*");
                await userManager.AddToRoleAsync(user, "Admin");
                if (user.PasswordHash == null)
                    await signInManager.SignInAsync(user, true);
                else
                {


                    var result = await signInManager.PasswordSignInAsync(user.Email, "Peter123*", true, false);
                    if (result.Succeeded)
                        Console.WriteLine("Ok");
                }



            } // if There not  any User in DB Create Default 




        }
    }
}
