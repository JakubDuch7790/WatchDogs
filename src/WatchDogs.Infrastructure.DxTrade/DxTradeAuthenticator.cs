using Contracts;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Security.Authentication;
using System.Text;
using System.Text.Json;
using static System.Net.WebRequestMethods;

namespace Infrastructure.DxTrade;

public class DxTradeAuthenticator : IDxTradeAuthenticator
{
    private const string SessionTokenHeaderName = "JSESSIONID";

    private readonly ILogger<DxTradeAuthenticator> _logger;

    private readonly IHttpClientFactory _httpClientFactory;
    private readonly DxTradeConnectionOptions _connectionOptions;

    public DxTradeAuthenticator(IOptions<DxTradeConnectionOptions> configuration, IHttpClientFactory httpClientFactory, ILogger<DxTradeAuthenticator> logger)
    {
        _connectionOptions = configuration.Value;
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public async Task<string> AuthenticateAsync()
    {
        //Due to the inconsistencies in documentation, specifically base URL, request body and response body,
        //authentication done with the help of: https://github.com/zLeki/DXTrade-Python-Demo/blob/main/main.py

        try
        {
            using var client = _httpClientFactory.CreateClient(DxTradeConstants.DxTradeAuthenticationClient);

            string jsonData = JsonSerializer.Serialize(_connectionOptions);

            var stringContent = new StringContent(jsonData, Encoding.UTF8, "application/json");

            var response = await client.PostAsync("login", stringContent);

            response.EnsureSuccessStatusCode();

            _logger.LogInformation("Authentication successful.");

            return GetSessionToken(response.Headers);
            
        }
        catch (Exception ex) 
        {
            _logger.LogError(ex, "HTTP request error occurred during authentication.");

            throw new AuthenticationException("An error occurred during authentication", ex);
        }
    }
    private static string GetSessionToken(HttpResponseHeaders headers)
    {
        var sessionData = headers.SelectMany(h => h.Value)
            .FirstOrDefault(h => h.StartsWith($"{SessionTokenHeaderName}=", StringComparison.OrdinalIgnoreCase));

        if (!string.IsNullOrWhiteSpace(sessionData))
        {
            var sessionToken = sessionData.Split(';').FirstOrDefault();

            if (!string.IsNullOrEmpty(sessionToken))
            {
                return sessionToken;
            }
        }

        throw new AuthenticationException(message: $"There is no '{SessionTokenHeaderName}' header in DxTrade login response.");

    }
}
