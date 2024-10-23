using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Spree.Client.Pages;
using Spree.Client.Services;
using Spree.Libraries;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.Services.AddScoped<IProductService, ClientServices>()
                .AddScoped<ICategoryService, ClientServices>();

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("https://localhost:7098/") });

await builder.Build().RunAsync();
