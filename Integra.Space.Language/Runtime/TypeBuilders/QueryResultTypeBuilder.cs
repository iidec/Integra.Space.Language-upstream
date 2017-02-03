//-----------------------------------------------------------------------
// <copyright file="QueryResultTypeBuilder.cs" company="Integra.Space.Language">
//     Copyright (c) Integra.Space.Language. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Language.Runtime
{
    using System;
    using System.Linq;
    using System.Reflection;
    using System.Reflection.Emit;

    /// <summary>
    /// Query result type builder class.
    /// </summary>
    internal class QueryResultTypeBuilder : SpaceTypeBuilder
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="QueryResultTypeBuilder"/> class.
        /// </summary>
        /// <param name="asmBuilder">Assembly builder.</param>
        /// <param name="queryId">Query identifier.</param>
        /// <param name="resultArrayType">Type of the result array.</param>
        public QueryResultTypeBuilder(AssemblyBuilder asmBuilder, string queryId, Type resultArrayType) : base(asmBuilder, "SpaceQueryResult_" + queryId, typeof(QueryResult<>).MakeGenericType(resultArrayType))
        {
        }

        /// <inheritdoc />
        public override Type CreateNewType()
        {
            TypeBuilder typeBuilder = this.CreateType();
            this.CreateConstructor(typeBuilder);

            return typeBuilder.CreateType();
        }
        
        /// <inheritdoc />
        protected override void CreateConstructor(TypeBuilder typeBuilder)
        {
            // creo el constructor de la clase
            ConstructorBuilder constructor = typeBuilder.DefineConstructor(MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName, CallingConventions.Standard, this.ParentType.GetConstructors()[0].GetParameters().Select(x => x.ParameterType).ToArray());

            // llamo al constructor base
            ConstructorInfo baseConstructor = this.ParentType.GetConstructors()[0];

            // creo el cuerpo de la llamada al constructor base
            ILGenerator ctorIL = constructor.GetILGenerator();
            ctorIL.Emit(OpCodes.Ldarg_0);                   // push "this"
            ctorIL.Emit(OpCodes.Ldarg_1);                   // push the first parameter
            ctorIL.Emit(OpCodes.Ldarg_2);                   // push the second parameter
            ctorIL.Emit(OpCodes.Ldarg_3);                   // push the third parameter
            ctorIL.Emit(OpCodes.Call, baseConstructor);     // llamo al constructor base
            ctorIL.Emit(OpCodes.Ret);
        }
    }
}
