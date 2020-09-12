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
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;
using Saxo.Enums;
using Microsoft.AspNetCore.Http;
using System.Net.Http.Headers;
using System.Net;
using Saxo.DtOs;
using System.Net.Sockets;
using System.IO;

namespace Saxo.Classes
{
    public sealed class SaxoClient : IDisposable, IBroker
    {
        #region Statics
        public static readonly JsonSerializerOptions DefaultSerializerOptions;
        static SaxoClient()
        {
            DefaultSerializerOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
            };
            DefaultSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        }

        #endregion Statics

        private static ILogger<SaxoClient> _logger = null;
        private readonly SaxoClientOptions _options;
        private readonly HttpClient _httpClient = null;
        private readonly static RandomStringBuilder _randomService = new RandomStringBuilder();
        private string _codeVerifier;
        private SaxoClientState _state = SaxoClientState.Initialized;
        private ApiToken _apiToken;
        private HttpListener _httpListener = null;
        private string _redirectUrl = null;

        /// <summary>
        /// State of the client
        /// </summary>
        public SaxoClientState State { get => _state; }

        /// <summary>
        /// The ApiToken of the client
        /// </summary>
        public ApiToken ApiToken { get => _apiToken; }

        /// <summary>
        /// Computed redirect url
        /// </summary>
        public string RedirectUrl { get => _redirectUrl; }

        /// <summary>
        /// The verification code
        /// </summary>
        public string CodeVerifier { get => _codeVerifier; }

        /// ///////////////////////////////////////////////////////////////////////////////
        /// 
        /// CONSTRUCTORS
        /// 
        /// ///////////////////////////////////////////////////////////////////////////////
        #region CONSTRUCTORS

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
        {
            _logger = logger;
            _options = options.Value;
            _httpClient = httpClient;

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            // client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "Your Oauth token");
        }

        #endregion CONSTRUCTORS

        /// <inheritdoc/>
        public async Task AuthenticateAsync(CancellationToken ct = default)
        {
            _state = SaxoClientState.Authenticating;
            try
            {
                switch (_options.AuthenticationType)
                {
                    case Enums.AuthenticationType.PKCE:
                        await PKCEAuthenticate(ct).ConfigureAwait(false);
                        break;
                    default:
                        throw new NotImplementedException();
                }
                _state = SaxoClientState.Authenticated;
            }
            catch(Exception)
            {
                _state = SaxoClientState.Initialized;
                throw;
            }
        }

        // TODO:
        // Spawn browser to direct user to Saxo Bank login with the url that is generated below:
        // browser will access callback url when login is successfull with authtoken
        // saxo bank can be contacted to swap auth token for an api_token to validate against the api

        private Task PKCEAuthenticate(CancellationToken ct = default)
        {
            // Get the listen port, and build the redirect url that will be called when
            // we have a token

            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets a random port for the loopback interface
        /// </summary>
        /// <returns></returns>
        private static int GetRandomUnusedPort()
        {
            var listener = new TcpListener(IPAddress.Loopback, 0);
            listener.Start();
            var port = ((IPEndPoint)listener.LocalEndpoint).Port;
            listener.Stop();
            return port;
        }

        private void StartListener()
        {
            try
            {
                var port = GetRandomUnusedPort();

                var uri = new Uri(_options.AppUrl);

                _httpListener = new HttpListener();
                _httpListener.Prefixes.Add($"{uri.Scheme}://{uri.Host}:{port}/");
                _httpListener.Start();
                _redirectUrl += uri.AbsoluteUri.Replace(uri.Host, uri.Host + ":" + port);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to start the redirect listener on url: {_redirectUrl}", ex);
            }
        }

        private void StopListener()
        {
            if(_httpListener != null)
            {
                _httpListener.Stop();
                _httpListener = null;
            }
        }

        /// <summary>
        /// Listens for incoming requests, and checks for a auth code being returned
        /// </summary>
        /// <param name="ct"></param>
        /// <returns></returns>
        private async Task<string> ListenForAuthCode(CancellationToken ct = default)
        {
            string authCode = null;
            while (!ct.IsCancellationRequested && _httpListener.IsListening)
            {
                HttpListenerContext httpContext = null;
                try
                {
                    httpContext = await _httpListener.GetContextAsync();
                    authCode = httpContext.Request.QueryString["code"];

                    if (!string.IsNullOrWhiteSpace(authCode))
                    {
                        using var writer = new StreamWriter(httpContext.Response.OutputStream);
                        await writer.WriteLineAsync("AuthCode received by App. Please close the browser.").ConfigureAwait(false);
                        writer.Close();
                        break;
                    }
                    else
                        throw new ArgumentException();
                }
                catch (Exception ex)
                {
                    using (var writer = new StreamWriter(httpContext.Response.OutputStream))
                    {
                        await writer.WriteLineAsync("Invalid AuthCode callback").ConfigureAwait(false);
                        writer.Close();
                    }
                    throw new Exception("Invalid AuthCode callback", ex);
                }
                finally
                {
                    if (httpContext != null)
                        httpContext.Response.Close();
                }
            }
            ct.ThrowIfCancellationRequested();
            return authCode;
        }

        /// <summary>
        /// Listen on a random port for a auth callback that should contain the auth_token we need to progress
        /// Function returns only when 
        /// </summary>
        /// <returns></returns>
        public async Task GetPKCEApiToken(CancellationToken ct = default)
        {
            string authCode = null;
            try
            {
                StartListener();
                authCode = await ListenForAuthCode(ct).ConfigureAwait(false);
            }
            catch(Exception ex)
            {
                throw new Exception("Error listening for AuthToken", ex);
            }
            finally
            {
                StopListener();
            }

            try
            {
                _apiToken = await GetApiToken(authCode, ct).ConfigureAwait(false);
                _state = SaxoClientState.Authenticated;
                _ = TokenRefresher(ct);
            }
            catch(Exception ex)
            {
                throw new Exception("Error getting Session and accesstoken", ex);
            }

            // We are authenticated here
        }

        private async Task TokenRefresher(CancellationToken ct = default)
        {
            _logger.LogDebug("Will refresh api token in {expire} seconds", _apiToken.ExpiresIn - 5);
            await Task.Delay((_apiToken.ExpiresIn * 1000) - 5000, ct);

            _logger.LogDebug("Refreshing api token...");

            var request = BuildPKCEGetTokenRefreshRequest();
            var response = await _httpClient.SendAsync(request, ct).ConfigureAwait(false);
            if (!response.IsSuccessStatusCode)
                throw new AccessViolationException(string.Format("Error Refreshing access token, http status: {0}-{1}", (int)response.StatusCode, response.ReasonPhrase));

            var newToken = JsonSerializer.Deserialize<ApiToken>(await (response.Content.ReadAsByteArrayAsync()).ConfigureAwait(false));
            _apiToken = newToken;
            _ = TokenRefresher(ct);
        }

        private async Task<ApiToken> GetApiToken(string authCode, CancellationToken ct = default)
        {
            var request = BuildPKCEGetTokenRequest(authCode);

            var response = await _httpClient.SendAsync(request, ct).ConfigureAwait(false);
            if (!response.IsSuccessStatusCode)
                throw new AccessViolationException(string.Format("Authentication error returned by the api, http status: {0}-{1}", (int)response.StatusCode, response.ReasonPhrase));

            return JsonSerializer.Deserialize<ApiToken>(await (response.Content.ReadAsByteArrayAsync()).ConfigureAwait(false));
        }

        private HttpRequestMessage BuildPKCEGetTokenRefreshRequest()
        {
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(string.Format("{0}/token", _options.AuthenticationUrl))
            };
            var fields = new Dictionary<string, string>
            {
                { "grant_type", "refresh_token" },
                { "refresh_token", _apiToken.RefreshToken },
                { "code_verifier", _codeVerifier },
            };
            request.Content = new FormUrlEncodedContent(fields);
            return request;
        }

        private HttpRequestMessage BuildPKCEGetTokenRequest(string authCode)
        {
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(string.Format("{0}/token", _options.AuthenticationUrl))
            };
            var fields = new Dictionary<string, string>
            {
                { "grant_type", "authorization_code" },
                { "code", authCode },
                { "client_id", _options.AppKey },
                { "code_verifier", _codeVerifier },
                { "redirect_uri", _redirectUrl },
            };
            request.Content = new FormUrlEncodedContent(fields);
            return request;
        }

        /// <summary>
        /// Gets the URL the client must use to validate the session to Saxo bank
        /// </summary>
        /// <returns></returns>
        public string GetPKCEAuthUrl()
        {
            _codeVerifier = _randomService.GetRandomString(43);
            var codeChallengeMethod = "S256";
            return string.Format(
                "{0}/authorize?response_type=code&client_id={1}&code_verifier={2}&redirect_uri={3}&code_challenge_method={4}&code_challenge={5}&state={6}",
                    _options.AuthenticationUrl,
                    _options.AppKey,
                    _codeVerifier,
                    Uri.EscapeDataString(_redirectUrl),
                    codeChallengeMethod,
                    GetPKCECodeChallenge(_codeVerifier),
                    _randomService.GetRandomString(8));
        }

        /// <summary>
        /// BASE64URL-ENCODE(SHA256(ASCII(code_verifier)))
        /// </summary>
        /// <param name="codeVerifier"></param>
        /// <returns></returns>
        private static string GetPKCECodeChallenge(string codeVerifier)
        {
            using var sha256 = SHA256.Create();
            var bytes = Encoding.ASCII.GetBytes(codeVerifier);
            var challengeBytes = sha256.ComputeHash(bytes);
            return Base64UrlEncoder.Encode(challengeBytes);
        }

        public void Dispose()
        {
            
            _httpClient?.Dispose();
        }
    }
}
