# Work in progress

# SaxoClient
Implementation of the Saxo Bank OpenApi in dotnet core

# Using SaxoClient with Dependency Injection and ASP.NET Standard 2.1

The SaxoClient offers first-class support for Dependency Injection, and implements the (HttpClientFactory
pattern)[https://docs.microsoft.com/en-us/dotnet/architecture/microservices/implement-resilient-applications/use-httpclientfactory-to-implement-resilient-http-requests],
that offer better resource management, and solves a number of problems with the better known (HttpClient)[https://aspnetmonsters.com/2016/08/2016-08-27-httpclientwrong/].
Additionally adding (automatic transient-fault handling)[https://docs.microsoft.com/en-us/dotnet/architecture/microservices/implement-resilient-applications/implement-http-call-retries-exponential-backoff-polly] is easy by using the (Polly library)[https://github.com/App-vNext/Polly].

## Adding the IStockTradeProvider interface to Dependency injection:

```c#
// ConfigureServices()  - Startup.cs

// Add the HttpClientFactory for the SaxoClient HttpClient, with Polly fault handling
services.AddHttpClient<SaxoClient>()
        .SetHandlerLifetime(TimeSpan.FromMinutes(5))  //Set lifetime to five minutes
        .AddPolicyHandler(GetRetryPolicy());

// Bind the SaxoClient configuration, using the Options interface
services.Configure<SaxoClientOptions>(
        Configuration.GetSection(SaxoClientOptions.SaxoClient));

// Add the SaxoClient as the provider of the ITradingProvider interface
services.AddTranscient<IStockTradeProvider, SaxoClient>();
```

## Initializing the IStockTradeProvider without Dependency Injection:

```c#
// main()

using (var httpClient = new HttpClient()) // httpClient lifetime is handled by caller - client should be kept for duration of SaxoClient lifetime
{
    var saxoClient = new SaxoClient(
        NullLogger<SaxoClient>.Instance,
        Options.Create(new SaxoClientOptions()),
        httpClient
    );
    
    saxoClient....
}

```