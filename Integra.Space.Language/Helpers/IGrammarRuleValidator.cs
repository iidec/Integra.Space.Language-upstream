//-----------------------------------------------------------------------
// <copyright file="IGrammarRuleValidator.cs" company="Integra.Space.Language">
//     Copyright (c) Integra.Space.Language. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Language
{
    /// <summary>
    /// Grammar rule validator interface.
    /// </summary>
    internal interface IGrammarRuleValidator
    {
        /// <summary>
        /// Validate whether a specified functionality is allowed.
        /// </summary>
        /// <param name="functionality">Functionality to validate.</param>
        /// <returns>A value indicating whether a functionality is allowed or not.</returns>
        bool IsAnAllowedFunctionality(EQLFunctionalityEnum functionality);
    }
}
