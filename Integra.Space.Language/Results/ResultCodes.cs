//-----------------------------------------------------------------------
// <copyright file="ResultCodes.cs" company="Integra.Space.Language">
//     Copyright (c) Integra.Space.Language. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Language
{
    /// <summary>
    /// Result codes class.
    /// </summary>
    internal enum ResultCodes
    {
        /// <summary>
        /// Success parse code.
        /// </summary>
        SuccessParseResultCode,

        /// <summary>
        /// Informative parse code.
        /// </summary>
        InfoParseResultCode,

        /// <summary>
        /// Warning parse code.
        /// </summary>
        WarningParseResultCode,

        /// <summary>
        /// Error parse code.
        /// </summary>
        ErrorParseResultCode,

        /// <summary>
        /// Grammar error code.
        /// </summary>
        GrammarError,

        /// <summary>
        /// Parse error code.
        /// </summary>
        ParseError,

        /// <summary>
        /// Duplicate extension code.
        /// </summary>
        DuplicateApplyExtension,

        /// <summary>
        /// Invalid command option code.
        /// </summary>
        InvalidCommandOption,

        /// <summary>
        /// Invalid command action code.
        /// </summary>
        InvalidCommandAction,
        
        /// <summary>
        /// Invalid system object code.
        /// </summary>
        InvalidSystemObjectType,

        /// <summary>
        /// Invalid command option code.
        /// </summary>
        DuplicateCommandOption,

        /// <summary>
        /// Repeated user code.
        /// </summary>
        RepeatedSystemObject,

        /// <summary>
        /// Command error code.
        /// </summary>
        CommandError,

        /// <summary>
        /// Repeated object code.
        /// </summary>
        RepeatedSystemObjectType,

        /// <summary>
        /// Duplicate column code.
        /// </summary>
        DuplicateColumn,

        /// <summary>
        /// Bad permission code.
        /// </summary>
        BadPermission,

        /// <summary>
        /// Invalid apply extension.
        /// </summary>
        InvalidApplyExtension,

        /// <summary>
        /// Invalid types code.
        /// </summary>
        InvalidTypes
    }
}
