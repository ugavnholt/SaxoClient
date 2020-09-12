using System;
using System.Collections.Generic;
using System.Text;

namespace Broker.Abstractions.Classes
{
    public sealed class OrderDepth
    {
        /// <summary>
        /// UTC Time of sample
        /// </summary>
        public DateTime Time { get; set; }

        /// <summary>
        /// Ordered by price descending, the number of buyorders at a given price
        /// </summary>
        public IList<MarkedOrder> BuyOrders { get; set; } = new List<MarkedOrder>();

        /// <summary>
        /// Ordered by price ascending, the number of sell orders at a given price
        /// </summary>
        public IList<MarkedOrder> SellOrders { get; set; } = new List<MarkedOrder>();
    }
}
