//-----------------------------------------------------------------------
// <copyright file="GoCommandNode.cs" company="Integra.Space.Language">
//     Copyright (c) Integra.Space.Common. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Language
{
    using System.Collections.Generic;
    using Common;

    /// <summary>
    /// Go command object class.
    /// </summary>
    internal sealed class GoCommandNode : CompiledCommand
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GoCommandNode"/> class.
        /// </summary>
        /// <param name="counter">Number of times the batch will be executed.</param>
        /// <param name="line">Line of the evaluated sentence.</param>
        /// <param name="column">Column evaluated sentence column.</param>
        /// <param name="nodeText">Text of the actual node.</param>
        public GoCommandNode(int counter, int line, int column, string nodeText) : base(ActionCommandEnum.Go, new HashSet<CommandObject>(new CommandObjectComparer()), null, line, column, nodeText)
        {
            this.Counter = counter;
        }

        /// <summary>
        /// Gets the number of times a batch will be executed.
        /// </summary>
        public int Counter { get; private set; }
    }
}
