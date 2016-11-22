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
        /// Initializes a new instance of the <see cref="CommandGrammar"/> class.
        /// </summary>
        public CommandGrammar() : base(false)
        {
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
            KeyTerm terminalTruncate = ToTerm("truncate", "truncate");

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
            KeyTerm terminalWrite = ToTerm("write", "write");
            /* 
             Otras palabras reservadas para permisos que estan definidas en otras partes: create, alter, start, stop, view
             */

            /* STATUS OPTION */
            KeyTerm terminalStatus = ToTerm("status", "status");
            ConstantTerminal terminalStatusValue = new ConstantTerminal("statusValue");
            terminalStatusValue.Add("on", true);
            terminalStatusValue.Add("off", false);
            terminalStatusValue.AstConfig.NodeType = null;
            terminalStatusValue.AstConfig.DefaultNodeCreator = () => new ValueASTNode<bool>();

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

            /* OPCIONES PARA EL OBJETO SOURCE */
            KeyTerm terminalCacheDurability = ToTerm("cache_durability", "cache_durability");
            KeyTerm terminalCacheSize = ToTerm("cache_size", "cache_size");
            KeyTerm terminalPersistent = ToTerm("persistent", "persistent");

            /* PARA ASIGNACIÓN DE PERMISOS */
            KeyTerm terminalTo = ToTerm("to", "to");
            KeyTerm terminalOption = ToTerm("option", "option");

            /* PARA AGREGAR O QUITAR USUARIOS DE ROLES */
            KeyTerm terminalAdd = ToTerm("add", "add");
            KeyTerm terminalRemove = ToTerm("remove", "remove");

            /* PARA MODIFICAR NOMBRE DE OBJETOS */
            KeyTerm terminalModify = ToTerm("modify", "modify");
            KeyTerm terminalName = ToTerm("name", "name");

            /* IDENTIFICADOR */
            IdentifierTerminal terminalId = new IdentifierTerminal("identifier", IdOptions.None);
            NumberLiteral terminalUnsignedIntValue = new NumberLiteral("unsigned_int_value", NumberOptions.IntOnly);

            /* SIMBOLOS */
            KeyTerm terminalComa = ToTerm(",", "coma");
            KeyTerm terminalPuntoYComa = ToTerm(";", "puntoYComa");
            KeyTerm terminalPunto = ToTerm(".", "punto");
            KeyTerm terminalParentesisIz = ToTerm("(", "parentesisIzquierdo");
            KeyTerm terminalParentesisDer = ToTerm(")", "parentesisDer");
            this.MarkPunctuation(terminalComa, terminalPuntoYComa, terminalPunto, terminalParentesisIz, terminalParentesisDer);

            /* QUERY */
            QuotedValueLiteral terminalQueryScript = new QuotedValueLiteral("terminalQuery", "{", "}", TypeCode.String);
            terminalQueryScript.AstConfig.NodeType = null;
            terminalQueryScript.AstConfig.DefaultNodeCreator = () => new IdentifierASTNode();

            /* CADENA */
            StringLiteral terminalCadena = new StringLiteral("cadena", "\"", StringOptions.AllowsAllEscapes);

            /* VALORES CONSTANTES */
            ConstantTerminal terminalUserOptionValue = new ConstantTerminal("userStatus", typeof(bool));
            terminalUserOptionValue.Add("enable", true);
            terminalUserOptionValue.Add("disable", false);
            terminalUserOptionValue.AstConfig.NodeType = null;
            terminalUserOptionValue.AstConfig.DefaultNodeCreator = () => new ValueASTNode<bool>();
            
            /* TIPOS */
            ConstantTerminal terminalType = new ConstantTerminal("dataTypes", typeof(Type));
            terminalType.Add("byte", typeof(byte));
            terminalType.Add("byte?", typeof(byte?));
            terminalType.Add("sbyte", typeof(sbyte));
            terminalType.Add("sbyte?", typeof(sbyte?));
            terminalType.Add("short", typeof(short));
            terminalType.Add("short?", typeof(short?));
            terminalType.Add("ushort", typeof(ushort));
            terminalType.Add("ushort?", typeof(ushort?));
            terminalType.Add("int", typeof(int));
            terminalType.Add("int?", typeof(int?));
            terminalType.Add("uint", typeof(uint));
            terminalType.Add("uint?", typeof(uint?));
            terminalType.Add("long", typeof(long));
            terminalType.Add("long?", typeof(long?));
            terminalType.Add("ulong", typeof(ulong));
            terminalType.Add("ulong?", typeof(ulong?));
            terminalType.Add("float", typeof(float));
            terminalType.Add("float?", typeof(float?));
            terminalType.Add("double", typeof(double));
            terminalType.Add("double?", typeof(double?));
            terminalType.Add("decimal", typeof(decimal));
            terminalType.Add("decimal?", typeof(decimal?));
            terminalType.Add("char", typeof(char));
            terminalType.Add("char?", typeof(char?));
            terminalType.Add("string", typeof(string));
            terminalType.Add("bool", typeof(bool));
            terminalType.Add("bool?", typeof(bool?));
            terminalType.Add("object", typeof(object));
            terminalType.Add("DateTime", typeof(DateTime));
            terminalType.Add("DateTime?", typeof(DateTime?));
            terminalType.Add("TimeSpan", typeof(TimeSpan));
            terminalType.Add("TimeSpan?", typeof(TimeSpan?));
            terminalType.AstConfig.NodeType = null;
            terminalType.AstConfig.DefaultNodeCreator = () => new ValueASTNode<Type>();

            /* COMENTARIOS */
            CommentTerminal comentarioLinea = new CommentTerminal("line_coment", "//", "\n", "\r\n");
            CommentTerminal comentarioBloque = new CommentTerminal("block_coment", "/*", "*/");
            NonGrammarTerminals.Add(comentarioLinea);
            NonGrammarTerminals.Add(comentarioBloque);

            /* NON TERMINALS */
            
            NonTerminal nt_METADATA_QUERY = new NonTerminal("METADATA_QUERY", typeof(CommandQueryForMetadataASTNode));
            nt_METADATA_QUERY.AstConfig.NodeType = null;
            nt_METADATA_QUERY.AstConfig.DefaultNodeCreator = () => new CommandQueryForMetadataASTNode();

            NonTerminal nt_TEMPORAL_STREAM = new NonTerminal("TEMPORAL_STREAM", typeof(TemporalStreamCommandASTNode));
            nt_TEMPORAL_STREAM.AstConfig.NodeType = null;
            nt_TEMPORAL_STREAM.AstConfig.DefaultNodeCreator = () => new TemporalStreamCommandASTNode();

            NonTerminal nt_COMMAND_NODE = new NonTerminal("COMMAND", typeof(CommandNode));
            nt_COMMAND_NODE.AstConfig.NodeType = null;
            nt_COMMAND_NODE.AstConfig.DefaultNodeCreator = () => new CommandNode();
            NonTerminal nt_COMMAND_NODE_LIST = new NonTerminal("COMMAND_LIST", typeof(CommandListASTNode));
            nt_COMMAND_NODE_LIST.AstConfig.NodeType = null;
            nt_COMMAND_NODE_LIST.AstConfig.DefaultNodeCreator = () => new CommandListASTNode();
            
            /* ID WITH PATH */
            NonTerminal nt_FOURTH_LEVEL_OBJECT_IDENTIFIER = new NonTerminal("FOURTH_LEVEL_OBJECT_IDENTIFIER", typeof(FourthLevelIdentifierASTNode));
            nt_FOURTH_LEVEL_OBJECT_IDENTIFIER.AstConfig.NodeType = null;
            nt_FOURTH_LEVEL_OBJECT_IDENTIFIER.AstConfig.DefaultNodeCreator = () => new FourthLevelIdentifierASTNode();
            NonTerminal nt_THIRD_LEVEL_OBJECT_IDENTIFIER = new NonTerminal("THIRD_LEVEL_OBJECT_IDENTIFIER", typeof(ThirdLevelIdentifierASTNode));
            nt_THIRD_LEVEL_OBJECT_IDENTIFIER.AstConfig.NodeType = null;
            nt_THIRD_LEVEL_OBJECT_IDENTIFIER.AstConfig.DefaultNodeCreator = () => new ThirdLevelIdentifierASTNode();
            NonTerminal nt_SECOND_LEVEL_OBJECT_IDENTIFIER = new NonTerminal("SECOND_LEVEL_OBJECT_IDENTIFIER", typeof(SecondLevelIdentifierASTNode));
            nt_SECOND_LEVEL_OBJECT_IDENTIFIER.AstConfig.NodeType = null;
            nt_SECOND_LEVEL_OBJECT_IDENTIFIER.AstConfig.DefaultNodeCreator = () => new SecondLevelIdentifierASTNode();
            /************************************************/

            /* SPACE OBJECTS CATEGORIES */
            NonTerminal nt_SPACE_OBJECTS = new NonTerminal("SPACE_OBJECTS", typeof(SpaceObjectASTNode));
            nt_SPACE_OBJECTS.AstConfig.NodeType = null;
            nt_SPACE_OBJECTS.AstConfig.DefaultNodeCreator = () => new SpaceObjectASTNode();

            NonTerminal nt_FOURTH_LEVEL_OBJECTS_TO_ALTER = new NonTerminal("FOURTH_LEVEL_OBJECTS_TO_ALTER", typeof(SpaceObjectASTNode));
            nt_FOURTH_LEVEL_OBJECTS_TO_ALTER.AstConfig.NodeType = null;
            nt_FOURTH_LEVEL_OBJECTS_TO_ALTER.AstConfig.DefaultNodeCreator = () => new SpaceObjectASTNode();
            NonTerminal nt_THIRD_LEVEL_OBJECTS_TO_ALTER = new NonTerminal("THIRD_LEVEL_OBJECTS_TO_ALTER", typeof(SpaceObjectASTNode));
            nt_THIRD_LEVEL_OBJECTS_TO_ALTER.AstConfig.NodeType = null;
            nt_THIRD_LEVEL_OBJECTS_TO_ALTER.AstConfig.DefaultNodeCreator = () => new SpaceObjectASTNode();
            NonTerminal nt_SECOND_LEVEL_OBJECTS_TO_ALTER = new NonTerminal("SECOND_LEVEL_OBJECTS_TO_ALTER", typeof(SpaceObjectASTNode));
            nt_SECOND_LEVEL_OBJECTS_TO_ALTER.AstConfig.NodeType = null;
            nt_SECOND_LEVEL_OBJECTS_TO_ALTER.AstConfig.DefaultNodeCreator = () => new SpaceObjectASTNode();

            NonTerminal nt_SECOND_LEVEL_OBJECTS_TO_TAKE_OWNERSHIP = new NonTerminal("SECOND_LEVEL_OBJECTS_TO_TAKE_OWNERSHIP", typeof(SpaceObjectASTNode));
            nt_SECOND_LEVEL_OBJECTS_TO_TAKE_OWNERSHIP.AstConfig.NodeType = null;
            nt_SECOND_LEVEL_OBJECTS_TO_TAKE_OWNERSHIP.AstConfig.DefaultNodeCreator = () => new SpaceObjectASTNode();
            NonTerminal nt_THIRD_LEVEL_OBJECTS_TO_TAKE_OWNERSHIP = new NonTerminal("THRID_LEVEL_OBJECTS_TO_TAKE_OWNERSHIP", typeof(SpaceObjectASTNode));
            nt_THIRD_LEVEL_OBJECTS_TO_TAKE_OWNERSHIP.AstConfig.NodeType = null;
            nt_THIRD_LEVEL_OBJECTS_TO_TAKE_OWNERSHIP.AstConfig.DefaultNodeCreator = () => new SpaceObjectASTNode();

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
            NonTerminal nt_GRANULAR_PERMISSION_FOR_ON_3 = new NonTerminal("GRANULAR_PERMISSION_FOR_ON_3", typeof(GranularPermissionASTNode));
            nt_GRANULAR_PERMISSION_FOR_ON_3.AstConfig.NodeType = null;
            nt_GRANULAR_PERMISSION_FOR_ON_3.AstConfig.DefaultNodeCreator = () => new GranularPermissionASTNode();
            NonTerminal nt_PERMISSION = new NonTerminal("PERMISSION", typeof(PermissionASTNode));
            nt_PERMISSION.AstConfig.NodeType = null;
            nt_PERMISSION.AstConfig.DefaultNodeCreator = () => new PermissionASTNode();
            /************************************************/
                        
            /* OBJECT WITH IDENTIFIER */           
            NonTerminal nt_SPACE_SERVER_PRINCIPALS_WITH_ID = new NonTerminal("SPACE_USER_OR_ROLE_WITH_ID", typeof(SpaceObjectWithIdASTNode));
            nt_SPACE_SERVER_PRINCIPALS_WITH_ID.AstConfig.NodeType = null;
            nt_SPACE_SERVER_PRINCIPALS_WITH_ID.AstConfig.DefaultNodeCreator = () => new SpaceObjectWithIdASTNode();
            NonTerminal nt_SPACE_DB_PRINCIPALS_WITH_ID = new NonTerminal("SPACE_SOURCE_OR_ROLE_WITH_ID", typeof(SpaceObjectWithIdASTNode));
            nt_SPACE_DB_PRINCIPALS_WITH_ID.AstConfig.NodeType = null;
            nt_SPACE_DB_PRINCIPALS_WITH_ID.AstConfig.DefaultNodeCreator = () => new SpaceObjectWithIdASTNode();
            /************************************************/

            /* LIST OF IDENTIFIERS */
            NonTerminal nt_FOURTH_LEVEL_ID_LIST = new NonTerminal("FOURTH_LEVEL_ID_LIST", typeof(Irony.Interpreter.Ast.StatementListNode));
            nt_FOURTH_LEVEL_ID_LIST.AstConfig.NodeType = null;
            nt_FOURTH_LEVEL_ID_LIST.AstConfig.DefaultNodeCreator = () => new Irony.Interpreter.Ast.StatementListNode();
            NonTerminal nt_THIRD_LEVEL_ID_LIST = new NonTerminal("THIRD_LEVEL_ID_LIST", typeof(Irony.Interpreter.Ast.StatementListNode));
            nt_THIRD_LEVEL_ID_LIST.AstConfig.NodeType = null;
            nt_THIRD_LEVEL_ID_LIST.AstConfig.DefaultNodeCreator = () => new Irony.Interpreter.Ast.StatementListNode();
            NonTerminal nt_THIRD_LEVEL_ID_LIST_2 = new NonTerminal("THIRD_LEVEL_ID_LIST", typeof(Irony.Interpreter.Ast.StatementListNode));
            nt_THIRD_LEVEL_ID_LIST_2.AstConfig.NodeType = null;
            nt_THIRD_LEVEL_ID_LIST_2.AstConfig.DefaultNodeCreator = () => new Irony.Interpreter.Ast.StatementListNode();
            NonTerminal nt_SECOND_LEVEL_ID_LIST = new NonTerminal("SECOND_LEVEL_ID_LIST", typeof(Irony.Interpreter.Ast.StatementListNode));
            nt_SECOND_LEVEL_ID_LIST.AstConfig.NodeType = null;
            nt_SECOND_LEVEL_ID_LIST.AstConfig.DefaultNodeCreator = () => new Irony.Interpreter.Ast.StatementListNode();
            /************************************************/

            /* PERMISSION OPTIONS */
            NonTerminal nt_PERMISSION_OPTION = new NonTerminal("PERMISSION_OPTION", typeof(PermissionOptionASTNode));
            nt_PERMISSION_OPTION.AstConfig.NodeType = null;
            nt_PERMISSION_OPTION.AstConfig.DefaultNodeCreator = () => new PermissionOptionASTNode();
            /************************************************/

            /* LOGIN OPTIONS */
            NonTerminal nt_LOGIN_OPTION_PASSWORD = new NonTerminal("LOGIN_OPTION_PASSWORD", typeof(CommandOptionASTNode<LoginOptionEnum>));
            nt_LOGIN_OPTION_PASSWORD.AstConfig.NodeType = null;
            nt_LOGIN_OPTION_PASSWORD.AstConfig.DefaultNodeCreator = () => new CommandOptionASTNode<LoginOptionEnum>();
            NonTerminal nt_LOGIN_OPTION = new NonTerminal("LOGIN_OPTION", typeof(CommandOptionASTNode<LoginOptionEnum>));
            nt_LOGIN_OPTION.AstConfig.NodeType = null;
            nt_LOGIN_OPTION.AstConfig.DefaultNodeCreator = () => new CommandOptionASTNode<LoginOptionEnum>();
            NonTerminal nt_LOGIN_OPTION_LIST = new NonTerminal("LOGIN_OPTION_LIST", typeof(DictionaryCommandOptionASTNode<LoginOptionEnum>));
            nt_LOGIN_OPTION_LIST.AstConfig.NodeType = null;
            nt_LOGIN_OPTION_LIST.AstConfig.DefaultNodeCreator = () => new DictionaryCommandOptionASTNode<LoginOptionEnum>();
            NonTerminal nt_LOGIN_OPTION_LIST_AUX = new NonTerminal("LOGIN_OPTION_LIST_AUX", typeof(CommandOptionListASTNode<LoginOptionEnum>));
            nt_LOGIN_OPTION_LIST_AUX.AstConfig.NodeType = null;
            nt_LOGIN_OPTION_LIST_AUX.AstConfig.DefaultNodeCreator = () => new CommandOptionListASTNode<LoginOptionEnum>();
            /************************************************/

            /* ROLE OPTIONS */
            NonTerminal nt_DB_ROLE_OPTION = new NonTerminal("DB_ROLE_OPTION", typeof(CommandOptionASTNode<RoleOptionEnum>));
            nt_DB_ROLE_OPTION.AstConfig.NodeType = null;
            nt_DB_ROLE_OPTION.AstConfig.DefaultNodeCreator = () => new CommandOptionASTNode<RoleOptionEnum>();
            NonTerminal nt_DB_ROLE_OPTION_LIST = new NonTerminal("DB_ROLE_OPTION_LIST", typeof(DictionaryCommandOptionASTNode<RoleOptionEnum>));
            nt_DB_ROLE_OPTION_LIST.AstConfig.NodeType = null;
            nt_DB_ROLE_OPTION_LIST.AstConfig.DefaultNodeCreator = () => new DictionaryCommandOptionASTNode<RoleOptionEnum>();
            NonTerminal nt_DB_ROLE_OPTION_LIST_AUX = new NonTerminal("DB_ROLE_OPTION_LIST_AUX", typeof(CommandOptionListASTNode<RoleOptionEnum>));
            nt_DB_ROLE_OPTION_LIST_AUX.AstConfig.NodeType = null;
            nt_DB_ROLE_OPTION_LIST_AUX.AstConfig.DefaultNodeCreator = () => new CommandOptionListASTNode<RoleOptionEnum>();
            /************************************************/

            /* SOURCE OPTIONS */
            NonTerminal nt_SOURCE_OPTION = new NonTerminal("SOURCE_OPTION", typeof(CommandOptionASTNode<SourceOptionEnum>));
            nt_SOURCE_OPTION.AstConfig.NodeType = null;
            nt_SOURCE_OPTION.AstConfig.DefaultNodeCreator = () => new CommandOptionASTNode<SourceOptionEnum>();
            NonTerminal nt_SOURCE_OPTION_LIST = new NonTerminal("SOURCE_OPTION_LIST", typeof(DictionaryCommandOptionASTNode<SourceOptionEnum>));
            nt_SOURCE_OPTION_LIST.AstConfig.NodeType = null;
            nt_SOURCE_OPTION_LIST.AstConfig.DefaultNodeCreator = () => new DictionaryCommandOptionASTNode<SourceOptionEnum>();
            NonTerminal nt_SOURCE_OPTION_LIST_AUX = new NonTerminal("SOURCE_OPTION_LIST_AUX", typeof(CommandOptionListASTNode<SourceOptionEnum>));
            nt_SOURCE_OPTION_LIST_AUX.AstConfig.NodeType = null;
            nt_SOURCE_OPTION_LIST_AUX.AstConfig.DefaultNodeCreator = () => new CommandOptionListASTNode<SourceOptionEnum>();
            /************************************************/

            /* DATABASE OPTIONS */
            NonTerminal nt_DATABASE_OPTION = new NonTerminal("DATABASE_OPTION", typeof(CommandOptionASTNode<DatabaseOptionEnum>));
            nt_DATABASE_OPTION.AstConfig.NodeType = null;
            nt_DATABASE_OPTION.AstConfig.DefaultNodeCreator = () => new CommandOptionASTNode<DatabaseOptionEnum>();
            NonTerminal nt_DATABASE_OPTION_LIST = new NonTerminal("DATABASE_OPTION_LIST", typeof(DictionaryCommandOptionASTNode<DatabaseOptionEnum>));
            nt_DATABASE_OPTION_LIST.AstConfig.NodeType = null;
            nt_DATABASE_OPTION_LIST.AstConfig.DefaultNodeCreator = () => new DictionaryCommandOptionASTNode<DatabaseOptionEnum>();
            NonTerminal nt_DATABASE_OPTION_LIST_AUX = new NonTerminal("DATABASE_OPTION_LIST_AUX", typeof(CommandOptionListASTNode<DatabaseOptionEnum>));
            nt_DATABASE_OPTION_LIST_AUX.AstConfig.NodeType = null;
            nt_DATABASE_OPTION_LIST_AUX.AstConfig.DefaultNodeCreator = () => new CommandOptionListASTNode<DatabaseOptionEnum>();
            /************************************************/

            /* USER OPTIONS */
            NonTerminal nt_USER_OPTION = new NonTerminal("USER_OPTION", typeof(CommandOptionASTNode<UserOptionEnum>));
            nt_USER_OPTION.AstConfig.NodeType = null;
            nt_USER_OPTION.AstConfig.DefaultNodeCreator = () => new CommandOptionASTNode<UserOptionEnum>();
            NonTerminal nt_USER_OPTION_LIST = new NonTerminal("USER_OPTION_LIST", typeof(DictionaryCommandOptionASTNode<UserOptionEnum>));
            nt_USER_OPTION_LIST.AstConfig.NodeType = null;
            nt_USER_OPTION_LIST.AstConfig.DefaultNodeCreator = () => new DictionaryCommandOptionASTNode<UserOptionEnum>();
            NonTerminal nt_USER_OPTION_LIST_AUX = new NonTerminal("USER_OPTION_LIST_AUX", typeof(CommandOptionListASTNode<UserOptionEnum>));
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
            NonTerminal nt_STREAM_OPTION_LIST_AUX = new NonTerminal("USER_OPTION_LIST_AUX", typeof(CommandOptionListASTNode<StreamOptionEnum>));
            nt_STREAM_OPTION_LIST_AUX.AstConfig.NodeType = null;
            nt_STREAM_OPTION_LIST_AUX.AstConfig.DefaultNodeCreator = () => new CommandOptionListASTNode<StreamOptionEnum>();
            /************************************************/

            /* SPACE PERMISSION LIST */
            NonTerminal nt_SPACE_PERMISSION_LIST = new NonTerminal("PERMISSION_LIST", typeof(ListASTNode<PermissionASTNode, PermissionNode>));
            nt_SPACE_PERMISSION_LIST.AstConfig.NodeType = null;
            nt_SPACE_PERMISSION_LIST.AstConfig.DefaultNodeCreator = () => new ListASTNode<PermissionASTNode, PermissionNode>();            
            NonTerminal nt_SPACE_PRINCIPAL_LIST = new NonTerminal("SERVER_PRINCIPAL_LIST", typeof(ListASTNode<SpaceObjectWithIdASTNode, CommandObject>));
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
            NonTerminal nt_ALTER_SOURCE_COLUMNS_STRUCTURE = new NonTerminal("ALTER_SOURCE_COLUMNS_STRUCTURE", typeof(AlterSourceColumnsStructureASTNode));
            nt_ALTER_SOURCE_COLUMNS_STRUCTURE.AstConfig.NodeType = null;
            nt_ALTER_SOURCE_COLUMNS_STRUCTURE.AstConfig.DefaultNodeCreator = () => new AlterSourceColumnsStructureASTNode();
            NonTerminal nt_ALTER_SOURCE_STATEMENTS = new NonTerminal("ALTER_SOURCE_STATEMENTS", typeof(AlterSourceStatementASTNode));
            nt_ALTER_SOURCE_STATEMENTS.AstConfig.NodeType = null;
            nt_ALTER_SOURCE_STATEMENTS.AstConfig.DefaultNodeCreator = () => new AlterSourceStatementASTNode();

            NonTerminal nt_ALTER_STREAM = new NonTerminal("ALTER_STREAM", typeof(AlterStreamASTNode));
            nt_ALTER_STREAM.AstConfig.NodeType = null;
            nt_ALTER_STREAM.AstConfig.DefaultNodeCreator = () => new AlterStreamASTNode();

            NonTerminal nt_SOURCE_COLUMN = new NonTerminal("SOURCE_COLUMN", typeof(SourceColumnsASTNode));
            nt_SOURCE_COLUMN.AstConfig.NodeType = null;
            nt_SOURCE_COLUMN.AstConfig.DefaultNodeCreator = () => new SourceColumnsASTNode();
            NonTerminal nt_SOURCE_COLUMN_LIST = new NonTerminal("SOURCE_COLUMN_LIST", typeof(DictionaryASTNode<SourceColumnsASTNode, string, Type>));
            nt_SOURCE_COLUMN_LIST.AstConfig.NodeType = null;
            nt_SOURCE_COLUMN_LIST.AstConfig.DefaultNodeCreator = () => new DictionaryASTNode<SourceColumnsASTNode, string, Type>();
            /************************************************/

            NonTerminal nt_PERMISSIONS_COMMANDS = new NonTerminal("PERMISSIONS_COMMANDS", typeof(PermissionCommandASTNode));
            nt_PERMISSIONS_COMMANDS.AstConfig.NodeType = null;
            nt_PERMISSIONS_COMMANDS.AstConfig.DefaultNodeCreator = () => new PermissionCommandASTNode();
            NonTerminal nt_ADD_OR_REMOVE_USERS_TO_ROLE_COMMAND = new NonTerminal("ADD_USERS_TO_ROLE_COMMAND", typeof(AddOrRemoveCommandASTNode));
            nt_ADD_OR_REMOVE_USERS_TO_ROLE_COMMAND.AstConfig.NodeType = null;
            nt_ADD_OR_REMOVE_USERS_TO_ROLE_COMMAND.AstConfig.DefaultNodeCreator = () => new AddOrRemoveCommandASTNode();
            NonTerminal nt_TAKE_OWNERSHIP = new NonTerminal("TAKE_OWNERSHIP", typeof(TakeOwnershipASTNode));
            nt_TAKE_OWNERSHIP.AstConfig.NodeType = null;
            nt_TAKE_OWNERSHIP.AstConfig.DefaultNodeCreator = () => new TakeOwnershipASTNode();

            NonTerminal nt_TRUNCATE_SOURCE = new NonTerminal("TRUNCATE_SOURCE", typeof(TruncateASTNode));
            nt_TRUNCATE_SOURCE.AstConfig.NodeType = null;
            nt_TRUNCATE_SOURCE.AstConfig.DefaultNodeCreator = () => new TruncateASTNode();

            /* RULES */
            
            nt_SECOND_LEVEL_ID_LIST.Rule = this.MakePlusRule(nt_SECOND_LEVEL_ID_LIST, terminalComa, nt_SECOND_LEVEL_OBJECT_IDENTIFIER);
            nt_THIRD_LEVEL_ID_LIST.Rule = this.MakePlusRule(nt_THIRD_LEVEL_ID_LIST, terminalComa, nt_THIRD_LEVEL_OBJECT_IDENTIFIER);
            nt_THIRD_LEVEL_ID_LIST_2.Rule = this.MakePlusRule(nt_THIRD_LEVEL_ID_LIST_2, nt_THIRD_LEVEL_OBJECT_IDENTIFIER);
            nt_FOURTH_LEVEL_ID_LIST.Rule = this.MakePlusRule(nt_FOURTH_LEVEL_ID_LIST, terminalComa, nt_FOURTH_LEVEL_OBJECT_IDENTIFIER);

            /* COMMAND ACTIONS */
            
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
            
            nt_SECOND_LEVEL_OBJECTS_TO_ALTER.Rule = terminalDatabase
                                                    | terminalLogin
                                                    | terminalEndpoint;

            nt_THIRD_LEVEL_OBJECTS_TO_ALTER.Rule = terminalUser
                                                    | terminalRole
                                                    | terminalSchema;

            nt_FOURTH_LEVEL_OBJECTS_TO_ALTER.Rule = terminalSource
                                                    | terminalStream
                                                    | terminalView;

            nt_SECOND_LEVEL_OBJECTS_TO_TAKE_OWNERSHIP.Rule = terminalDatabase
                                                            | terminalEndpoint;

            nt_THIRD_LEVEL_OBJECTS_TO_TAKE_OWNERSHIP.Rule = terminalRole
                                                            | terminalSchema;

            nt_SPACE_OBJECTS_FOR_STATUS_PERMISSIONS.Rule = terminalSource;

            nt_SPACE_SERVER_PRINCIPALS.Rule = terminalLogin;

            nt_SPACE_DB_PRINCIPALS.Rule = terminalRole
                                        | terminalUser;

            /************************************************/

            /* OBJECT WITH IDENTIFIER */
            nt_FOURTH_LEVEL_OBJECT_IDENTIFIER.Rule = terminalId + terminalPunto + terminalId + terminalPunto + terminalId
                                                    | terminalId + terminalPunto + terminalId
                                                    | terminalId;

            nt_THIRD_LEVEL_OBJECT_IDENTIFIER.Rule = terminalId + terminalPunto + terminalId
                                                    | terminalId;

            nt_SECOND_LEVEL_OBJECT_IDENTIFIER.Rule = terminalId;

            /************************************************/

            /* OBJECT WITH IDENTIFIER */
            
            nt_SPACE_SERVER_PRINCIPALS_WITH_ID.Rule = nt_SPACE_SERVER_PRINCIPALS + nt_SECOND_LEVEL_OBJECT_IDENTIFIER;

            nt_SPACE_DB_PRINCIPALS_WITH_ID.Rule = nt_SPACE_DB_PRINCIPALS + nt_THIRD_LEVEL_OBJECT_IDENTIFIER;

            /************************************************/

            /* PERMISSIONS */

            nt_PERMISSION.Rule = nt_GRANULAR_PERMISSION
                                | nt_GRANULAR_PERMISSION_FOR_ON_1 + terminalOn + nt_SECOND_LEVEL_OBJECTS_TO_ALTER + nt_SECOND_LEVEL_OBJECT_IDENTIFIER
                                | nt_GRANULAR_PERMISSION_FOR_ON_1 + terminalOn + nt_THIRD_LEVEL_OBJECTS_TO_ALTER + nt_THIRD_LEVEL_OBJECT_IDENTIFIER
                                | nt_GRANULAR_PERMISSION_FOR_ON_1 + terminalOn + nt_FOURTH_LEVEL_OBJECTS_TO_ALTER + nt_FOURTH_LEVEL_OBJECT_IDENTIFIER
                                | nt_GRANULAR_PERMISSION_FOR_ON_3 + terminalOn + nt_SECOND_LEVEL_OBJECTS_TO_ALTER + nt_SECOND_LEVEL_OBJECT_IDENTIFIER
                                | nt_GRANULAR_PERMISSION_FOR_ON_2 + terminalOn + nt_SPACE_OBJECTS_FOR_STATUS_PERMISSIONS + nt_FOURTH_LEVEL_OBJECT_IDENTIFIER
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
                                                    | terminalControl
                                                    | terminalAlter
                                                    | terminalTake + terminalOwnership;

            nt_GRANULAR_PERMISSION_FOR_ON_2.Rule = terminalRead
                                                    | terminalWrite;

            nt_GRANULAR_PERMISSION_FOR_ON_3.Rule = terminalConnect;

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
            
            /* CRUD commands */

            /* CREATE */

            nt_CREATE_LOGIN.Rule = terminalCreate + terminalLogin + nt_SECOND_LEVEL_OBJECT_IDENTIFIER + terminalWith + nt_LOGIN_OPTION_PASSWORD + nt_LOGIN_OPTION_LIST_AUX;
            nt_LOGIN_OPTION_LIST_AUX.Rule = terminalComa + nt_LOGIN_OPTION_LIST
                                            | this.Empty;
            nt_LOGIN_OPTION_PASSWORD.Rule = terminalPassword + terminalEqual + terminalCadena;
            nt_LOGIN_OPTION_LIST.Rule = this.MakePlusRule(nt_LOGIN_OPTION_LIST, terminalComa, nt_LOGIN_OPTION);
            nt_LOGIN_OPTION.Rule = terminalDefaultDatabase + terminalEqual + terminalId
                                        | terminalPassword + terminalEqual + terminalCadena
                                        | terminalName + terminalEqual + terminalId
                                        | terminalStatus + terminalEqual + terminalStatusValue;
            nt_CREATE_DATABASE.Rule = terminalCreate + terminalDatabase + nt_SECOND_LEVEL_OBJECT_IDENTIFIER + nt_DATABASE_OPTION_LIST_AUX;
            nt_DATABASE_OPTION_LIST_AUX.Rule = terminalWith + nt_DATABASE_OPTION_LIST
                                            | this.Empty;
            nt_DATABASE_OPTION_LIST.Rule = this.MakePlusRule(nt_DATABASE_OPTION_LIST, terminalComa, nt_DATABASE_OPTION);
            nt_DATABASE_OPTION.Rule = terminalStatus + terminalEqual + terminalStatusValue
                                    | terminalName + terminalEqual + terminalId;
            nt_CREATE_USER.Rule = terminalCreate + terminalUser + nt_THIRD_LEVEL_OBJECT_IDENTIFIER + nt_USER_OPTION_LIST_AUX;
            nt_USER_OPTION_LIST_AUX.Rule = terminalWith + nt_USER_OPTION_LIST
                                            | this.Empty;
            nt_USER_OPTION_LIST.Rule = this.MakePlusRule(nt_USER_OPTION_LIST, terminalComa, nt_USER_OPTION);
            nt_USER_OPTION.Rule = terminalDefaultSchema + terminalEqual + terminalId
                                        | terminalName + terminalEqual + terminalId
                                        | terminalLogin + terminalEqual + terminalId
                                        | terminalStatus + terminalEqual + terminalStatusValue;
            nt_CREATE_ROLE.Rule = terminalCreate + terminalRole + nt_THIRD_LEVEL_OBJECT_IDENTIFIER + nt_DB_ROLE_OPTION_LIST_AUX;
            nt_DB_ROLE_OPTION_LIST_AUX.Rule = terminalWith + nt_DB_ROLE_OPTION_LIST
                                            | this.Empty;
            nt_DB_ROLE_OPTION_LIST.Rule = this.MakePlusRule(nt_DB_ROLE_OPTION_LIST, terminalComa, nt_DB_ROLE_OPTION);
            nt_DB_ROLE_OPTION.Rule = terminalStatus + terminalEqual + terminalStatusValue
                                    | terminalName + terminalEqual + terminalId
                                    | terminalAdd + terminalEqual + nt_THIRD_LEVEL_ID_LIST_2
                                    | terminalRemove + terminalEqual + nt_THIRD_LEVEL_ID_LIST_2;
            nt_CREATE_SCHEMA.Rule = terminalCreate + terminalSchema + nt_THIRD_LEVEL_OBJECT_IDENTIFIER;
            nt_CREATE_STREAM.Rule = terminalCreate + terminalStream + nt_FOURTH_LEVEL_OBJECT_IDENTIFIER + terminalQueryScript + nt_STREAM_OPTION_LIST_AUX;
            nt_STREAM_OPTION_LIST_AUX.Rule = terminalWith + nt_STREAM_OPTION_LIST
                                            | this.Empty;
            nt_CREATE_SOURCE.Rule = terminalCreate + terminalSource + nt_FOURTH_LEVEL_OBJECT_IDENTIFIER + terminalParentesisIz + nt_SOURCE_COLUMN_LIST + terminalParentesisDer + nt_SOURCE_OPTION_LIST_AUX;
            nt_SOURCE_OPTION_LIST_AUX.Rule = terminalWith + nt_SOURCE_OPTION_LIST
                                            | this.Empty;
            nt_SOURCE_OPTION_LIST.Rule = this.MakePlusRule(nt_SOURCE_OPTION_LIST, terminalComa, nt_SOURCE_OPTION);
            nt_SOURCE_OPTION.Rule = terminalStatus + terminalEqual + terminalStatusValue
                                    | terminalName + terminalEqual + terminalId
                                    | terminalCacheDurability + terminalEqual + terminalUnsignedIntValue
                                    | terminalCacheSize + terminalEqual + terminalUnsignedIntValue
                                    | terminalPersistent + terminalEqual + terminalStatusValue;

            nt_SOURCE_COLUMN.Rule = terminalId + terminalType;
            nt_SOURCE_COLUMN_LIST.Rule = this.MakePlusRule(nt_SOURCE_COLUMN_LIST, terminalComa, nt_SOURCE_COLUMN);

            /* ALTER */

            nt_ALTER_LOGIN.Rule = terminalAlter + terminalLogin + nt_SECOND_LEVEL_OBJECT_IDENTIFIER + terminalWith + nt_LOGIN_OPTION_LIST;
            nt_ALTER_DATABASE.Rule = terminalAlter + terminalDatabase + nt_SECOND_LEVEL_OBJECT_IDENTIFIER + terminalWith + nt_DATABASE_OPTION_LIST;

            nt_ALTER_USER.Rule = terminalAlter + terminalUser + nt_THIRD_LEVEL_OBJECT_IDENTIFIER + terminalWith + nt_USER_OPTION_LIST;
            nt_ALTER_ROLE.Rule = terminalAlter + terminalRole + nt_THIRD_LEVEL_OBJECT_IDENTIFIER + terminalWith + nt_DB_ROLE_OPTION_LIST;
            nt_ALTER_SCHEMA.Rule = terminalAlter + terminalSchema + nt_THIRD_LEVEL_OBJECT_IDENTIFIER + terminalWith + terminalName + terminalEqual + terminalId;

            nt_ALTER_SOURCE.Rule = terminalAlter + terminalSource + nt_FOURTH_LEVEL_OBJECT_IDENTIFIER + nt_ALTER_SOURCE_STATEMENTS;
            nt_ALTER_SOURCE_COLUMNS_STRUCTURE.Rule = terminalAdd + nt_SOURCE_COLUMN_LIST
                                                    | terminalRemove + nt_SOURCE_COLUMN_LIST;
            nt_ALTER_SOURCE_STATEMENTS.Rule = terminalWith + nt_SOURCE_OPTION_LIST
                                                | nt_ALTER_SOURCE_COLUMNS_STRUCTURE;

            nt_ALTER_STREAM.Rule = terminalAlter + terminalStream + nt_FOURTH_LEVEL_OBJECT_IDENTIFIER + terminalWith + nt_STREAM_OPTION_LIST;
            nt_STREAM_OPTION_LIST.Rule = this.MakePlusRule(nt_STREAM_OPTION_LIST, terminalComa, nt_STREAM_OPTION);
            nt_STREAM_OPTION.Rule = terminalQuery + terminalEqual + terminalQueryScript
                                        | terminalName + terminalEqual + terminalId
                                        | terminalStatus + terminalEqual + terminalStatusValue;

            /* DROP */

            nt_DROP_COMMAND.Rule = terminalDrop + nt_FOURTH_LEVEL_OBJECTS_TO_ALTER + nt_FOURTH_LEVEL_ID_LIST
                                    | terminalDrop + nt_THIRD_LEVEL_OBJECTS_TO_ALTER + nt_THIRD_LEVEL_ID_LIST
                                    | terminalDrop + nt_SECOND_LEVEL_OBJECTS_TO_ALTER + nt_SECOND_LEVEL_ID_LIST;
            
            /************************************************/

            /* Permission commands */

            nt_PERMISSION_OPTION.Rule = terminalWith + terminalGrant + terminalOption
                                        | this.Empty;

            nt_PERMISSIONS_COMMANDS.Rule = terminalGrant + nt_SPACE_PERMISSION_LIST + terminalTo + nt_SPACE_PRINCIPAL_LIST + nt_PERMISSION_OPTION
                                            | terminalDeny + nt_SPACE_PERMISSION_LIST + terminalTo + nt_SPACE_PRINCIPAL_LIST
                                            | terminalRevoke + nt_SPACE_PERMISSION_LIST + terminalTo + nt_SPACE_PRINCIPAL_LIST;

            /* Add USERS TO ROLES */

            nt_ADD_OR_REMOVE_USERS_TO_ROLE_COMMAND.Rule = terminalAdd + nt_THIRD_LEVEL_ID_LIST + terminalTo + nt_THIRD_LEVEL_ID_LIST
                                                            | terminalRemove + nt_THIRD_LEVEL_ID_LIST + terminalTo + nt_THIRD_LEVEL_ID_LIST;

            /* TAKE OWNERSHIP */

            nt_TAKE_OWNERSHIP.Rule = terminalTake + terminalOwnership + terminalOn + nt_SECOND_LEVEL_OBJECTS_TO_TAKE_OWNERSHIP + nt_SECOND_LEVEL_OBJECT_IDENTIFIER
                                    | terminalTake + terminalOwnership + terminalOn + nt_THIRD_LEVEL_OBJECTS_TO_TAKE_OWNERSHIP + nt_THIRD_LEVEL_OBJECT_IDENTIFIER
                                    | terminalTake + terminalOwnership + terminalOn + nt_FOURTH_LEVEL_OBJECTS_TO_ALTER + nt_FOURTH_LEVEL_OBJECT_IDENTIFIER;

            /************************************************/
            
            /* TRUNCATE */

            nt_TRUNCATE_SOURCE.Rule = terminalTruncate + terminalSource + nt_FOURTH_LEVEL_OBJECT_IDENTIFIER;

            /************************************************/

            /* QUERY METADATA */

            nt_METADATA_QUERY.Rule = new QueryGrammarForMetadata(this.Empty).QueryForMetadata; // this.CreateQueryForMetadataGrammar();

            /************************************************/

            /* TEMPORAL STREAM */

            nt_TEMPORAL_STREAM.Rule = new TemporalStreamGrammar(this.Empty, this.MakeStarRule).TemporalStream;

            /************************************************/

            nt_COMMAND_NODE.Rule = nt_PERMISSIONS_COMMANDS
                                    | nt_ADD_OR_REMOVE_USERS_TO_ROLE_COMMAND
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
                                    | nt_TRUNCATE_SOURCE
                                    /*| nt_METADATA_QUERY*/
                                    | nt_TEMPORAL_STREAM;

            nt_COMMAND_NODE_LIST.Rule = this.MakePlusRule(nt_COMMAND_NODE_LIST, terminalPuntoYComa, nt_COMMAND_NODE);

            this.Root = nt_COMMAND_NODE_LIST;

            this.LanguageFlags = Irony.Parsing.LanguageFlags.CreateAst;
        }
    }
}
