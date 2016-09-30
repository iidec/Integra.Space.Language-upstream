//-----------------------------------------------------------------------
// <copyright file="CommandGrammar.cs" company="Integra.Space.Language">
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
    using ASTNodes.MetadataQuery;
    using ASTNodes.QuerySections;
    using ASTNodes.Root;
    using Common;
    using Irony.Interpreter;
    using Irony.Parsing;

    /// <summary>
    /// Command grammar class.
    /// </summary>
    [Language("CommandGrammar", "0.4", "")]
    internal class CommandGrammar : InterpretedLanguageGrammar
    {
        /// <summary>
        /// Expression grammar
        /// </summary>
        private QueryGrammarForMetadata queryGrammarForMetadata;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="CommandGrammar"/> class.
        /// </summary>
        public CommandGrammar() : base(false)
        {
            this.queryGrammarForMetadata = new QueryGrammarForMetadata();
            this.CreateGrammar();
        }

        /// <summary>
        /// Specify the command grammar.
        /// </summary>
        public void CreateGrammar()
        {
            /* ACTIONS */
            KeyTerm terminalCreate = ToTerm("create", "create");
            KeyTerm terminalDrop = ToTerm("drop", "drop");
            KeyTerm terminalAlter = ToTerm("alter", "alter");
            KeyTerm terminalStart = ToTerm("start", "start");
            KeyTerm terminalStop = ToTerm("stop", "stop");
            KeyTerm terminalGrant = ToTerm("grant", "grant");
            KeyTerm terminalRevoke = ToTerm("revoke", "revoke");
            KeyTerm terminalDeny = ToTerm("deny", "deny");
            KeyTerm terminalUse = ToTerm("use", "use");

            /* VIEWS COMMAND */
            KeyTerm terminalSelect = ToTerm("select", "select");
            KeyTerm terminalFrom = ToTerm("from", "from");
            KeyTerm terminalWhere = ToTerm("where", "where");

            /* SPACE OBJECTS */
            KeyTerm terminalServer = ToTerm("server", "server");
            KeyTerm terminalEndpoint = ToTerm("endpoint", "endpoint");
            KeyTerm terminalLogin = ToTerm("login", "login");
            KeyTerm terminalDatabase = ToTerm("database", "database");
            KeyTerm terminalRole = ToTerm("role", "role");
            KeyTerm terminalUser = ToTerm("user", "user");
            KeyTerm terminalSchema = ToTerm("schema", "schema");
            KeyTerm terminalSource = ToTerm("source", "source");
            KeyTerm terminalStream = ToTerm("stream", "stream");
            KeyTerm terminalView = ToTerm("view", "view");

            /* GRANULAR PERMISSIONS */
            KeyTerm terminalControl = ToTerm("control", "control");
            KeyTerm terminalDefinition = ToTerm("definition", "definition");
            KeyTerm terminalTake = ToTerm("take", "take");
            KeyTerm terminalOwnership = ToTerm("ownership", "ownership");
            KeyTerm terminalAny = ToTerm("any", "any");
            KeyTerm terminalReferences = ToTerm("references", "references");
            KeyTerm terminalConnect = ToTerm("connect", "connect");
            KeyTerm terminalAuthenticate = ToTerm("authenticate", "authenticate");
            KeyTerm terminalRead = ToTerm("read", "read");
            /* 
             Otras palabras reservadas para permisos que estan definidas en otras partes: create, alter, start, stop, view
             */

            /* GENERALES PARA COMANDOS */
            KeyTerm terminalWith = ToTerm("with", "with");
            KeyTerm terminalEqual = ToTerm("=", "equal");
            KeyTerm terminalOn = ToTerm("on", "on");
            KeyTerm terminalAuthorization = ToTerm("authorization", "authorization");

            /* OPCIONES DEL OBJETO LOGIN */
            KeyTerm terminalDefaultDatabase = ToTerm("default_database", "default_database");
            KeyTerm terminalPassword = ToTerm("password", "password");

            /* OPCIONES DEL OBJETO USUARIO */
            KeyTerm terminalDefaultSchema = ToTerm("default_schema", "default_schema");

            /* OPCIONES DEL OBJETO STREAM */
            KeyTerm terminalQuery = ToTerm("query", "query");

            /* PARA ASIGNACIÓN DE PERMISOS */
            KeyTerm terminalTo = ToTerm("to", "to");
            KeyTerm terminalOption = ToTerm("option", "option");

            /* PARA ASIGNACIÓN DE ROLES */
            KeyTerm terminalAdd = ToTerm("add", "add");

            /* PARA MODIFICAR NOMBRE DE OBJETOS */
            KeyTerm terminalModify = ToTerm("modify", "modify");
            KeyTerm terminalName = ToTerm("name", "name");

            /* IDENTIFICADOR */
            IdentifierTerminal terminalId = new IdentifierTerminal("identifier", IdOptions.None);

            /* SIMBOLOS */
            KeyTerm terminalComa = ToTerm(",", "coma");
            KeyTerm terminalPuntoYComa = ToTerm(";", "puntoYComa");
            this.MarkPunctuation(terminalComa, terminalPuntoYComa);

            /* QUERY */
            QuotedValueLiteral terminalQueryScript = new QuotedValueLiteral("terminalQuery", "{", "}", TypeCode.String);
            terminalQueryScript.AstConfig.NodeType = null;
            terminalQueryScript.AstConfig.DefaultNodeCreator = () => new IdentifierNode();

            /* CADENA */
            StringLiteral terminalCadena = new StringLiteral("cadena", "\"", StringOptions.AllowsAllEscapes);

            /* VALORES CONSTANTES */
            ConstantTerminal terminalUserOptionValue = new ConstantTerminal("userStatus", typeof(bool));
            terminalUserOptionValue.Add("enable", true);
            terminalUserOptionValue.Add("disable", false);
            terminalUserOptionValue.AstConfig.NodeType = null;
            terminalUserOptionValue.AstConfig.DefaultNodeCreator = () => new ValueASTNode<bool>();

            /* COMENTARIOS */
            CommentTerminal comentarioLinea = new CommentTerminal("line_coment", "//", "\n", "\r\n");
            CommentTerminal comentarioBloque = new CommentTerminal("block_coment", "/*", "*/");
            NonGrammarTerminals.Add(comentarioLinea);
            NonGrammarTerminals.Add(comentarioBloque);

            /* NON TERMINALS */

            NonTerminal nt_METADATA_QUERY = new NonTerminal("METADATA_QUERY", typeof(CommandQueryForMetadataASTNode));
            nt_METADATA_QUERY.AstConfig.NodeType = null;
            nt_METADATA_QUERY.AstConfig.DefaultNodeCreator = () => new CommandQueryForMetadataASTNode();

            NonTerminal nt_COMMAND_NODE = new NonTerminal("COMMAND", typeof(CommandNode));
            nt_COMMAND_NODE.AstConfig.NodeType = null;
            nt_COMMAND_NODE.AstConfig.DefaultNodeCreator = () => new CommandNode();
            NonTerminal nt_COMMAND_NODE_LIST = new NonTerminal("COMMAND_LIST", typeof(CommandListASTNode));
            nt_COMMAND_NODE_LIST.AstConfig.NodeType = null;
            nt_COMMAND_NODE_LIST.AstConfig.DefaultNodeCreator = () => new CommandListASTNode();

            /* ACTIONS */
            NonTerminal nt_STATUS_ACTIONS = new NonTerminal("STATUS_ACTIONS", typeof(SpaceActionASTNode));
            nt_STATUS_ACTIONS.AstConfig.NodeType = null;
            nt_STATUS_ACTIONS.AstConfig.DefaultNodeCreator = () => new SpaceActionASTNode();
            /************************************************/

            /* SPACE OBJECTS CATEGORIES */
            NonTerminal nt_SPACE_OBJECTS = new NonTerminal("SPACE_OBJECTS", typeof(SpaceObjectASTNode));
            nt_SPACE_OBJECTS.AstConfig.NodeType = null;
            nt_SPACE_OBJECTS.AstConfig.DefaultNodeCreator = () => new SpaceObjectASTNode();
            NonTerminal nt_OBJECTS_TO_ALTER = new NonTerminal("OBJECTS_TO_ALTER", typeof(SpaceObjectASTNode));
            nt_OBJECTS_TO_ALTER.AstConfig.NodeType = null;
            nt_OBJECTS_TO_ALTER.AstConfig.DefaultNodeCreator = () => new SpaceObjectASTNode();
            NonTerminal nt_OBJECTS_TO_TAKE_OWNERSHIP = new NonTerminal("OBJECTS_TO_TAKE_OWNERSHIP", typeof(SpaceObjectASTNode));
            nt_OBJECTS_TO_TAKE_OWNERSHIP.AstConfig.NodeType = null;
            nt_OBJECTS_TO_TAKE_OWNERSHIP.AstConfig.DefaultNodeCreator = () => new SpaceObjectASTNode();
            NonTerminal nt_SPACE_OBJECTS_FOR_STATUS_PERMISSIONS = new NonTerminal("SPACE_OBJECTS_FOR_STATUS_PERMISSIONS", typeof(SpaceObjectASTNode));
            nt_SPACE_OBJECTS_FOR_STATUS_PERMISSIONS.AstConfig.NodeType = null;
            nt_SPACE_OBJECTS_FOR_STATUS_PERMISSIONS.AstConfig.DefaultNodeCreator = () => new SpaceObjectASTNode();
            NonTerminal nt_SPACE_SERVER_PRINCIPALS = new NonTerminal("SPACE_SERVER_PRINCIPALS", typeof(SpaceObjectASTNode));
            nt_SPACE_SERVER_PRINCIPALS.AstConfig.NodeType = null;
            nt_SPACE_SERVER_PRINCIPALS.AstConfig.DefaultNodeCreator = () => new SpaceObjectASTNode();
            NonTerminal nt_SPACE_DB_PRINCIPALS = new NonTerminal("DB_SPACE_PRINCIPALS", typeof(SpaceObjectASTNode));
            nt_SPACE_DB_PRINCIPALS.AstConfig.NodeType = null;
            nt_SPACE_DB_PRINCIPALS.AstConfig.DefaultNodeCreator = () => new SpaceObjectASTNode();
            /************************************************/

            /* USE COMMAND */
            NonTerminal nt_USE = new NonTerminal("USE", typeof(UseASTNode));
            nt_USE.AstConfig.NodeType = null;
            nt_USE.AstConfig.DefaultNodeCreator = () => new UseASTNode();
            /************************************************/

            /* PERMISSIONS */
            NonTerminal nt_GRANULAR_PERMISSION = new NonTerminal("GRANULAR_PERMISSION", typeof(GranularPermissionASTNode));
            nt_GRANULAR_PERMISSION.AstConfig.NodeType = null;
            nt_GRANULAR_PERMISSION.AstConfig.DefaultNodeCreator = () => new GranularPermissionASTNode();
            NonTerminal nt_GRANULAR_PERMISSION_ANY = new NonTerminal("GRANULAR_PERMISSION_ANY", typeof(GranularPermissionASTNode));
            nt_GRANULAR_PERMISSION_ANY.AstConfig.NodeType = null;
            nt_GRANULAR_PERMISSION_ANY.AstConfig.DefaultNodeCreator = () => new GranularPermissionASTNode();
            NonTerminal nt_GRANULAR_PERMISSION_FOR_ON_1 = new NonTerminal("GRANULAR_PERMISSION_FOR_ON_1", typeof(GranularPermissionASTNode));
            nt_GRANULAR_PERMISSION_FOR_ON_1.AstConfig.NodeType = null;
            nt_GRANULAR_PERMISSION_FOR_ON_1.AstConfig.DefaultNodeCreator = () => new GranularPermissionASTNode();
            NonTerminal nt_GRANULAR_PERMISSION_FOR_ON_2 = new NonTerminal("GRANULAR_PERMISSION_FOR_ON_2", typeof(GranularPermissionASTNode));
            nt_GRANULAR_PERMISSION_FOR_ON_2.AstConfig.NodeType = null;
            nt_GRANULAR_PERMISSION_FOR_ON_2.AstConfig.DefaultNodeCreator = () => new GranularPermissionASTNode();
            NonTerminal nt_PERMISSION = new NonTerminal("PERMISSION", typeof(PermissionASTNode));
            nt_PERMISSION.AstConfig.NodeType = null;
            nt_PERMISSION.AstConfig.DefaultNodeCreator = () => new PermissionASTNode();
            /************************************************/
                        
            /* OBJECT WITH IDENTIFIER */
            NonTerminal nt_SPACE_OBJECTS_FOR_STATUS_PERMISSIONS_WITH_ID = new NonTerminal("SPACE_OBJECTS_FOR_STATUS_PERMISSIONS_WITH_ID", typeof(SpaceObjectWithIdASTNode));
            nt_SPACE_OBJECTS_FOR_STATUS_PERMISSIONS_WITH_ID.AstConfig.NodeType = null;
            nt_SPACE_OBJECTS_FOR_STATUS_PERMISSIONS_WITH_ID.AstConfig.DefaultNodeCreator = () => new SpaceObjectWithIdASTNode();
            NonTerminal nt_SPACE_SERVER_PRINCIPALS_WITH_ID = new NonTerminal("SPACE_USER_OR_ROLE_WITH_ID", typeof(SpaceObjectWithIdASTNode));
            nt_SPACE_SERVER_PRINCIPALS_WITH_ID.AstConfig.NodeType = null;
            nt_SPACE_SERVER_PRINCIPALS_WITH_ID.AstConfig.DefaultNodeCreator = () => new SpaceObjectWithIdASTNode();
            NonTerminal nt_SPACE_DB_PRINCIPALS_WITH_ID = new NonTerminal("SPACE_SOURCE_OR_ROLE_WITH_ID", typeof(SpaceObjectWithIdASTNode));
            nt_SPACE_DB_PRINCIPALS_WITH_ID.AstConfig.NodeType = null;
            nt_SPACE_DB_PRINCIPALS_WITH_ID.AstConfig.DefaultNodeCreator = () => new SpaceObjectWithIdASTNode();
            /************************************************/

            /* LIST OF IDENTIFIERS */
            NonTerminal nt_ID_LIST = new NonTerminal("ID_LIST", typeof(Irony.Interpreter.Ast.StatementListNode));
            nt_ID_LIST.AstConfig.NodeType = null;
            nt_ID_LIST.AstConfig.DefaultNodeCreator = () => new Irony.Interpreter.Ast.StatementListNode();
            /************************************************/

            /* PERMISSION OPTIONS */
            NonTerminal nt_PERMISSION_OPTION = new NonTerminal("PERMISSION_OPTION", typeof(PermissionOptionASTNode));
            nt_PERMISSION_OPTION.AstConfig.NodeType = null;
            nt_PERMISSION_OPTION.AstConfig.DefaultNodeCreator = () => new PermissionOptionASTNode();
            /************************************************/

            /* LOGIN OPTIONS */
            NonTerminal nt_LOGIN_OPTION_PASSWORD = new NonTerminal("SPACE_LOGIN_OPTION_PASSWORD", typeof(CommandOptionASTNode<LoginOptionEnum>));
            nt_LOGIN_OPTION_PASSWORD.AstConfig.NodeType = null;
            nt_LOGIN_OPTION_PASSWORD.AstConfig.DefaultNodeCreator = () => new CommandOptionASTNode<LoginOptionEnum>();
            NonTerminal nt_LOGIN_OPTION = new NonTerminal("SPACE_LOGIN_OPTION_DEFAULT_DB", typeof(CommandOptionASTNode<LoginOptionEnum>));
            nt_LOGIN_OPTION.AstConfig.NodeType = null;
            nt_LOGIN_OPTION.AstConfig.DefaultNodeCreator = () => new CommandOptionASTNode<LoginOptionEnum>();
            NonTerminal nt_LOGIN_OPTION_LIST = new NonTerminal("SPACE_LOGIN_OPTION_LIST", typeof(DictionaryCommandOptionASTNode<LoginOptionEnum>));
            nt_LOGIN_OPTION_LIST.AstConfig.NodeType = null;
            nt_LOGIN_OPTION_LIST.AstConfig.DefaultNodeCreator = () => new DictionaryCommandOptionASTNode<LoginOptionEnum>();
            NonTerminal nt_LOGIN_OPTION_LIST_AUX = new NonTerminal("PERMISSION_LIST", typeof(CommandOptionListASTNode<LoginOptionEnum>));
            nt_LOGIN_OPTION_LIST_AUX.AstConfig.NodeType = null;
            nt_LOGIN_OPTION_LIST_AUX.AstConfig.DefaultNodeCreator = () => new CommandOptionListASTNode<LoginOptionEnum>();
            /************************************************/

            /* USER OPTIONS */
            NonTerminal nt_SPACE_USER_OPTION = new NonTerminal("USER_OPTION", typeof(CommandOptionASTNode<UserOptionEnum>));
            nt_SPACE_USER_OPTION.AstConfig.NodeType = null;
            nt_SPACE_USER_OPTION.AstConfig.DefaultNodeCreator = () => new CommandOptionASTNode<UserOptionEnum>();
            NonTerminal nt_SPACE_USER_OPTION_LIST = new NonTerminal("USER_OPTION_LIST", typeof(DictionaryCommandOptionASTNode<UserOptionEnum>));
            nt_SPACE_USER_OPTION_LIST.AstConfig.NodeType = null;
            nt_SPACE_USER_OPTION_LIST.AstConfig.DefaultNodeCreator = () => new DictionaryCommandOptionASTNode<UserOptionEnum>();
            NonTerminal nt_USER_OPTION_LIST_AUX = new NonTerminal("PERMISSION_LIST", typeof(CommandOptionListASTNode<UserOptionEnum>));
            nt_USER_OPTION_LIST_AUX.AstConfig.NodeType = null;
            nt_USER_OPTION_LIST_AUX.AstConfig.DefaultNodeCreator = () => new CommandOptionListASTNode<UserOptionEnum>();
            /************************************************/

            /* STREAM OPTIONS */
            NonTerminal nt_STREAM_OPTION = new NonTerminal("STREAM_OPTION", typeof(CommandOptionASTNode<StreamOptionEnum>));
            nt_STREAM_OPTION.AstConfig.NodeType = null;
            nt_STREAM_OPTION.AstConfig.DefaultNodeCreator = () => new CommandOptionASTNode<StreamOptionEnum>();
            NonTerminal nt_STREAM_OPTION_LIST = new NonTerminal("STREAM_OPTION_LIST", typeof(DictionaryCommandOptionASTNode<StreamOptionEnum>));
            nt_STREAM_OPTION_LIST.AstConfig.NodeType = null;
            nt_STREAM_OPTION_LIST.AstConfig.DefaultNodeCreator = () => new DictionaryCommandOptionASTNode<StreamOptionEnum>();
            /************************************************/

            /* SPACE PERMISSION LIST */
            NonTerminal nt_SPACE_PERMISSION_LIST = new NonTerminal("SPACE_PERMISSION_LIST", typeof(ListASTNode<PermissionASTNode, PermissionNode>));
            nt_SPACE_PERMISSION_LIST.AstConfig.NodeType = null;
            nt_SPACE_PERMISSION_LIST.AstConfig.DefaultNodeCreator = () => new ListASTNode<PermissionASTNode, PermissionNode>();
            
            NonTerminal nt_SPACE_PRINCIPAL_LIST = new NonTerminal("SPACE_SERVER_PRINCIPAL_LIST", typeof(ListASTNode<SpaceObjectWithIdASTNode, CommandObject>));
            nt_SPACE_PRINCIPAL_LIST.AstConfig.NodeType = null;
            nt_SPACE_PRINCIPAL_LIST.AstConfig.DefaultNodeCreator = () => new ListASTNode<SpaceObjectWithIdASTNode, CommandObject>();
            /************************************************/

            /* CRUD COMMANDS */
            NonTerminal nt_DROP_COMMAND = new NonTerminal("DROP_COMMAND", typeof(DropCommandASTNode));
            nt_DROP_COMMAND.AstConfig.NodeType = null;
            nt_DROP_COMMAND.AstConfig.DefaultNodeCreator = () => new DropCommandASTNode();

            NonTerminal nt_CREATE_LOGIN = new NonTerminal("CREATE_LOGIN", typeof(CreateLoginASTNode));
            nt_CREATE_LOGIN.AstConfig.NodeType = null;
            nt_CREATE_LOGIN.AstConfig.DefaultNodeCreator = () => new CreateLoginASTNode();
            NonTerminal nt_CREATE_DATABASE = new NonTerminal("CREATE_DATABASE", typeof(CreateDatabaseASTNode));
            nt_CREATE_DATABASE.AstConfig.NodeType = null;
            nt_CREATE_DATABASE.AstConfig.DefaultNodeCreator = () => new CreateDatabaseASTNode();
            NonTerminal nt_CREATE_USER = new NonTerminal("CREATE_USER", typeof(CreateUserASTNode));
            nt_CREATE_USER.AstConfig.NodeType = null;
            nt_CREATE_USER.AstConfig.DefaultNodeCreator = () => new CreateUserASTNode();
            NonTerminal nt_CREATE_ROLE = new NonTerminal("CREATE_ROLE", typeof(CreateRoleASTNode));
            nt_CREATE_ROLE.AstConfig.NodeType = null;
            nt_CREATE_ROLE.AstConfig.DefaultNodeCreator = () => new CreateRoleASTNode();
            NonTerminal nt_CREATE_SOURCE = new NonTerminal("CREATE_SOURCE", typeof(CreateSourceASTNode));
            nt_CREATE_SOURCE.AstConfig.NodeType = null;
            nt_CREATE_SOURCE.AstConfig.DefaultNodeCreator = () => new CreateSourceASTNode();
            NonTerminal nt_CREATE_SCHEMA = new NonTerminal("CREATE_SCHEMA", typeof(CreateSchemaASTNode));
            nt_CREATE_SCHEMA.AstConfig.NodeType = null;
            nt_CREATE_SCHEMA.AstConfig.DefaultNodeCreator = () => new CreateSchemaASTNode();
            NonTerminal nt_CREATE_STREAM = new NonTerminal("CREATE_STREAM", typeof(CreateStreamASTNode));
            nt_CREATE_STREAM.AstConfig.NodeType = null;
            nt_CREATE_STREAM.AstConfig.DefaultNodeCreator = () => new CreateStreamASTNode();

            NonTerminal nt_ALTER_LOGIN = new NonTerminal("ALTER_LOGIN", typeof(AlterLoginASTNode));
            nt_ALTER_LOGIN.AstConfig.NodeType = null;
            nt_ALTER_LOGIN.AstConfig.DefaultNodeCreator = () => new AlterLoginASTNode();
            NonTerminal nt_ALTER_USER = new NonTerminal("ALTER_USER", typeof(AlterUserASTNode));
            nt_ALTER_USER.AstConfig.NodeType = null;
            nt_ALTER_USER.AstConfig.DefaultNodeCreator = () => new AlterUserASTNode();
            NonTerminal nt_ALTER_DATABASE = new NonTerminal("ALTER_DATABASE", typeof(AlterDatabaseASTNode));
            nt_ALTER_DATABASE.AstConfig.NodeType = null;
            nt_ALTER_DATABASE.AstConfig.DefaultNodeCreator = () => new AlterDatabaseASTNode();
            NonTerminal nt_ALTER_ROLE = new NonTerminal("ALTER_ROLE", typeof(AlterRoleASTNode));
            nt_ALTER_ROLE.AstConfig.NodeType = null;
            nt_ALTER_ROLE.AstConfig.DefaultNodeCreator = () => new AlterRoleASTNode();
            NonTerminal nt_ALTER_SCHEMA = new NonTerminal("ALTER_SCHEMA", typeof(AlterSchemaASTNode));
            nt_ALTER_SCHEMA.AstConfig.NodeType = null;
            nt_ALTER_SCHEMA.AstConfig.DefaultNodeCreator = () => new AlterSchemaASTNode();
            NonTerminal nt_ALTER_SOURCE = new NonTerminal("ALTER_SOURCE", typeof(AlterSourceASTNode));
            nt_ALTER_SOURCE.AstConfig.NodeType = null;
            nt_ALTER_SOURCE.AstConfig.DefaultNodeCreator = () => new AlterSourceASTNode();
            NonTerminal nt_ALTER_STREAM = new NonTerminal("ALTER_STREAM", typeof(AlterStreamASTNode));
            nt_ALTER_STREAM.AstConfig.NodeType = null;
            nt_ALTER_STREAM.AstConfig.DefaultNodeCreator = () => new AlterStreamASTNode();
            /************************************************/

            NonTerminal nt_STATUS_COMMANDS = new NonTerminal("STATUS_COMMANDS", typeof(StatusCommandASTNode));
            nt_STATUS_COMMANDS.AstConfig.NodeType = null;
            nt_STATUS_COMMANDS.AstConfig.DefaultNodeCreator = () => new StatusCommandASTNode();
            NonTerminal nt_PERMISSIONS_COMMANDS = new NonTerminal("PERMISSIONS_COMMANDS", typeof(PermissionCommandASTNode));
            nt_PERMISSIONS_COMMANDS.AstConfig.NodeType = null;
            nt_PERMISSIONS_COMMANDS.AstConfig.DefaultNodeCreator = () => new PermissionCommandASTNode();
            NonTerminal nt_ADD_USERS_TO_ROLE_COMMAND = new NonTerminal("ADD_USERS_TO_ROLE_COMMAND", typeof(AddCommandASTNode));
            nt_ADD_USERS_TO_ROLE_COMMAND.AstConfig.NodeType = null;
            nt_ADD_USERS_TO_ROLE_COMMAND.AstConfig.DefaultNodeCreator = () => new AddCommandASTNode();
            NonTerminal nt_TAKE_OWNERSHIP = new NonTerminal("TAKE_OWNERSHIP", typeof(TakeOwnershipASTNode));
            nt_TAKE_OWNERSHIP.AstConfig.NodeType = null;
            nt_TAKE_OWNERSHIP.AstConfig.DefaultNodeCreator = () => new TakeOwnershipASTNode();

            /* RULES */

            /* COMMAND ACTIONS */

            nt_STATUS_ACTIONS.Rule = terminalStart
                                    | terminalStop;
            
            /************************************************/

            /* SPACE OBJECTS CATEGORIES */

            nt_SPACE_OBJECTS.Rule = terminalServer
                                    | terminalEndpoint
                                    | terminalLogin
                                    | terminalDatabase
                                    | terminalUser
                                    | terminalRole
                                    | terminalSchema
                                    | terminalSource
                                    | terminalStream
                                    | terminalView;

            nt_OBJECTS_TO_ALTER.Rule = terminalSchema
                                        | terminalSource
                                        | terminalStream
                                        | terminalView
                                        | terminalUser
                                        | terminalRole
                                        | terminalDatabase
                                        | terminalLogin
                                        | terminalEndpoint;

            nt_OBJECTS_TO_TAKE_OWNERSHIP.Rule = terminalRole
                                                | terminalDatabase
                                                | terminalEndpoint
                                                | terminalSchema
                                                | terminalSource
                                                | terminalStream
                                                | terminalView;

            nt_SPACE_OBJECTS_FOR_STATUS_PERMISSIONS.Rule = terminalSource
                                                        | terminalStream;

            nt_SPACE_SERVER_PRINCIPALS.Rule = terminalLogin;

            nt_SPACE_DB_PRINCIPALS.Rule = terminalRole
                                        | terminalUser;
                        
            /************************************************/

            /* OBJECT WITH IDENTIFIER */
            
            nt_SPACE_OBJECTS_FOR_STATUS_PERMISSIONS_WITH_ID.Rule = nt_SPACE_OBJECTS_FOR_STATUS_PERMISSIONS + terminalId;

            nt_SPACE_SERVER_PRINCIPALS_WITH_ID.Rule = nt_SPACE_SERVER_PRINCIPALS + terminalId;

            nt_SPACE_DB_PRINCIPALS_WITH_ID.Rule = nt_SPACE_DB_PRINCIPALS + terminalId;

            /************************************************/

            /* PERMISSIONS */

            nt_PERMISSION.Rule = nt_GRANULAR_PERMISSION
                                | nt_GRANULAR_PERMISSION_FOR_ON_1 + terminalOn + nt_OBJECTS_TO_ALTER + terminalId
                                | nt_GRANULAR_PERMISSION_FOR_ON_2 + terminalOn + nt_SPACE_OBJECTS_FOR_STATUS_PERMISSIONS + terminalId
                                | nt_GRANULAR_PERMISSION_ANY;

            nt_GRANULAR_PERMISSION.Rule = terminalControl + terminalServer
                                            | terminalCreate + terminalView
                                            | terminalCreate + terminalSource
                                            | terminalCreate + terminalStream
                                            | terminalCreate + terminalSchema
                                            | terminalCreate + terminalRole
                                            | terminalCreate + terminalEndpoint
                                            | terminalCreate + terminalDatabase
                                            | terminalAuthenticate + terminalServer
                                            | terminalAuthenticate;
            
            nt_GRANULAR_PERMISSION_FOR_ON_1.Rule = terminalView + terminalDefinition
                                            | terminalTake + terminalOwnership
                                            | terminalControl
                                            | terminalConnect
                                            | terminalAlter;

            nt_GRANULAR_PERMISSION_FOR_ON_2.Rule = terminalRead
                                                    | terminalStop
                                                    | terminalStart;

            nt_GRANULAR_PERMISSION_ANY.Rule = terminalView + terminalAny + terminalDefinition
                                            | terminalView + terminalAny + terminalDatabase
                                            | terminalConnect + terminalAny + terminalDatabase
                                            | terminalCreate + terminalAny + terminalDatabase
                                            | terminalAlter + terminalAny + terminalUser
                                            | terminalAlter + terminalAny + terminalSchema
                                            | terminalAlter + terminalAny + terminalRole
                                            | terminalAlter + terminalAny + terminalLogin
                                            | terminalAlter + terminalAny + terminalEndpoint
                                            | terminalAlter + terminalAny + terminalDatabase;

            /************************************************/

            /* LIST OF PERMISSIONS AND PRINCIPALS */

            nt_SPACE_PERMISSION_LIST.Rule = this.MakePlusRule(nt_SPACE_PERMISSION_LIST, terminalComa, nt_PERMISSION);

            nt_SPACE_PRINCIPAL_LIST.Rule = this.MakePlusRule(nt_SPACE_PRINCIPAL_LIST, terminalComa, nt_SPACE_SERVER_PRINCIPALS_WITH_ID)
                                            | this.MakePlusRule(nt_SPACE_PRINCIPAL_LIST, terminalComa, nt_SPACE_DB_PRINCIPALS_WITH_ID);

            /************************************************/

            /* COMMANDS */

            /* USE */

            nt_USE.Rule = terminalUse + terminalId;
                        
            /************************************************/

            /* START & STOP */

            nt_STATUS_COMMANDS.Rule = nt_STATUS_ACTIONS + nt_SPACE_OBJECTS_FOR_STATUS_PERMISSIONS_WITH_ID;

            /************************************************/

            /* CRUD commands */

            /* ALTER */

            nt_ALTER_LOGIN.Rule = terminalAlter + terminalLogin + terminalId + terminalWith + nt_LOGIN_OPTION_LIST;

            nt_ALTER_USER.Rule = terminalAlter + terminalUser + terminalId + terminalWith + nt_SPACE_USER_OPTION_LIST;

            nt_ALTER_DATABASE.Rule = terminalAlter + terminalDatabase + terminalId + terminalWith + terminalName + terminalEqual + terminalId;

            nt_ALTER_ROLE.Rule = terminalAlter + terminalRole + terminalId + terminalWith + terminalName + terminalEqual + terminalId;

            nt_ALTER_SCHEMA.Rule = terminalAlter + terminalSchema + terminalId + terminalWith + terminalName + terminalEqual + terminalId;

            nt_ALTER_SOURCE.Rule = terminalAlter + terminalSource + terminalId + terminalWith + terminalName + terminalEqual + terminalId;

            nt_ALTER_STREAM.Rule = terminalAlter + terminalStream + terminalId + terminalWith + nt_STREAM_OPTION_LIST;
            nt_STREAM_OPTION_LIST.Rule = this.MakePlusRule(nt_STREAM_OPTION_LIST, terminalComa, nt_STREAM_OPTION);
            nt_STREAM_OPTION.Rule = terminalQuery + terminalEqual + terminalQueryScript
                                        | terminalName + terminalEqual + terminalId;

            /* DROP */

            nt_DROP_COMMAND.Rule = terminalDrop + nt_OBJECTS_TO_ALTER + nt_ID_LIST;

            nt_ID_LIST.Rule = this.MakePlusRule(nt_ID_LIST, terminalComa, terminalId);
            
            /* CREATE */

            nt_CREATE_SCHEMA.Rule = terminalCreate + terminalSchema + terminalId;

            nt_CREATE_STREAM.Rule = terminalCreate + terminalStream + terminalId + terminalQueryScript;

            nt_CREATE_ROLE.Rule = terminalCreate + terminalRole + terminalId;

            nt_CREATE_USER.Rule = terminalCreate + terminalUser + terminalId + nt_USER_OPTION_LIST_AUX;
            nt_USER_OPTION_LIST_AUX.Rule = terminalWith + nt_SPACE_USER_OPTION_LIST
                                            | this.Empty;
            nt_SPACE_USER_OPTION_LIST.Rule = this.MakePlusRule(nt_SPACE_USER_OPTION_LIST, terminalComa, nt_SPACE_USER_OPTION);
            nt_SPACE_USER_OPTION.Rule = terminalDefaultSchema + terminalEqual + terminalId
                                        | terminalName + terminalEqual + terminalId;

            nt_CREATE_LOGIN.Rule = terminalCreate + terminalLogin + terminalId + terminalWith + nt_LOGIN_OPTION_PASSWORD + nt_LOGIN_OPTION_LIST_AUX;
            nt_LOGIN_OPTION_LIST_AUX.Rule = terminalComa + nt_LOGIN_OPTION_LIST
                                            | this.Empty;
            nt_LOGIN_OPTION_PASSWORD.Rule = terminalPassword + terminalEqual + terminalCadena;
            nt_LOGIN_OPTION_LIST.Rule = this.MakePlusRule(nt_LOGIN_OPTION_LIST, terminalComa, nt_LOGIN_OPTION);
            nt_LOGIN_OPTION.Rule = terminalDefaultDatabase + terminalEqual + terminalId
                                        | terminalPassword + terminalEqual + terminalCadena
                                        | terminalName + terminalEqual + terminalId;

            nt_CREATE_DATABASE.Rule = terminalCreate + terminalDatabase + terminalId;

            nt_CREATE_SOURCE.Rule = terminalCreate + terminalSource + terminalId;

            /************************************************/

            /* Permission commands */

            nt_PERMISSION_OPTION.Rule = terminalWith + terminalGrant + terminalOption
                                        | this.Empty;

            nt_PERMISSIONS_COMMANDS.Rule = terminalGrant + nt_SPACE_PERMISSION_LIST + terminalTo + nt_SPACE_PRINCIPAL_LIST + nt_PERMISSION_OPTION
                                            | terminalDeny + nt_SPACE_PERMISSION_LIST + terminalTo + nt_SPACE_PRINCIPAL_LIST
                                            | terminalRevoke + nt_SPACE_PERMISSION_LIST + terminalTo + nt_SPACE_PRINCIPAL_LIST;

            /* Add USERS TO ROLES */

            nt_ADD_USERS_TO_ROLE_COMMAND.Rule = terminalAdd + nt_ID_LIST + terminalTo + nt_ID_LIST;

            /* TAKE OWNERSHIP */

            nt_TAKE_OWNERSHIP.Rule = terminalTake + terminalOwnership + terminalOn + nt_OBJECTS_TO_TAKE_OWNERSHIP + terminalId;

            /************************************************/

            /* QUERY METADATA */
            nt_METADATA_QUERY.Rule = this.CreateQueryForMetadataGrammar();
            /************************************************/

            nt_COMMAND_NODE.Rule = nt_STATUS_COMMANDS
                                    | nt_PERMISSIONS_COMMANDS
                                    | nt_ADD_USERS_TO_ROLE_COMMAND
                                    | nt_DROP_COMMAND
                                    | nt_CREATE_LOGIN
                                    | nt_CREATE_DATABASE
                                    | nt_CREATE_USER
                                    | nt_CREATE_ROLE
                                    | nt_CREATE_SCHEMA
                                    | nt_CREATE_SOURCE
                                    | nt_CREATE_STREAM
                                    | nt_ALTER_LOGIN
                                    | nt_ALTER_USER
                                    | nt_ALTER_DATABASE
                                    | nt_ALTER_ROLE
                                    | nt_ALTER_SCHEMA
                                    | nt_ALTER_SOURCE
                                    | nt_ALTER_STREAM
                                    | nt_USE
                                    | nt_TAKE_OWNERSHIP
                                    | nt_METADATA_QUERY;

            nt_COMMAND_NODE_LIST.Rule = this.MakePlusRule(nt_COMMAND_NODE_LIST, terminalPuntoYComa, nt_COMMAND_NODE);

            this.Root = nt_COMMAND_NODE_LIST;

            this.LanguageFlags = Irony.Parsing.LanguageFlags.CreateAst;
        }

        /// <summary>
        /// Creates the query for metadata grammar.
        /// </summary>
        /// <returns>Query for metadata root node.</returns>
        public NonTerminal CreateQueryForMetadataGrammar()
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

            // Marcamos los terminales, definidos hasta el momento, como palabras reservadas
            this.MarkReservedWords(this.KeyTerms.Keys.ToArray());

            /* SIMBOLOS */
            KeyTerm terminalComa = ToTerm(",", "coma");

            /* CONSTANTES E IDENTIFICADORES */
            Terminal terminalNumero = TerminalFactory.CreateCSharpNumber("number");
            terminalNumero.AstConfig.NodeType = null;
            terminalNumero.AstConfig.DefaultNodeCreator = () => new NumberNode();

            RegexBasedTerminal terminalId = new RegexBasedTerminal("[a-zA-Z]+([a-zA-Z]|[0-9]|[_])*");
            terminalId.Name = "identifier";
            terminalId.AstConfig.NodeType = null;
            terminalId.AstConfig.DefaultNodeCreator = () => new IdentifierNode();

            /* COMENTARIOS */
            CommentTerminal comentarioLinea = new CommentTerminal("line_coment", "//", "\n", "\r\n");
            CommentTerminal comentarioBloque = new CommentTerminal("block_coment", "/*", "*/");
            NonGrammarTerminals.Add(comentarioLinea);
            NonGrammarTerminals.Add(comentarioBloque);

            /* NO TERMINALES */
            ExpressionGrammarForMetadata expressionGrammar = new ExpressionGrammarForMetadata();
            NonTerminal nt_LOGIC_EXPRESSION = expressionGrammar.LogicExpression;

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
            NonTerminal nt_QUERY_FOR_METADATA = new NonTerminal("METADATA_QUERY", typeof(QueryForMetadataASTNode));
            nt_QUERY_FOR_METADATA.AstConfig.NodeType = null;
            nt_QUERY_FOR_METADATA.AstConfig.DefaultNodeCreator = () => new QueryForMetadataASTNode();
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
            nt_QUERY_FOR_METADATA.Rule = nt_SOURCE_DEFINITION + nt_WHERE + nt_SELECT + nt_ORDER_BY;
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
            nt_SOURCE_DEFINITION.Rule = nt_FROM;
            /* **************************** */
            /* FROM */
            nt_FROM.Rule = terminalFrom + nt_ID_OR_ID_WITH_ALIAS;
            /* **************************** */
            /* ID OR ID WITH ALIAS */
            nt_ID_OR_ID_WITH_ALIAS.Rule = terminalId + terminalAs + terminalId
                                            | terminalId;
            /* **************************** */
            /* WHERE */
            nt_WHERE.Rule = terminalWhere + nt_LOGIC_EXPRESSION
                            | this.Empty;
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
            nt_VALUES_WITH_ALIAS_FOR_PROJECTION.Rule = expressionGrammar.ProjectionValue + terminalAs + terminalId;

            return nt_QUERY_FOR_METADATA;
        }
    }
}
