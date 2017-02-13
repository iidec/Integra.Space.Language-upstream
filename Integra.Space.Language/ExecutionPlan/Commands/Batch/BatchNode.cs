//-----------------------------------------------------------------------
// <copyright file="BatchNode.cs" company="Integra.Space.Language">
//     Copyright (c) Integra.Space.Common. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Language
{
    using System.Collections.Generic;

    /// <summary>
    /// Batch node class.
    /// </summary>
    internal class BatchNode
    {
        /// <summary>
        /// Go command.
        /// </summary>
        private GoCommandNode go;

        /// <summary>
        /// Initializes a new instance of the <see cref="BatchNode"/> class.
        /// </summary>
        public BatchNode()
        {
            this.Commands = new List<SystemCommand>();
            this.go = new GoCommandNode(1, 0, 0, "go");
        }

        /// <summary>
        /// Gets the batch.
        /// </summary>
        public List<SystemCommand> Commands { get; private set; }

        /// <summary>
        /// Gets or sets the go command.
        /// </summary>
        public GoCommandNode Go
        {
            get
            {
                return this.go;
            }

            set
            {
                if (this.go == null)
                {
                    this.go = value;
                }
            }
        }
    }
}
