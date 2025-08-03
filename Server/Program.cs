using WarehouseManagement.DataBase;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddDataBase(builder.Configuration);

WebApplication app = builder.Build();

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