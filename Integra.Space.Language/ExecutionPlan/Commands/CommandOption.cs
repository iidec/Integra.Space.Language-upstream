//-----------------------------------------------------------------------
// <copyright file="CommandOption.cs" company="Integra.Space.Language">
//     Copyright (c) Integra.Space.Common. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Language
{
    using System.Diagnostics.Contracts;

    /// <summary>
    /// Command option class.
    /// </summary>
    /// <typeparam name="T">Option type enumerable.</typeparam>
    internal class CommandOption<T> where T : struct, System.IConvertible
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommandOption{T}"/> class.
        /// </summary>
        /// <param name="option">User option.</param>
        /// <param name="value">User option value.</param>
        public CommandOption(T option, object value)
        {
            Contract.Assert(value != null);

            this.Option = option;
            this.Value = value;
        }

        /// <summary>
        /// Gets the user option.
        /// </summary>
        public T Option { get; private set; }

        /// <summary>
        /// Gets the user option value.
        /// </summary>
        public object Value { get; private set; }
    }
}
