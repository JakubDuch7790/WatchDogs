using Contracts;
using Infrastructure.DxTrade;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WatchDogs.Test;
public class DxTradeAuthenticatorTests
{
    private const string SessionTokenHeaderName = "JSESSIONID";
    private readonly Mock<IHttpClientFactory> _httpClientFactoryMock;
    private readonly Mock<ISessionTokenStorage> _sessionTokenStorageMock;
    private readonly DxTradeConnectionOptions _connectionOptions;
    private readonly DxTradeAuthenticator _authenticator;

    [Fact]
    public void AuthenticateAsync_Should()
    {
        
    }
    
}


