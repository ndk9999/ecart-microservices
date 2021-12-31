using Mango.Web.Models;
using Mango.Web.Models.Dto;
using Mango.Web.Services.Interfaces;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace Mango.Web.Services
{
    public class GenericService : IGenericService
    {
        private readonly IHttpClientFactory _httpClient;

        public GenericService(IHttpClientFactory httpClient)
        {
            _httpClient = httpClient;
            Response = new ResponseDto();
        }

        public ResponseDto Response { get; set; }

        public async Task<T> SendAsync<T>(ApiRequest request)
        {
            try
            {
                var client = _httpClient.CreateClient("MangoApi");
                var message = new HttpRequestMessage();

                message.Headers.Add("Accept", "application/json");
                message.RequestUri = new Uri(request.Url);

                client.DefaultRequestHeaders.Clear();

                if (request.Payload != null)
                {
                    message.Content = new StringContent(
                        JsonConvert.SerializeObject(request.Payload),
                        Encoding.UTF8, "application/json");
                }

                if (!string.IsNullOrWhiteSpace(request.AccessToken))
                {
                    client.DefaultRequestHeaders.Authorization = 
                        new AuthenticationHeaderValue("Bearer", request.AccessToken);
                }

                message.Method = request.Type switch
                {
                    ApiType.POST => HttpMethod.Post,
                    ApiType.PUT => HttpMethod.Put,
                    ApiType.DELETE => HttpMethod.Delete,
                    _ => HttpMethod.Get,
                };

                var apiResponse = await client.SendAsync(message);
                var apiContent = await apiResponse.Content.ReadAsStringAsync();
                var responseDto = JsonConvert.DeserializeObject<T>(apiContent);

                return responseDto;
            }
            catch (Exception ex)
            {
                var dto = new ResponseDto()
                {
                    IsSuccess = false,
                    DisplayMessage = "Error",
                    ErrorMessages = new List<string> { ex.Message }
                };
                var apiContent = JsonConvert.SerializeObject(dto);
                var responseDto = JsonConvert.DeserializeObject<T>(apiContent);

                return responseDto;
            }
        }

        public void Dispose()
        {
            GC.SuppressFinalize(true);
        }

    }
}
