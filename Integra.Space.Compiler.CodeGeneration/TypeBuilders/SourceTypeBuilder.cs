//-----------------------------------------------------------------------
// <copyright file="SourceTypeBuilder.cs" company="Integra.Space.Language">
//     Copyright (c) Integra.Space.Language. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Compiler
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Reflection.Emit;

    /// <summary>
    /// Source type builder class.
    /// </summary>
    internal class SourceTypeBuilder : SpaceTypeBuilder
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SourceTypeBuilder"/> class.
        /// </summary>
        /// <param name="asmBuilder">Assembly builder.</param>
        /// <param name="typeSignature">Name of the new type.</param>
        /// <param name="parentType">Parent type of the new type</param>
        /// <param name="fields">Fields of the new type.</param>
        public SourceTypeBuilder(AssemblyBuilder asmBuilder, string typeSignature, Type parentType, IEnumerable<FieldNode> fields) : base(asmBuilder, typeSignature, parentType, fields)
        {
        }

        /// <inheritdoc />
        public override Type CreateNewType()
        {
            TypeBuilder typeBuilder = this.CreateType();
            foreach (var field in this.Fields)
            {
                this.CreateProperty(typeBuilder, field, true);
            }

            this.CreateConstructor(typeBuilder);            
            Type newType = typeBuilder.CreateType();
            return newType;
        }
    }
}
