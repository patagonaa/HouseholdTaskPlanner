using Microsoft.Extensions.Options;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace HouseholdTaskPlanner.TelegramBot
{
    public class TaskplannerApiHttpClientFactory
    {
        private readonly ApiConfiguration _apiConfiguration;

        public TaskplannerApiHttpClientFactory(IOptions<ApiConfiguration> options)
        {
            _apiConfiguration = options.Value;
        }

        public HttpClient Get()
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
