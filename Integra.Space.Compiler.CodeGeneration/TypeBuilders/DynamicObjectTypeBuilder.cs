//-----------------------------------------------------------------------
// <copyright file="DynamicObjectTypeBuilder.cs" company="Integra.Space.Language">
//     Copyright (c) Integra.Space.Language. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Compiler
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
        /// Random for type name.
        /// </summary>
        [ThreadStatic]
        private static Random r1 = new Random();
        
        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicObjectTypeBuilder"/> class.
        /// </summary>
        /// <param name="asmBuilder">Assembly builder.</param>
        /// <param name="queryId">Query identifier.</param>
        /// <param name="listOfFields">List of fields.</param>
        public DynamicObjectTypeBuilder(AssemblyBuilder asmBuilder, string queryId, List<FieldNode> listOfFields) : base(asmBuilder, string.Format("SpaceDynamicObject_{0}_{1}", queryId, r1.Next(0, 10000)), typeof(object), listOfFields)
        {
        }

        /// <inheritdoc />
        public override Type CreateNewType()
        {
            TypeBuilder typeBuilder = this.CreateType();
            foreach (var field in this.Fields)
            {
                this.CreateProperty(typeBuilder, field, false);
            }

            this.CreateConstructor(typeBuilder);

            return typeBuilder.CreateType();
        }

        /// <inheritdoc />
        protected override void CreateConstructor(TypeBuilder typeBuilder)
        {
            ConstructorBuilder constructor = typeBuilder.DefineDefaultConstructor(MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName);
        }
    }
}
