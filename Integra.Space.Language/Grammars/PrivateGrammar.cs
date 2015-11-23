//-----------------------------------------------------------------------
// <copyright file="PrivateGrammar.cs" company="Integra.Space.Language">
//     Copyright (c) Integra.Space.Language. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Language.Grammars
{
    using Integra.Space.Language.ASTNodes.Commands;
    using Integra.Space.Language.ASTNodes.Constants;
    using Integra.Space.Language.ASTNodes.Lists;
    using Integra.Space.Language.ASTNodes.Root;
    using Integra.Space.Language.ASTNodes.SystemViews;
    using Irony.Interpreter;
    using Irony.Parsing;

    /// <summary>
    /// EQLGrammar grammar for the commands and the predicates 
    /// </summary>
    [Language("PrivateGrammar", "0.1", "")]
    internal sealed class PrivateGrammar : InterpretedLanguageGrammar
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PrivateGrammar"/> class
        /// </summary>
        public PrivateGrammar()
            : base(false)
        {
            this.Grammar(false);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PrivateGrammar"/> class
        /// </summary>
        /// <param name="prueba">flag for tests</param>
        public PrivateGrammar(bool prueba)
            : base(false)
        {
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
            KeyTerm terminalBoot = ToTerm("boot", "boot");
            KeyTerm terminalEngine = ToTerm("engine", "engine");
            KeyTerm terminalSelect = ToTerm("select", "select");
            KeyTerm terminalFrom = ToTerm("from", "from");
            KeyTerm terminalWhere = ToTerm("where", "where");

            /* CONSTANTES E IDENTIFICADORES */
            RegexBasedTerminal terminalId = new RegexBasedTerminal("[a-zA-Z]+([a-zA-Z]|[0-9]|[_])*");
            terminalId.AstConfig.NodeType = null;
            terminalId.AstConfig.DefaultNodeCreator = () => new IdentifierNode();
            
            /*** NO TERMINALES DE LA GRAMATICA ***/
            NonTerminal nt_BOOT_ENGINE = new NonTerminal("BOOT_ENGINE", typeof(EngineActionsNode));
            nt_BOOT_ENGINE.AstConfig.NodeType = null;
            nt_BOOT_ENGINE.AstConfig.DefaultNodeCreator = () => new EngineActionsNode();
            NonTerminal nt_COMMAND_NODE = new NonTerminal("COMMAND_NODE", typeof(CommandNode));
            nt_COMMAND_NODE.AstConfig.NodeType = null;
            nt_COMMAND_NODE.AstConfig.DefaultNodeCreator = () => new CommandNode();
            NonTerminal nt_COMMAND_LIST = new NonTerminal("COMMAND_LIST", typeof(CommandListNode));
            nt_COMMAND_LIST.AstConfig.NodeType = null;
            nt_COMMAND_LIST.AstConfig.DefaultNodeCreator = () => new CommandListNode();

            /* BOOT ENGINE */
            nt_BOOT_ENGINE.Rule = terminalBoot + terminalEngine;
            /* **************************** */
            /* COMANDOS */
            nt_COMMAND_NODE.Rule = nt_BOOT_ENGINE;
            /* **************************** */
            /* LISTA DE COMANDOS */
            nt_COMMAND_LIST.Rule = nt_COMMAND_LIST + nt_COMMAND_NODE
                                    | nt_COMMAND_NODE;
            /* **************************** */
          
            if (prueba)
            {
                this.Root = nt_COMMAND_LIST;
            }
            else
            {
                this.Root = nt_COMMAND_LIST;
            }

            this.LanguageFlags = Irony.Parsing.LanguageFlags.CreateAst;
        }
    }
}
