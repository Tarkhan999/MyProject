using Microsoft.EntityFrameworkCore;
using FiorelloBackendPractice.Data;
using FiorelloBackendPractice.Services;
using FiorelloBackendPractice.Services.Interfaces; // AppDbContext-in olduğu qovluq (əgər fərqlidirsə, düzəldin)

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// BAZA BAĞLANTISI KODU BURAYA ƏLAVƏ OLUNMALIDIR:
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultDatabase")));
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
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();