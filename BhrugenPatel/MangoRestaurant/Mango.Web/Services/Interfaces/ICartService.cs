using Mango.Web.Models.Dto;

namespace Mango.Web.Services.Interfaces
{
    public interface ICartService
    {
        Task<T> GetCartByUserIdAsync<T>(string userId, string accessToken = null);

        Task<T> AddToCartAsync<T>(CartDto cartDto, string accessToken = null);

        Task<T> UpdateCartAsync<T>(CartDto cartDto, string accessToken = null);

        Task<T> RemoveFromCartAsync<T>(int itemId, string accessToken = null);

        Task<T> ClearCartAsync<T>(string userId, string accessToken = null);
    }
}
