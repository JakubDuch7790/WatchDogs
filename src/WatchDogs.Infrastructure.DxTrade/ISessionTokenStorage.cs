using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.DxTrade
{
    public interface ISessionTokenStorage
    {
        Task<string> GetSessionTokenAsync();
        Task SetSessionTokenAsync(string sessionToken);
    }
}
