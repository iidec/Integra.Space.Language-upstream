//-----------------------------------------------------------------------
// <copyright file="BindingParameter.cs" company="Integra.Space.Language">
//     Copyright (c) Integra.Space.Language. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Language
{
    /// <summary>
    /// Binding parameter class.s
    /// </summary>
    internal class BindingParameter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BindingParameter"/> class.
        /// </summary>
        /// <param name="name">Parameter name.</param>
        /// <param name="value">Parameter value.</param>
        public BindingParameter(string name, object value)
        {
            this.Name = name;
            this.Value = value;
        }

        /// <summary>
        /// Gets the name of the parameter.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the value of the parameter.
        /// </summary>
        public object Value { get; private set; }
    }
}
