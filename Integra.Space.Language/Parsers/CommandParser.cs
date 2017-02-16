//-----------------------------------------------------------------------
// <copyright file="CommandParser.cs" company="Integra.Space.Language">
//     Copyright (c) Integra.Space.Language. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Language
{
    using Exceptions;
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
        /// Initializes a new instance of the <see cref="CommandParser"/> class.
        /// </summary>
        /// <param name="commandText">Command text</param>
        /// <param name="ruleValidator">Grammar rule validator.</param>
        public CommandParser(string commandText, IGrammarRuleValidator ruleValidator)
        {
            this.commandText = commandText;
            this.ruleValidator = ruleValidator;
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
        public BatchNode[] Evaluate()
        {
            return (BatchNode[])this.EvaluateParseTree();
        }

        /// <summary>
        /// Implements the logic to parse commands.
        /// </summary>
        /// <param name="parameters">Binding parameters.</param>
        /// <returns>Execution plan.</returns>
        protected object EvaluateParseTree(params BindingParameter[] parameters)
        {
            try
            {
                ScriptApp app = new ScriptApp(new CommandGrammarRuntime(this.ruleValidator));

                foreach (var parameter in parameters)
                {
                    app.Globals.Add(parameter.Name, parameter.Value);
                }

                return app.Evaluate(this.ParseTree);
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
