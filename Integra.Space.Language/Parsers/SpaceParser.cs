//-----------------------------------------------------------------------
// <copyright file="SpaceParser.cs" company="Integra.Space.Language">
//     Copyright (c) Integra.Space.Language. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Language
{
    using System.Linq;
    using Exceptions;
    using Irony.Interpreter;
    using Irony.Parsing;

    /// <summary>
    /// Space parser class.
    /// </summary>
    /// <typeparam name="TGrammar">Grammar type.</typeparam>
    /// <typeparam name="TLanguageRuntime">Language runtime class.</typeparam>
    internal abstract class SpaceParser<TGrammar, TLanguageRuntime>
        where TGrammar : InterpretedLanguageGrammar, new()
        where TLanguageRuntime : LanguageRuntime, new()
    {
        /// <summary>
        /// Command text.
        /// </summary>
        private string commandText;

        /// <summary>
        /// Parse tree.
        /// </summary>
        private ParseTree parseTree;

        /// <summary>
        /// Initializes a new instance of the <see cref="SpaceParser{TGrammar, TLanguageRuntime}"/> class.
        /// </summary>
        /// <param name="commandText">Command text</param>
        public SpaceParser(string commandText)
        {
            this.commandText = commandText;
        }

        /// <summary>
        /// Gets the parse tree.
        /// </summary>
        public ParseTree ParseTree
        {
            get
            {
                if (this.parseTree == null)
                {
                    this.parseTree = this.Parse();
                }

                return this.parseTree;
            }
        }

        /// <summary>
        /// Implements the logic to parse commands.
        /// </summary>
        /// <returns>Execution plan.</returns>
        protected object EvaluateParseTree()
        {
            try
            {
                ParseTree parseTree = this.Parse();
                Irony.Interpreter.ScriptApp app = new Irony.Interpreter.ScriptApp(new TLanguageRuntime());
                return app.Evaluate(parseTree);
            }
            catch (SyntaxException e)
            {
                throw e;
            }
            catch (System.Exception e)
            {
                throw new ParseException(Resources.SR.InterpretationException, e);
            }
        }

        /// <summary>
        /// Implements the logic to parse commands.
        /// </summary>
        /// <returns>The parse tree.</returns>
        private ParseTree Parse()
        {
            TGrammar grammar = new TGrammar();
            LanguageData language = new LanguageData(grammar);
            ParseTree parseTreeAux = null;
            if (language.ErrorLevel == GrammarErrorLevel.NoError)
            {
                Parser parser = new Parser(language);
                parseTreeAux = parser.Parse(this.commandText);
                if (parseTreeAux.HasErrors())
                {
                    foreach (var parserMessage in parseTreeAux.ParserMessages)
                    {
                        throw new SyntaxException(Resources.SR.SyntaxError(parserMessage.Message, parserMessage.Location.Line, parserMessage.Location.Column));
                    }
                }
            }
            else
            {
                string errorString = string.Join(",", language.Errors);
                throw new System.Exception(string.Format("The language data has the following grammar error level '{0}'. Errors:\n{1}", language.ErrorLevel, errorString));
            }

            return parseTreeAux;
        }
    }
}
