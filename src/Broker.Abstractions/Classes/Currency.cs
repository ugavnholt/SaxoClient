using System;
using System.Collections.Generic;
using System.Text;

namespace Broker.Abstractions.Classes
{
    public sealed class Currency
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        public string Symbol { get; set; }

        public static Dictionary<Guid, Currency> Currencies { get; set; } = new Dictionary<Guid, Currency>
        {
            { Guid.Parse("2E4D5B7E-5657-4B4B-BC77-2D20800A3D67"), new Currency { Id = Guid.Parse("2E4D5B7E-5657-4B4B-BC77-2D20800A3D67"), Name = "DKK", Symbol = "kr" } },
            { Guid.Parse("B6B7B4D8-E646-4F07-96A6-A0F8218D4C5D"), new Currency { Id = Guid.Parse("B6B7B4D8-E646-4F07-96A6-A0F8218D4C5D"), Name = "USD", Symbol = "$" } },
            { Guid.Parse("FCEA4EBB-1007-4629-8D46-3294ADBAD0B8"), new Currency { Id = Guid.Parse("FCEA4EBB-1007-4629-8D46-3294ADBAD0B8"), Name = "EUR", Symbol = "€" } },
            { Guid.Parse("4D4EA6C7-C792-467B-905D-B08BDC0CDB23"), new Currency { Id = Guid.Parse("4D4EA6C7-C792-467B-905D-B08BDC0CDB23"), Name = "GBP", Symbol = "£" } },
        };

        /// <summary>
        /// Try to lookup an exchange given its shortName
        /// </summary>
        /// <param name="shortName"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static bool TryGetCurrency(string name, out Currency result)
        {
            result = null;
            name = name.ToUpperInvariant();
            foreach (var c in Currencies)
                if (c.Value.Name == name)
                {
                    result = c.Value;
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
        public static bool TryGetCurrency(Guid id, out Currency result)
        {
            return Currencies.TryGetValue(id, out result);
        }
    }
}
