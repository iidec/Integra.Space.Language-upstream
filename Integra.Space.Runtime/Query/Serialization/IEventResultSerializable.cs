//-----------------------------------------------------------------------
// <copyright file="IEventResultSerializable.cs" company="Ingetra.Space.Runtime">
//     Copyright (c) Ingetra.Space.Runtime. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space
{
    /// <summary>
    /// Event Result Serializable
    /// </summary>
    public interface IEventResultSerializable
    {
        /// <summary>
        /// Serialize an object through IL emit.
        /// </summary>
        /// <param name="writer">Query result writer.</param>
        void Serialize(IQueryResultWriter writer);
    }
}
