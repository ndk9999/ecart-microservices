using Mango.Services.ShoppingCartApi.Models.Dto;

namespace Mango.Services.ShoppingCartApi.Repositories
{
    public interface ICartRepository
    {
        Task<CartDto> GetCartByUserId(string userId);

        Task<CartDto> CreateUpdateCart(CartDto cart);

        Task<bool> RemoveFromCart(int cartDetailId);

        Task<bool> ClearCart(string userId);
    }
}
