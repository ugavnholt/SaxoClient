using System;
using System.Collections.Generic;
using System.Text;

namespace Broker.Abstractions.Classes
{
    public sealed class MarkedTrade
    {
        /// <summary>
        /// UTC Time of the order
        /// </summary>
        public DateTime Time { get; set; }

        /// <summary>
        /// Number of instruments traded
        /// </summary>
        public int Amount { get; set; }

        /// <summary>
        /// Price per instrument
        /// </summary>
        public double Price { get; set; }
    }
}
