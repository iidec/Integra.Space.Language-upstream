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
        private int position;

        /// <summary>
        /// parameter type.
        /// </summary>
        private Type type;

        /// <summary>
        /// Initializes a new instance of the <see cref="ScopeParameter"/> class.
        /// </summary>
        /// <param name="position">Parameter position.</param>
        /// <param name="type">Parameter type.</param>
        public ScopeParameter(int position, Type type)
        {
            this.position = position;
            this.type = type;
        }

        /// <summary>
        /// Gets the parameter name.
        /// </summary>
        public int Position
        {
            get
            {
                return this.position;
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
