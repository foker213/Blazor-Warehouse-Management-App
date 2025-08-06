using WarehouseManagement.DataBase;
using WarehouseManagement.Application;

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

using (IServiceScope scope = app.Services.CreateScope())
{
    WarehouseDbContext warehouseDbContext = scope.ServiceProvider.GetRequiredService<WarehouseDbContext>();
    warehouseDbContext.Database.EnsureCreated();
}

app.Run();