using Microsoft.EntityFrameworkCore;
using Recreation.Data;
using Recreation.Data.Repository;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("RecreationDatabase")
    ?? throw new InvalidOperationException("Connection string RecreationDatabase not foud");

builder.Services.AddDbContext<RecreationContext>(options =>
{
    options.UseNpgsql(connectionString, o => o.UseNetTopologySuite());
    options.LogTo(Console.WriteLine, LogLevel.Information);
});

builder.Services.AddScoped<IRepository, Repository>();

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();
app.DatabaseCreate<RecreationContext>();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseExceptionHandler("/Home/Error");
    
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
