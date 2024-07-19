using Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System.Net.Http;

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

        public Task AuthenticateAsync()
        {
            var client = _httpClientFactory.CreateClient();

            throw new NotImplementedException();
        }
    }
}
