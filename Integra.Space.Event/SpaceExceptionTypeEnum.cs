//-----------------------------------------------------------------------
// <copyright file="SpaceExceptionTypeEnum.cs" company="Integra.Space.Common">
//     Copyright (c) Integra.Space.Common. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Integra.Space.Event
{
    /// <summary>
    /// Space exception type enumerable
    /// </summary>
    public enum SpaceExceptionTypeEnum
    {
        /// <summary>
        /// Unexpected exception type
        /// </summary>
        UnexpectedException = 0,

        /// <summary>
        /// SyntaxException type
        /// </summary>
        SyntaxException = 1,

        /// <summary>
        /// RuntimeException type
        /// </summary>
        RuntimeException = 2,

        /// <summary>
        /// ParseException type
        /// </summary>
        ParseException = 3,

        /// <summary>
        /// CompilationException type
        /// </summary>
        CompilationException = 4,

        /// <summary>
        /// NonExistentObjectException type
        /// </summary>
        NonExistentObjectException = 5
    }
}
