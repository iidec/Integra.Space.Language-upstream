//-----------------------------------------------------------------------
// <copyright file="QueryGrammar.cs" company="Integra.Space.Language">
//     Copyright (c) Integra.Space.Language. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Language.Grammars
{
    using System;
    using System.Linq;
    using ASTNodes;
    using ASTNodes.Constants;
    using ASTNodes.Identifier;
    using ASTNodes.Lists;
    using ASTNodes.QuerySections;
    using ASTNodes.UserQuery;
    using Irony.Interpreter;
    using Irony.Parsing;

    /// <summary>
    /// EQLGrammar grammar for the commands and the predicates 
    /// </summary>
    [Language("EQLGrammar", "0.4", "")]
    internal sealed class QueryGrammar : InterpretedLanguageGrammar
    {
        /// <summary>
        /// Expression grammar
        /// </summary>
        private ExpressionGrammar expressionGrammar;
                
        /// <summary>
        /// Initializes a new instance of the <see cref="QueryGrammar"/> class
        /// </summary>
        public QueryGrammar()
            : base(false)
        {
            this.expressionGrammar = new ExpressionGrammar();
            this.CreateGrammar();
        }

        /// <summary>
        /// Specify the query grammar.
        /// </summary>
        public void CreateGrammar()
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
            KeyTerm terminalAs = ToTerm("as", "as");
            KeyTerm terminalLeft = ToTerm("left", "left");
            KeyTerm terminalRight = ToTerm("right", "right");
            KeyTerm terminalCross = ToTerm("cross", "cross");
            KeyTerm terminalInner = ToTerm("inner", "inner");
            KeyTerm terminalJoin = ToTerm("join", "join");
            KeyTerm terminalOn = ToTerm("on", "on");
            KeyTerm terminalTimeout = ToTerm("timeout", "timeout");
            KeyTerm terminalWith = ToTerm("with", "with");
            KeyTerm terminalgroup = ToTerm("group", "group");
            KeyTerm terminalSelect = ToTerm("select", "select");
            KeyTerm terminalTop = ToTerm("top", "top");

            // Marcamos los terminales, definidos hasta el momento, como palabras reservadas
            this.MarkReservedWords(this.KeyTerms.Keys.ToArray());

            /* SIMBOLOS */
            KeyTerm terminalComa = ToTerm(",", "coma");

            /* CONSTANTES E IDENTIFICADORES */
            Terminal terminalDateTimeValue = new QuotedValueLiteral("datetimeValue", "'", TypeCode.String);
            terminalDateTimeValue.AstConfig.NodeType = null;
            terminalDateTimeValue.AstConfig.DefaultNodeCreator = () => new DateTimeOrTimespanNode();
            Terminal terminalNumero = TerminalFactory.CreateCSharpNumber("number");
            terminalNumero.AstConfig.NodeType = null;
            terminalNumero.AstConfig.DefaultNodeCreator = () => new NumberNode();

            RegexBasedTerminal terminalId = new RegexBasedTerminal("[a-zA-Z]+([a-zA-Z]|[0-9]|[_])*");
            terminalId.Name = "identifier";
            terminalId.AstConfig.NodeType = null;
            terminalId.AstConfig.DefaultNodeCreator = () => new IdentifierASTNode();

            /* COMENTARIOS */
            CommentTerminal comentarioLinea = new CommentTerminal("line_coment", "//", "\n", "\r\n");
            CommentTerminal comentarioBloque = new CommentTerminal("block_coment", "/*", "*/");
            NonGrammarTerminals.Add(comentarioLinea);
            NonGrammarTerminals.Add(comentarioBloque);

            /* NO TERMINALES */
            NonTerminal nt_LOGIC_EXPRESSION = this.expressionGrammar.LogicExpression;
            NonTerminal nt_LOGIC_EXPRESSION_FOR_ON_CONDITION = this.expressionGrammar.LogicExpressionForOnCondition;

            NonTerminal nt_GROUP_BY_OP = new NonTerminal("GROUP_BY_OP", typeof(PassNode));
            nt_GROUP_BY_OP.AstConfig.NodeType = null;
            nt_GROUP_BY_OP.AstConfig.DefaultNodeCreator = () => new PassNode();
            NonTerminal nt_WHERE = new NonTerminal("WHERE", typeof(WhereNode));
            nt_WHERE.AstConfig.NodeType = null;
            nt_WHERE.AstConfig.DefaultNodeCreator = () => new WhereNode();
            NonTerminal nt_FROM = new NonTerminal("FROM", typeof(SourceNode));
            nt_FROM.AstConfig.NodeType = null;
            nt_FROM.AstConfig.DefaultNodeCreator = () => new SourceNode(0);
            NonTerminal nt_WITH = new NonTerminal("WITH", typeof(SourceNode));
            nt_WITH.AstConfig.NodeType = null;
            nt_WITH.AstConfig.DefaultNodeCreator = () => new SourceNode(1);
            NonTerminal nt_JOIN_SOURCE = new NonTerminal("JOIN_SOURCE", typeof(SourceNode));
            nt_JOIN_SOURCE.AstConfig.NodeType = null;
            nt_JOIN_SOURCE.AstConfig.DefaultNodeCreator = () => new SourceNode(0);
            NonTerminal nt_APPLY_WINDOW = new NonTerminal("APPLY_WINDOW", typeof(ApplyWindowNode));
            nt_APPLY_WINDOW.AstConfig.NodeType = null;
            nt_APPLY_WINDOW.AstConfig.DefaultNodeCreator = () => new ApplyWindowNode();
            NonTerminal nt_ORDER_BY = new NonTerminal("ORDER_BY", typeof(OrderByNode));
            nt_ORDER_BY.AstConfig.NodeType = null;
            nt_ORDER_BY.AstConfig.DefaultNodeCreator = () => new OrderByNode();
            NonTerminal nt_LIST_OF_VALUES_FOR_ORDER_BY = new NonTerminal("LIST_OF_VALUES", typeof(ListNodeOrderBy));
            nt_LIST_OF_VALUES_FOR_ORDER_BY.AstConfig.NodeType = null;
            nt_LIST_OF_VALUES_FOR_ORDER_BY.AstConfig.DefaultNodeCreator = () => new ListNodeOrderBy();
            NonTerminal nt_USER_QUERY = new NonTerminal("USER_QUERY", typeof(UserQueryNode));
            nt_USER_QUERY.AstConfig.NodeType = null;
            nt_USER_QUERY.AstConfig.DefaultNodeCreator = () => new UserQueryNode();            
            NonTerminal nt_ID_WITH_ALIAS = new NonTerminal("ID_WITH_ALIAS", typeof(ConstantValueWithAliasNode));
            nt_ID_WITH_ALIAS.AstConfig.NodeType = null;
            nt_ID_WITH_ALIAS.AstConfig.DefaultNodeCreator = () => new ConstantValueWithAliasNode();
            NonTerminal nt_JOIN = new NonTerminal("JOIN", typeof(JoinNode));
            nt_JOIN.AstConfig.NodeType = null;
            nt_JOIN.AstConfig.DefaultNodeCreator = () => new JoinNode();
            NonTerminal nt_ON = new NonTerminal("ON", typeof(OnNode));
            nt_ON.AstConfig.NodeType = null;
            nt_ON.AstConfig.DefaultNodeCreator = () => new OnNode();
            NonTerminal nt_SOURCE_DEFINITION = new NonTerminal("SOURCE_DEFINITION", typeof(PassNode));
            nt_SOURCE_DEFINITION.AstConfig.NodeType = null;
            nt_SOURCE_DEFINITION.AstConfig.DefaultNodeCreator = () => new PassNode();
            NonTerminal nt_TIMEOUT = new NonTerminal("TIMEOUT", typeof(TimeoutNode));
            nt_TIMEOUT.AstConfig.NodeType = null;
            nt_TIMEOUT.AstConfig.DefaultNodeCreator = () => new TimeoutNode();
            NonTerminal nt_ID_OR_ID_WITH_ALIAS = new NonTerminal("ID_OR_ID_WITH_ALIAS", typeof(ConstantValueWithOptionalAliasNode));
            nt_ID_OR_ID_WITH_ALIAS.AstConfig.NodeType = null;
            nt_ID_OR_ID_WITH_ALIAS.AstConfig.DefaultNodeCreator = () => new ConstantValueWithOptionalAliasNode();
            NonTerminal nt_JOIN_TYPE = new NonTerminal("JOIN_TYPE", typeof(PassNode));
            nt_JOIN_TYPE.AstConfig.NodeType = null;
            nt_JOIN_TYPE.AstConfig.DefaultNodeCreator = () => new PassNode();

            /* GROUP BY */
            NonTerminal nt_VALUES_WITH_ALIAS_FOR_GROUP_BY = new NonTerminal("VALUES_WITH_ALIAS", typeof(ConstantValueWithAliasNode));
            nt_VALUES_WITH_ALIAS_FOR_GROUP_BY.AstConfig.NodeType = null;
            nt_VALUES_WITH_ALIAS_FOR_GROUP_BY.AstConfig.DefaultNodeCreator = () => new ConstantValueWithAliasNode();
            NonTerminal nt_LIST_OF_VALUES_FOR_GROUP_BY = new NonTerminal("LIST_OF_VALUES", typeof(PlanNodeListNode));
            nt_LIST_OF_VALUES_FOR_GROUP_BY.AstConfig.NodeType = null;
            nt_LIST_OF_VALUES_FOR_GROUP_BY.AstConfig.DefaultNodeCreator = () => new PlanNodeListNode();
            NonTerminal nt_GROUP_BY = new NonTerminal("GROUP_BY", typeof(GroupByNode));
            nt_GROUP_BY.AstConfig.NodeType = null;
            nt_GROUP_BY.AstConfig.DefaultNodeCreator = () => new GroupByNode();

            /* PROJECTION */
            NonTerminal nt_VALUES_WITH_ALIAS_FOR_PROJECTION = new NonTerminal("VALUES_WITH_ALIAS", typeof(ConstantValueWithAliasNode));
            nt_VALUES_WITH_ALIAS_FOR_PROJECTION.AstConfig.NodeType = null;
            nt_VALUES_WITH_ALIAS_FOR_PROJECTION.AstConfig.DefaultNodeCreator = () => new ConstantValueWithAliasNode();
            NonTerminal nt_LIST_OF_VALUES_FOR_PROJECTION = new NonTerminal("LIST_OF_VALUES", typeof(PlanNodeListNode));
            nt_LIST_OF_VALUES_FOR_PROJECTION.AstConfig.NodeType = null;
            nt_LIST_OF_VALUES_FOR_PROJECTION.AstConfig.DefaultNodeCreator = () => new PlanNodeListNode();
            NonTerminal nt_SELECT = new NonTerminal("SELECT", typeof(SelectNode));
            nt_SELECT.AstConfig.NodeType = null;
            nt_SELECT.AstConfig.DefaultNodeCreator = () => new SelectNode();
            NonTerminal nt_TOP = new NonTerminal("TOP", typeof(TopNode));
            nt_TOP.AstConfig.NodeType = null;
            nt_TOP.AstConfig.DefaultNodeCreator = () => new TopNode();

            /* USER QUERY */
            nt_USER_QUERY.Rule = nt_SOURCE_DEFINITION + nt_WHERE + nt_SELECT
                                    | nt_SOURCE_DEFINITION + nt_WHERE + nt_APPLY_WINDOW + nt_GROUP_BY_OP + nt_SELECT + nt_ORDER_BY;
            /* **************************** */
            /* APPLY WINDOW */
            nt_APPLY_WINDOW.Rule = terminalApply + terminalWindow + terminalOf + terminalDateTimeValue;
            /* **************************** */
            /* ORDER BY */
            nt_ORDER_BY.Rule = terminalOrder + terminalBy + nt_LIST_OF_VALUES_FOR_ORDER_BY
                                | terminalOrder + terminalBy + terminalAsc + nt_LIST_OF_VALUES_FOR_ORDER_BY
                                | terminalOrder + terminalBy + terminalDesc + nt_LIST_OF_VALUES_FOR_ORDER_BY
                                | this.Empty;

            nt_LIST_OF_VALUES_FOR_ORDER_BY.Rule = nt_LIST_OF_VALUES_FOR_ORDER_BY + terminalComa + terminalId
                                                    | terminalId;
            /* **************************** */
            /* SOURCE DEFINITION */
            nt_SOURCE_DEFINITION.Rule = nt_FROM
                                        | nt_JOIN;
            /* **************************** */
            /* FROM */
            nt_FROM.Rule = terminalFrom + nt_ID_OR_ID_WITH_ALIAS;
            /* **************************** */
            /* JOIN SOURCE */
            nt_JOIN_SOURCE.Rule = terminalJoin + nt_ID_OR_ID_WITH_ALIAS;
            /* **************************** */
            /* WITH */
            nt_WITH.Rule = terminalWith + nt_ID_OR_ID_WITH_ALIAS;
            /* **************************** */
            /* ID OR ID WITH ALIAS */
            nt_ID_OR_ID_WITH_ALIAS.Rule = terminalId + terminalAs + terminalId
                                            | terminalId;
            /* **************************** */
            /* JOIN */
            nt_JOIN.Rule = nt_JOIN_TYPE + nt_JOIN_SOURCE + nt_WHERE + nt_WITH + nt_WHERE + nt_ON + nt_TIMEOUT;
            /* **************************** */
            /* JOIN TYPE */
            nt_JOIN_TYPE.Rule = terminalLeft
                                | terminalRight
                                | terminalCross
                                | terminalInner
                                | this.Empty;
            /* **************************** */
            /* ON */
            nt_ON.Rule = terminalOn + nt_LOGIC_EXPRESSION_FOR_ON_CONDITION;
            /* **************************** */
            /* TIMEOUT */
            nt_TIMEOUT.Rule = terminalTimeout + terminalDateTimeValue;
            /* **************************** */
            /* WHERE */
            nt_WHERE.Rule = terminalWhere + nt_LOGIC_EXPRESSION
                            | this.Empty;
            /* **************************** */
            /* OPTIONAL GROUP BY */
            nt_GROUP_BY_OP.Rule = nt_GROUP_BY
                                    | this.Empty;
            /* **************************** */            
            /* GROUP BY */
            nt_GROUP_BY.Rule = terminalgroup + terminalBy + nt_LIST_OF_VALUES_FOR_GROUP_BY;
            /* **************************** */
            /* LISTA DE VALORES */
            nt_LIST_OF_VALUES_FOR_GROUP_BY.Rule = nt_LIST_OF_VALUES_FOR_GROUP_BY + terminalComa + nt_VALUES_WITH_ALIAS_FOR_GROUP_BY
                                    | nt_VALUES_WITH_ALIAS_FOR_GROUP_BY;
            /* **************************** */
            /* VALORES CON ALIAS */
            nt_VALUES_WITH_ALIAS_FOR_GROUP_BY.Rule = this.expressionGrammar.Values + terminalAs + terminalId;

            /* SELECT */
            nt_SELECT.Rule = terminalSelect + nt_TOP + nt_LIST_OF_VALUES_FOR_PROJECTION
                                        | terminalSelect + nt_LIST_OF_VALUES_FOR_PROJECTION;
            /* **************************** */
            /* TOP */
            nt_TOP.Rule = terminalTop + terminalNumero;
            /* **************************** */
            /* LISTA DE VALORES */
            nt_LIST_OF_VALUES_FOR_PROJECTION.Rule = nt_LIST_OF_VALUES_FOR_PROJECTION + terminalComa + nt_VALUES_WITH_ALIAS_FOR_PROJECTION
                                    | nt_VALUES_WITH_ALIAS_FOR_PROJECTION;
            /* **************************** */
            /* VALORES CON ALIAS */
            nt_VALUES_WITH_ALIAS_FOR_PROJECTION.Rule = this.expressionGrammar.ProjectionValue + terminalAs + terminalId;

            this.Root = nt_USER_QUERY;

            this.LanguageFlags = Irony.Parsing.LanguageFlags.CreateAst;
        }
    }
}
