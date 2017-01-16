//-----------------------------------------------------------------------
// <copyright file="DefaultRuleValidator.cs" company="Integra.Space.Language">
//     Copyright (c) Integra.Space.Language. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Language
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Default rule validator class.
    /// </summary>
    internal class DefaultRuleValidator : IGrammarRuleValidator
    {
        /// <summary>
        /// Default functionalities.
        /// </summary>
        private static Dictionary<EQLFunctionalityEnum, bool> functionalities = new Dictionary<EQLFunctionalityEnum, bool>
        {
            { EQLFunctionalityEnum.CreateSource, true },
            { EQLFunctionalityEnum.CreateStream, true },
            { EQLFunctionalityEnum.DropSource, true },
            { EQLFunctionalityEnum.DropStream, true },
            { EQLFunctionalityEnum.Insert, true },
            { EQLFunctionalityEnum.Join, true },
            { EQLFunctionalityEnum.TemporalStream, true }
        };

        /// <inheritdoc />
        public bool IsAnAllowedFunctionality(EQLFunctionalityEnum functionality)
        {
            if (functionalities.ContainsKey(functionality))
            {
                return functionalities[functionality];
            }

            return false;
        }
    }
}
