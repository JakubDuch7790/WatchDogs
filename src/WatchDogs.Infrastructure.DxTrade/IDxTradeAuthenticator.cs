using Contracts;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.DxTrade
{
    public interface IDxTradeAuthenticator
    {
        /// <exception cref="AuthenticationException">
        /// Failed authentication.
        /// </exception>
        Task AuthenticateAsync();
    }
}
