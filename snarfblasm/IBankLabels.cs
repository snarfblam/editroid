using System;
using System.Collections.Generic;
using System.Text;

namespace Romulus.Plugin
{

    /// <summary>
    /// Represents an object that allows a builder to specify names or comments for addresses.
    /// </summary>
    public interface IAddressLabels
    {
        IBankLabels Ram { get; }
        IBankLabelList Banks { get; }
    }
    /// <summary>
    /// Used by IAddressLabels to represent ROM banks.
    /// </summary>
    public interface IBankLabelList
    {
        /// <summary>
        /// Gets a bank by index.
        /// </summary>
        /// <param name="i">The index of the bank.</param>
        /// <returns>An IBankLabels object for the bank.</returns>
        /// <exception cref="ArgumentException">Thrown if i is less than zero or greater than the number of banks present/supported.</exception>
        IBankLabels this[int i] { get; }
    }
    /// <summary>
    /// Used by IAddressLabels to represent labels within a given bank.
    /// </summary>
    public interface IBankLabels
    {
        void AddComment(ushort address, string comment);
        void AddLabel(ushort address, string label);
        void AddLabel(ushort address, string label, string comment);
        void AddArrayLabel(ushort address, string label, int byteCount);
        void AddArrayLabel(ushort address, string label, int byteCount, string comment);
    }
}
