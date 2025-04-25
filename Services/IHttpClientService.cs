using System.Threading.Tasks;
using plato.Models;

namespace plato.Services
{
    public interface IHttpClientService
    {
        Task<HttpResponseModel> SendRequestAsync(HttpRequestModel request);
        Task<string> FormatResponseAsync(HttpResponseModel response);
        string BuildUrlWithQueryParameters(string baseUrl, System.Collections.Generic.Dictionary<string, string> parameters);
    }
} 