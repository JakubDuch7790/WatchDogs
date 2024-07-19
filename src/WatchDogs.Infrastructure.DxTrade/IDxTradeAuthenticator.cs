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
        // Possible exeptions : HttpException, NotAuthorizedException, UnauthorizedAccessException,
        // AuthenticationException, InvalidCredentialException.

        /// <exception cref="UnauthorizedAccessException">
        /// Failed authentication.
        /// </exception>
        Task AuthenticateAsync();
    }
}
