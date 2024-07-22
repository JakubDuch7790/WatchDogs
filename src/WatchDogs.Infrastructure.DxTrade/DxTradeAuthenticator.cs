using Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System.Net.Http;
using static System.Net.Mime.MediaTypeNames;
using System.Text.Json;
using System.Text;

namespace Infrastructure.DxTrade
{
    public class DxTradeAuthenticator : IDxTradeAuthenticator
    {
        private readonly IHttpClientFactory _httpClientFactory;

        private readonly DxTradeConnectionOptions _connectionOptions;

        public DxTradeAuthenticator(IOptions<DxTradeConnectionOptions> configuration, IHttpClientFactory httpClientFactory)
        {
            _connectionOptions = configuration.Value;
            _httpClientFactory = httpClientFactory;
        }

        public async Task AuthenticateAsync()
        {
            var client = _httpClientFactory.CreateClient("DxTradeAuthenticatorClient");

            client.DefaultRequestHeaders.Add("Accept", "application/json");

            var jsonData = JsonSerializer.Serialize(_connectionOptions);

            var requestContent = new StringContent(jsonData, Encoding.UTF8, "application/json");

            var response = await client.PostAsync(client.BaseAddress + "login", requestContent);

            response.EnsureSuccessStatusCode();
        }
    }
}
