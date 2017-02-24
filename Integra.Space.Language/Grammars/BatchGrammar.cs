//-----------------------------------------------------------------------
// <copyright file="BatchGrammar.cs" company="Integra.Space.Language">
//     Copyright (c) Integra.Space.Language. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Language.Grammars
{
    using System;
    using System.Collections.Generic;
    using ASTNodes;
    using ASTNodes.Commands;
    using ASTNodes.Identifier;
    using ASTNodes.MetadataQuery;
    using ASTNodes.Root;
    using Common;
    using Irony.Interpreter;
    using Irony.Parsing;

    /// <summary>
    /// Command grammar class.
    /// </summary>
    [Language("BatchGrammar", "0.1", "")]
    internal class BatchGrammar : InterpretedLanguageGrammar
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BatchGrammar"/> class.
        /// </summary>
        public BatchGrammar() : base(false)
        {
            this.CreateGrammar();
        }

        /// <summary>
        /// Creates the batch grammar.
        /// </summary>
        public void CreateGrammar()
        {
            /* go terminal */
            RegexBasedTerminal terminalGo = new RegexBasedTerminal("go", @"\s(go)", new string[] { });
            NumberLiteral unsignedIntValueTerminal = new NumberLiteral("unsigned_int_value", NumberOptions.IntOnly);
            RegexBasedTerminal batchStringTerminal = new RegexBasedTerminal("go", @"[^\s(go)]*", new string[] { });

            /* COMENTARIOS */
            CommentTerminal comentarioLinea = new CommentTerminal("line_coment", "//", "\n", "\r\n");
            CommentTerminal comentarioBloque = new CommentTerminal("block_coment", "/*", "*/");
            NonGrammarTerminals.Add(comentarioLinea);
            NonGrammarTerminals.Add(comentarioBloque);

            /* GO */
            NonTerminal nt_GO = new NonTerminal("GO", typeof(GoASTNode));
            nt_GO.AstConfig.NodeType = null;
            nt_GO.AstConfig.DefaultNodeCreator = () => new GoASTNode();

            NonTerminal nt_SCRIPT_ITEM = new NonTerminal("SCRIPT");
            nt_SCRIPT_ITEM.AstConfig.NodeType = null;

            NonTerminal nt_SCRIPT = new NonTerminal("SCRIPT");
            nt_SCRIPT.AstConfig.NodeType = null;

            /* RULES DEFINITION */
            nt_GO.Rule = terminalGo
                         | terminalGo + unsignedIntValueTerminal;
            
            nt_SCRIPT_ITEM.Rule = terminalGo
                            | batchStringTerminal;

            nt_SCRIPT.Rule = this.MakePlusRule(nt_SCRIPT, nt_SCRIPT_ITEM);

            this.Root = nt_SCRIPT;
            this.LanguageFlags = LanguageFlags.CreateAst;
        }
    }
}
