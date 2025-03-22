using Blazored.LocalStorage;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Scalar.AspNetCore;
using Spree.Client.Pages.OtherPages;
using Spree.Client.Services;
using Spree.Components;
using Spree.Data;
using Spree.Interface;
using Spree.Libraries.AuthState;
using Spree.Services;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHttpContextAccessor();
//builder.Services.AddSwaggerGen(swagger =>
//{
//    //This is to generate the default UI of swagger Documentation
//    swagger.SwaggerDoc("v1", new OpenApiInfo
//    {
//        Version = "v1",
//        Title = "Asp.NET 8 Web API",
//        Description = "Authentication with JWT"
//    });
//    //To enable Authorization 
//    swagger.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
//    {
//        Name = "Authorization",
//        Type = SecuritySchemeType.ApiKey,
//        Scheme = "Bearer",
//        BearerFormat = "JWT",
//        In = ParameterLocation.Header,
//    });
//    swagger.AddSecurityRequirement(new OpenApiSecurityRequirement()
//{
//    {
//        new OpenApiSecurityScheme
//        {
//            Reference = new OpenApiReference
//            {
//                Type = ReferenceType.SecurityScheme,
//                Id = "Bearer"
//            }
//        },Array.Empty<string>()
//    }
//    });

//});

builder.Services.AddDbContext<StoringData>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")
        ?? throw new InvalidOperationException("Connection string not found"));
});

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateIssuerSigningKey = true,
        ValidateLifetime = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
    };
});

builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();

builder.Services.AddScoped<IAccount, AccountService>();
builder.Services.AddScoped<IProduct, ProductService>()
                .AddScoped<ICategory, CategoryService>()
                .AddScoped<IAdminProduct, AdminProductServices>();

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("https://localhost:7098/") });

// Client Side
builder.Services.AddScoped<MessageDialogService>();
builder.Services.AddScoped<IAccountService, ClientServices>()
                .AddScoped<ICategoryService, ClientServices>()
                .AddScoped<IProductService, ClientServices>()
                .AddScoped<ICartService, ClientServices>()
                .AddScoped<IAdminProductService, AdminClientService>();

builder.Services.AddBlazoredLocalStorage();
builder.Services.AddOpenApi();

var app = builder.Build();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
    app.MapOpenApi();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.MapScalarApiReference(); // scalar/v1

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();
//app.UseSwagger();
//app.UseSwaggerUI(s =>
//{
//    s.SwaggerEndpoint("/swagger/v1/swagger.json", "Your API Name V1");
//});

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(Spree.Client._Imports).Assembly);
app.MapControllers();
app.UseAuthentication();
app.UseAuthorization();
app.Run();
