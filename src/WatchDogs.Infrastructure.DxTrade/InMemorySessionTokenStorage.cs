using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.DxTrade;

public class InMemorySessionTokenStorage : ISessionTokenStorage
{
    private string SessionToken { get; set; }

    public Task<string> GetSessionTokenAsync()
    {
        return Task.FromResult(SessionToken);
    }

    public Task SetSessionTokenAsync(string sessionToken)
    {
        SessionToken = sessionToken;

        return Task.CompletedTask;
    }
}
