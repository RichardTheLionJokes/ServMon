using ServMon.Models;
using Microsoft.EntityFrameworkCore;
using ServMon.Services.SrvMon;
using ServMon.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("settings.json", false, true);

// Add services to the container.
string? connection = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ServMonContext>(options => options.UseSqlServer(connection));
builder.Configuration.Bind("Project", new Config());
Config.ConnectionString = connection;

builder.Services.AddRazorPages();

builder.Services.AddHostedService<SrvScanner>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();
