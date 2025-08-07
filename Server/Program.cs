using WarehouseManagement.DataBase;
using WarehouseManagement.Application;
using WarehouseManagement.Server.Extensions;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddApplication()
    .AddDataBase(builder.Configuration);

builder.Services.AddControllers();

WebApplication app = builder.Build();

app.MapControllers();

app.UseBlazorFrameworkFiles();
app.UseStaticFiles();
app.MapFallbackToFile("index.html");

if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}

await app.MigrateDataBase();

app.Run();