using Broker.Abstractions.BaseClasses;
using System;
using System.Collections.Generic;
using System.Text;
using Broker.Abstractions.Enums;

namespace Broker.Abstractions.Classes
{
    /// <summary>
    /// Class that represents a stock, that can be traded
    /// </summary>
    public sealed class StockInstrument : Instrument
    {
        /// <summary>
        /// Id of the underlaying company
        /// </summary>
        public Guid CompanyId { get; set; }

        /// <summary>
        /// Date the commodity was registered on its exchange
        /// </summary>
        public DateTime RegistrationDate { get; set; }

        /// <summary>
        /// Date the comnmodity was taken off the exchange
        /// </summary>
        public DateTime? UnregistrationDate { get; set; }

        public StockInstrument(Guid id, Guid currencyId, Guid exchangeId, string tickerName, string caption) 
            : base(id, InstrumentType.Stock, currencyId, exchangeId, tickerName, caption)
        {

        }

        public StockInstrument(Guid id, Currency currency, Exchange exchange, string tickerName, string caption)
            : base(id, InstrumentType.Stock, currency.Id, exchange.Id, tickerName, caption)
        {

        }
    }
}
