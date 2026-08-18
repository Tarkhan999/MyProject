using FiorelloBackendPractice.Data;
using FiorelloBackendPractice.Models;
using FiorelloBackendPractice.Services;
using FiorelloBackendPractice.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSession();

// 1. EKSİK OLAN KISIM: DbContext'i sisteme tanıtıyoruz
builder.Services.AddDbContext<AppDbContext>(options =>
{
    // "DefaultConnection" kısmı appsettings.json dosyasındaki isimle aynı olmalıdır.
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultDatabase"));
});

// 2. Identity ayarları (DbContext'ten sonra gelmeli)
builder.Services.AddIdentity<AppUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>();

builder.Services.Configure<IdentityOptions>(options =>
{
    // Password settings.
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 6;
    options.Password.RequiredUniqueChars = 1;
    options.User.RequireUniqueEmail = true;
});

// Servis kayıtları
builder.Services.AddScoped<ISliderService, SliderService>();
builder.Services.AddScoped<IBlogService, BlogService>();
builder.Services.AddScoped<ICategoryService, CategoyService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ISettingService, SettingService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();

app.UseAuthorization();
app.UseSession();

app.MapControllerRoute(name:"areas",pattern:"{area:exists}/{controller=Dashboard}/{action=Index}/{id?}");
app.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
app.Run();