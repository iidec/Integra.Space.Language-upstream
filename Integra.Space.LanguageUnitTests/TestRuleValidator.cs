//-----------------------------------------------------------------------
// <copyright file="TestRuleValidator.cs" company="Integra.Space.Language">
//     Copyright (c) Integra.Space.Language. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.LanguageUnitTests
{
    using System.Collections.Generic;
    using Language;

    /// <summary>
    /// Test rule validator class.
    /// </summary>
    public class TestRuleValidator : IGrammarRuleValidator
    {
        /// <summary>
        /// Test functionalities.
        /// </summary>
        private static Dictionary<EQLFunctionalityEnum, bool> functionalities = new Dictionary<EQLFunctionalityEnum, bool>
        {
            { EQLFunctionalityEnum.CreateSource, true },
            { EQLFunctionalityEnum.CreateStream, true },
            { EQLFunctionalityEnum.DropSource, true },
            { EQLFunctionalityEnum.DropStream, true },
            { EQLFunctionalityEnum.Insert, true },
            { EQLFunctionalityEnum.Join, true },
            { EQLFunctionalityEnum.TemporalStream, true },
            { EQLFunctionalityEnum.CreateDatabaseRole, false },
        };

        /// <inheritdoc />
        public bool IsAnAllowedFunctionality(EQLFunctionalityEnum functionality)
        {
            return true;

            if (functionalities.ContainsKey(functionality))
            {
                return functionalities[functionality];
            }

            return false;
        }
    }
}
