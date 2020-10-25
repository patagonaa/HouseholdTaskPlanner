using Refit;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace HouseholdPlanner.Common
{
    public abstract class ApiRemoteRepository<TApi>
    {
        private readonly IApiConfiguration _apiConfiguration;

        public TApi Api { get; }

        public ApiRemoteRepository(IApiConfiguration apiConfiguration)
        {
            Api = RestService.For<TApi>(GetHttpClient());
            _apiConfiguration = apiConfiguration;
        }

        private HttpClient GetHttpClient()
        {
            var httpClient = new HttpClient
            {
                BaseAddress = new Uri(_apiConfiguration.BackendLocation),
            };

            if (!string.IsNullOrWhiteSpace(_apiConfiguration.BasicAuth))
            {
                var authToken = Encoding.UTF8.GetBytes(_apiConfiguration.BasicAuth);
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(authToken));
            }

            return httpClient;
        }
    }
}
