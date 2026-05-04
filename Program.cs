using DotNetCoreSqlDb.Models;
using Microsoft.EntityFrameworkCore;
using Azure.Identity;
using Azure.Storage.Blobs;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.AddAzureWebAppDiagnostics();

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddSingleton(x =>
    new BlobServiceClient(
        new Uri("https://statican.blob.core.windows.net"),
        new DefaultAzureCredential()));

var connectionString = builder.Configuration.GetConnectionString("MyDbConnection");

builder.Services.AddDbContext<MyDatabaseContext>(options =>
                    options.UseSqlite("Data Source=localdatabase.db"));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<MyDatabaseContext>();
    context.Database.EnsureCreated(); 
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else {
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Todos}/{action=Index}/{id?}");

app.Run();
