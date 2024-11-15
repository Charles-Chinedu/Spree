using Spree.Client.PrivateModel;
using Spree.Libraries.Models;
using static Spree.Libraries.Response.CustomResponses;

namespace Spree.Client.Services
{
    public interface ICartService
    {
        public Action? CartAction { get; set; }
        public int CartCount { get; set; }
        Task GetCartCount();
        Task<ServiceResponse> AddToCart(Product model, int updateQuantity, bool isAdding);
        Task<List<Order>> MyOrder();
        Task<ServiceResponse> DeleteCart(Order cart);
    }
}
