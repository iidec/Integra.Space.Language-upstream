//-----------------------------------------------------------------------
// <copyright file="QueryGrammarForMetadata.cs" company="Integra.Space.Language">
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
    using ASTNodes.MetadataQuery;
    using ASTNodes.QuerySections;
    using Irony.Interpreter;
    using Irony.Parsing;

    /// <summary>
    /// EQLGrammar grammar for the commands and the predicates 
    /// </summary>
    [Language("QueryGrammarForMetadata", "0.4", "")]
    internal sealed class QueryGrammarForMetadata : InterpretedLanguageGrammar
    {
        /// <summary>
        /// Expression grammar
        /// </summary>
        private ExpressionGrammarForMetadata expressionGrammar;

        /// <summary>
        /// Query for metadata.
        /// </summary>
        private NonTerminal queryForMetadata;

        /// <summary>
        /// Terminal empty of the parent grammar.
        /// </summary>
        private Terminal terminalEmpty;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="QueryGrammarForMetadata"/> class
        /// </summary>
        public QueryGrammarForMetadata()
            : base(false)
        {
            this.expressionGrammar = new ExpressionGrammarForMetadata();
            this.terminalEmpty = this.Empty;
            this.CreateGrammar();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryGrammarForMetadata"/> class
        /// </summary>
        /// <param name="empty">Terminal empty of the parent grammar.</param>
        public QueryGrammarForMetadata(Terminal empty)
            : base(false)
        {
            this.expressionGrammar = new ExpressionGrammarForMetadata();
            this.terminalEmpty = empty;
            this.CreateGrammar();
        }

        /// <summary>
        /// Gets the non-terminal for query the metadata.
        /// </summary>
        public NonTerminal QueryForMetadata
        {
            get
            {
                return this.queryForMetadata;
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
            KeyTerm terminalOrder = ToTerm("order", "order");
            KeyTerm terminalAsc = ToTerm("asc", "asc");
            KeyTerm terminalDesc = ToTerm("desc", "desc");
            KeyTerm terminalBy = ToTerm("by", "by");
            KeyTerm terminalAs = ToTerm("as", "as");
            KeyTerm terminalSelect = ToTerm("select", "select");
            KeyTerm terminalTop = ToTerm("top", "top");
            KeyTerm terminalSys = ToTerm("sys", "sys");

            // Marcamos los terminales, definidos hasta el momento, como palabras reservadas
            this.MarkReservedWords(this.KeyTerms.Keys.ToArray());

            /* SIMBOLOS */
            KeyTerm terminalComa = ToTerm(",", "coma");
            KeyTerm terminalPunto = ToTerm(".", "punto");

            /* CONSTANTES E IDENTIFICADORES */
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
            
            NonTerminal nt_WHERE = new NonTerminal("WHERE", typeof(WhereNode));
            nt_WHERE.AstConfig.NodeType = null;
            nt_WHERE.AstConfig.DefaultNodeCreator = () => new WhereNode();
            NonTerminal nt_FROM = new NonTerminal("FROM", typeof(SourceForMetadataASTNode));
            nt_FROM.AstConfig.NodeType = null;
            nt_FROM.AstConfig.DefaultNodeCreator = () => new SourceForMetadataASTNode(0);
            NonTerminal nt_ORDER_BY = new NonTerminal("ORDER_BY", typeof(OrderByNode));
            nt_ORDER_BY.AstConfig.NodeType = null;
            nt_ORDER_BY.AstConfig.DefaultNodeCreator = () => new OrderByNode();
            NonTerminal nt_LIST_OF_VALUES_FOR_ORDER_BY = new NonTerminal("LIST_OF_VALUES", typeof(ListNodeOrderBy));
            nt_LIST_OF_VALUES_FOR_ORDER_BY.AstConfig.NodeType = null;
            nt_LIST_OF_VALUES_FOR_ORDER_BY.AstConfig.DefaultNodeCreator = () => new ListNodeOrderBy();
            this.queryForMetadata = new NonTerminal("METADATA_QUERY", typeof(QueryForMetadataASTNode));
            this.queryForMetadata.AstConfig.NodeType = null;
            this.queryForMetadata.AstConfig.DefaultNodeCreator = () => new QueryForMetadataASTNode();
            NonTerminal nt_SOURCE_DEFINITION = new NonTerminal("SOURCE_DEFINITION", typeof(PassNode));
            nt_SOURCE_DEFINITION.AstConfig.NodeType = null;
            nt_SOURCE_DEFINITION.AstConfig.DefaultNodeCreator = () => new PassNode();
            NonTerminal nt_ID_OR_ID_WITH_ALIAS = new NonTerminal("ID_OR_ID_WITH_ALIAS", typeof(ConstantValueWithOptionalAliasNode));
            nt_ID_OR_ID_WITH_ALIAS.AstConfig.NodeType = null;
            nt_ID_OR_ID_WITH_ALIAS.AstConfig.DefaultNodeCreator = () => new ConstantValueWithOptionalAliasNode();
            
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
            this.queryForMetadata.Rule = nt_SOURCE_DEFINITION + nt_WHERE + nt_SELECT + nt_ORDER_BY;
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
            nt_SOURCE_DEFINITION.Rule = nt_FROM;
            /* **************************** */
            /* FROM */
            nt_FROM.Rule = terminalFrom + terminalSys + terminalPunto + nt_ID_OR_ID_WITH_ALIAS;
            /* **************************** */
            /* ID OR ID WITH ALIAS */
            nt_ID_OR_ID_WITH_ALIAS.Rule = terminalId + terminalAs + terminalId
                                            | terminalId;
            /* **************************** */
            /* WHERE */
            nt_WHERE.Rule = terminalWhere + nt_LOGIC_EXPRESSION
                            | this.terminalEmpty;
            /* **************************** */

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
            
            this.Root = this.queryForMetadata;

            this.LanguageFlags = Irony.Parsing.LanguageFlags.CreateAst;
        }
    }
}
