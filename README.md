# Work in progress

# SaxoClient
Implementation of the Saxo Bank OpenApi in dotnet core

This is a work in progress, currently only the PKCE auth flow is working.

To test the current version, use the project "ConsoleClient".

To prepare the project login to [Saxos developer portal](https://developer.saxobank.com)

Go to apps, and create an app for the project:
* Name - any name
* Description - any description
* Redirect URLs - something that starts with http://localhost/xxx, for instance http://localhost/saxoconsoleclient
* Grant type - PKCE

When the application have been registered, save the App Key, and redirect url.

In visual studio right-click the `ConsoleClient` project, and select `Manage User Secrets`, and enter the following:

```json
{
  "SaxoClient:AppKey": "<app key>",
  "SaxoClient:AppName": "http://localhost/<redirect url>"
}
```

Run the project - it will pause with an output like:

```
Waiting for external callback from Uri https://sim.logonvalidation.net/authorize?response_type=code&client_id=d3a57833706042fa9d1e767ace72bc52&code_verifier=xlZW9FRI0tyJKX7mzHEmacDSFNrhLYe3ghhfAs3bCCb&redirect_uri=http%3A%2F%2Flocalhost%3A53961%2Fsaxoconsoleclient&code_challenge_method=S256&code_challenge=wOfLRS8T4IPqg0RwkO8iMSs7fWCnikw3YMTfL5wfMqw&state=U9B.S8iU
```

Copy the link into a browser and access the url - use your developer portal demo credentials to authenticate.

When authentication is successfull you should see a message like: 

```
Authenticated, access token expires in: 1199, refresh token: 7497f6cd-6d06-43c5-a8ee-87930fea1e0b expires in: 3599
```


# Using SaxoClient with Dependency Injection and ASP.NET Standard 2.1

The SaxoClient offers first-class support for Dependency Injection, and implements the [HttpClientFactory
pattern](https://docs.microsoft.com/en-us/dotnet/architecture/microservices/implement-resilient-applications/use-httpclientfactory-to-implement-resilient-http-requests),
that offer better resource management, and solves a 
[number of problems with the better known HttpClient](https://aspnetmonsters.com/2016/08/2016-08-27-httpclientwrong/).
Additionally adding 
[automatic transient-fault handling](https://docs.microsoft.com/en-us/dotnet/architecture/microservices/implement-resilient-applications/implement-http-call-retries-exponential-backoff-polly) is easy by using the 
[Polly library](https://github.com/App-vNext/Polly).

## Adding the IExchangeProvider interface to Dependency injection:

```c#
// ConfigureServices()  - Startup.cs

// Add the HttpClientFactory for the SaxoClient HttpClient, with Polly fault handling
services.AddHttpClient<IExchangeProvider, SaxoClient>()
        .SetHandlerLifetime(TimeSpan.FromMinutes(5))  //Set lifetime to five minutes
        .AddPolicyHandler(GetRetryPolicy());

// Bind the SaxoClient configuration, using the Options interface
services.Configure<SaxoClientOptions>(
        Configuration.GetSection(SaxoClientOptions.SaxoClient));

// Add the SaxoClient as the provider of the ITradingProvider interface
services.AddTranscient<IStockTradeProvider, SaxoClient>();
```

## Initializing the IExchangeProvider without Dependency Injection:

```c#
// main()

using (var httpClient = new HttpClient()) // httpClient lifetime is handled by caller - client should be kept for duration of SaxoClient lifetime
{
    IExchangeProvider exchangeProvider = new SaxoClient(
        NullLogger<SaxoClient>.Instance,
        Options.Create(new SaxoClientOptions()),
        httpClient
    );
    
    exchangeProvider....
}

```
