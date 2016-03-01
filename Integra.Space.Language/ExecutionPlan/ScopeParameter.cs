//-----------------------------------------------------------------------
// <copyright file="ScopeParameter.cs" company="Ingetra.Vision.Language">
//     Copyright (c) Ingetra.Vision.Language. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Language
{
    using System;

    /// <summary>
    /// Scope parameter class
    /// </summary>
    internal sealed class ScopeParameter
    {
        /// <summary>
        /// parameter name.
        /// </summary>
        private string name;

        /// <summary>
        /// parameter type.
        /// </summary>
        private Type type;

        /// <summary>
        /// Initializes a new instance of the <see cref="ScopeParameter"/> class.
        /// </summary>
        /// <param name="name">Parameter name.</param>
        /// <param name="type">Parameter type.</param>
        public ScopeParameter(string name, Type type)
        {
            this.name = name;
            this.type = type;
        }

        /// <summary>
        /// Gets the parameter name.
        /// </summary>
        public string Name
        {
            get
            {
                return this.name;
            }
        }

        /// <summary>
        /// Gets the parameter type.
        /// </summary>
        public Type Type
        {
            get
            {
                return this.type;
            }
        }
    }
}
