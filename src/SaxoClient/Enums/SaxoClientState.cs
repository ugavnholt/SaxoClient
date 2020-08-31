using System;
using System.Collections.Generic;
using System.Text;

namespace Saxo.Enums
{
    /// <summary>
    /// The various states the SaxoClient api can be in
    /// </summary>
    public enum SaxoClientState
    {
        /// <summary>
        /// Client initialized
        /// </summary>
        Initialized,

        /// <summary>
        /// Authentication requested
        /// </summary>
        Authenticating,

        /// <summary>
        /// Authenticated
        /// </summary>
        Authenticated,
    }
}
