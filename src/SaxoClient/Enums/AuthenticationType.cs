using System;
using System.Collections.Generic;
using System.Text;

namespace Saxo.Enums
{
    /// <summary>
    /// Describe the various authentication types supported by the api
    /// </summary>
    public enum AuthenticationType
    {
        /// <summary>
        /// OAuth 2 Code Flow - see https://developer.saxobank.com/openapi/learn/oauth-authorization-code-grant
        /// </summary>
        CodeFlow,

        /// <summary>
        /// Certificate based authentication (CBA), see: https://github.com/SaxoBank/openapi-samples-csharp/tree/master/authentication/Authentication_Cba
        /// * Not implemented yet *
        /// </summary>
        Certificate,

        /// <summary>
        /// Authorization Code Grant (RFC 6749) with Proof Key for Code Exchange (RFC 7636),
        /// see: https://developer.saxobank.com/openapi/learn/oauth-authorization-code-grant-pkce
        /// </summary>
        PKCE,


    }
}
