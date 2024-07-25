using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.DxTrade;

public class InMemorySessionTokenStorage : ISessionTokenStorage
{
    private string sessionToken;

    public Task<string> GetSessionTokenAsync()
    {
        return Task.FromResult(sessionToken);
    }

    public Task SetSessionTokenAsync(string st)
    {
        sessionToken = st;

        return Task.CompletedTask;
    }
}
