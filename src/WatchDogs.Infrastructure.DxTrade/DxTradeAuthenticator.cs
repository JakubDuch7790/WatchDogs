using Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Infrastructure.DxTrade
{
    public class DxTradeAuthenticator : IDxTradeAuthenticator
    {
        private readonly DxTradeConnectionOptions _connectionOptions;
        public DxTradeAuthenticator(IOptions<DxTradeConnectionOptions> configuration)
        {
            _connectionOptions = configuration.Value;
        }

        public Task AuthenticateAsync()
        {
            throw new NotImplementedException();
        }
    }
}
