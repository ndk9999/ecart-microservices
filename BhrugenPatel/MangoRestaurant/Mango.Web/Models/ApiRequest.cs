namespace Mango.Web.Models
{
    public class ApiRequest
    {
        public ApiType Type { get; set; } = ApiType.GET;

        public string Url { get; set; }

        public object Payload { get; set; }

        public string AccessToken { get; set; }
    }
}
