using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Myte.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false; // Desativando a confirmação de conta
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 6;
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddControllersWithViews();

// Configure IdentityInitializer
builder.Services.AddScoped<IdentityInitializer>();

// Configuração das políticas de autorização
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequerPerfilAdmin",
        policy => policy.RequireRole("Admin"));
    options.AddPolicy("RequerPerfilFuncOuAdmin",
        policy => policy.RequireRole("Func"));

    options.AddPolicy("RequerFuncOuAdmin", policy =>
        policy.RequireAssertion(context =>
            context.User.IsInRole("Admin") || context.User.IsInRole("Func")
        ));

    //options.AddPolicy("RequerFuncOuAdmin", policy =>
    //    policy.RequireAssertion(context =>
    //        context.User.HasClaim(c => c.Type == "Perfil" && (c.Value == "Func" || c.Value == "Admin"))
    //    ));

});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

await CriarPerfilUsuariosAsync(app);

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "AreaAdmin",
    pattern: "{area:exists}/{controller=Admin}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

async Task CriarPerfilUsuariosAsync(WebApplication app)
{
    var scopedFactory = app.Services.GetService<IServiceScopeFactory>();

    using (var scope = scopedFactory.CreateScope())
    {
        var service = scope.ServiceProvider.GetService<IdentityInitializer>();
        await service.SeedAdminAsync();
        await service.SeedFuncAsync();
    }
}

app.Run();
