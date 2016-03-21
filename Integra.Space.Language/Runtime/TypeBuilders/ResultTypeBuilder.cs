//-----------------------------------------------------------------------
// <copyright file="ResultTypeBuilder.cs" company="Integra.Space.Language">
//     Copyright (c) Integra.Space.Language. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Language.Runtime
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Reflection.Emit;

    /// <summary>
    /// Result type builder
    /// </summary>
    public static class ResultTypeBuilder
    {
        /// <summary>
        /// Doc goes here
        /// </summary>
        /// <param name="queryId">Query identifier</param>
        /// <param name="arrayDeResultados">Projection array item result type</param>
        /// <returns>Type created</returns>
        public static Type CreateResultType(string queryId, Type arrayDeResultados)
        {
            // creo el assembly
            var typeSignature = "SpaceQueryResultDynamicType_" + DateTime.Now.Millisecond;
            var an = new AssemblyName(typeSignature);
            AssemblyBuilder assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(an, AssemblyBuilderAccess.Run);

            // creo el módulo 
            ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule("ResultModule");

            // creo el tipo
            TypeBuilder tb = moduleBuilder.DefineType(typeSignature, TypeAttributes.Public | TypeAttributes.Class | TypeAttributes.AutoClass | TypeAttributes.AnsiClass | TypeAttributes.BeforeFieldInit | TypeAttributes.AutoLayout | TypeAttributes.Serializable, null);

            // establezco la clase padre
            Type parentType = typeof(QueryResult<>).MakeGenericType(arrayDeResultados);
            tb.SetParent(parentType);

            // creo el constructor de la clase
            ConstructorBuilder constructor = tb.DefineConstructor(MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName, CallingConventions.Standard, parentType.GetConstructors()[0].GetParameters().Select(x => x.ParameterType).ToArray());

            // llamo al constructor base
            ConstructorInfo baseConstructor = parentType.GetConstructors()[0];

            // creo el cuerpo de la llamada al constructor base
            ILGenerator ctorIL = constructor.GetILGenerator();
            ctorIL.Emit(OpCodes.Ldarg_0);                   // push "this"
            ctorIL.Emit(OpCodes.Ldarg_1);                   // push the first parameter
            ctorIL.Emit(OpCodes.Ldarg_2);                   // push the second parameter
            ctorIL.Emit(OpCodes.Ldarg_3);                   // push the third parameter
            ctorIL.Emit(OpCodes.Call, baseConstructor);     // llamo al constructor base
            ctorIL.Emit(OpCodes.Ret);
            
            return tb.CreateType();
        }
    }    
}
