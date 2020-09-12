using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Saxo;
using Saxo.Classes;
using Saxo.Interfaces;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Threading;

namespace ConsoleClient
{
    class Program
    {
        private static ILogger<Program> _logger;
        private static IConfiguration _configuration;
        private static SaxoClientOptions _saxoConfig;
        private static SaxoClient _saxoClient = null;
        private static HttpClientHandler _handler = null;
        private static HttpClient _httpClient = null;
        private readonly static CancellationTokenSource _cts = new CancellationTokenSource();
        

        static void Main(string[] args)
        {
            _ = args;
            try
            {
                Init();
                _logger.LogInformation("initialization complete");

                foreach (var tz in TimeZoneInfo.GetSystemTimeZones())
                {
                    _logger.LogInformation("'{id}' '{name}' '{caption}' ({baseOffset})",
                        tz.Id,
                        tz.StandardName,
                        tz.DisplayName,
                        tz.BaseUtcOffset);
                }

                var getTokenTask = _saxoClient.GetPKCEApiToken(_cts.Token);
                
                _logger.LogInformation("Redirect listener listening on {redirectUrl}", _saxoClient.RedirectUrl);

                var authUri = _saxoClient.GetPKCEAuthUrl();
                _logger.LogInformation("Waiting for external callback from Uri {uri}", authUri);

                getTokenTask.Wait();
                _logger.LogInformation("Authenticated, access token expires in: {expires}, refresh token: {refresh} expires in: {refreshExpires}", 
                    _saxoClient.ApiToken.ExpiresIn,
                    _saxoClient.ApiToken.RefreshToken,
                    _saxoClient.ApiToken.RefreshTokenExpiresIn);

                // Next things to do: get instrument info: https://gateway.saxobank.com/sim/openapi/ref/v1/instruments?AssetTypes=Stock

                // Get prices for a list of instruments: https://gateway.saxobank.com/sim/openapi/trade/v1/infoprices/list?AccountKey=iX5VtUtBMBQDzOl1Z5px1Q==&Uics=2047,1311,2046,17749,16&AssetType=FxSpot&Amount=100000&FieldGroups=DisplayAndFormat,Quote



                Console.ReadLine();
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Uncaught exception");
                throw;
            }
            finally
            {
                Cleanup();
            }
        }

        static void Init()
        {
            // the type specified here is just so the secrets library can 
            // find the UserSecretId we added in the csproj file, and the secrets defined for it
            var builder = new ConfigurationBuilder()
                .AddUserSecrets<Program>();

            _configuration = builder.Build();

            var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole(options => {  options.DisableColors = false; options.IncludeScopes = false; }).SetMinimumLevel(LogLevel.Debug));
            _logger = loggerFactory.CreateLogger<Program>();

            _saxoConfig = new SaxoClientOptions
            {
                // Set the appkey to whatever value is stores in the secrets manager
                AppKey = _configuration["SaxoClient:AppKey"],
                AppUrl = _configuration["SaxoClient:AppName"],
            };

            _handler = new HttpClientHandler { UseCookies = false, AllowAutoRedirect = false };
            _httpClient = new HttpClient(_handler);
            _saxoClient = new SaxoClient(
                    loggerFactory.CreateLogger<SaxoClient>(),
                    Options.Create(_saxoConfig),
                    _httpClient);
        }

        static void Cleanup()
        {
            if (_saxoClient != null)
            {
                _saxoClient.Dispose();
                _saxoClient = null;
            }
            if (_httpClient != null)
            {
                _httpClient.Dispose();
                _httpClient = null;
            }
            if (_handler != null)
            {
                _handler.Dispose();
                _handler = null;
            }

            _cts.Dispose();
        }
    }
}
