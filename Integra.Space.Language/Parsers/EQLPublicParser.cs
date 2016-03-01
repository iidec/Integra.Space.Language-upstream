//-----------------------------------------------------------------------
// <copyright file="EQLPublicParser.cs" company="Integra.Space.Language">
//     Copyright (c) Integra.Space.Language. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Language
{
    using System.Collections.Generic;
    using Exceptions;
    using Integra.Space.Language.Grammars;
    using Irony.Parsing;

    /// <summary>
    /// Class that implements the logic to parse commands
    /// </summary>
    internal sealed class EQLPublicParser
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
        /// Initializes a new instance of the <see cref="EQLPublicParser"/> class.
        /// </summary>
        /// <param name="commandText">Command text</param>
        public EQLPublicParser(string commandText)
        {
            this.commandText = commandText;
        }

        /// <summary>
        /// Gets the parse tree generated.
        /// </summary>
        public ParseTree ParseTree
        {
            get
            {
                return this.parseTree;
            }
        }

        /// <summary>
        /// Implements the logic to parse commands.
        /// </summary>
        /// <returns>Execution plan.</returns>
        public List<PlanNode> Evaluate()
        {
            List<PlanNode> nodes = null;

            try
            {
                this.Parse();
                Irony.Interpreter.ScriptApp app = new Irony.Interpreter.ScriptApp(new EQLLanguageRuntime());
                nodes = (List<PlanNode>)app.Evaluate(this.parseTree);
            }
            catch (SyntaxException e)
            {
                throw e;
            }
            catch (System.Exception e)
            {
                throw new ParseException(Resources.SR.InterpretationException, e);
            }

            return nodes;
        }

        /// <summary>
        /// Implements the logic to parse commands.
        /// </summary>
        public void Parse()
        {
            EQLGrammar grammar = new EQLGrammar();
            LanguageData language = new LanguageData(grammar);
            Parser parser = new Parser(language);
            this.parseTree = parser.Parse(this.commandText);
            if (this.parseTree.HasErrors())
            {
                foreach (var parserMessage in this.parseTree.ParserMessages)
                {
                    throw new SyntaxException(Resources.SR.SyntaxError(parserMessage.Message, parserMessage.Location.Line, parserMessage.Location.Column));
                }
            }
        }
    }
}
