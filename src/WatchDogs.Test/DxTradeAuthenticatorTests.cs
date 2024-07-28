using Contracts;
using Infrastructure.DxTrade;
using Microsoft.Extensions.Options;
using Moq;
using RichardSzalay.MockHttp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Authentication;
using System.Text;
using System.Threading.Tasks;

namespace WatchDogs.Test;

public class DxTradeAuthenticatorTests
{
    private const string _testingToken = "JSESSIONID=sometoken";
    private const string _testingURL = "https://dxtrade.ftmo.com/api/auth/";

    private readonly Mock<IHttpClientFactory> _httpClientFactoryMock = new Mock<IHttpClientFactory>();
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

        _authenticator = new DxTradeAuthenticator(optionsMock.Object, _httpClientFactoryMock.Object);
    }

    [Fact]
    public async Task AuthenticateAsync_ShouldGenerateAndStoreSessionToken_InCaseOfSuccessfullResponse()
    {
        //Arrange
        _httpClientFactoryMock.Setup(mockClient => mockClient.CreateClient(DxTradeConstants.DxTradeAuthenticationClient))
            .Returns(new HttpClient(_handlerMock)
            {
                BaseAddress =  new Uri(_testingURL)
            });

        _handlerMock.When(HttpMethod.Post, _testingURL+"login").Respond(req =>
        {
            var response = new HttpResponseMessage(HttpStatusCode.OK);
            response.Headers.Add("SessionTokenHeaderName", _testingToken);
            return response;
        });
        //Act

        string token = await _authenticator.AuthenticateAsync();

        //Assert

        Assert.Equal(_testingToken, token);
    }

    [Fact]
    public async Task AuthenticateAsync_ShouldThrowAnAuthenticationException_InCaseOfUnsuccessfullResponse()
    {
        //Arrange
        _httpClientFactoryMock.Setup(mockClient => mockClient.CreateClient(DxTradeConstants.DxTradeAuthenticationClient))
            .Returns(new HttpClient(_handlerMock)
            {
                BaseAddress = new Uri(_testingURL)
            });

        _handlerMock.When(HttpMethod.Post, _testingURL+"login").Respond(HttpStatusCode.Forbidden);

        //Act and Assert

        await Assert.ThrowsAsync<AuthenticationException>(_authenticator.AuthenticateAsync);
    }
}


