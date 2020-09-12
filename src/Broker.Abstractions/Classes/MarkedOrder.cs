using Broker.Abstractions.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Broker.Abstractions.Classes
{
    /// <summary>
    /// Simplest order - an amount at a price
    /// </summary>
    public class MarkedOrder
    {
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
