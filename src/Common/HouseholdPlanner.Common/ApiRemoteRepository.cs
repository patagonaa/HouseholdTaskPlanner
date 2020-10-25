using Refit;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace HouseholdPlanner.Common
{
    public abstract class ApiRemoteRepository<TApi>
    {
        public TApi Api { get; }

        public ApiRemoteRepository(IApiConfiguration apiConfiguration)
        {
            Api = RestService.For<TApi>(GetHttpClient(apiConfiguration));
        }

        private HttpClient GetHttpClient(IApiConfiguration apiConfiguration)
        {
            var httpClient = new HttpClient
            {
                BaseAddress = new Uri(apiConfiguration.BackendLocation),
            };

            if (!string.IsNullOrWhiteSpace(apiConfiguration.BasicAuth))
            {
                var authToken = Encoding.UTF8.GetBytes(apiConfiguration.BasicAuth);
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(authToken));
            }

            return httpClient;
        }
    }
}
