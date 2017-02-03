//-----------------------------------------------------------------------
// <copyright file="IQueryResultSerializable.cs" company="Ingetra.Space.Runtime">
//     Copyright (c) Ingetra.Space.Runtime. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space
{
    /// <summary>
    /// Query Result Serializable.
    /// </summary>
    public interface IQueryResultSerializable
    {
        /// <summary>
        /// Serialize an object through IL emit.
        /// </summary>
        /// <param name="writer">Query result writer.</param>
        void Serialize(IQueryResultWriter writer);
    }
}
