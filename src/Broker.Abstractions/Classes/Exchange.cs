using System;
using System.Collections.Generic;
using System.Text;
using Broker.Abstractions.BaseClasses;

namespace Broker.Abstractions.Classes
{
    public sealed class Exchange
    {
        /// <summary>
        /// Internal id of the exchange
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// The short name of the exchange
        /// </summary>
        public string ShortName { get; set; }

        /// <summary>
        /// The full name of the exchange
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// The timezone info of the exchange
        /// </summary>
        public TimeZoneInfo TimeZone { get; set; }

        /// <summary>
        /// Local time when the exchange opens - be aware of daylight savings when converting to UTC
        /// </summary>
        public TimeSpan OpenLocalTime { get; set; }

        /// <summary>
        /// Local time when the exchange closes - be aware of daylight savings when converting to UTC
        /// </summary>
        public TimeSpan CloseLocalTime { get; set; }

        /// <summary>
        /// List of commodities that are registered on the exchange
        /// </summary>
        public Dictionary<string, Instrument> Commodities { get; set; } = new Dictionary<string, Instrument>();

        /// <summary>
        /// Static list of the exchanges we deal with, see https://en.wikipedia.org/wiki/List_of_stock_exchanges for details
        /// </summary>
        public static Dictionary<Guid, Exchange> Exchanges { get; set; } = new Dictionary<Guid, Exchange>
        {
            { Guid.Parse("0C3DD39B-952F-4322-AA4D-12FA52BB6945"), new Exchange {Id = Guid.Parse("0C3DD39B-952F-4322-AA4D-12FA52BB6945"), ShortName = "NYSE", FullName = "New York Stock Exchange", TimeZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time"), OpenLocalTime = new TimeSpan(9,30,0), CloseLocalTime = new TimeSpan(16,0,0) } },
            { Guid.Parse("3CFA648E-25C1-4755-93AE-2496F00B016A"), new Exchange {Id = Guid.Parse("3CFA648E-25C1-4755-93AE-2496F00B016A"), ShortName = "NASDAQ", FullName = "Nasdaq", TimeZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time"), OpenLocalTime = new TimeSpan(9,30,0), CloseLocalTime = new TimeSpan(16,0,0) } },
            { Guid.Parse("1C3497A8-E135-44DB-BAAE-A73D5117CDC0"), new Exchange {Id = Guid.Parse("1C3497A8-E135-44DB-BAAE-A73D5117CDC0"), ShortName = "LSE", FullName = "London Stock Exchange", TimeZone = TimeZoneInfo.FindSystemTimeZoneById("GMT Standard Time"), OpenLocalTime = new TimeSpan(8,0,0), CloseLocalTime = new TimeSpan(16,30,0) } },
            { Guid.Parse("C5A0507E-107F-427C-8AD5-9265662C5A6E"), new Exchange {Id = Guid.Parse("C5A0507E-107F-427C-8AD5-9265662C5A6E"), ShortName = "OMXC", FullName = "Nasdaq Copenhagen", TimeZone = TimeZoneInfo.FindSystemTimeZoneById("Romance Standard Time"), OpenLocalTime = new TimeSpan(9,0,0), CloseLocalTime = new TimeSpan(17,0,0) } },
        };

        /// <summary>
        /// Try to lookup an exchange given its shortName
        /// </summary>
        /// <param name="shortName"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static bool TryGetExchange(string shortName, out Exchange result)
        {
            result = null;
            shortName = shortName.ToUpperInvariant();
            foreach (var e in Exchanges)
                if (e.Value.ShortName == shortName)
                {
                    result = e.Value;
                    return true;
                }

            return false;
        }

        /// <summary>
        /// Try to lookup an exchange given its id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static bool TryGetExchange(Guid id, out Exchange result)
        {
            return Exchanges.TryGetValue(id, out result);
        }
    }
}
