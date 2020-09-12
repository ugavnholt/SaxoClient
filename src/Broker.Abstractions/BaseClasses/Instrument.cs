using Broker.Abstractions.Classes;
using Broker.Abstractions.Enums;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Broker.Abstractions.BaseClasses
{
    /// <summary>
    /// Class that represents a tradeable instrument
    /// </summary>
    public abstract class Instrument
    {
        /// <summary>
        /// The id of the instrument
        /// </summary>
        public Guid Id { get; private set; } 

        /// <summary>
        /// Id of the exchange the instrument is traded on
        /// </summary>
        public Guid ExchangeId { get; private set; }

        /// <summary>
        /// Id of the currency the instrument is traded in
        /// </summary>
        public Guid CurrencyId { get; private set; }

        /// <summary>
        /// The type of the instrument
        /// </summary>
        public InstrumentType InstrumentType { get; private set; }

        /// <summary>
        /// The name of the ticker, ie TSLA = Tesla Motors
        /// </summary>
        public string TickerName { get; private set; }

        /// <summary>
        /// Full name of the commodity ie, "Tesla Motors Inc." 
        /// </summary>
        public string Caption { get; private set; }

        /// <summary>
        /// Constructor taking all basic information
        /// </summary>
        /// <param name="id"></param>
        /// <param name="instrumentType"></param>
        /// <param name="currencyId"></param>
        /// <param name="exchangeId"></param>
        /// <param name="tickerName"></param>
        /// <param name="caption"></param>
        public Instrument(Guid id, InstrumentType instrumentType, Guid currencyId, Guid exchangeId, string tickerName, string caption)
        {
            Id = id;
            CurrencyId = currencyId;
            ExchangeId = exchangeId;
            InstrumentType = instrumentType;
            TickerName = tickerName;
            Caption = caption;
        }

        /// <summary>
        /// Constructor taking all basic information
        /// </summary>
        /// <param name="id"></param>
        /// <param name="instrumentType"></param>
        /// <param name="currency"></param>
        /// <param name="exchange"></param>
        /// <param name="tickerName"></param>
        /// <param name="caption"></param>
        public Instrument(Guid id, InstrumentType instrumentType, Currency currency, Exchange exchange, string tickerName, string caption)
            : this(id, instrumentType, currency.Id, exchange.Id, tickerName, caption)
        {
        }
    }
}
