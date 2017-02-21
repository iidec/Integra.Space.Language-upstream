//-----------------------------------------------------------------------
// <copyright file="SpaceParser.cs" company="Integra.Space.Language">
//     Copyright (c) Integra.Space.Language. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Language
{
    using System.Diagnostics;
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
        /// Result list.
        /// </summary>
        private ParseContext context;

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
        /// <param name="parameters">Binding parameters.</param>
        /// <returns>Execution plan.</returns>
        protected virtual object EvaluateParseTree(params BindingParameter[] parameters)
        {
            ScriptApp app = new ScriptApp(new TLanguageRuntime());

            try
            {
                foreach (var parameter in parameters)
                {
                    app.Globals.Add(parameter.Name, parameter.Value);
                }

                return app.Evaluate(this.ParseTree);
            }
            catch (System.Exception e)
            {
                // Get stack trace for the exception with source file information
                StackTrace st = new StackTrace(e, true);

                // Get the top stack frame
                StackFrame frame = st.GetFrame(0);

                this.context.Results.Add(new ParseErrorResult((int)ResultCodes.ParseError, string.Format("File name: {0}. Message: {1}", frame.GetFileName(), e.Message), frame.GetFileLineNumber(), frame.GetFileColumnNumber()));
                return null;
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
                        this.context.Results.Add(new ParseErrorResult((int)ResultCodes.ParseError, parserMessage.Message, parserMessage.Location.Line, parserMessage.Location.Column));
                    }
                }
            }
            else
            {
                string errorString = string.Join(",", language.Errors);
                this.context.Results.Add(new Common.ErrorResult((int)ResultCodes.GrammarError, string.Format("The language data has the following grammar error level: {0}. {1}", language.ErrorLevel, errorString)));
            }

            return parseTreeAux;
        }
    }
}
