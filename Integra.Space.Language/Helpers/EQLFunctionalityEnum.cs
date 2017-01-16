//-----------------------------------------------------------------------
// <copyright file="EQLFunctionalityEnum.cs" company="Integra.Space.Language">
//     Copyright (c) Integra.Space.Language. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Language
{
    /// <summary>
    /// EQL functionality enumerator.
    /// </summary>
    internal enum EQLFunctionalityEnum
    {
        /// <summary>
        /// Create source functionality.
        /// </summary>
        CreateSource,
        
        /// <summary>
        /// Create stream functionality.
        /// </summary>
        CreateStream,

        /// <summary>
        /// Drop source functionality.
        /// </summary>
        DropSource,

        /// <summary>
        /// Drop stream functionality.
        /// </summary>
        DropStream,

        /// <summary>
        /// Insert functionality.
        /// </summary>
        Insert,

        /// <summary>
        /// Temporal stream functionality.
        /// </summary>
        TemporalStream,

        /// <summary>
        /// Join functionality.
        /// </summary>
        Join
    }
}
