using Mango.Web.Models;
using Mango.Web.Models.Dto;

namespace Mango.Web.Services.Interfaces
{
    public interface IGenericService : IDisposable
    {
        ResponseDto Response { get; set; }

        Task<T> SendAsync<T>(ApiRequest request);
    }
}
