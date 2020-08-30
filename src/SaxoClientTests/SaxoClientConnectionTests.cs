using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Saxo;
using Saxo.Classes;
using System;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Saxo.Tests
{
    public sealed class SaxoClientConnectionTests
    {
        private readonly ITestOutputHelper _outputHelper;
        private readonly ILogger<SaxoClientConnectionTests> _logger;

        public SaxoClientConnectionTests(ITestOutputHelper outputHelper)
        {
            _outputHelper = outputHelper;

            var loggerFactory = new LoggerFactory();
            loggerFactory.AddProvider(new XunitLoggerProvider(outputHelper));
            _logger = loggerFactory.CreateLogger<SaxoClientConnectionTests>();
            _logger.LogInformation("Test Initialized");
        }

        [Fact]
        public ValueTask CreateConnectDispose()
        {
            using (var client = new SaxoClient(NullLogger<SaxoClient>.Instance, new SaxoClientOptions { }))
            {

            }
            return new ValueTask();
        }
    }
}
