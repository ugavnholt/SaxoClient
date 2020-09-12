using Broker.Abstractions.Classes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Broker.Abstractions.BaseClasses
{
    public abstract class Filter
    {
        /// <summary>
        /// Name of the filter
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Inputs the filter listens for
        /// </summary>
        public Dictionary<string, FilterStream> Inputs { get; set; } = new Dictionary<string, FilterStream>();

        /// <summary>
        /// Outputs the filter generates
        /// </summary>
        public Dictionary<string, FilterStream> Outputs { get; set; } = new Dictionary<string, FilterStream>();
    }
}
