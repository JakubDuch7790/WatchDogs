using Contracts;
using Infrastructure.DxTrade;
using Microsoft.Extensions.Options;
using Moq;
using RichardSzalay.MockHttp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace WatchDogs.Test;
public class DxTradeAuthenticatorTests
{
    private readonly Mock<IHttpClientFactory> _httpClientFactoryMock = new Mock<IHttpClientFactory>();
    private readonly Mock<ISessionTokenStorage> _sessionTokenStorageMock = new Mock<ISessionTokenStorage>();
    private readonly MockHttpMessageHandler _handlerMock = new MockHttpMessageHandler();
    private readonly DxTradeAuthenticator _authenticator;

    public DxTradeAuthenticatorTests()
    {
        var optionsMock = new Mock<IOptions<DxTradeConnectionOptions>>();

        var connectionOptions = new DxTradeConnectionOptions
        {
            Vendor = "ftmo",
            Username = "username",
            Password = "password"
        };

        optionsMock.Setup(o => o.Value).Returns(connectionOptions);

        _authenticator = new DxTradeAuthenticator(optionsMock.Object, _httpClientFactoryMock.Object, _sessionTokenStorageMock.Object);
    }

    [Fact]
    public async Task AuthenticateAsync_ShouldGenerateAndStoreSessionToken_InCaseOfSuccessfullResponse()
    {
        //Arrange
        _httpClientFactoryMock.Setup(mockClient => mockClient.CreateClient(DxTradeConstants.DxTradeAuthenticationClient))
            .Returns(new HttpClient(_handlerMock)
            {
                BaseAddress =  new Uri("https://dxtrade.ftmo.com/api/auth/")
            });

        _handlerMock.When(HttpMethod.Post, "https://dxtrade.ftmo.com/api/auth/login").Respond(req =>
        {
            var response = new HttpResponseMessage(HttpStatusCode.OK);
            response.Headers.Add("SessionTokenHeaderName", "JSESSIONID=sometoken; Path=/; Secure; HttpOnly; SameSite=Lax");
            return response;
        });
        //Act

        await _authenticator.AuthenticateAsync();

        //Assert

        _sessionTokenStorageMock.Verify(stsm => stsm.SetSessionTokenAsync("sometoken"));
    }
    
}


