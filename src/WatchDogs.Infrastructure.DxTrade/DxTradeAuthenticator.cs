using Contracts;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace Infrastructure.DxTrade
{
    public class DxTradeAuthenticator : IDxTradeAuthenticator
    {
        private const string CookieGroupName = "SET-COOKIE";
        private const string SessionTokenHeaderName = "JSESSIONID";

        private readonly IHttpClientFactory _httpClientFactory;

        private readonly DxTradeConnectionOptions _connectionOptions;

        public DxTradeAuthenticator(IOptions<DxTradeConnectionOptions> configuration, IHttpClientFactory httpClientFactory)
        {
            _connectionOptions = configuration.Value;
            _httpClientFactory = httpClientFactory;
        }

        public async Task AuthenticateAsync()
        {
            //TODO: rename the client to something like DxTradeAuthenticationClient, rename it in Program.cs as well
            using var client = _httpClientFactory.CreateClient("myClient");

            //Try replacing this with JsonSerializer.Serialize(_connectionOptions) to see whether the result is the same
            //If so, replace this with it
            string jsonString = JsonSerializer.Serialize(new
            {
                username = _connectionOptions.Username,
                password = _connectionOptions.Password,
                vendor = _connectionOptions.Vendor
            });

            var stringContent = new StringContent(jsonString, Encoding.UTF8, "application/json");

            var response = await client.PostAsync("login", stringContent);

            string sessionToken = GetSessionToken(response.Headers);
        }

        private static string GetSessionToken(HttpResponseHeaders headers)
        {
            // This merges all the headers together. Play around with it and try to get only the token header
            var sessionToken = headers.SelectMany(h => h.Value)
                .FirstOrDefault(h => h.StartsWith($"{SessionTokenHeaderName}="));

            if (string.IsNullOrWhiteSpace(sessionToken))
            {
                throw new InvalidOperationException("Coskaj nedobre");
            }

            return sessionToken;
        }
    }
}
