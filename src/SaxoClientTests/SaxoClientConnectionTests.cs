using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Saxo;
using Saxo.Classes;
using Saxo.Interfaces;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Saxo.Tests
{
    public sealed class SaxoClientConnectionTests
    {
        private readonly ITestOutputHelper _outputHelper;
        private readonly ILogger<SaxoClientConnectionTests> _logger;
        private readonly IConfiguration _configuration;
        private readonly ILoggerFactory _loggerFactory;

        public SaxoClientConnectionTests(ITestOutputHelper outputHelper)
        {
            _outputHelper = outputHelper;

            // the type specified here is just so the secrets library can 
            // find the UserSecretId we added in the csproj file, and the secrets defined for it
            var builder = new ConfigurationBuilder()
                .AddUserSecrets<SaxoClientConnectionTests>();
            _configuration = builder.Build();

            _loggerFactory = new LoggerFactory();
            _loggerFactory.AddProvider(new XunitLoggerProvider(outputHelper));
            _logger = _loggerFactory.CreateLogger<SaxoClientConnectionTests>();
            _logger.LogInformation("Test Initialized");
        }

        [Fact]
        public void CreateConnectDispose()
        {
            using (var handler = new HttpClientHandler { UseCookies = false, AllowAutoRedirect = false })
            using (var httpClient = new HttpClient(handler)) // httpClient lifetime is handled by caller - client should be kept for duration of SaxoClient lifetime
            {

                var config = new SaxoClientOptions
                {
                    // Set the appkey to whatever value is stores in the secrets manager
                    AppKey = _configuration["SaxoClient:AppKey"],
                };

                Assert.NotEmpty(config.AppKey);

                using (var saxoClient = new SaxoClient(
                    _loggerFactory.CreateLogger<SaxoClient>(),
                    Options.Create(config),
                    httpClient))
                {

                }
            }
        }

        [Fact]
        public void BuildPKCEAuthUrl()
        {
            using (var handler = new HttpClientHandler { UseCookies = false, AllowAutoRedirect = false })
            using (var httpClient = new HttpClient(handler)) // httpClient lifetime is handled by caller - client should be kept for duration of SaxoClient lifetime
            {

                var config = new SaxoClientOptions
                {
                    // Set the appkey to whatever value is stores in the secrets manager
                    AppKey = _configuration["SaxoClient:AppKey"],
                };

                Assert.NotEmpty(config.AppKey);

                using (var saxoClient = new SaxoClient(
                    _loggerFactory.CreateLogger<SaxoClient>(),
                    Options.Create(config),
                    httpClient))
                {
                    var authUri = saxoClient.GetPKCEAuthUrl();
                    _logger.LogInformation("Got the authentication Uri: {uri}", authUri);

                    Assert.StartsWith(config.AuthenticationUrl, authUri);
                    Assert.Contains(config.AppKey, authUri);
                    Assert.Contains(saxoClient.CodeVerifier, authUri);
                    Assert.Contains("S256", authUri);
                    Assert.Contains(Uri.EscapeDataString(saxoClient.RedirectUrl), authUri);
                }
            }
        }
    }
}
