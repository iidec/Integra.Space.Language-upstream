//-----------------------------------------------------------------------
// <copyright file="BnfTermExtensions.cs" company="Integra.Space.Language">
//     Copyright (c) Integra.Space.Language. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Language
{
    using Irony.Parsing;

    /// <summary>
    /// BNF terminal extension class.
    /// </summary>
    internal static class BnfTermExtensions
    {
        /// <summary>
        /// Add a new expression.
        /// </summary>
        /// <param name="expresion">Expression to which we'll add the new expression.</param>
        /// <param name="term">Expression to add.</param>
        /// <param name="functionality">Logic functionality of the term parameter.</param>
        /// <param name="validator">Functionality validator.</param>
        /// <returns>A new expression: expression | term.</returns>
        public static BnfExpression AddOr(this BnfTerm expresion, BnfTerm term, EQLFunctionalityEnum functionality, IGrammarRuleValidator validator)
        {
            if (validator.IsAnAllowedFunctionality(functionality))
            {
                return expresion | term;
            }
            else
            {
                return (BnfExpression)expresion;
            }
        }

        /// <summary>
        /// Add a new expression.
        /// </summary>
        /// <param name="expresion">Expression to which we'll add the new expression.</param>
        /// <param name="term">Expression to add.</param>
        /// <param name="functionality">Logic functionality of the term parameter.</param>
        /// <param name="validator">Functionality validator.</param>
        /// <returns>A new expression: expression | term.</returns>
        public static BnfExpression AddOr(this BnfExpression expresion, BnfTerm term, EQLFunctionalityEnum functionality, IGrammarRuleValidator validator)
        {
            if (validator.IsAnAllowedFunctionality(functionality))
            {
                return expresion | term;
            }
            else
            {
                return expresion;
            }
        }
    }
}
