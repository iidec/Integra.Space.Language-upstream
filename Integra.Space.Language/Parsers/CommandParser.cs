//-----------------------------------------------------------------------
// <copyright file="CommandParser.cs" company="Integra.Space.Language">
//     Copyright (c) Integra.Space.Language. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Language
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using Common;
    using Integra.Space.Language.Grammars;
    using Irony.Interpreter;
    using Irony.Parsing;

    /// <summary>
    /// Class that implements the logic to parse conditional expressions
    /// </summary>
    internal class CommandParser
    {
        /// <summary>
        /// Command text.
        /// </summary>
        private string commandText;

        /// <summary>
        /// Grammar rule validator.
        /// </summary>
        private IGrammarRuleValidator ruleValidator;

        /// <summary>
        /// Parse tree.
        /// </summary>
        private ParseTree parseTree;

        /// <summary>
        /// Result list.
        /// </summary>
        private ParseContext context;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandParser"/> class.
        /// </summary>
        /// <param name="commandText">Command text</param>
        /// <param name="ruleValidator">Grammar rule validator.</param>
        public CommandParser(string commandText, IGrammarRuleValidator ruleValidator)
        {
            this.commandText = commandText;
            this.ruleValidator = ruleValidator;
            this.context = new ParseContext();
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
        /// <returns>Parse context.</returns>
        public ParseContext Evaluate()
        {
            BatchNode[] batches = (BatchNode[])this.EvaluateParseTree();

            if (batches != null)
            {
                this.context.Batches = batches;
            }

            return this.context;
        }

        /// <summary>
        /// Implements the logic to parse commands.
        /// </summary>
        /// <param name="parameters">Binding parameters.</param>
        /// <returns>Execution plan.</returns>
        protected object EvaluateParseTree(params BindingParameter[] parameters)
        {
            ScriptApp app = new ScriptApp(new CommandGrammarRuntime(this.ruleValidator));

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
            CommandGrammar grammar = new CommandGrammar(this.ruleValidator);
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
