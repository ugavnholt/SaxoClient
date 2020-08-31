using Saxo.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Saxo.Classes
{
    /// <summary>
    /// Options that can be used to configure the Saxo Client
    /// 
    /// To bind via the options pattern:
    /// var saxoClientOptions = new SaxoClientOptions();
    /// Configuration.GetSection(SaxoClientOptions.SaxoClient).Bind(saxoClientOptions);
    /// 
    /// To maske available through DI:
    /// services.Configure<SaxoClientOptions>(Configuration.GetSection(
    ///                                       SaxoClientOptions.SaxoClient));
    /// </summary>
    public sealed class SaxoClientOptions
    {
        /// <summary>
        /// What the configuration section is expected to be called
        /// </summary>
        public const string SaxoClient = "SaxoClient";

        /// <summary>
        /// A URL uniquely representing your app.
        /// oAuth: redirect_uri
        /// </summary>
        public string AppUrl { get; set; } = "http://localhost/mytestapp";

        /// <summary>
        /// The URL of the Saxo Bank authentication & authorization server.
        /// oauth: authorization_url = AuthenticationUrl + "/authorize"
        /// oauth: token_url = AuthenticationUrl + "/token"
        /// </summary>
        public string AuthenticationUrl { get; set; } = "https://sim.logonvalidation.net";

        /// <summary>
        /// The Application key identifying your application.
        /// oAuth: client_id
        /// </summary>
        public string AppKey { get; set; } = "1234-5678-9101";

        /// <summary>
        /// The Application "secret" identifying your application.
        /// oAuth: client_secret
        /// </summary>
        public string AppSecret { get; set; } = "abcdefghijklmn";

        /// <summary>
        /// Base URL for calling OpenAPI REST endpoints.
        /// oAuth: client_secret
        /// </summary>
        public string OpenApiBaseUrl { get; set; } = "https://gateway.saxobank.com/sim/openapi/";

        /// <summary>
        /// Type of authentication used
        /// </summary>
        public AuthenticationType AuthenticationType { get; set; } = AuthenticationType.PKCE;

        /// <summary>
        /// The redirect uri, that is used for oAuth
        /// </summary>
        public string RedirectUri { get; set; } = "http://localhost/saxoclient";
    }
}
