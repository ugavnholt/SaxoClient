using Saxo.DtOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Saxo.Classes
{
    /// <summary>
    /// Represents an api session to the backend - each api session will be subject to various rate limitations
    /// which is why it makes sense to establish multiple sessions with the backend
    /// </summary>
    public sealed class ApiSession
    {
        private readonly ApiToken _apiToken;

        public ApiSession(ApiToken apiToken)
        {
            _apiToken = apiToken;
        }
    }
}
