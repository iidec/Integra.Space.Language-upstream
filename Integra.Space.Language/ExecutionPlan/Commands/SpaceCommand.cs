//-----------------------------------------------------------------------
// <copyright file="SpaceCommand.cs" company="Integra.Space.Common">
//     Copyright (c) Integra.Space.Common. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Language
{
    using System.Collections.Generic;

    /// <summary>
    /// Command action node class.
    /// </summary>
    internal abstract class SpaceCommand : ISpaceCommand
    {
        /// <summary>
        /// Line of the evaluated sentence.
        /// </summary>
        private int line;

        /// <summary>
        /// Column of the evaluated sentence.
        /// </summary>
        private int column;

        /// <summary>
        /// Text of the actual command.
        /// </summary>
        private string commandText;

        /// <summary>
        /// Initializes a new instance of the <see cref="SpaceCommand"/> class.
        /// </summary>
        /// <param name="line">Line of the evaluated sentence.</param>
        /// <param name="column">Column evaluated sentence column.</param>
        /// <param name="commandText">Text of the actual node.</param>
        public SpaceCommand(int line, int column, string commandText)
        {
            this.line = line;
            this.column = column;
            this.commandText = commandText;
        }

        /// <summary>
        /// Gets the line of the evaluated sentence
        /// </summary>
        public int Line
        {
            get
            {
                return this.line;
            }
        }

        /// <summary>
        /// Gets the evaluated sentence column
        /// </summary>
        public int Column
        {
            get
            {
                return this.column;
            }
        }

        /// <summary>
        /// Gets the text of the actual node
        /// </summary>
        public string CommandText
        {
            get
            {
                return this.commandText;
            }
        }
    }
}
