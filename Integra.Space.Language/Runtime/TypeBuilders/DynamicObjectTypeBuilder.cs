﻿//-----------------------------------------------------------------------
// <copyright file="DynamicObjectTypeBuilder.cs" company="Integra.Space.Language">
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
    /// Dynamic object type builder class.
    /// </summary>
    internal class DynamicObjectTypeBuilder : SpaceTypeBuilder
    {
        /// <summary>
        /// Fields to create in the new type.
        /// </summary>
        private List<FieldNode> listOfFields;

        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicObjectTypeBuilder"/> class.
        /// </summary>
        /// <param name="asmBuilder">Assembly builder.</param>
        /// <param name="queryId">Query identifier.</param>
        /// <param name="listOfFields">List of fields</param>
        public DynamicObjectTypeBuilder(AssemblyBuilder asmBuilder, string queryId, List<FieldNode> listOfFields) : base(asmBuilder, "SpaceDynamicObject_" + queryId, null)
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

        /// <inheritdoc />
        protected override void CreateConstructor(TypeBuilder typeBuilder)
        {
            ConstructorBuilder constructor = typeBuilder.DefineDefaultConstructor(MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName);
        }
    }
}
