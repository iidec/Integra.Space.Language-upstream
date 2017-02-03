//-----------------------------------------------------------------------
// <copyright file="AlterObjectNode.cs" company="Integra.Space.Common">
//     Copyright (c) Integra.Space.Common. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Language
{
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using Common;

    /// <summary>
    /// Action over object node class.
    /// </summary>
    /// <typeparam name="TOption">Command option type enumerable.</typeparam>
    internal abstract class AlterObjectNode<TOption> : SpaceCrudCommandNode where TOption : struct, System.IConvertible
    {
        /// <summary>
        /// Command options.
        /// </summary>
        private Dictionary<TOption, object> options;

        /// <summary>
        /// Initializes a new instance of the <see cref="AlterObjectNode{TOption}"/> class.
        /// </summary>
        /// <param name="commandObject">Command object.</param>
        /// <param name="options">Command options.</param>
        /// <param name="line">Line of the evaluated sentence.</param>
        /// <param name="column">Column evaluated sentence column.</param>
        /// <param name="nodeText">Text of the actual node.</param>
        public AlterObjectNode(CommandObject commandObject, Dictionary<TOption, object> options, int line, int column, string nodeText) : base(ActionCommandEnum.Alter, commandObject, line, column, nodeText)
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
