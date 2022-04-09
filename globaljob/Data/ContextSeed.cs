using globaljob.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace globaljob.Data
{
    public static class ContextSeed
    {
        public static async Task SeedRolesAsync(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            //Seed Roles
            await roleManager.CreateAsync(new IdentityRole(Roles.Roles.SuperAdmin.ToString()));
            await roleManager.CreateAsync(new IdentityRole(Roles.Roles.Admin.ToString()));
            await roleManager.CreateAsync(new IdentityRole(Roles.Roles.Chercheur.ToString()));
            await roleManager.CreateAsync(new IdentityRole(Roles.Roles.Recruteur.ToString()));
        }
    }
}
