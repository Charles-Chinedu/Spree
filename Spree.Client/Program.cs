using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Spree.Client.Pages.OtherPages;
using Spree.Client.Services;

internal class Program
{
    private static async Task Main(string[] args)
    {
        var builder = WebAssemblyHostBuilder.CreateDefault(args);
        builder.Services.AddScoped<IProductService, ClientServices>()
                        .AddScoped<ICategoryService, ClientServices>()
                        .AddScoped<ICartService, ClientServices>();

        builder.Services.AddScoped<MessageDialogService>();

        builder.Services.AddBlazoredLocalStorage();

        builder.Services.AddScoped(sp => new HttpClient 
        {
            BaseAddress = new Uri("https://localhost:7098/")
        });

        await builder.Build().RunAsync();
    }
}