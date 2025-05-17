using ConsultancyApplication.Core.Entities;
using ConsultancyApplication.Core.Interfaces.Repositories;
using ConsultancyApplication.Core.Interfaces.Services;
using ConsultancyApplication.Infrastructure.APIClients;
using ConsultancyApplication.Infrastructure.Data;
using ConsultancyApplication.Infrastructure.Repositories;
using ConsultancyApplication.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 1. ApplicationDbContext i�in ba�lant� c�mlesi ayarlan�yor.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2. Repository kay�tlar� (Core katman�ndaki aray�zlerden)
builder.Services.AddScoped<ITokenRepository, TokenRepository>();
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<IConsumptionRepository, ConsumptionRepository>();
builder.Services.AddScoped<ICurrentEndexesRepository, CurrentEndexesRepository>();
builder.Services.AddScoped<IEndOfMonthEndexesRepository, EndOfMonthEndexesRepository>();

// 3. Servis kay�tlar� (Core katman�ndaki servis aray�zleri)
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<IConsumptionService, ConsumptionService>();
builder.Services.AddScoped<ICurrentEndexesService, CurrentEndexesService>();
builder.Services.AddScoped<IEndOfMonthEndexesService, EndOfMonthEndexesService>(); 
builder.Services.AddSingleton<UserSession>(); // T�m uygulama i�in 1 oturum

// 4. ApiClient i�in HttpClient kayd�
builder.Services.AddHttpClient<ApiClient>();

// 5. MVC Controller'lar ve Razor View'ler i�in gerekli ayar
builder.Services.AddControllersWithViews();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
        builder.AllowAnyOrigin()
               .AllowAnyHeader()
               .AllowAnyMethod());
});

var app = builder.Build();

// Geli�tirme ortam� d���ndaysa hata sayfas� ayarlan�yor.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // app.UseHsts();
}

app.UseStaticFiles();
app.UseRouting();
                               
app.UseCors("AllowAll"); 
app.UseDeveloperExceptionPage();

// Identity ve Authorization middleware'leri
app.UseAuthentication();
app.UseAuthorization();

// Varsay�lan route ayar�
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}"
);
app.Run();
