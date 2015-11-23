//-----------------------------------------------------------------------
// <copyright file="EQLGrammar.cs" company="Integra.Space.Language">
//     Copyright (c) Integra.Space.Language. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Language.Grammars
{
    using System;
    using System.Linq;
    using ASTNodes.Constants;
    using ASTNodes.Lists;
    using ASTNodes.QuerySections;
    using ASTNodes.Root;
    using ASTNodes.UserQuery;
    using Irony.Interpreter;
    using Irony.Parsing;

    /// <summary>
    /// EQLGrammar grammar for the commands and the predicates 
    /// </summary>
    [Language("EQLGrammar", "0.4", "")]
    internal sealed class EQLGrammar : InterpretedLanguageGrammar
    {
        /// <summary>
        /// Expression grammar
        /// </summary>
        private ExpressionGrammar expressionGrammar;

        /// <summary>
        /// Projection grammar
        /// </summary>
        private ProjectionGrammar projectionGrammar;

        /// <summary>
        /// Group by grammar
        /// </summary>
        private GroupByGrammar groupByGrammar;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="EQLGrammar"/> class
        /// </summary>
        public EQLGrammar()
            : base(false)
        {
            this.expressionGrammar = new ExpressionGrammar();
            this.projectionGrammar = new ProjectionGrammar();
            this.groupByGrammar = new GroupByGrammar();
            this.Grammar();
        }

        /// <summary>
        /// Specify the language grammar
        /// </summary>
        public void Grammar()
        {
            /*** TERMINALES DE LA GRAMATICA ***/

            /* PALABRAS RESERVADAS */
            KeyTerm terminalFrom = ToTerm("from", "from");
            KeyTerm terminalWhere = ToTerm("where", "where");
            KeyTerm terminalApply = ToTerm("apply", "apply");
            KeyTerm terminalWindow = ToTerm("window", "window");
            KeyTerm terminalOf = ToTerm("of", "of");
            KeyTerm terminalOrder = ToTerm("order", "order");
            KeyTerm terminalAsc = ToTerm("asc", "asc");
            KeyTerm terminalDesc = ToTerm("desc", "desc");
            KeyTerm terminalBy = ToTerm("by", "by");
            
            // Marcamos los terminales, definidos hasta el momento, como palabras reservadas
            this.MarkReservedWords(this.KeyTerms.Keys.ToArray());

            /* SIMBOLOS */
            KeyTerm terminalComa = ToTerm(",", "coma");

            /* CONSTANTES E IDENTIFICADORES */
            Terminal terminalDateTimeValue = new QuotedValueLiteral("datetimeValue", "'", TypeCode.String);
            terminalDateTimeValue.AstConfig.NodeType = null;
            terminalDateTimeValue.AstConfig.DefaultNodeCreator = () => new DateTimeOrTimespanNode();

            RegexBasedTerminal terminalId = new RegexBasedTerminal("[a-zA-Z]+([a-zA-Z]|[0-9]|[_])*");
            terminalId.AstConfig.NodeType = null;
            terminalId.AstConfig.DefaultNodeCreator = () => new IdentifierNode();
            
            /* COMENTARIOS */
            CommentTerminal comentarioLinea = new CommentTerminal("comentario_linea", "//", "\n", "\r\n");
            CommentTerminal comentarioBloque = new CommentTerminal("comentario_bloque", "/*", "*/");
            NonGrammarTerminals.Add(comentarioLinea);
            NonGrammarTerminals.Add(comentarioBloque);

            /* NO TERMINALES */            
            NonTerminal nt_LOGIC_EXPRESSION = this.expressionGrammar.LogicExpression;
            NonTerminal nt_SELECT = this.projectionGrammar.ProjectionList;
            NonTerminal nt_GROUP_BY = this.groupByGrammar.GroupList;

            NonTerminal nt_WHERE = new NonTerminal("WHERE", typeof(WhereNode));
            nt_WHERE.AstConfig.NodeType = null;
            nt_WHERE.AstConfig.DefaultNodeCreator = () => new WhereNode();
            NonTerminal nt_FROM = new NonTerminal("FROM", typeof(FromNode));
            nt_FROM.AstConfig.NodeType = null;
            nt_FROM.AstConfig.DefaultNodeCreator = () => new FromNode();
            NonTerminal nt_APPLY_WINDOW = new NonTerminal("APPLY_WINDOW", typeof(ApplyWindowNode));
            nt_APPLY_WINDOW.AstConfig.NodeType = null;
            nt_APPLY_WINDOW.AstConfig.DefaultNodeCreator = () => new ApplyWindowNode();
            NonTerminal nt_ORDER_BY = new NonTerminal("ORDER_BY", typeof(OrderByNode));
            nt_ORDER_BY.AstConfig.NodeType = null;
            nt_ORDER_BY.AstConfig.DefaultNodeCreator = () => new OrderByNode();
            NonTerminal nt_LIST_OF_VALUES_FOR_ORDER_BY = new NonTerminal("LIST_OF_VALUES", typeof(PlanNodeListNode));
            nt_LIST_OF_VALUES_FOR_ORDER_BY.AstConfig.NodeType = null;
            nt_LIST_OF_VALUES_FOR_ORDER_BY.AstConfig.DefaultNodeCreator = () => new PlanNodeListNode();
            NonTerminal nt_VALUE_FOR_ORDER_BY = new NonTerminal("VALUES_WITH_ALIAS", typeof(ConstantValueWithoutAliasNode));
            nt_VALUE_FOR_ORDER_BY.AstConfig.NodeType = null;
            nt_VALUE_FOR_ORDER_BY.AstConfig.DefaultNodeCreator = () => new ConstantValueWithoutAliasNode();        
            NonTerminal nt_USER_QUERY = new NonTerminal("USER_QUERY", typeof(UserQueryNode));
            nt_USER_QUERY.AstConfig.NodeType = null;
            nt_USER_QUERY.AstConfig.DefaultNodeCreator = () => new UserQueryNode();
            NonTerminal nt_COMMAND_NODE = new NonTerminal("COMMAND_NODE", typeof(CommandNode));
            nt_COMMAND_NODE.AstConfig.NodeType = null;
            nt_COMMAND_NODE.AstConfig.DefaultNodeCreator = () => new CommandNode();
            
            /* USER QUERY */
            nt_USER_QUERY.Rule = nt_FROM + nt_SELECT
                                    | nt_FROM + nt_APPLY_WINDOW + nt_SELECT
                                    | nt_FROM + nt_WHERE + nt_SELECT
                                    | nt_FROM + nt_APPLY_WINDOW + nt_SELECT + nt_ORDER_BY
                                    | nt_FROM + nt_WHERE + nt_APPLY_WINDOW + nt_SELECT
                                    | nt_FROM + nt_APPLY_WINDOW + nt_GROUP_BY + nt_SELECT
                                    | nt_FROM + nt_WHERE + nt_APPLY_WINDOW + nt_GROUP_BY + nt_SELECT
                                    | nt_FROM + nt_WHERE + nt_APPLY_WINDOW + nt_SELECT + nt_ORDER_BY
                                    | nt_FROM + nt_APPLY_WINDOW + nt_GROUP_BY + nt_SELECT + nt_ORDER_BY
                                    | nt_FROM + nt_WHERE + nt_APPLY_WINDOW + nt_GROUP_BY + nt_SELECT + nt_ORDER_BY;
            /* **************************** */
            /* APPLY WINDOW */
            nt_APPLY_WINDOW.Rule = terminalApply + terminalWindow + terminalOf + terminalDateTimeValue;
            /* **************************** */
            /* ORDER BY */
            nt_ORDER_BY.Rule = terminalOrder + terminalBy + nt_LIST_OF_VALUES_FOR_ORDER_BY
                                | terminalOrder + terminalBy + terminalAsc + nt_LIST_OF_VALUES_FOR_ORDER_BY
                                | terminalOrder + terminalBy + terminalDesc + nt_LIST_OF_VALUES_FOR_ORDER_BY;

            nt_LIST_OF_VALUES_FOR_ORDER_BY.Rule = nt_LIST_OF_VALUES_FOR_ORDER_BY + terminalComa + nt_VALUE_FOR_ORDER_BY
                                                    | nt_VALUE_FOR_ORDER_BY;

            nt_VALUE_FOR_ORDER_BY.Rule = terminalId;            
            /* **************************** */
            /* FROM */
            nt_FROM.Rule = terminalFrom + terminalId;
            /* **************************** */
            /* WHERE */
            nt_WHERE.Rule = terminalWhere + nt_LOGIC_EXPRESSION;
            /* **************************** */
            /* COMANDOS */
            nt_COMMAND_NODE.Rule = nt_USER_QUERY;
            /* **************************** */

            this.Root = nt_COMMAND_NODE;

            this.LanguageFlags = Irony.Parsing.LanguageFlags.CreateAst;
        }
    }
}
