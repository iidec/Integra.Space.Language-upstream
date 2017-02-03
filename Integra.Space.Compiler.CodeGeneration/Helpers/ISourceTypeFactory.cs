//-----------------------------------------------------------------------
// <copyright file="ISourceTypeFactory.cs" company="Integra.Space.Language">
//     Copyright (c) Integra.Space.Language. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Compiler
{
    using System;

    /// <summary>
    /// Source type factory interface.
    /// </summary>
    internal interface ISourceTypeFactory
    {
        /// <summary>
        /// Get the type for the specify source.
        /// </summary>
        /// <param name="source">Source from with you will generate the type.</param>
        /// <returns>Source type.</returns>
        Type GetSourceType(Common.CommandObject source);
    }
}
