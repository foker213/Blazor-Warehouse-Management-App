using WarehouseManagement.DataBase;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddDataBase(builder.Configuration);

var app = builder.Build();

app.UseBlazorFrameworkFiles();
app.UseStaticFiles();
app.MapFallbackToFile("index.html");

if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}

app.Run();
