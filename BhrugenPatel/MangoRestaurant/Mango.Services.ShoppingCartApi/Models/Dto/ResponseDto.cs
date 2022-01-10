using Newtonsoft.Json;

namespace Mango.Services.ShoppingCartApi.Models.Dto
{
    public class ResponseDto
    {
        public bool IsSuccess { get; set; } = true;

        public List<string> ErrorMessages { get; set; }

        public string DisplayMessage { get; set; }

        public object Result { get; set; }


        public T GetResultAs<T>()
        {
            var jsonData = Convert.ToString(Result);
            return JsonConvert.DeserializeObject<T>(jsonData);
        }

        public static ResponseDto Ok(object result, string displayMessage = null)
        {
            return new() { Result = result, DisplayMessage = displayMessage };
        }

        public static ResponseDto Error(Exception ex, string displayMessage = null)
        {
            return new()
            {
                IsSuccess = false,
                DisplayMessage = displayMessage,
                ErrorMessages = new List<string>() { ex.Message }
            };
        }

        public static ResponseDto Error(string displayMessage, params string[] errorMessages)
        {
            return new()
            {
                IsSuccess = false,
                DisplayMessage = displayMessage,
                ErrorMessages = new List<string>(errorMessages)
            };
        }
    }
}
