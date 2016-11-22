//-----------------------------------------------------------------------
// <copyright file="TemporalStreamGrammar.cs" company="Integra.Space.Language">
//     Copyright (c) Integra.Space.Language. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Language.Grammars
{
    using System;
    using System.Linq;
    using ASTNodes;
    using ASTNodes.Commands;
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
    [Language("TemporalStreamGrammar", "0.4", "")]
    internal sealed class TemporalStreamGrammar : InterpretedLanguageGrammar
    {
        /// <summary>
        /// Expression grammar
        /// </summary>
        private ExpressionGrammar expressionGrammar;

        /// <summary>
        /// Terminal empty of the parent grammar.
        /// </summary>
        private Terminal terminalEmpty;

        /// <summary>
        /// Query for metadata.
        /// </summary>
        private NonTerminal temporalStream;

        /// <summary>
        /// MakeStarRule function of the parent grammar.
        /// </summary>
        private Func<NonTerminal, BnfTerm, BnfExpression> makeStarRuleOfParentGrammar;

        /// <summary>
        /// Initializes a new instance of the <see cref="TemporalStreamGrammar"/> class
        /// </summary>
        public TemporalStreamGrammar()
            : base(false)
        {
            this.expressionGrammar = new ExpressionGrammar();
            this.CreateGrammar();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TemporalStreamGrammar"/> class
        /// </summary>
        /// <param name="empty">Terminal empty of the parent grammar.</param>
        /// <param name="makeStarRuleOfParentGrammar">MakeStarRule function of the parent grammar.</param>
        public TemporalStreamGrammar(Terminal empty, Func<NonTerminal, BnfTerm, BnfExpression> makeStarRuleOfParentGrammar)
            : base(false)
        {
            this.expressionGrammar = new ExpressionGrammar();
            this.terminalEmpty = empty;
            this.makeStarRuleOfParentGrammar = makeStarRuleOfParentGrammar;
            this.CreateGrammar();
        }

        /// <summary>
        /// Gets the non-terminal for a temporal stream.
        /// </summary>
        public NonTerminal TemporalStream
        {
            get
            {
                return this.temporalStream;
            }
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
            KeyTerm terminalRepetition = ToTerm("repetition", "repetition");
            KeyTerm terminalDuration = ToTerm("duration", "duration");
            KeyTerm terminalInto = ToTerm("into", "into");

            // Marcamos los terminales, definidos hasta el momento, como palabras reservadas
            this.MarkReservedWords(this.KeyTerms.Keys.ToArray());
            
            /* SIMBOLOS */
            KeyTerm terminalComa = ToTerm(",", "coma");
            KeyTerm terminalPunto = ToTerm(".", "punto");
            this.MarkPunctuation(terminalPunto);

            /* CONSTANTES E IDENTIFICADORES */
            Terminal terminalDateTimeValue = new QuotedValueLiteral("datetimeValue", "'", TypeCode.String);
            terminalDateTimeValue.AstConfig.NodeType = null;
            terminalDateTimeValue.AstConfig.DefaultNodeCreator = () => new DateTimeOrTimespanNode();
            Terminal terminalNumero = TerminalFactory.CreateCSharpNumber("number");
            terminalNumero.AstConfig.NodeType = null;
            terminalNumero.AstConfig.DefaultNodeCreator = () => new NumberNode();
            NumberLiteral terminalUnsignedIntValue = new NumberLiteral("unsigned_int_value", NumberOptions.IntOnly);
            terminalUnsignedIntValue.AstConfig.NodeType = null;
            terminalUnsignedIntValue.AstConfig.DefaultNodeCreator = () => new NumberNode();

            IdentifierTerminal terminalId = new IdentifierTerminal("identifier", IdOptions.None);
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

            /* ID WITH PATH */
            NonTerminal nt_FOURTH_LEVEL_OBJECT_IDENTIFIER = new NonTerminal("FOURTH_LEVEL_OBJECT_IDENTIFIER", typeof(FourthLevelIdentifierPlanNodeASTNode));
            nt_FOURTH_LEVEL_OBJECT_IDENTIFIER.AstConfig.NodeType = null;
            nt_FOURTH_LEVEL_OBJECT_IDENTIFIER.AstConfig.DefaultNodeCreator = () => new FourthLevelIdentifierPlanNodeASTNode();

            NonTerminal nt_GROUP_BY_OP = new NonTerminal("GROUP_BY_OP", typeof(PassASTNode));
            nt_GROUP_BY_OP.AstConfig.NodeType = null;
            nt_GROUP_BY_OP.AstConfig.DefaultNodeCreator = () => new PassASTNode();
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

            NonTerminal nt_APPLY_EXTENSIONS = new NonTerminal("APPLY_EXTENSION", typeof(ApplyExtensionListASTNode<PassASTNode>));
            nt_APPLY_EXTENSIONS.AstConfig.NodeType = null;
            nt_APPLY_EXTENSIONS.AstConfig.DefaultNodeCreator = () => new ApplyExtensionListASTNode<PassASTNode>();
            NonTerminal nt_APPLY_EXTENSION = new NonTerminal("APPLY_EXTENSION", typeof(PassASTNode));
            nt_APPLY_EXTENSION.AstConfig.NodeType = null;
            nt_APPLY_EXTENSION.AstConfig.DefaultNodeCreator = () => new PassASTNode();
            NonTerminal nt_APPLY_DURATION = new NonTerminal("APPLY_DURATION", typeof(ApplyExtesnsionNode));
            nt_APPLY_DURATION.AstConfig.NodeType = null;
            nt_APPLY_DURATION.AstConfig.DefaultNodeCreator = () => new ApplyExtesnsionNode();
            NonTerminal nt_APPLY_REPETITION = new NonTerminal("APPLY_REPETITION", typeof(ApplyExtesnsionNode));
            nt_APPLY_REPETITION.AstConfig.NodeType = null;
            nt_APPLY_REPETITION.AstConfig.DefaultNodeCreator = () => new ApplyExtesnsionNode();

            NonTerminal nt_ORDER_BY = new NonTerminal("ORDER_BY", typeof(OrderByNode));
            nt_ORDER_BY.AstConfig.NodeType = null;
            nt_ORDER_BY.AstConfig.DefaultNodeCreator = () => new OrderByNode();
            NonTerminal nt_LIST_OF_VALUES_FOR_ORDER_BY = new NonTerminal("LIST_OF_VALUES", typeof(ListNodeOrderBy));
            nt_LIST_OF_VALUES_FOR_ORDER_BY.AstConfig.NodeType = null;
            nt_LIST_OF_VALUES_FOR_ORDER_BY.AstConfig.DefaultNodeCreator = () => new ListNodeOrderBy();
            this.temporalStream = new NonTerminal("USER_QUERY", typeof(TemporalStreamASTNode));
            this.temporalStream.AstConfig.NodeType = null;
            this.temporalStream.AstConfig.DefaultNodeCreator = () => new TemporalStreamASTNode();            
            NonTerminal nt_ID_WITH_ALIAS = new NonTerminal("ID_WITH_ALIAS", typeof(ConstantValueWithAliasNode));
            nt_ID_WITH_ALIAS.AstConfig.NodeType = null;
            nt_ID_WITH_ALIAS.AstConfig.DefaultNodeCreator = () => new ConstantValueWithAliasNode();
            NonTerminal nt_JOIN = new NonTerminal("JOIN", typeof(JoinNode));
            nt_JOIN.AstConfig.NodeType = null;
            nt_JOIN.AstConfig.DefaultNodeCreator = () => new JoinNode();
            NonTerminal nt_ON = new NonTerminal("ON", typeof(OnNode));
            nt_ON.AstConfig.NodeType = null;
            nt_ON.AstConfig.DefaultNodeCreator = () => new OnNode();
            NonTerminal nt_SOURCE_DEFINITION = new NonTerminal("SOURCE_DEFINITION", typeof(PassASTNode));
            nt_SOURCE_DEFINITION.AstConfig.NodeType = null;
            nt_SOURCE_DEFINITION.AstConfig.DefaultNodeCreator = () => new PassASTNode();
            NonTerminal nt_TIMEOUT = new NonTerminal("TIMEOUT", typeof(TimeoutNode));
            nt_TIMEOUT.AstConfig.NodeType = null;
            nt_TIMEOUT.AstConfig.DefaultNodeCreator = () => new TimeoutNode();
            NonTerminal nt_ID_OR_ID_WITH_ALIAS = new NonTerminal("ID_OR_ID_WITH_ALIAS", typeof(ConstantValueWithOptionalAliasNode));
            nt_ID_OR_ID_WITH_ALIAS.AstConfig.NodeType = null;
            nt_ID_OR_ID_WITH_ALIAS.AstConfig.DefaultNodeCreator = () => new ConstantValueWithOptionalAliasNode();
            NonTerminal nt_JOIN_TYPE = new NonTerminal("JOIN_TYPE", typeof(PassASTNode));
            nt_JOIN_TYPE.AstConfig.NodeType = null;
            nt_JOIN_TYPE.AstConfig.DefaultNodeCreator = () => new PassASTNode();

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

            /* INTO */
            NonTerminal nt_INTO = new NonTerminal("INTO", typeof(IntoASTNode));
            nt_INTO.AstConfig.NodeType = null;
            nt_INTO.AstConfig.DefaultNodeCreator = () => new IntoASTNode();

            /* USER QUERY */
            this.temporalStream.Rule = nt_SOURCE_DEFINITION + nt_WHERE + nt_SELECT + nt_APPLY_EXTENSIONS + nt_INTO
                                    | nt_SOURCE_DEFINITION + nt_WHERE + nt_APPLY_WINDOW + nt_GROUP_BY_OP + nt_SELECT + nt_ORDER_BY + nt_APPLY_EXTENSIONS + nt_INTO;
            /* **************************** */
            /* APPLY WINDOW */
            nt_APPLY_WINDOW.Rule = terminalApply + terminalWindow + terminalOf + terminalDateTimeValue;
            /* **************************** */
            /* APPLY DURATION */
            nt_APPLY_DURATION.Rule = terminalApply + terminalDuration + terminalOf + terminalDateTimeValue;
            /* **************************** */
            /* APPLY REPETITION */
            nt_APPLY_REPETITION.Rule = terminalApply + terminalRepetition + terminalOf + terminalUnsignedIntValue;
            /* **************************** */
            /* APPLY REPETITION */
            nt_APPLY_EXTENSION.Rule = nt_APPLY_REPETITION
                                        | nt_APPLY_DURATION;
            
            nt_APPLY_EXTENSIONS.Rule = this.makeStarRuleOfParentGrammar.Invoke(nt_APPLY_EXTENSIONS, nt_APPLY_EXTENSION);
            /* **************************** */
            /* ORDER BY */
            nt_ORDER_BY.Rule = terminalOrder + terminalBy + nt_LIST_OF_VALUES_FOR_ORDER_BY
                                | terminalOrder + terminalBy + terminalAsc + nt_LIST_OF_VALUES_FOR_ORDER_BY
                                | terminalOrder + terminalBy + terminalDesc + nt_LIST_OF_VALUES_FOR_ORDER_BY
                                | this.terminalEmpty;

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
            nt_ID_OR_ID_WITH_ALIAS.Rule = nt_FOURTH_LEVEL_OBJECT_IDENTIFIER + terminalAs + terminalId
                                            | nt_FOURTH_LEVEL_OBJECT_IDENTIFIER;
            /* **************************** */
            /* JOIN */
            nt_JOIN.Rule = nt_JOIN_TYPE + nt_JOIN_SOURCE + nt_WHERE + nt_WITH + nt_WHERE + nt_ON + nt_TIMEOUT;
            /* **************************** */
            /* JOIN TYPE */
            nt_JOIN_TYPE.Rule = terminalLeft
                                | terminalRight
                                | terminalCross
                                | terminalInner
                                | this.terminalEmpty;
            /* **************************** */
            /* ON */
            nt_ON.Rule = terminalOn + nt_LOGIC_EXPRESSION_FOR_ON_CONDITION;
            /* **************************** */
            /* TIMEOUT */
            nt_TIMEOUT.Rule = terminalTimeout + terminalDateTimeValue;
            /* **************************** */
            /* WHERE */
            nt_WHERE.Rule = terminalWhere + nt_LOGIC_EXPRESSION
                            | this.terminalEmpty;
            /* **************************** */
            /* OPTIONAL GROUP BY */
            nt_GROUP_BY_OP.Rule = nt_GROUP_BY
                                    | this.terminalEmpty;
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
            /* **************************** */

            /* SYSTEM OBJECT IDENTIFIER */
            nt_FOURTH_LEVEL_OBJECT_IDENTIFIER.Rule = terminalId + terminalPunto + terminalId + terminalPunto + terminalId
                                                    | terminalId + terminalPunto + terminalId
                                                    | terminalId;
            /* **************************** */

            /* INTO */
            nt_INTO.Rule = terminalInto + nt_FOURTH_LEVEL_OBJECT_IDENTIFIER
                            | this.terminalEmpty;
            /* **************************** */

            this.Root = this.temporalStream;

            this.LanguageFlags = Irony.Parsing.LanguageFlags.CreateAst;
        }
    }
}
