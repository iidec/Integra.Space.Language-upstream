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
            if (term is NonTerminal)
            {
                if (((NonTerminal)term).Rule == null)
                {
                    return (BnfExpression)expresion;
                }
            }

            if (validator.IsAnAllowedFunctionality(functionality))
            {
                if (expresion == null)
                {
                    return (BnfExpression)term;
                }
                else
                {
                    return expresion | term;
                }
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
            if (term is NonTerminal)
            {
                if (((NonTerminal)term).Rule == null)
                {
                    return expresion;
                }
            }

            if (validator.IsAnAllowedFunctionality(functionality))
            {
                if (expresion == null)
                {
                    return (BnfExpression)term;
                }
                else
                {
                    return expresion | term;
                }
            }
            else
            {
                return expresion;
            }
        }

        /// <summary>
        /// Add a new expression.
        /// </summary>
        /// <param name="expresion">Expression to which we'll add the new expression.</param>
        /// <param name="term">Expression to add.</param>
        /// <returns>A new expression: expression | term.</returns>
        public static BnfExpression AddOr(this BnfTerm expresion, BnfTerm term)
        {
            if (term is NonTerminal)
            {
                if (((NonTerminal)term).Rule == null)
                {
                    return (BnfExpression)expresion;
                }
            }

            if (expresion == null)
            {
                return (BnfExpression)term;
            }
            else
            {
                return expresion | term;
            }
        }

        /// <summary>
        /// Add a new expression.
        /// </summary>
        /// <param name="expresion">Expression to which we'll add the new expression.</param>
        /// <param name="term">Expression to add.</param>
        /// <returns>A new expression: expression | term.</returns>
        public static BnfExpression AddOr(this BnfExpression expresion, BnfTerm term)
        {
            if (term is NonTerminal)
            {
                if (((NonTerminal)term).Rule == null)
                {
                    return (BnfExpression)expresion;
                }
            }

            if (expresion == null)
            {
                return (BnfExpression)term;
            }
            else
            {
                return expresion | term;
            }
        }

        /// <summary>
        /// Add a default BNF expression.
        /// </summary>
        /// <param name="expression">Incoming expression.</param>
        /// <returns>The incoming expression if is not null, or the default expression if expression is null.</returns>
        public static BnfExpression AddDefault(this BnfExpression expression)
        {
            if (expression == null)
            {
                // se agrega aqui el terminal para que se cree unicamente cuando no se haya definido ninguna funcionalidad.
                return new KeyTerm("go", "go");
            }
            else
            {
                return expression;
            }
        }

        /// <summary>
        /// Add the first rule to the rule set.
        /// </summary>
        /// <param name="first">First expression to add.</param>
        /// <param name="functionality">Logic functionality of the term parameter.</param>
        /// <param name="validator">Functionality validator.</param>
        /// <returns>The first expression or null if is not allowed.</returns>
        public static BnfExpression AddFirst(this BnfTerm first, EQLFunctionalityEnum functionality, IGrammarRuleValidator validator)
        {
            if (validator.IsAnAllowedFunctionality(functionality))
            {
                if (first is NonTerminal)
                {
                    if (((NonTerminal)first).Rule != null)
                    {
                        return (BnfExpression)first;
                    }
                }
                else
                {
                    return (BnfExpression)first;
                }
            }

            return null;
        }

        /// <summary>
        /// Add the first rule to the rule set.
        /// </summary>
        /// <param name="first">First expression to add.</param>
        /// <returns>The first expression or null if is not allowed.</returns>
        public static BnfExpression AddFirst(this BnfTerm first)
        {
            if (first is NonTerminal)
            {
                if (((NonTerminal)first).Rule != null)
                {
                    return (BnfExpression)first;
                }
            }
            else
            {
                return (BnfExpression)first;
            }

            return null;
        }
    }
}
