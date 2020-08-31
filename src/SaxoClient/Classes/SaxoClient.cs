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

namespace Saxo.Classes
{
    public sealed class SaxoClient : IDisposable, IExchangeProvider
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
        private static RandomStringBuilder _randomService = new RandomStringBuilder();
        private string _codeVerifier;
        private string _authCode;
        private SaxoClientState _state = SaxoClientState.Initialized; 
        private ApiToken _apiToken;

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

        private async Task PKCEAuthenticate(CancellationToken ct = default)
        {
            var request = BuildPKCEAuthenticationRequest();
            // First step is to get the auth token
            var response = await _httpClient.SendAsync(request, ct).ConfigureAwait(false);
            if(response.StatusCode == HttpStatusCode.Found)
            {
                // TODO: retrieve auth code
                
            }
            else if (response.StatusCode == HttpStatusCode.OK)
            {
                // TODO: retrieve auth code
                
            }
            else
            {
                throw new AccessViolationException(string.Format("Authentication error returned by the api, http status: {0}-{1}", (int)response.StatusCode, response.ReasonPhrase));
            }

            // Next step is to get the 
            response = await _httpClient.SendAsync(BuildPKCEGetTokenRequest(), ct).ConfigureAwait(false);
            if(response.IsSuccessStatusCode)
            {
                _apiToken = JsonSerializer.Deserialize<ApiToken>(await(response.Content.ReadAsByteArrayAsync()));
            }
            else
            {
                throw new AccessViolationException(string.Format("Error getting access token, http status: {0}-{1}", (int)response.StatusCode, response.ReasonPhrase));
            }

        }

        /// <summary>
        /// Listen on a random port for a auth callback that should contain the auth_token we need to progress
        /// </summary>
        /// <returns></returns>
        public Task GetAuthCode()
        { }

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
                    Uri.EscapeDataString(_options.RedirectUri),
                    codeChallengeMethod,
                    GetPKCECodeChallenge(_codeVerifier),
                    _randomService.GetRandomString(8));
        }

        private HttpRequestMessage BuildPKCEGetTokenRequest()
        {
            var request = new HttpRequestMessage();
            request.Method = HttpMethod.Post;

            request.RequestUri = new Uri(string.Format("{0}/token", _options.AuthenticationUrl));
            request.Content = new FormUrlEncodedContent(new Dictionary<string, string> 
            { 
                { "grant_type", "authorization_code" },
                { "client_id", _options.AppKey },
                { "code", _authCode },
                { "redirect_uri", Uri.EscapeDataString(_options.RedirectUri) },
                { "code_verifier", _codeVerifier },

            });
            request.Content.Headers.ContentType = new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded");

            return request;
        }

        /// <summary>
        /// BASE64URL-ENCODE(SHA256(ASCII(code_verifier)))
        /// </summary>
        /// <param name="codeVerifier"></param>
        /// <returns></returns>
        private static string GetPKCECodeChallenge(string codeVerifier)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = Encoding.ASCII.GetBytes(codeVerifier);
                var challengeBytes = sha256.ComputeHash(bytes);
                return Base64UrlEncoder.Encode(challengeBytes);
            }
        }

        public void Dispose()
        {
            
            _httpClient?.Dispose();
        }
    }
}
