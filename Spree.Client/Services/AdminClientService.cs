using Spree.Library.AuthState;
using Spree.Library.DTOs;
using Spree.Library.Models;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using static Spree.Library.Response.CustomResponses;

namespace Spree.Client.Services
{
    public class AdminClientService(HttpClient httpClient) : IAdminProductService 
    {
        private readonly HttpClient _httpClient = httpClient;
        private const string AdminProductBaseUrl = "api/AdminProduct";
        private const string AuthenticationBaseUrl = "api/Account";

        public async Task<ServiceResponse> AddProductAsync(Product model)
        {
            GetProtectedClient();
            var product = await _httpClient.PostAsJsonAsync($"{AdminProductBaseUrl}/Add-Product", model);
            var response = await product.Content.ReadFromJsonAsync<ServiceResponse>();

            return response!;
        }

        public async Task<ServiceResponse> DeleteProductAsync(int id)
        {
            GetProtectedClient();
            var product = await _httpClient.DeleteFromJsonAsync<ServiceResponse>($"{AdminProductBaseUrl}/Delete-Product/{id}");
            if (!product!.Flag) return new ServiceResponse(false, "Failed to delete Product");
            return new ServiceResponse(true, "Deleted Successfully");
        }

        public async Task<ServiceResponse> EditProductAsync(Product model)
        {
            GetProtectedClient();
            var response = await _httpClient.PutAsJsonAsync($"{AdminProductBaseUrl}/Edit-Product", model);
            var updatedProduct = await response.Content.ReadFromJsonAsync<Product>();
            return new ServiceResponse(true, "Edited Successfully");
        }

        public async Task<List<Product>> GetInStockAsync(bool inStock)
        {
            GetProtectedClient();
            var product = await _httpClient.GetAsync($"{AdminProductBaseUrl}/In-Stock");
            var response = await product.Content.ReadFromJsonAsync<List<Product>>();
            return response!;
        }

        public async Task<List<Product>> GetAllProductsAsync()
        {
            GetProtectedClient();
            var response = await _httpClient.GetAsync($"{AdminProductBaseUrl}/All-Products");
            bool check = CheckIfUnauthorized(response);
            if (check)
            {
                await GetRefreshToken();
                return await GetAllProductsAsync();
            }
            var products = await response.Content.ReadFromJsonAsync<List<Product>>();
            return products!;

        }

        public async Task<Product> GetProductByIdAsync(int id)
        {
            GetProtectedClient();
            var product = await _httpClient.GetAsync($"{AdminProductBaseUrl}/Single-Product/{id}");
            bool check = CheckIfUnauthorized(product);
            if (check)
            {
                await GetRefreshToken();
                return await GetProductByIdAsync(id);
            }
            var response = await product.Content.ReadFromJsonAsync<Product>();
            return response!;
        }

        private async Task GetRefreshToken()
        {
            var response = await _httpClient.PostAsJsonAsync($"{AuthenticationBaseUrl}/refresh-token", new UserSession() { JWToken = Constants.JWToken });
            var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
            Constants.JWToken = result!.JWToken;
        }

        private void GetProtectedClient()
        {
            if (Constants.JWToken == "") return;
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", Constants.JWToken);
        }

        private bool CheckIfUnauthorized(HttpResponseMessage httpResponseMessage)
        {
            if (httpResponseMessage.StatusCode == HttpStatusCode.Unauthorized)
                return true;
            else
                return false;
        }
    }
}
