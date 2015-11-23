//-----------------------------------------------------------------------
// <copyright file="EQLGrammar.cs" company="Integra.Space.Language">
//     Copyright (c) Integra.Space.Language. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Language.Grammars
{
    using System;
    using System.Linq;
    using ASTNodes.Commands.Create;
    using ASTNodes.Commands.Drop;
    using ASTNodes.Commands.General;
    using ASTNodes.Commands.Publish;
    using ASTNodes.Commands.Receive;
    using ASTNodes.Commands.StartStop;
    using ASTNodes.Commands.Trace;
    using ASTNodes.Constants;
    using ASTNodes.Lists;
    using ASTNodes.Permissions;
    using ASTNodes.QuerySections;
    using ASTNodes.Root;
    using ASTNodes.SystemViews;
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
            this.Grammar(false);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EQLGrammar"/> class
        /// </summary>
        /// <param name="prueba">flag for tests</param>
        public EQLGrammar(bool prueba)
            : base(false)
        {
            this.expressionGrammar = new ExpressionGrammar();
            this.projectionGrammar = new ProjectionGrammar();
            this.Grammar(prueba);
        }

        /// <summary>
        /// Specify the language grammar
        /// </summary>
        /// <param name="prueba">flag for tests</param>
        public void Grammar(bool prueba)
        {
            /*** TERMINALES DE LA GRAMATICA ***/

            /* PALABRAS RESERVADAS */
            KeyTerm terminalPublish = ToTerm("publish", "publish");
            KeyTerm terminalRecive = ToTerm("receive", "receive");
            KeyTerm terminalSelect = ToTerm("select", "select");
            KeyTerm terminalFrom = ToTerm("from", "from");
            KeyTerm terminalWhere = ToTerm("where", "where");
            KeyTerm terminalAs = ToTerm("as", "as");
            KeyTerm terminalStream = ToTerm("stream", "stream");
            KeyTerm terminalSource = ToTerm("source", "source");
            KeyTerm terminalJoin = ToTerm("join", "join");
            KeyTerm terminalWith = ToTerm("with", "with");
            KeyTerm terminalOn = ToTerm("on", "on");
            KeyTerm terminalApply = ToTerm("apply", "apply");
            KeyTerm terminalWindow = ToTerm("window", "window");
            KeyTerm terminalOf = ToTerm("of", "of");
            KeyTerm terminalGroup = ToTerm("group", "group");
            KeyTerm terminalOrder = ToTerm("order", "order");
            KeyTerm terminalAsc = ToTerm("asc", "asc");
            KeyTerm terminalDesc = ToTerm("desc", "desc");
            KeyTerm terminalBy = ToTerm("by", "by");
            KeyTerm terminalTo = ToTerm("to", "to");
            KeyTerm terminalRole = ToTerm("role", "role");
            KeyTerm terminalUser = ToTerm("user", "user");
            KeyTerm terminalPassword = ToTerm("password", "password");
            KeyTerm terminalStatus = ToTerm("status", "status");
            KeyTerm terminalDrop = ToTerm("drop", "drop");
            KeyTerm terminalGrant = ToTerm("grant", "grant");
            KeyTerm terminalRevoke = ToTerm("revoke", "revoke");
            KeyTerm terminalDeny = ToTerm("deny", "deny");
            KeyTerm terminalServer = ToTerm("server", "server");
            KeyTerm terminalStart = ToTerm("start", "start");
            KeyTerm terminalStop = ToTerm("stop", "stop");
            KeyTerm terminalTrace = ToTerm("trace", "trace");
            KeyTerm terminalSet = ToTerm("set", "set");
            KeyTerm terminalLevel = ToTerm("level", "level");
            KeyTerm terminalEngine = ToTerm("engine", "engine");
            KeyTerm terminalAdd = ToTerm("add", "add");
            KeyTerm terminalNext = ToTerm("next", "next");
            
            /* IDENTIFICADORES RESERVADOS */
            KeyTerm terminalConnectRole = ToTerm("ConnectRole", "ConnectRole");
            KeyTerm terminalSystemAdminRole = ToTerm("SystemAdminRole", "SystemAdminRole");
            KeyTerm terminalSourceAdminRole = ToTerm("SourceAdminRole", "SourceAdminRole");
            KeyTerm terminalStreamAdminRole = ToTerm("StreamAdminRole", "StreamAdminRole");
            KeyTerm terminalSystemReaderRole = ToTerm("SystemReaderRole", "SystemReaderRole");
            KeyTerm terminalSourceReaderRole = ToTerm("SourceReaderRole", "SourceReaderRole");
            KeyTerm terminalStreamReaderRole = ToTerm("StreamReaderRole", "StreamReaderRole");
                        
            /* VISTAS */
            KeyTerm terminalSources = ToTerm("System.Sources", "System.Sources");
            KeyTerm terminalStreams = ToTerm("System.Streams", "System.Streams");
            KeyTerm terminalPList = ToTerm("System.PList", "System.PList");
            KeyTerm terminalConditions = ToTerm("System.Conditions", "System.Conditions");
            KeyTerm terminalDependcies = ToTerm("System.Dependencies", "System.Dependencies");
            KeyTerm terminalSourceTrace = ToTerm("System.SourceTrace", "System.SourceTrace");
            KeyTerm terminalStreamTrace = ToTerm("System.StreamTrace", "System.StreamTrace");
            KeyTerm terminalEngineTrace = ToTerm("System.EngineTrace", "System.EngineTrace");
            KeyTerm terminalRoles = ToTerm("System.Roles", "System.Roles");
            KeyTerm terminalUsers = ToTerm("System.Users", "System.Users");
            KeyTerm terminalRoleMembers = ToTerm("System.RoleMembers", "System.RoleMembers");
            KeyTerm terminalPermissions = ToTerm("System.Permissions", "System.Permissions");
            
            /* VISTAS DEL SISTEMA */
            RegexBasedTerminal terminalUnidadDeTexto = new RegexBasedTerminal("texto", @"((?!where)|(?!from)|(?!select)).+");
            terminalUnidadDeTexto.AstConfig.NodeType = null;
            terminalUnidadDeTexto.AstConfig.DefaultNodeCreator = () => new StringNode();

            /* EVENTOS */
            KeyTerm terminalEvent = ToTerm("event", "event");

            /* OPERADORES LOGICOS */
            KeyTerm terminalAnd = ToTerm("and", "and");
            KeyTerm terminalOr = ToTerm("or", "or");

            // Marcamos los terminales, definidos hasta el momento, como palabras reservadas
            this.MarkReservedWords(this.KeyTerms.Keys.ToArray());

            /* SIMBOLOS */
            KeyTerm terminalParentesisIz = ToTerm("(", "parentesisIzEQLGrammar");
            KeyTerm terminalParentesisDer = ToTerm(")", "parentesisDerEQLGrammar");
            KeyTerm terminalComa = ToTerm(",", "coma");
            KeyTerm terminalIgual = ToTerm("=", "igual");

            /* CONSTANTES E IDENTIFICADORES */
            Terminal terminalNumero = TerminalFactory.CreateCSharpNumber("numero");
            terminalNumero.AstConfig.NodeType = null;
            terminalNumero.AstConfig.DefaultNodeCreator = () => new NumberNode();
            Terminal terminalCadena = TerminalFactory.CreateCSharpString("cadena");
            terminalCadena.AstConfig.NodeType = null;
            terminalCadena.AstConfig.DefaultNodeCreator = () => new StringNode();
            ConstantTerminal terminalBool = new ConstantTerminal("constanteBool");
            terminalBool.Add("true", true);
            terminalBool.Add("false", false);
            terminalBool.AstConfig.NodeType = null;
            terminalBool.AstConfig.DefaultNodeCreator = () => new BooleanNode();
            Terminal terminalDateTimeValue = new QuotedValueLiteral("datetimeValue", "'", TypeCode.String);
            terminalDateTimeValue.AstConfig.NodeType = null;
            terminalDateTimeValue.AstConfig.DefaultNodeCreator = () => new DateTimeOrTimespanNode();
            ConstantTerminal terminalNull = new ConstantTerminal("constanteNull");
            terminalNull.Add("null", null);
            terminalNull.AstConfig.NodeType = null;
            terminalNull.AstConfig.DefaultNodeCreator = () => new NullValueNode();
            ConstantTerminal terminalUserStatus = new ConstantTerminal("terminalUserStatus");
            terminalUserStatus.Add("enable", "enable");
            terminalUserStatus.Add("disable", "disable");
            terminalUserStatus.AstConfig.NodeType = null;
            terminalUserStatus.AstConfig.DefaultNodeCreator = () => new ConstantStringValueNode();
            ConstantTerminal terminalCreateAlter = new ConstantTerminal("terminalCreateAlter");
            terminalCreateAlter.Add("create", "create");
            terminalCreateAlter.Add("alter", "alter");
            terminalCreateAlter.AstConfig.NodeType = null;
            terminalCreateAlter.AstConfig.DefaultNodeCreator = () => new ConstantStringValueNode();

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
            NonTerminal nt_JOIN = new NonTerminal("JOIN", typeof(JoinNode));
            nt_JOIN.AstConfig.NodeType = null;
            nt_JOIN.AstConfig.DefaultNodeCreator = () => new JoinNode();
            NonTerminal nt_ON = new NonTerminal("ON", typeof(OnConditionNode));
            nt_ON.AstConfig.NodeType = null;
            nt_ON.AstConfig.DefaultNodeCreator = () => new OnConditionNode();
            NonTerminal nt_WITH = new NonTerminal("WITH", typeof(WithNode));
            nt_WITH.AstConfig.NodeType = null;
            nt_WITH.AstConfig.DefaultNodeCreator = () => new WithNode();
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

            NonTerminal nt_PARAMETER = new NonTerminal("PARAMETER", typeof(ParameterNode));
            nt_PARAMETER.AstConfig.NodeType = null;
            nt_PARAMETER.AstConfig.DefaultNodeCreator = () => new ParameterNode();
            NonTerminal nt_PARAMETER_LIST = new NonTerminal("PARAMETER_LIST", typeof(PlanNodeListNode));
            nt_PARAMETER_LIST.AstConfig.NodeType = null;
            nt_PARAMETER_LIST.AstConfig.DefaultNodeCreator = () => new ParameterListNode();

            NonTerminal nt_CREATE_STREAM = new NonTerminal("CREATE_STREAM", typeof(CreateStreamNode));
            nt_CREATE_STREAM.AstConfig.NodeType = null;
            nt_CREATE_STREAM.AstConfig.DefaultNodeCreator = () => new CreateStreamNode();
            NonTerminal nt_CREATE_SOURCE = new NonTerminal("CREATE_SOURCE", typeof(AddSourceNode));
            nt_CREATE_SOURCE.AstConfig.NodeType = null;
            nt_CREATE_SOURCE.AstConfig.DefaultNodeCreator = () => new AddSourceNode();
            NonTerminal nt_CREATE_ROLE = new NonTerminal("CREATE_ROLE", typeof(CreateRole));
            nt_CREATE_ROLE.AstConfig.NodeType = null;
            nt_CREATE_ROLE.AstConfig.DefaultNodeCreator = () => new CreateRole();
            NonTerminal nt_CREATE_USER = new NonTerminal("CREATE_USER", typeof(CreateUser));
            nt_CREATE_USER.AstConfig.NodeType = null;
            nt_CREATE_USER.AstConfig.DefaultNodeCreator = () => new CreateUser();
            NonTerminal nt_USER_DEFINED_OBJECTS = new NonTerminal("USER_DEFINED_OBJECTS", typeof(UserDefinedObjectsNode));
            nt_USER_DEFINED_OBJECTS.AstConfig.NodeType = null;
            nt_USER_DEFINED_OBJECTS.AstConfig.DefaultNodeCreator = () => new UserDefinedObjectsNode();
            NonTerminal nt_DROP = new NonTerminal("DROP", typeof(DropUserDefinedObjectNode));
            nt_DROP.AstConfig.NodeType = null;
            nt_DROP.AstConfig.DefaultNodeCreator = () => new DropUserDefinedObjectNode();
            NonTerminal nt_PERMISSIONS = new NonTerminal("PERMISSIONS", typeof(PermissionsNode));
            nt_PERMISSIONS.AstConfig.NodeType = null;
            nt_PERMISSIONS.AstConfig.DefaultNodeCreator = () => new PermissionsNode();
            NonTerminal nt_USER_OR_ROLE = new NonTerminal("PERMISSIONS_BODY", typeof(PermissionsBodyNode));
            nt_USER_OR_ROLE.AstConfig.NodeType = null;
            nt_USER_OR_ROLE.AstConfig.DefaultNodeCreator = () => new PermissionsBodyNode();
            NonTerminal nt_SECURE_OBJECTS_TYPE = new NonTerminal("SECURE_OBJECTS_TYPE", typeof(SecureObjectsTypeNode));
            nt_SECURE_OBJECTS_TYPE.AstConfig.NodeType = null;
            nt_SECURE_OBJECTS_TYPE.AstConfig.DefaultNodeCreator = () => new SecureObjectsTypeNode();
            NonTerminal nt_SECURE_OBJECTS = new NonTerminal("SECURE_OBJECTS", typeof(SecureObjectsNode));
            nt_SECURE_OBJECTS.AstConfig.NodeType = null;
            nt_SECURE_OBJECTS.AstConfig.DefaultNodeCreator = () => new SecureObjectsNode();
            NonTerminal nt_OBJECTS_TO_START_OR_STOP = new NonTerminal("OBJECTS_TO_START_OR_STOP", typeof(ObjectToStartOrStopNode));
            nt_OBJECTS_TO_START_OR_STOP.AstConfig.NodeType = null;
            nt_OBJECTS_TO_START_OR_STOP.AstConfig.DefaultNodeCreator = () => new ObjectToStartOrStopNode();
            NonTerminal nt_START_STOP = new NonTerminal("START_STOP", typeof(StartStopNode));
            nt_START_STOP.AstConfig.NodeType = null;
            nt_START_STOP.AstConfig.DefaultNodeCreator = () => new StartStopNode();
            NonTerminal nt_OBJECTS_TO_TRACE = new NonTerminal("OBJECTS_TO_TRACE", typeof(ObjectToStartOrStopNode));
            nt_OBJECTS_TO_TRACE.AstConfig.NodeType = null;
            nt_OBJECTS_TO_TRACE.AstConfig.DefaultNodeCreator = () => new ObjectToStartOrStopNode();
            NonTerminal nt_SET_TRACE = new NonTerminal("SET_TRACE", typeof(SetTraceNode));
            nt_SET_TRACE.AstConfig.NodeType = null;
            nt_SET_TRACE.AstConfig.DefaultNodeCreator = () => new SetTraceNode();

            NonTerminal nt_PUBLISH = new NonTerminal("PUBLISH", typeof(PublishNode));
            nt_PUBLISH.AstConfig.NodeType = null;
            nt_PUBLISH.AstConfig.DefaultNodeCreator = () => new PublishNode();
            NonTerminal nt_RECEIVE = new NonTerminal("RECEIVE", typeof(ReceiveNode));
            nt_RECEIVE.AstConfig.NodeType = null;
            nt_RECEIVE.AstConfig.DefaultNodeCreator = () => new ReceiveNode();
            NonTerminal nt_USER_QUERY = new NonTerminal("USER_QUERY", typeof(UserQueryNode));
            nt_USER_QUERY.AstConfig.NodeType = null;
            nt_USER_QUERY.AstConfig.DefaultNodeCreator = () => new UserQueryNode();
            NonTerminal nt_SYSTEM_VIEW = new NonTerminal("SYSTEM_VIEW", typeof(SystemViewNode));
            nt_SYSTEM_VIEW.AstConfig.NodeType = null;
            nt_SYSTEM_VIEW.AstConfig.DefaultNodeCreator = () => new SystemViewNode();
            NonTerminal nt_VIEWS = new NonTerminal("VIEWS", typeof(StringNode));
            nt_VIEWS.AstConfig.NodeType = null;
            nt_VIEWS.AstConfig.DefaultNodeCreator = () => new StringNode();

            NonTerminal nt_COMMAND_NODE = new NonTerminal("COMMAND_NODE", typeof(CommandNode));
            nt_COMMAND_NODE.AstConfig.NodeType = null;
            nt_COMMAND_NODE.AstConfig.DefaultNodeCreator = () => new CommandNode();
            NonTerminal nt_COMMAND_LIST = new NonTerminal("COMMAND_LIST", typeof(CommandListNode));
            nt_COMMAND_LIST.AstConfig.NodeType = null;
            nt_COMMAND_LIST.AstConfig.DefaultNodeCreator = () => new CommandListNode();

            /* SYSTEM VIEWS */
            nt_SYSTEM_VIEW.Rule = terminalSelect + terminalUnidadDeTexto + terminalFrom + terminalUnidadDeTexto + terminalWhere + terminalUnidadDeTexto
                                    | terminalSelect + terminalUnidadDeTexto + terminalFrom + terminalUnidadDeTexto;

            nt_VIEWS.Rule = terminalSources 
                            | terminalStreams 
                            | terminalPList 
                            | terminalConditions 
                            | terminalDependcies 
                            | terminalSourceTrace
                            | terminalStreamTrace
                            | terminalEngineTrace
                            | terminalRoles 
                            | terminalUsers 
                            | terminalRoleMembers
                            | terminalPermissions;

            /* **************************** */
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
            /* PUBLISH */
            nt_PUBLISH.Rule = terminalPublish + terminalEvent + terminalTo + terminalId;
            /* **************************** */
            /* RECEIVE */
            nt_RECEIVE.Rule = terminalRecive + terminalFrom + terminalId;
            /* **************************** */
            /* SET TRACE */
            nt_SET_TRACE.Rule = terminalSet + terminalTrace + terminalLevel + terminalNumero + terminalTo + nt_OBJECTS_TO_TRACE;

            nt_OBJECTS_TO_TRACE.Rule = terminalSource
                                                | terminalStream
                                                | terminalEngine
                                                | terminalId;
            /* **************************** */
            /* START o STOP */
            nt_START_STOP.Rule = terminalStart + nt_OBJECTS_TO_START_OR_STOP + terminalId
                                    | terminalStop + nt_OBJECTS_TO_START_OR_STOP + terminalId;

            nt_OBJECTS_TO_START_OR_STOP.Rule = terminalSource
                                                | terminalStream;
            /* **************************** */

            /* PERMISOS */
            nt_PERMISSIONS.Rule = terminalGrant + nt_SECURE_OBJECTS_TYPE + nt_SECURE_OBJECTS + terminalTo + nt_USER_OR_ROLE
                                    | terminalRevoke + nt_SECURE_OBJECTS_TYPE + nt_SECURE_OBJECTS + terminalTo + nt_USER_OR_ROLE
                                    | terminalDeny + nt_SECURE_OBJECTS_TYPE + nt_SECURE_OBJECTS + terminalTo + nt_USER_OR_ROLE;

            nt_USER_OR_ROLE.Rule = terminalUser + terminalId 
                                        | terminalRole + terminalId;

            nt_SECURE_OBJECTS_TYPE.Rule = terminalStream 
                                        | terminalRole 
                                        | terminalServer + terminalRole;

            nt_SECURE_OBJECTS.Rule = terminalConnectRole
                                     | terminalSystemAdminRole 
                                     | terminalSourceAdminRole 
                                     | terminalStreamAdminRole 
                                     | terminalSystemReaderRole
                                     | terminalSourceReaderRole
                                     | terminalStreamReaderRole;
                                     
            /* **************************** */

            /* DROP */
            nt_DROP.Rule = terminalDrop + nt_USER_DEFINED_OBJECTS + terminalId;

            nt_USER_DEFINED_OBJECTS.Rule = terminalSource
                                            | terminalStream
                                            | terminalRole
                                            | terminalUser;
            /* **************************** */

            /* EXPRESIONES CREATE */

            /* CREAR ROL */
            nt_CREATE_ROLE.Rule = this.ToTerm("create", "createWordAtRole") + terminalRole + terminalId;
            /* **************************** */
            /* CREAR USUARIO */
            nt_CREATE_USER.Rule = terminalCreateAlter + terminalUser + terminalId + terminalWith + terminalPassword + terminalIgual + terminalCadena + terminalComa + terminalStatus + terminalIgual + terminalUserStatus;
            /* **************************** */
            /* CREAR FUENTE */
            nt_CREATE_SOURCE.Rule = terminalAdd + terminalSource + terminalId;
            /* **************************** */
            /* CREAR FLUJO */
            nt_CREATE_STREAM.Rule = terminalCreateAlter + terminalStream + terminalId + terminalAs + nt_FROM + nt_WHERE + nt_SELECT
                                    | terminalCreateAlter + terminalStream + terminalId + terminalAs + nt_JOIN + nt_WITH + nt_ON + nt_APPLY_WINDOW + nt_WHERE + nt_SELECT;
            /* **************************** */

            /* **************************** */
            /* JOIN */
            nt_JOIN.Rule = terminalJoin + terminalId + terminalAs + terminalId;
            /* **************************** */
            /* WITH */
            nt_WITH.Rule = terminalWith + terminalId + terminalAs + terminalId;
            /* **************************** */
            /* ON */
            nt_ON.Rule = terminalOn + nt_LOGIC_EXPRESSION;
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
            /*nt_COMMAND_NODE.Rule = nt_CREATE_SOURCE
                                        | nt_CREATE_STREAM
                                        | nt_CREATE_ROLE
                                        | nt_CREATE_USER
                                        | nt_DROP
                                        | nt_PERMISSIONS
                                        | nt_START_STOP
                                        | nt_SET_TRACE
                                        | nt_PUBLISH
                                        | nt_RECEIVE
                                        | nt_SYSTEM_VIEW
                                        | nt_USER_QUERY;*/

            nt_COMMAND_NODE.Rule = nt_USER_QUERY;
            /* **************************** */
            /* LISTA DE COMANDOS */
            nt_COMMAND_LIST.Rule = nt_COMMAND_LIST + terminalNext + nt_COMMAND_NODE
                                    | nt_COMMAND_NODE;
            /* **************************** */

            if (prueba)
            {
                this.Root = nt_LOGIC_EXPRESSION;
            }
            else
            {
                this.Root = nt_COMMAND_LIST;
            }

            this.LanguageFlags = Irony.Parsing.LanguageFlags.CreateAst;
        }
    }
}
