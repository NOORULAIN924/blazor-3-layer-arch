using ProductManagementApp.Components;
using ProductManagementApp.Business.Interfaces;
using ProductManagementApp.Business.Services;
using ProductManagementApp.DataAccessLayer.Interfaces;
using ProductManagementApp.DataAccessLayer.Repositories;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();


builder.Services.AddSingleton<IProductRepository, ProductRepositoryJson>();

builder.Services.AddScoped<IProductService, ProductService>();