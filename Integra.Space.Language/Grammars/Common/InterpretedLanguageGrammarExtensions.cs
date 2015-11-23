//-----------------------------------------------------------------------
// <copyright file="InterpretedLanguageGrammarExtensions.cs" company="Integra.Space.Language">
//     Copyright (c) Integra.Space.Language. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Language.Grammars
{
    using Irony.Interpreter;
    using Irony.Parsing;

    /// <summary>
    /// InterpretedLanguageGrammar extensions
    /// </summary>
    internal static class InterpretedLanguageGrammarExtensions
    {
        /// <summary>
        /// Add precedence and associativity to InterpretedLanguageGrammar
        /// </summary>
        /// <param name="grammar">Grammar to add precedence and associativity</param>
        public static void AddPrecedenceAndAssociativity(this InterpretedLanguageGrammar grammar)
        {
            /* PRECEDENCIA Y ASOCIATIVIDAD */
            grammar.RegisterOperators(40, Associativity.Right, grammar.KeyTerms["parentesisIz"], grammar.KeyTerms["parentesisDer"]);
            grammar.RegisterOperators(35, Associativity.Right, grammar.KeyTerms["menos"]);
            grammar.RegisterOperators(30, Associativity.Right, grammar.KeyTerms["igualIgual"], grammar.KeyTerms["noIgual"], grammar.KeyTerms["mayorIgual"], grammar.KeyTerms["mayorQue"], grammar.KeyTerms["menorIgual"], grammar.KeyTerms["menorQue"], grammar.KeyTerms["like"], grammar.KeyTerms["in"]);
            grammar.RegisterOperators(20, Associativity.Right, grammar.KeyTerms["terminalAnd"]);
            grammar.RegisterOperators(10, Associativity.Right, grammar.KeyTerms["terminalOr"]);
            grammar.RegisterOperators(5, Associativity.Right, grammar.KeyTerms["terminalNot"]);
        }

        /// <summary>
        /// Add punctuation to InterpretedLanguageGrammar
        /// </summary>
        /// <param name="grammar">Grammar to add punctuation</param>
        public static void AddMarkPunctuation(this InterpretedLanguageGrammar grammar)
        {
            grammar.MarkPunctuation(grammar.KeyTerms["("], grammar.KeyTerms[")"], grammar.KeyTerms["["], grammar.KeyTerms["]"], grammar.KeyTerms["{"], grammar.KeyTerms["}"]);
        }

        /// <summary>
        /// Add brace pairs to InterpretedLanguageGrammar
        /// </summary>
        /// <param name="grammar">Grammar to add brace pairs</param>
        public static void AddBracePair(this InterpretedLanguageGrammar grammar)
        {
            grammar.RegisterBracePair("(", ")");
            grammar.RegisterBracePair("[", "]");
            grammar.RegisterBracePair("{", "}");
        }
    }
}
