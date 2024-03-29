﻿//-----------------------------------------------------------------------
// <copyright file="GroupByGrammar.cs" company="Integra.Space.Language">
//     Copyright (c) Integra.Space.Language. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Language.Grammars
{
    using System.Linq;
    using ASTNodes.Constants;
    using ASTNodes.Identifier;
    using ASTNodes.Lists;
    using ASTNodes.QuerySections;
    using Irony.Interpreter;
    using Irony.Parsing;

    /// <summary>
    /// Projection grammar for the commands
    /// </summary>
    [Language("GroupByGrammar", "0.1", "")]
    internal sealed class GroupByGrammar : InterpretedLanguageGrammar
    {
        /// <summary>
        /// Expression grammar
        /// </summary>
        private ExpressionGrammar valueGrammar;

        /// <summary>
        /// Expression grammar
        /// </summary>
        private NonTerminal groupList;

        /// <summary>
        /// Initializes a new instance of the <see cref="GroupByGrammar"/> class
        /// </summary>
        public GroupByGrammar()
            : base(false)
        {
            this.valueGrammar = new ExpressionGrammar();
            this.CreateGrammar();
        }

        /// <summary>
        /// Gets the nonterminal for expression conditions
        /// </summary>
        public NonTerminal GroupList
        {
            get
            {
                return this.groupList;
            }
        }

        /// <summary>
        /// Specify the language grammar
        /// </summary>
        public void CreateGrammar()
        {
            // reserved words
            KeyTerm terminalgroup = ToTerm("group", "group");
            KeyTerm terminalBy = ToTerm("by", "by");
            KeyTerm terminalAs = ToTerm("as", "as");
            KeyTerm terminalComa = ToTerm(",", "coma");

            // Marcamos los terminales, definidos hasta el momento, como palabras reservadas
            this.MarkReservedWords(this.KeyTerms.Keys.ToArray());

            // terminals
            RegexBasedTerminal terminalId = new RegexBasedTerminal("[a-zA-Z]+([a-zA-Z]|[0-9]|[_])*");
            terminalId.Name = "identifier";
            terminalId.AstConfig.NodeType = null;
            terminalId.AstConfig.DefaultNodeCreator = () => new IdentifierNode();
            
            // nonterminals            
            NonTerminal nt_VALUES_WITH_ALIAS_FOR_GROUP_BY = new NonTerminal("VALUES_WITH_ALIAS", typeof(ConstantValueWithAliasNode));
            nt_VALUES_WITH_ALIAS_FOR_GROUP_BY.AstConfig.NodeType = null;
            nt_VALUES_WITH_ALIAS_FOR_GROUP_BY.AstConfig.DefaultNodeCreator = () => new ConstantValueWithAliasNode();
            NonTerminal nt_LIST_OF_VALUES_FOR_GROUP_BY = new NonTerminal("LIST_OF_VALUES", typeof(PlanNodeListNode));
            nt_LIST_OF_VALUES_FOR_GROUP_BY.AstConfig.NodeType = null;
            nt_LIST_OF_VALUES_FOR_GROUP_BY.AstConfig.DefaultNodeCreator = () => new PlanNodeListNode();
            this.groupList = new NonTerminal("GROUP_BY", typeof(GroupByNode));
            this.groupList.AstConfig.NodeType = null;
            this.groupList.AstConfig.DefaultNodeCreator = () => new GroupByNode();

            /* SELECT */
            this.groupList.Rule = terminalgroup + terminalBy + nt_LIST_OF_VALUES_FOR_GROUP_BY;
            /* **************************** */
            /* LISTA DE VALORES */
            nt_LIST_OF_VALUES_FOR_GROUP_BY.Rule = nt_LIST_OF_VALUES_FOR_GROUP_BY + terminalComa + nt_VALUES_WITH_ALIAS_FOR_GROUP_BY
                                    | nt_VALUES_WITH_ALIAS_FOR_GROUP_BY;
            /* **************************** */
            /* VALORES CON ALIAS */
            nt_VALUES_WITH_ALIAS_FOR_GROUP_BY.Rule = this.valueGrammar.Values + terminalAs + terminalId;

            this.Root = this.groupList;

            this.LanguageFlags = Irony.Parsing.LanguageFlags.CreateAst;
        }
    }
}
