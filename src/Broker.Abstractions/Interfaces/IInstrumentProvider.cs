using Broker.Abstractions.BaseClasses;
using Broker.Abstractions.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Broker.Abstractions.Interfaces
{
    /// <summary>
    /// Interface of a system that provide information about instruments, as well as historic and
    /// current trade information
    /// </summary>
    interface IInstrumentProvider
    {
        /// <summary>
        /// Returns all instruments that in contain the search expression, and type if defined
        /// returns empty list if nothing matches
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        List<Instrument> SearchInstruments(string expression, InstrumentType type = InstrumentType.Undefined);

        /// <summary>
        /// Gets information about an instrument that matches a given name and type if defined
        /// </summary>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        bool TryGetInstrumentByNameType(string name, out Instrument result, InstrumentType type = InstrumentType.Undefined);
    }
}
