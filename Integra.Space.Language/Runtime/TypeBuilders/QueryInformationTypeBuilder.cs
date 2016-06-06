//-----------------------------------------------------------------------
// <copyright file="QueryInformationTypeBuilder.cs" company="Integra.Space.Language">
//     Copyright (c) Integra.Space.Language. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Language.Runtime
{
    using System;
    using System.Reflection;
    using System.Reflection.Emit;

    /// <summary>
    /// Query information type builder class.
    /// </summary>
    internal class QueryInformationTypeBuilder : SpaceTypeBuilder
    {
        /// <summary>
        /// Type created for the actual query. This type inherits from SpaceQuery abstract class.
        /// </summary>
        private Type queryType;

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryInformationTypeBuilder"/> class.
        /// </summary>
        /// <param name="asmBuilder">Assembly builder.</param>
        /// <param name="queryId">Query identifier.</param>
        /// <param name="queryType">Type created for the query.</param>
        public QueryInformationTypeBuilder(AssemblyBuilder asmBuilder, string queryId, Type queryType) : base(asmBuilder, "QueryInformartion_" + queryId, null)
        {
            this.queryType = queryType;
        }

        /// <inheritdoc />
        public override Type CreateNewType()
        {
            TypeBuilder typeBuilder = this.CreateType();
            this.CreateConstructor(typeBuilder);

            typeBuilder.AddInterfaceImplementation(typeof(IQueryInformation));

            var getQueryTypeBuilder = typeBuilder.DefineMethod("GetQueryType", MethodAttributes.Public | MethodAttributes.Final | MethodAttributes.Virtual);
            getQueryTypeBuilder.SetReturnType(typeof(Type));

            var getQueryTypeIL = getQueryTypeBuilder.GetILGenerator();
            LocalBuilder localVariableType = getQueryTypeIL.DeclareLocal(typeof(Type));
            getQueryTypeIL.Emit(OpCodes.Nop);
            getQueryTypeIL.Emit(OpCodes.Ldtoken, this.queryType);
            getQueryTypeIL.Emit(OpCodes.Call, typeof(Type).GetMethod("GetTypeFromHandle"));
            getQueryTypeIL.Emit(OpCodes.Stloc_0);
            getQueryTypeIL.Emit(OpCodes.Ldloc_0);
            getQueryTypeIL.Emit(OpCodes.Ret);

            var getReferencedAssembliesBuilder = typeBuilder.DefineMethod("GetReferencedAssemblies", MethodAttributes.Public | MethodAttributes.Final | MethodAttributes.Virtual);
            getReferencedAssembliesBuilder.SetReturnType(typeof(AssemblyName[]));

            var getReferencedAssembliesIL = getReferencedAssembliesBuilder.GetILGenerator();
            LocalBuilder localVariableAssemblyNames = getReferencedAssembliesIL.DeclareLocal(typeof(AssemblyName[]));
            getReferencedAssembliesIL.Emit(OpCodes.Nop);
            getReferencedAssembliesIL.Emit(OpCodes.Ldtoken, this.queryType);
            getReferencedAssembliesIL.Emit(OpCodes.Call, typeof(Type).GetMethod("GetTypeFromHandle"));
            getReferencedAssembliesIL.Emit(OpCodes.Callvirt, typeof(Type).GetProperty("Assembly").GetGetMethod());
            getReferencedAssembliesIL.Emit(OpCodes.Callvirt, typeof(Assembly).GetMethod("GetReferencedAssemblies"));
            getReferencedAssembliesIL.Emit(OpCodes.Stloc_0);
            getReferencedAssembliesIL.Emit(OpCodes.Ldloc_0);
            getReferencedAssembliesIL.Emit(OpCodes.Ret);

            return typeBuilder.CreateType();
        }

        /// <inheritdoc />
        protected override void CreateConstructor(TypeBuilder typeBuilder)
        {
            ConstructorBuilder constructor = typeBuilder.DefineDefaultConstructor(MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName);
        }
    }
}
