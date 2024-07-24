using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.DxTrade;

public class SessionTokenManager
{
    private string SessionToken { get; set; }

    public void AddSessionToken(string sessionToken)
    {
        SessionToken = sessionToken;
    }

    public string GetSessionToken()
    {
        return SessionToken;
    }
}
