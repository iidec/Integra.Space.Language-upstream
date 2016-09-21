//-----------------------------------------------------------------------
// <copyright file="CreateObjectNode.cs" company="Integra.Space.Common">
//     Copyright (c) Integra.Space.Common. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Language
{
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using Common;

    /// <summary>
    /// Action over object node class.
    /// </summary>
    /// <typeparam name="TOption">Command option type enumerable.</typeparam>
    internal class CreateObjectNode<TOption> : SpaceCrudCommandNode where TOption : struct, System.IConvertible
    {
        /// <summary>
        /// Command options.
        /// </summary>
        private Dictionary<TOption, object> options;

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateObjectNode{TOption}"/> class.
        /// </summary>
        /// <param name="commandObject">Command object.</param>
        /// <param name="options">Command options.</param>
        /// <param name="line">Line of the evaluated sentence.</param>
        /// <param name="column">Column evaluated sentence column.</param>
        /// <param name="nodeText">Text of the actual node.</param>
        /// <param name="schemaName">Schema name for the command execution.</param>
        /// <param name="databaseName">Database name for the command execution.</param>
        public CreateObjectNode(CommandObject commandObject, Dictionary<TOption, object> options, int line, int column, string nodeText, string schemaName, string databaseName) : base(ActionCommandEnum.Create, commandObject, line, column, nodeText, schemaName, databaseName)
        {
            Contract.Assert(options != null);
            this.options = options;
        }

        /// <summary>
        /// Gets the command options.
        /// </summary>
        public Dictionary<TOption, object> Options
        {
            get
            {
                return this.options;
            }
        }
    }
}
