using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using NominaApp.Data;
using NominaApp.Data.Sqlite;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddSingleton<WeatherForecastService>();

builder.Services.AddSingleton<SqliteConnectionFactory>();
builder.Services.AddSingleton<SqliteDbInitializer>();

builder.Services.AddSingleton<NominaApp.Services.Queries.ISqlQueryExecutor, NominaApp.Services.Queries.SqlQueryExecutor>();
builder.Services.AddSingleton<NominaApp.Services.CatalogosService>();
builder.Services.AddSingleton<NominaApp.Services.ReportesService>();
builder.Services.AddScoped<NominaApp.Services.ExcelExportService>();
builder.Services.AddSingleton<NominaApp.Services.Commands.SqlCommandExecutor>();
builder.Services.AddSingleton<NominaApp.Services.MovimientosService>();
builder.Services.AddSingleton<NominaApp.Services.PrestamosService>();
builder.Services.AddSingleton<NominaApp.Services.AbonosService>();

var app = builder.Build();

// Initialize SQLite schema
var sqliteInit = app.Services.GetRequiredService<SqliteDbInitializer>();
sqliteInit.Initialize();

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

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
