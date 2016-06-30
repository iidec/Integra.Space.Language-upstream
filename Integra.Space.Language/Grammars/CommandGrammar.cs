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
    using ASTNodes.QuerySections;
    using ASTNodes.Root;
    using ASTNodes.UserQuery;
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

            /* SPACE OBJECTS */
            KeyTerm terminalSource = ToTerm("source", "source");
            KeyTerm terminalStream = ToTerm("stream", "stream");
            KeyTerm terminalRole = ToTerm("role", "role");
            KeyTerm terminalUser = ToTerm("user", "user");
            KeyTerm terminalPermissions = ToTerm("permissions", "permissions");

            /* PARA CREACIÓN DE USUARIOS */
            KeyTerm terminalWith = ToTerm("with", "with");
            KeyTerm terminalPassword = ToTerm("password", "password");
            KeyTerm terminalStatus = ToTerm("status", "status");
            KeyTerm terminalEnable = ToTerm("enable", "enable");
            KeyTerm terminalDisable = ToTerm("disable", "disable");
            KeyTerm terminalEqual = ToTerm("=", "equal");

            /* PARA ASIGNACIÓN DE PERMISOS */
            KeyTerm terminalTo = ToTerm("to", "to");
            KeyTerm terminalRead = ToTerm("read", "read");
            KeyTerm terminalManage = ToTerm("manage", "manage");

            /* IDENTIFICADOR */
            IdentifierTerminal terminalId = new IdentifierTerminal("identifier", IdOptions.None);

            /* SIMBOLOS */
            KeyTerm terminalComa = ToTerm(",", "coma");
            this.MarkPunctuation(terminalComa);

            /* QUERY */
            QuotedValueLiteral terminalQuery = new QuotedValueLiteral("terminalQuery", "{", "}", TypeCode.String);
            terminalQuery.AstConfig.NodeType = null;
            terminalQuery.AstConfig.DefaultNodeCreator = () => new IdentifierNode();

            /* CADENA */
            StringLiteral terminalCadena = new StringLiteral("cadena", "'", StringOptions.AllowsAllEscapes);

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

            NonTerminal nt_COMMAND_NODE = new NonTerminal("COMMAND_NODE", typeof(CommandNode));
            nt_COMMAND_NODE.AstConfig.NodeType = null;
            nt_COMMAND_NODE.AstConfig.DefaultNodeCreator = () => new CommandNode();

            /* ACTIONS */
            NonTerminal nt_PERMISSION_ACTIONS = new NonTerminal("PERMISSION_ACTIONS", typeof(SpaceActionASTNode));
            nt_PERMISSION_ACTIONS.AstConfig.NodeType = null;
            nt_PERMISSION_ACTIONS.AstConfig.DefaultNodeCreator = () => new SpaceActionASTNode();
            NonTerminal nt_DROP_ACTION = new NonTerminal("DROP_ACTION", typeof(SpaceActionASTNode));
            nt_DROP_ACTION.AstConfig.NodeType = null;
            nt_DROP_ACTION.AstConfig.DefaultNodeCreator = () => new SpaceActionASTNode();
            NonTerminal nt_CREATE_ACTION = new NonTerminal("CREATE_ACTION", typeof(SpaceActionASTNode));
            nt_CREATE_ACTION.AstConfig.NodeType = null;
            nt_CREATE_ACTION.AstConfig.DefaultNodeCreator = () => new SpaceActionASTNode();
            NonTerminal nt_STATUS_ACTIONS = new NonTerminal("STATUS_ACTIONS", typeof(SpaceActionASTNode));
            nt_STATUS_ACTIONS.AstConfig.NodeType = null;
            nt_STATUS_ACTIONS.AstConfig.DefaultNodeCreator = () => new SpaceActionASTNode();
            NonTerminal nt_CREATE_AND_ALTER_ACTIONS = new NonTerminal("CRUD_ACTIONS", typeof(SpaceActionASTNode));
            nt_CREATE_AND_ALTER_ACTIONS.AstConfig.NodeType = null;
            nt_CREATE_AND_ALTER_ACTIONS.AstConfig.DefaultNodeCreator = () => new SpaceActionASTNode();
            /************************************************/

            /* SPACE OBJECTS CATEGORIES */
            NonTerminal nt_SPACE_OBJECTS = new NonTerminal("SPACE_OBJECTS", typeof(SpaceObjectASTNode));
            nt_SPACE_OBJECTS.AstConfig.NodeType = null;
            nt_SPACE_OBJECTS.AstConfig.DefaultNodeCreator = () => new SpaceObjectASTNode();
            NonTerminal nt_SPACE_OBJECTS_FOR_STATUS_PERMISSIONS = new NonTerminal("SPACE_OBJECTS_FOR_STATUS_PERMISSIONS", typeof(SpaceObjectASTNode));
            nt_SPACE_OBJECTS_FOR_STATUS_PERMISSIONS.AstConfig.NodeType = null;
            nt_SPACE_OBJECTS_FOR_STATUS_PERMISSIONS.AstConfig.DefaultNodeCreator = () => new SpaceObjectASTNode();
            NonTerminal nt_SPACE_USER_OR_ROLE = new NonTerminal("SPACE_USER_OR_ROLE", typeof(SpaceObjectASTNode));
            nt_SPACE_USER_OR_ROLE.AstConfig.NodeType = null;
            nt_SPACE_USER_OR_ROLE.AstConfig.DefaultNodeCreator = () => new SpaceObjectASTNode();
            NonTerminal nt_SPACE_STREAM = new NonTerminal("SPACE_STREAM", typeof(SpaceObjectASTNode));
            nt_SPACE_STREAM.AstConfig.NodeType = null;
            nt_SPACE_STREAM.AstConfig.DefaultNodeCreator = () => new SpaceObjectASTNode();
            NonTerminal nt_SPACE_USER = new NonTerminal("SPACE_USER", typeof(SpaceObjectASTNode));
            nt_SPACE_USER.AstConfig.NodeType = null;
            nt_SPACE_USER.AstConfig.DefaultNodeCreator = () => new SpaceObjectASTNode();
            NonTerminal nt_SPACE_SOURCE_OR_ROLE = new NonTerminal("SPACE_SOURCE_OR_ROLE", typeof(SpaceObjectASTNode));
            nt_SPACE_SOURCE_OR_ROLE.AstConfig.NodeType = null;
            nt_SPACE_SOURCE_OR_ROLE.AstConfig.DefaultNodeCreator = () => new SpaceObjectASTNode();
            /************************************************/

            /* PERMISSIONS */
            NonTerminal nt_SPACE_CRUD_PERMISSIONS = new NonTerminal("SPACE_CRUD_PERMISSIONS", typeof(SpacePermissionASTNode));
            nt_SPACE_CRUD_PERMISSIONS.AstConfig.NodeType = null;
            nt_SPACE_CRUD_PERMISSIONS.AstConfig.DefaultNodeCreator = () => new SpacePermissionASTNode();
            NonTerminal nt_SPACE_STATUS_PERMISSIONS = new NonTerminal("SPACE_STATUS_PERMISSIONS", typeof(SpacePermissionASTNode));
            nt_SPACE_STATUS_PERMISSIONS.AstConfig.NodeType = null;
            nt_SPACE_STATUS_PERMISSIONS.AstConfig.DefaultNodeCreator = () => new SpacePermissionASTNode();
            NonTerminal nt_SPACE_ASSIGN_PERMISSIONS = new NonTerminal("SPACE_ASSIGN_PERMISSIONS", typeof(SpacePermissionASTNode));
            nt_SPACE_ASSIGN_PERMISSIONS.AstConfig.NodeType = null;
            nt_SPACE_ASSIGN_PERMISSIONS.AstConfig.DefaultNodeCreator = () => new SpacePermissionASTNode();
            NonTerminal nt_SPACE_READ_PERMISSION = new NonTerminal("SPACE_READ_PERMISSION", typeof(SpacePermissionASTNode));
            nt_SPACE_READ_PERMISSION.AstConfig.NodeType = null;
            nt_SPACE_READ_PERMISSION.AstConfig.DefaultNodeCreator = () => new SpacePermissionASTNode();
            NonTerminal nt_SPACE_PERMISSION = new NonTerminal("SPACE_READ_PERMISSION_WITH_OBJECT", typeof(SpacePermissionWithObjectGroupASTNode));
            nt_SPACE_PERMISSION.AstConfig.NodeType = null;
            nt_SPACE_PERMISSION.AstConfig.DefaultNodeCreator = () => new SpacePermissionWithObjectGroupASTNode();
            /************************************************/

            /* PERMISSIONS WITH OBJECTS */
            NonTerminal nt_SPACE_CRUD_PERMISSION_WITH_OBJECT = new NonTerminal("SPACE_CRUD_PERMISSION_WITH_OBJECT", typeof(SpacePermissionWithObjectASTNode));
            nt_SPACE_CRUD_PERMISSION_WITH_OBJECT.AstConfig.NodeType = null;
            nt_SPACE_CRUD_PERMISSION_WITH_OBJECT.AstConfig.DefaultNodeCreator = () => new SpacePermissionWithObjectASTNode();
            NonTerminal nt_SPACE_STATUS_PERMISSION_WITH_OBJECT = new NonTerminal("SPACE_STATUS_PERMISSION_WITH_OBJECT", typeof(SpacePermissionWithObjectASTNode));
            nt_SPACE_STATUS_PERMISSION_WITH_OBJECT.AstConfig.NodeType = null;
            nt_SPACE_STATUS_PERMISSION_WITH_OBJECT.AstConfig.DefaultNodeCreator = () => new SpacePermissionWithObjectASTNode();
            NonTerminal nt_SPACE_READ_PERMISSION_WITH_OBJECT = new NonTerminal("SPACE_READ_PERMISSION_WITH_OBJECT", typeof(SpacePermissionWithObjectASTNode));
            nt_SPACE_READ_PERMISSION_WITH_OBJECT.AstConfig.NodeType = null;
            nt_SPACE_READ_PERMISSION_WITH_OBJECT.AstConfig.DefaultNodeCreator = () => new SpacePermissionWithObjectASTNode();
            /************************************************/

            /* USER OPTIONS */
            NonTerminal nt_SPACE_USER_OPTION = new NonTerminal("SPACE_USER_OPTION", typeof(SpaceUserOptionASTNode));
            nt_SPACE_USER_OPTION.AstConfig.NodeType = null;
            nt_SPACE_USER_OPTION.AstConfig.DefaultNodeCreator = () => new SpaceUserOptionASTNode();
            /************************************************/

            /* OBJECT WITH IDENTIFIER */
            NonTerminal nt_SPACE_OBJECT_WITH_ID = new NonTerminal("SPACE_OBJECT_WITH_ID", typeof(SpaceObjectWithIdASTNode));
            nt_SPACE_OBJECT_WITH_ID.AstConfig.NodeType = null;
            nt_SPACE_OBJECT_WITH_ID.AstConfig.DefaultNodeCreator = () => new SpaceObjectWithIdASTNode();
            NonTerminal nt_SPACE_STREAM_WITH_ID = new NonTerminal("SPACE_STREAM_WITH_ID", typeof(SpaceObjectWithIdASTNode));
            nt_SPACE_STREAM_WITH_ID.AstConfig.NodeType = null;
            nt_SPACE_STREAM_WITH_ID.AstConfig.DefaultNodeCreator = () => new SpaceObjectWithIdASTNode();
            NonTerminal nt_SPACE_USER_WITH_ID = new NonTerminal("SPACE_USER_WITH_ID", typeof(SpaceObjectWithIdASTNode));
            nt_SPACE_USER_WITH_ID.AstConfig.NodeType = null;
            nt_SPACE_USER_WITH_ID.AstConfig.DefaultNodeCreator = () => new SpaceObjectWithIdASTNode();
            NonTerminal nt_SPACE_OBJECTS_FOR_STATUS_PERMISSIONS_WITH_ID = new NonTerminal("SPACE_OBJECTS_FOR_STATUS_PERMISSIONS_WITH_ID", typeof(SpaceObjectWithIdASTNode));
            nt_SPACE_OBJECTS_FOR_STATUS_PERMISSIONS_WITH_ID.AstConfig.NodeType = null;
            nt_SPACE_OBJECTS_FOR_STATUS_PERMISSIONS_WITH_ID.AstConfig.DefaultNodeCreator = () => new SpaceObjectWithIdASTNode();
            NonTerminal nt_SPACE_USER_OR_ROLE_WITH_ID = new NonTerminal("SPACE_USER_OR_ROLE_WITH_ID", typeof(SpaceObjectWithIdASTNode));
            nt_SPACE_USER_OR_ROLE_WITH_ID.AstConfig.NodeType = null;
            nt_SPACE_USER_OR_ROLE_WITH_ID.AstConfig.DefaultNodeCreator = () => new SpaceObjectWithIdASTNode();
            NonTerminal nt_SPACE_SOURCE_OR_ROLE_WITH_ID = new NonTerminal("SPACE_SOURCE_OR_ROLE_WITH_ID", typeof(SpaceObjectWithIdASTNode));
            nt_SPACE_SOURCE_OR_ROLE_WITH_ID.AstConfig.NodeType = null;
            nt_SPACE_SOURCE_OR_ROLE_WITH_ID.AstConfig.DefaultNodeCreator = () => new SpaceObjectWithIdASTNode();
            /************************************************/

            /* SPACE PERMISSION LIST */
            NonTerminal nt_SPACE_PERMISSION_LIST = new NonTerminal("SPACE_PERMISSION_LIST", typeof(ListASTNode<SpacePermissionWithObjectGroupASTNode, Tuple<SpacePermissionsEnum, SpaceObjectEnum, string>>));
            nt_SPACE_PERMISSION_LIST.AstConfig.NodeType = null;
            nt_SPACE_PERMISSION_LIST.AstConfig.DefaultNodeCreator = () => new ListASTNode<SpacePermissionWithObjectGroupASTNode, Tuple<SpacePermissionsEnum, SpaceObjectEnum, string>>();
            NonTerminal nt_SPACE_USER_OPTION_LIST = new NonTerminal("SPACE_USER_OPTION_LIST", typeof(ListASTNode<SpaceUserOptionASTNode, CommandContext.SpaceUserOption>));
            nt_SPACE_USER_OPTION_LIST.AstConfig.NodeType = null;
            nt_SPACE_USER_OPTION_LIST.AstConfig.DefaultNodeCreator = () => new ListASTNode<SpaceUserOptionASTNode, CommandContext.SpaceUserOption>();
            /************************************************/

            /* CRUD COMMANDS */
            NonTerminal nt_CRUD_COMMANDS = new NonTerminal("CRUD_COMMANDS", typeof(SpaceCRUDCommandASTNode));
            nt_CRUD_COMMANDS.AstConfig.NodeType = null;
            nt_CRUD_COMMANDS.AstConfig.DefaultNodeCreator = () => new SpaceCRUDCommandASTNode();
            NonTerminal nt_DROP_COMMAND = new NonTerminal("DROP_COMMAND", typeof(SpaceDropCommandASTNode));
            nt_DROP_COMMAND.AstConfig.NodeType = null;
            nt_DROP_COMMAND.AstConfig.DefaultNodeCreator = () => new SpaceDropCommandASTNode();
            NonTerminal nt_SIMPLE_CREATE_COMMAND = new NonTerminal("SIMPLE_CREATE_COMMAND", typeof(SpaceSimpleCreateCommandASTNode));
            nt_SIMPLE_CREATE_COMMAND.AstConfig.NodeType = null;
            nt_SIMPLE_CREATE_COMMAND.AstConfig.DefaultNodeCreator = () => new SpaceSimpleCreateCommandASTNode();
            NonTerminal nt_CREATE_AND_ALTER_STREAM_COMMAND = new NonTerminal("CREATE_AND_ALTER_STREAM_COMMAND", typeof(SpaceCreateAndAlterStreamCommandASTNode));
            nt_CREATE_AND_ALTER_STREAM_COMMAND.AstConfig.NodeType = null;
            nt_CREATE_AND_ALTER_STREAM_COMMAND.AstConfig.DefaultNodeCreator = () => new SpaceCreateAndAlterStreamCommandASTNode();
            NonTerminal nt_CREATE_OR_ALTER_USER_COMMAND = new NonTerminal("CREATE_AND_ALTER_USER_COMMAND", typeof(SpaceCreateAndAlterUserCommandASTNode));
            nt_CREATE_OR_ALTER_USER_COMMAND.AstConfig.NodeType = null;
            nt_CREATE_OR_ALTER_USER_COMMAND.AstConfig.DefaultNodeCreator = () => new SpaceCreateAndAlterUserCommandASTNode();
            /************************************************/

            NonTerminal nt_STATUS_COMMANDS = new NonTerminal("STATUS_COMMANDS", typeof(SpaceStatusCommandASTNode));
            nt_STATUS_COMMANDS.AstConfig.NodeType = null;
            nt_STATUS_COMMANDS.AstConfig.DefaultNodeCreator = () => new SpaceStatusCommandASTNode();
            NonTerminal nt_PERMISSIONS_COMMANDS = new NonTerminal("PERMISSIONS_COMMANDS", typeof(SpacePermissionCommandASTNode));
            nt_PERMISSIONS_COMMANDS.AstConfig.NodeType = null;
            nt_PERMISSIONS_COMMANDS.AstConfig.DefaultNodeCreator = () => new SpacePermissionCommandASTNode();

            /* RULES */

            /* COMMAND ACTIONS */

            nt_PERMISSION_ACTIONS.Rule = terminalGrant 
                                        | terminalRevoke 
                                        | terminalDeny;

            nt_STATUS_ACTIONS.Rule = terminalStart 
                                    | terminalStop;

            nt_CREATE_AND_ALTER_ACTIONS.Rule = terminalCreate 
                                                | terminalAlter;

            nt_CREATE_ACTION.Rule = terminalCreate;
            
            nt_DROP_ACTION.Rule = terminalDrop;

            /************************************************/

            /* SPACE OBJECTS CATEGORIES */

            nt_SPACE_OBJECTS.Rule = terminalSource 
                                    | terminalStream 
                                    | terminalRole 
                                    | terminalUser;

            nt_SPACE_OBJECTS_FOR_STATUS_PERMISSIONS.Rule = terminalSource
                                                        | terminalStream;

            nt_SPACE_USER_OR_ROLE.Rule = terminalRole
                                        | terminalUser;

            nt_SPACE_STREAM.Rule = terminalStream;

            nt_SPACE_USER.Rule = terminalUser;

            nt_SPACE_SOURCE_OR_ROLE.Rule = terminalSource
                                            | terminalRole;

            /************************************************/

            /* OBJECT WITH IDENTIFIER */

            nt_SPACE_OBJECT_WITH_ID.Rule = nt_SPACE_OBJECTS + terminalId;

            nt_SPACE_STREAM_WITH_ID.Rule = nt_SPACE_STREAM + terminalId;

            nt_SPACE_USER_WITH_ID.Rule = nt_SPACE_USER + terminalId;

            nt_SPACE_OBJECTS_FOR_STATUS_PERMISSIONS_WITH_ID.Rule = nt_SPACE_OBJECTS_FOR_STATUS_PERMISSIONS + terminalId;

            nt_SPACE_USER_OR_ROLE_WITH_ID.Rule = nt_SPACE_USER_OR_ROLE + terminalId;

            nt_SPACE_SOURCE_OR_ROLE_WITH_ID.Rule = nt_SPACE_SOURCE_OR_ROLE + terminalId;

            /************************************************/

            /* PERMISSIONS */

            nt_SPACE_CRUD_PERMISSIONS.Rule = terminalCreate
                                        | terminalDrop
                                        | terminalAlter;

            nt_SPACE_STATUS_PERMISSIONS.Rule = terminalStart
                                            | terminalStop;

            nt_SPACE_ASSIGN_PERMISSIONS.Rule = terminalGrant
                                                | terminalRevoke
                                                | terminalDeny;

            nt_SPACE_READ_PERMISSION.Rule = terminalRead;

            nt_SPACE_PERMISSION.Rule = nt_SPACE_CRUD_PERMISSION_WITH_OBJECT
                                        | nt_SPACE_STATUS_PERMISSION_WITH_OBJECT
                                        | nt_SPACE_READ_PERMISSION_WITH_OBJECT;

            /************************************************/

            /* PERMISSIONS WITH OBJECTS */

            nt_SPACE_CRUD_PERMISSION_WITH_OBJECT.Rule = nt_SPACE_CRUD_PERMISSIONS + nt_SPACE_OBJECTS;

            nt_SPACE_STATUS_PERMISSION_WITH_OBJECT.Rule = nt_SPACE_STATUS_PERMISSIONS + nt_SPACE_OBJECTS_FOR_STATUS_PERMISSIONS_WITH_ID;

            nt_SPACE_READ_PERMISSION_WITH_OBJECT.Rule = nt_SPACE_READ_PERMISSION + nt_SPACE_OBJECTS_FOR_STATUS_PERMISSIONS_WITH_ID;

            /************************************************/

            /* LIST OF PERMISSIONS */

            nt_SPACE_PERMISSION_LIST.Rule = this.MakePlusRule(nt_SPACE_PERMISSION_LIST, terminalComa, nt_SPACE_PERMISSION);

            /************************************************/

            /* COMMANDS */

            nt_STATUS_COMMANDS.Rule = nt_STATUS_ACTIONS + nt_SPACE_OBJECTS_FOR_STATUS_PERMISSIONS_WITH_ID;

            nt_PERMISSIONS_COMMANDS.Rule = nt_PERMISSION_ACTIONS + nt_SPACE_PERMISSION_LIST + terminalTo + nt_SPACE_USER_OR_ROLE_WITH_ID;

            nt_CRUD_COMMANDS.Rule = nt_CREATE_AND_ALTER_STREAM_COMMAND
                                    | nt_CREATE_OR_ALTER_USER_COMMAND
                                    | nt_SIMPLE_CREATE_COMMAND
                                    | nt_DROP_COMMAND;

            nt_DROP_COMMAND.Rule = nt_DROP_ACTION + nt_SPACE_OBJECT_WITH_ID;

            nt_CREATE_AND_ALTER_STREAM_COMMAND.Rule = nt_CREATE_AND_ALTER_ACTIONS + nt_SPACE_STREAM_WITH_ID + terminalQuery;

            nt_CREATE_OR_ALTER_USER_COMMAND.Rule = nt_CREATE_AND_ALTER_ACTIONS + nt_SPACE_USER_WITH_ID + nt_SPACE_USER_OPTION_LIST;

            nt_SPACE_USER_OPTION.Rule = terminalPassword + terminalCadena
                                        | terminalStatus + terminalUserOptionValue;

            nt_SPACE_USER_OPTION_LIST.Rule = this.MakePlusRule(nt_SPACE_USER_OPTION_LIST, nt_SPACE_USER_OPTION);

            nt_SIMPLE_CREATE_COMMAND.Rule = nt_CREATE_ACTION + nt_SPACE_SOURCE_OR_ROLE_WITH_ID;

            nt_COMMAND_NODE.Rule = nt_STATUS_COMMANDS
                                    | nt_PERMISSIONS_COMMANDS
                                    | nt_CRUD_COMMANDS;

            this.Root = nt_COMMAND_NODE;

            this.LanguageFlags = Irony.Parsing.LanguageFlags.CreateAst;
        }
    }
}
