//-----------------------------------------------------------------------
// <copyright file="ExtractedEventDataTypeBuilder.cs" company="Integra.Space.Language">
//     Copyright (c) Integra.Space.Language. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Language.Runtime
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Reflection.Emit;

    /// <summary>
    /// Extracted event data type builder class.
    /// </summary>
    internal class ExtractedEventDataTypeBuilder : SpaceTypeBuilder
    {
        /// <summary>
        /// Fields to create in the new type.
        /// </summary>
        private List<FieldNode> listOfFields;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExtractedEventDataTypeBuilder"/> class.
        /// </summary>
        /// <param name="asmBuilder">Assembly builder.</param>
        /// <param name="listOfFields">List of fields</param>
        /// <param name="position">Position of the extracted type: 0 (left) or 1 (right).</param>
        public ExtractedEventDataTypeBuilder(AssemblyBuilder asmBuilder, List<FieldNode> listOfFields, int position) : base(asmBuilder, "SpaceExtracted_" + position, typeof(ExtractedEventData))
        {
            this.listOfFields = listOfFields;
        }

        /// <inheritdoc />
        public override Type CreateNewType()
        {
            TypeBuilder typeBuilder = this.CreateType();
            this.CreateConstructor(typeBuilder);

            foreach (var field in this.listOfFields)
            {
                this.CreateProperty(typeBuilder, field);
            }

            return typeBuilder.CreateType();
        }
    }
}
