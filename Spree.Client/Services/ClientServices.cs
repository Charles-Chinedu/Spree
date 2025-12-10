using Spree.Libraries.AuthState;
using Spree.Libraries.DTOs;
using Spree.Libraries.Models;
using Blazored.LocalStorage;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using static Spree.Libraries.Response.CustomResponses;
using Spree.Client.PrivateModel;
using System.Text.Json;
using Microsoft.AspNetCore.Http;

namespace Spree.Client.Services
{
    public class ClientServices(HttpClient httpClient, ILocalStorageService localStorageService) : IProductService, ICategoryService, IAccountService, ICartService
    {
        private readonly HttpClient _httpClient = httpClient;
        private readonly ILocalStorageService _localStorage = localStorageService;

        private const string ProductBaseUrl = "api/Product";
        private const string CategoryBaseUrl = "api/Category";
        private const string AuthenticationBaseUrl = "api/account";

        public List<Product> AllProducts { get; set; }
        public List<Product> InStock { get; set; }
        public List<Category> AllCategories { get; set; }
        public List<Product> ProductsByCategory { get; set; }
        public Action? CartAction { get; set; }
        public int CartCount { get; set; }

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
            try
            {
                GetProtectedClient();
                var response = await _httpClient.GetAsync($"{CategoryBaseUrl}/All-Categories");
                bool unauthorized = CheckIfUnauthorized(response);
                if (unauthorized)
                {
                    await GetRefreshToken();
                    return await GetAllCategoriesAsync();
                }
                return await response.Content.ReadFromJsonAsync<List<Category>>() ?? new List<Category>();
            }
            catch
            {
                return new List<Category>();
            }
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
            if (apiResponse!.JWToken != null)
            {
                Constants.JWToken = apiResponse.JWToken;
                await MergeCartWithDatabase();
            }
            return apiResponse!;
        }

        private async Task MergeCartWithDatabase()
        {
            string cartString = await GetCartFromLocalStorage();
            if (!string.IsNullOrEmpty(cartString))
            {
                var localCart = JsonSerializer.Deserialize<List<CartStorage>>(cartString);
                if (localCart != null && localCart.Count > 0)
                {
                    var response = await _httpClient.PostAsJsonAsync("api/cart/merge", localCart);
                    if (response.IsSuccessStatusCode)
                    {
                        await RemoveCartFromLocalStorage();
                    }
                }
            }
        }

        public async Task<LoginResponse> RefreshToken(UserSession userSession)
        {
            var response = await _httpClient.PostAsJsonAsync($"{AuthenticationBaseUrl}/refrresh-token", userSession);
            if (!response.IsSuccessStatusCode)
                return new LoginResponse(false, "Error occured", null!);

            var apiResponse = await response.Content.ReadFromJsonAsync<LoginResponse>();
            return apiResponse!;
        }

        // Cart
        public async Task GetCartCount()
        {
            string cartString = await GetCartFromLocalStorage()!;
            if (string.IsNullOrEmpty(cartString))
                CartCount = 0;
            else
                CartCount = JsonSerializer.Deserialize<IList<CartStorage>>(cartString)!.Count;
            CartAction?.Invoke();

        }
        public async Task<ServiceResponse> AddToCart(Product model, int updateQuantity, bool isAdding)
        {
            string message = string.Empty;
            var myCart = new List<CartStorage>();

            var getCartFromStorage = await GetCartFromLocalStorage();
            if (!string.IsNullOrEmpty(getCartFromStorage))
            {
                myCart = JsonSerializer.Deserialize<List<CartStorage>>(getCartFromStorage);
            }

            var cartItem = myCart!.FirstOrDefault(_ => _.ProductId == model.Id);

            if (isAdding)
            {
                if (cartItem != null)
                {
                    int newQuantity = cartItem.Quantity + updateQuantity;
                    if (newQuantity > model.Quantity)
                    {
                        message = "Not enough stock available.";
                        return new ServiceResponse(false, message);
                    }
                    cartItem.Quantity = newQuantity;
                    message = "Product added Successfully";
                }
                else
                {
                    if (updateQuantity > model.Quantity)
                    {
                        message = "Not enough stock available.";
                        return new ServiceResponse(false, message);
                    }
                    myCart!.Add(new CartStorage() { ProductId = model.Id, Quantity = updateQuantity });
                    message = "Product Added.";
                }

                model.Quantity -= updateQuantity;

            }
            else
            {
                if (cartItem != null)
                {
                    if (cartItem.Quantity <= updateQuantity)
                    {
                        myCart!.Remove(cartItem);
                        message = "Product removed Successfully";
                    }
                    else
                    {
                        cartItem.Quantity -= updateQuantity;
                        message = "Product removed Successfully";
                    }

                    model.Quantity += updateQuantity;
                }
                else
                {
                    return new ServiceResponse(false, "Product not found");
                }
            }

            await RemoveCartFromLocalStorage();
            await SetCartToLocalStorage(JsonSerializer.Serialize(myCart));
            var isLoggedIn = !string.IsNullOrEmpty(Constants.JWToken);
            if (isLoggedIn)
            {
                var response = await _httpClient.PostAsJsonAsync("api/cart/merge", myCart);
                if (response.IsSuccessStatusCode)
                {
                    await GetCartCount(); // Update cart count after merge
                }
            }

            return new ServiceResponse(true, message);

        }



        public async Task<List<Order>> MyOrder()
        {
            var cartList = new List<Order>();
            string myListJson = await GetCartFromLocalStorage();
            if (string.IsNullOrEmpty(myListJson)) return null!;

            var myCartList = JsonSerializer.Deserialize<IList<CartStorage>>(myListJson);
            if (myCartList == null)
            {
                return new List<Order>();
            }

            foreach (var cartItem in myCartList!)
            {
                var product = await GetProductById(cartItem.ProductId);
                if (product == null)
                    continue;
                cartList.Add(new Order
                {
                    Id = cartItem.ProductId,
                    Name = product.Name,
                    Quantity = cartItem.Quantity,
                    Price = product.Price,
                    Image = product.ImageUrl
                });

            }

            await GetCartCount();
            return cartList;
        }

        public async Task<ServiceResponse> DeleteCart(Order cart)
        {
            var cartList = await GetCartFromLocalStorage();
            var myCartList = JsonSerializer.Deserialize<IList<CartStorage>>(cartList);
            if (myCartList == null) return new ServiceResponse(false, "Product not found");
            var cartItem = myCartList.FirstOrDefault(_ => _.ProductId == cart.Id);
            myCartList.Remove(cartItem!);
            await RemoveCartFromLocalStorage();
            await SetCartToLocalStorage(JsonSerializer.Serialize(myCartList));
            await GetCartCount();
            return new ServiceResponse(true, "Product Removed");
        }

        private async Task<string> GetCartFromLocalStorage() => await _localStorage.GetItemAsStringAsync("cart");
        private async Task SetCartToLocalStorage(string cart) => await _localStorage.SetItemAsStringAsync("cart", cart);
        private async Task RemoveCartFromLocalStorage() => await _localStorage.RemoveItemAsync("cart");

        //private bool IsUserLoggedIn()
        //{
        //    var user = _httpContextAccessor.HttpContext?.User ?? _anonymous;
        //    return user.Identity?.IsAuthenticated ?? false;
        //}
    }
}
