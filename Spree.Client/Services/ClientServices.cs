using Spree.Libraries.AuthState;
using Spree.Libraries.DTOs;
using Spree.Libraries.Models;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using static Spree.Libraries.Response.CustomResponses;

namespace Spree.Client.Services
{
    public class ClientServices(HttpClient httpClient ) : IProductService, ICategoryService, IAccountService
    {
        private readonly HttpClient _httpClient = httpClient;
        private const string ProductBaseUrl = "api/Product";
        private const string CategoryBaseUrl = "api/Category";
        private const string AuthenticationBaseUrl = "api/account";

        public List<Product> AllProducts { get; set; }
        public List<Product> InStock { get; set; }
        public List<Category> AllCategories { get; set; }
        public List<Product> ProductsByCategory { get; set; }

        //Product
        public async Task<List<Product>> GetAllProductsAsync()
        {
            var products = await _httpClient.GetAsync($"{ProductBaseUrl}/All-Products");
            bool check = CheckIfUnauthorized(products);
            if (check)
            {
                await GetRefreshToken();
                return await GetAllProductsAsync();
            }
            var response = await products.Content.ReadFromJsonAsync<List<Product>>();
            return response!;
        }

        public async Task<Product> GetProductById(int productId)
        {
            var product = await _httpClient.GetAsync($"{ProductBaseUrl}/Single-Product/{productId}");
            var response = await product.Content.ReadFromJsonAsync<Product>();
            return response!;
        }

        public async Task<List<Product>> GetProductsByCategory(int categoryId)
        {
            var allProducts = await GetAllProductsAsync();
            var productsByCategory = allProducts.Where(_ => _.CategoryId == categoryId).ToList();
            return productsByCategory;
        }

        //Category
        public async Task<ServiceResponse> AddCategory(Category model)
        {
            GetProtectedClient();
            var response = await _httpClient.PostAsJsonAsync($"{CategoryBaseUrl}/Add-Category", model);
            var category = await response.Content.ReadFromJsonAsync<ServiceResponse>();
            return category!;
        }

        public async Task<List<Category>> GetAllCategoriesAsync()
        {
            GetProtectedClient();
            var response = await _httpClient.GetAsync($"{CategoryBaseUrl}/All-Categories");
            bool unauthorized = CheckIfUnauthorized(response);
            if (unauthorized)
            {
                await GetRefreshToken();
                return await GetAllCategoriesAsync();
            }
            var categories = await response.Content.ReadFromJsonAsync<List<Category>>();
            return categories!;
        }

        public async Task<ServiceResponse> EditCategory(Category model)
        {
            GetProtectedClient();
            var response = await _httpClient.PutAsJsonAsync($"{CategoryBaseUrl}/Edit-Category", model);
            var updatedCategory = await response.Content.ReadFromJsonAsync<ServiceResponse>();
            return new ServiceResponse(true, "Edited Successfully");
        }

        public async Task<ServiceResponse> DeleteCategoryAsync(int Id)
        {
            GetProtectedClient();
            var response = await _httpClient.DeleteFromJsonAsync<ServiceResponse>($"{CategoryBaseUrl}/Delete-Category/{Id}");
            if (!response!.Flag) return new ServiceResponse(false, "Failed to delete Category");
            return new ServiceResponse(true, "Deleted Successfully");
        }

        // Account
        public async Task<RegistrationResponse> RegisterAsync(RegisterDTO model)
        {
            var response = await _httpClient.PostAsJsonAsync($"{AuthenticationBaseUrl}/register", model);
            if (!response.IsSuccessStatusCode)
                return new RegistrationResponse(false, "Error Occured");

            var apiResponse = await response.Content.ReadFromJsonAsync<RegistrationResponse>();
            return apiResponse!;
        }

        private static bool CheckIfUnauthorized(HttpResponseMessage httpResponseMessage)
        {
            if (httpResponseMessage.StatusCode == HttpStatusCode.Unauthorized)
                return true;
            else
                return false;
        }

        private void GetProtectedClient()
        {
            if (Constants.JWToken == "") return;

            _httpClient.DefaultRequestHeaders.Authorization = 
                new AuthenticationHeaderValue("Bearer", Constants.JWToken);
        }

        private async Task GetRefreshToken()
        {
            var response = await httpClient.PostAsJsonAsync($"{AuthenticationBaseUrl}/refresh-token", new UserSession() { JWToken = Constants.JWToken });
            var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
            Constants.JWToken = result!.JWToken;
        }

        public async Task<LoginResponse> LoginAsync(LoginDTO model)
        {
            var response = await _httpClient.PostAsJsonAsync($"{AuthenticationBaseUrl}/login", model);
            if (!response.IsSuccessStatusCode)
                return new LoginResponse(false, "Error occured", null!);

            var apiResponse = await response.Content.ReadFromJsonAsync<LoginResponse>();
            return apiResponse!; 
        }

        public async Task<LoginResponse> RefreshToken(UserSession userSession)
        {
            var response = await _httpClient.PostAsJsonAsync($"{AuthenticationBaseUrl}/refrresh-token", userSession);
            if (!response.IsSuccessStatusCode)
                return new LoginResponse(false, "Error occured", null!);

            var apiResponse = await response.Content.ReadFromJsonAsync<LoginResponse>();
            return apiResponse!;
        }

    }
}
