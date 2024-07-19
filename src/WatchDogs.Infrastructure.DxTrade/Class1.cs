using Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Infrastructure.DxTrade
{
    public class Auth : AuthenticateDX
    {
        private readonly DxTradeConnectionOptions _connectionOptions;
        public Auth(IOptions<DxTradeConnectionOptions> configuration)
        {
            _connectionOptions = configuration.Value;
        }

        public void Login()
        {
            throw new NotImplementedException();
        }
    }
}
