//-----------------------------------------------------------------------
// <copyright file="SpaceUserOption.cs" company="Integra.Space.Common">
//     Copyright (c) Integra.Space.Common. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Language.CommandContext
{
    using System.Diagnostics.Contracts;

    /// <summary>
    /// Space user option class node.
    /// </summary>
    internal class SpaceUserOption
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SpaceUserOption"/> class.
        /// </summary>
        /// <param name="option">User option.</param>
        /// <param name="value">User option value.</param>
        public SpaceUserOption(SpaceUserOptionEnum option, object value)
        {
            Contract.Assert(value != null);

            this.Option = option;
            this.Value = value;
        }

        /// <summary>
        /// Gets the user option.
        /// </summary>
        public SpaceUserOptionEnum Option { get; private set; }

        /// <summary>
        /// Gets the user option value.
        /// </summary>
        public object Value { get; private set; }
    }
}
