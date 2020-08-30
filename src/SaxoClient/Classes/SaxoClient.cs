using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Saxo.Interfaces;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Saxo.Classes
{
    public sealed class SaxoClient : IDisposable, IStockTradeProvider
    {
        private static ILogger<SaxoClient> _logger = null;
        private readonly SaxoClientOptions _options;
        private readonly HttpClient _httpClient = null;

        /// ///////////////////////////////////////////////////////////////////////////////
        /// 
        /// CONSTRUCTORS
        /// 
        /// ///////////////////////////////////////////////////////////////////////////////
        #region CONSTRUCTORS

        public SaxoClient(
            ILogger<SaxoClient> logger, 
            SaxoClientOptions options,
            HttpClient httpClient)
        {
            _options = options;
            _logger ??= logger;
            _httpClient = httpClient;
        }

        /// <summary>
        /// Constructor for Dependency Injection
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="options">
        /// options should be supplied by DI by calling
        /// services.Configure<SaxoClientOptions>(Configuration.GetSection(
        ///                                       SaxoClientOptions.SaxoClient));
        ///                                       
        /// or in tests and stuff:
        /// Options.Create(new SaxoClientOptions());
        /// </param>
        /// <param name="httpClient">
        /// Preferably the httpClient should be suppled via the HttpClientFactory, by running
        /// services.AddHttpClient<SaxoClient>(); in ConfigureServices()
        /// otherwise a client can be supplied directly, and the lifetime can be managed by the user
        /// </param>
        public SaxoClient(
            ILogger<SaxoClient> logger, 
            IOptions<SaxoClientOptions> options,
            HttpClient httpClient)
            : this(logger, options.Value, httpClient)
        {
            Options.Create(new SaxoClientOptions());
        }

        #endregion CONSTRUCTORS

        public async ValueTask<bool> Connect(CancellationToken ct = default)
        {


            return false;
        }

        public void Dispose()
        {
            _httpClient?.Dispose();
            _httpClient = null;
        }
    }
}
