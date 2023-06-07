using Assignment_CS5.Database;
using Assignment_CS5.Helpers;
using Assignment_CS5.IServices;
using Assignment_CS5.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews().AddDataAnnotationsLocalization(); ;
var connectionString = builder.Configuration.GetConnectionString("connection");
builder.Services.AddDbContext<MyDbContext>(options => options.UseSqlServer(connectionString));
builder.Services.AddTransient<IMenuSvc,MenuSvc>();
builder.Services.AddTransient<IEmployeeSvc, EmployeeSvc>();
builder.Services.AddTransient<ICustomerSvc, CustomerSvc>();
builder.Services.AddTransient<IOrderSvc, OrderSvc>();
builder.Services.AddTransient<IOrderDetailSvc, OrderDetailSvc>();
builder.Services.AddTransient<IPayPalService, PayPalService>();


builder.Services.AddTransient<IUploadHelper, UploadHelper>();
builder.Services.AddTransient<IEncodeHelper, EncodeHelper>();
builder.Services.AddDistributedMemoryCache(); // Đăng ký dv lưu cache trong bộ nhớ
builder.Services.AddSession(option =>
{
    option.IdleTimeout = TimeSpan.FromMinutes(30);
});

builder.Services.AddCors();

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
app.UseSession();
app.UseCors(builder => builder
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader());
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
