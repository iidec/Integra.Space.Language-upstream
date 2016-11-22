//-----------------------------------------------------------------------
// <copyright file="SourceTypeBuilder.cs" company="Integra.Space.Language">
//     Copyright (c) Integra.Space.Language. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Language.Runtime
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Reflection.Emit;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Source type builder class.
    /// </summary>
    internal class SourceTypeBuilder : Space.Language.Runtime.SpaceTypeBuilder
    {
        /// <summary>
        /// Finds of the new type.
        /// </summary>
        private IEnumerable<FieldNode> fields;

        /// <summary>
        /// Initializes a new instance of the <see cref="SourceTypeBuilder"/> class.
        /// </summary>
        /// <param name="asmBuilder">Assembly builder.</param>
        /// <param name="typeSignature">Name of the new type.</param>
        /// <param name="parentType">Parent type of the new type</param>
        /// <param name="fields">Fields of the new type.</param>
        public SourceTypeBuilder(AssemblyBuilder asmBuilder, string typeSignature, Type parentType, IEnumerable<FieldNode> fields) : base(asmBuilder, typeSignature, parentType)
        {
            Contract.Assert(asmBuilder != null);
            Contract.Assert(!string.IsNullOrWhiteSpace(typeSignature));
            Contract.Assert(fields != null);
            Contract.Assert(parentType != null);

            this.fields = fields;
        }

        /// <inheritdoc />
        public override Type CreateNewType()
        {
            TypeBuilder typeBuilder = this.CreateType();
            this.CreateConstructor(typeBuilder);

            foreach (var field in this.fields)
            {
                this.CreateProperty(typeBuilder, field);
            }

            Type newType = typeBuilder.CreateType();
            return newType;
        }
    }
}
