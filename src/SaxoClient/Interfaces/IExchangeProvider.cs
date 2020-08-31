using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Saxo.Interfaces
{
    /// <summary>
    /// Attempts to authenticate with the provider
    /// </summary>
    /// <returns></returns>
    public interface IExchangeProvider
    {
        Task AuthenticateAsync(CancellationToken ct = default);
    }
}
