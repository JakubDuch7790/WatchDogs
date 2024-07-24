using Contracts;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Security.Authentication;
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
            //Due to the inconsistencies in documentation, specifically base URL, request body and response body,
            //authentication done with the help of: https://github.com/zLeki/DXTrade-Python-Demo/blob/main/main.py

            try
            {
                using var client = _httpClientFactory.CreateClient("DxTradeAuthenticationClient");

                string jsonData = JsonSerializer.Serialize(_connectionOptions);

                var stringContent = new StringContent(jsonData, Encoding.UTF8, "application/json");

                var response = await client.PostAsync("login", stringContent);

                if (response.IsSuccessStatusCode)
                {
                    //string sessionToken = GetSessionToken(response.Headers);

                    //Which option is better?

                    //var sessionToken = new SessionTokenManager { SessionToken = GetSessionToken(response.Headers) };

                    var sessionToken1 = new SessionTokenManager();

                    sessionToken1.AddSessionToken(GetSessionToken(response.Headers));
                }

                else
                {
                    throw new AuthenticationException(response.StatusCode.ToString());
                }

            }
            catch (AuthenticationException ex) 
            {
                // I have to register that logger.
            }
        }

        private static string GetSessionToken(HttpResponseHeaders headers)
        {
            var sessionData = headers.SelectMany(h => h.Value)
                .FirstOrDefault(h => h.StartsWith($"{SessionTokenHeaderName}="));

            if (!string.IsNullOrWhiteSpace(sessionData))
            {
                var sessionToken = sessionData.Split(';').FirstOrDefault();

                if (!string.IsNullOrEmpty(sessionToken))
                {
                    return sessionToken;
                }
            }

            throw new AuthenticationException(message: "Sorry for inconvenience but there's been a problem with something." +
                " Please try again later or contact our tech support team.");

        }
    }
}
