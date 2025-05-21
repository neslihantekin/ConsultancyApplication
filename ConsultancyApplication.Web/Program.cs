using ConsultancyApplication.Core.Entities;
using ConsultancyApplication.Core.Interfaces.Repositories;
using ConsultancyApplication.Core.Interfaces.Services;
using ConsultancyApplication.Infrastructure.APIClients;
using ConsultancyApplication.Infrastructure.Data;
using ConsultancyApplication.Infrastructure.Repositories;
using ConsultancyApplication.Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 1. DbContext ayarý + Identity yapýlandýrmasý
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// 2. Servis ve Repository kayýtlarý
builder.Services.AddScoped<ITokenRepository, TokenRepository>();
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<IConsumptionRepository, ConsumptionRepository>();
builder.Services.AddScoped<ICurrentEndexesRepository, CurrentEndexesRepository>();
builder.Services.AddScoped<IEndOfMonthEndexesRepository, EndOfMonthEndexesRepository>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<IConsumptionService, ConsumptionService>();
builder.Services.AddScoped<ICurrentEndexesService, CurrentEndexesService>();
builder.Services.AddScoped<IEndOfMonthEndexesService, EndOfMonthEndexesService>();
builder.Services.AddSingleton<UserSession>(); // Custom oturum yapýsý

builder.Services.AddHttpClient<ApiClient>(); // API Client kaydý

// 3. MVC & View
builder.Services.AddControllersWithViews();

// 4. Cookie yönlendirme ayarlarý
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
});

// 5. CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder => builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
});

var app = builder.Build();

// 6. Admin kullanýcýyý ve Admin rolünü seed et
using (var scope = app.Services.CreateScope())
{
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    string adminEmail = "admin@demo.com";
    string adminPassword = "Admin123!";

    // Rol yoksa oluþtur
    if (!await roleManager.RoleExistsAsync("Admin"))
    {
        await roleManager.CreateAsync(new IdentityRole("Admin"));
    }

    // Admin kullanýcý yoksa oluþtur
    var adminUser = await userManager.FindByEmailAsync(adminEmail);
    if (adminUser == null)
    {
        adminUser = new IdentityUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            EmailConfirmed = true
        };
        await userManager.CreateAsync(adminUser, adminPassword);
        await userManager.AddToRoleAsync(adminUser, "Admin");
    }
}

// 7. Middleware sýralamasý
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // app.UseHsts();
}

app.UseStaticFiles();
app.UseRouting();

app.UseCors("AllowAll");

app.UseAuthentication(); // Identity kullanýcý oturumu
app.UseAuthorization();

// 8. Varsayýlan route
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}"
);

app.Run();
