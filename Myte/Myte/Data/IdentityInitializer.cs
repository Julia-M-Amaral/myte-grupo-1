using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Myte.Models;


namespace Myte.Data
{
    public class IdentityInitializer
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _config;

        public IdentityInitializer(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, ApplicationDbContext context, IConfiguration config)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
            _config = config;
        }

        public async Task SeedAdminAsync()
        {
            var roleAdmin = "Admin";
            if (!await _roleManager.RoleExistsAsync(roleAdmin))
            {
                
                await _roleManager.CreateAsync(new IdentityRole(roleAdmin));
            }

            var adminEmail = _config["AdminCredentials:Email"];
            var adminPassword = _config["AdminCredentials:Password"];

            var admin = await _userManager.FindByEmailAsync(adminEmail);

         
            //SeedUsersAsync
            if (admin == null)
            {
                admin = new IdentityUser { UserName = adminEmail, Email = adminEmail, EmailConfirmed = true };
                var result = await _userManager.CreateAsync(admin, adminPassword);

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(admin, roleAdmin);
                }
            }


            else
            {
                if (!await _userManager.IsInRoleAsync(admin, roleAdmin))
                {
                    await _userManager.AddToRoleAsync(admin, roleAdmin);
                }
            }

        }

        public async Task SeedFuncAsync()
        {
            if (!await _roleManager.RoleExistsAsync("Func"))
            {
                IdentityRole role = new IdentityRole();
                role.Name = "Func";
                role.NormalizedName = "Func";
                role.ConcurrencyStamp = Guid.NewGuid().ToString();

                IdentityResult roleResult = await _roleManager.CreateAsync(role);
            }

            if (await _userManager.FindByEmailAsync("usuario@localhost") == null)
            {
                IdentityUser user = new IdentityUser();
                user.UserName = "usuario@localhost";
                user.Email = "usuario@localhost";
                user.NormalizedUserName = "USUARIO@LOCALHOST";
                user.NormalizedEmail = "USUARIO@LOCALHOST";
                user.EmailConfirmed = true;
                user.LockoutEnabled = false;
                user.SecurityStamp = Guid.NewGuid().ToString();

                IdentityResult result = await _userManager.CreateAsync(user, "Numsey#2023");

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "Func");
                }
            }


        }
    }

}
    

