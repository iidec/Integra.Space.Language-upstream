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
            ctorIL.Emit(OpCodes.Ldarg_3);                   // push the second parameter
            ctorIL.Emit(OpCodes.Call, baseConstructor);     // llamo al constructor base
            ctorIL.Emit(OpCodes.Ret);
            
            /*
            // obtengo el método Serialize del padre
            MethodInfo parentSerialize = parentType.GetMethod("Serialize", new Type[] { typeof(IQueryResultWriter) });

            // defino el cuerpo que sobreescribirá al cuerpo del método Serialize
            MethodBuilder serializeMethod = tb.DefineMethod("Serialize", MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName, CallingConventions.Standard, null, new Type[] { typeof(IQueryResultWriter) });

            // defino el parámetro de tipo IQueryResultWriter
            serializeMethod.DefineParameter(1, ParameterAttributes.In, "writer");
            
            // obtengo los métodos de escritura a utilizar
            MethodInfo writeStartQueryResultMethod = typeof(IQueryResultWriter).GetMethods().Where(x => x.Name == "WriteStartQueryResult").Single();
            MethodInfo writeEndQueryResultMethod = typeof(IQueryResultWriter).GetMethods().Where(x => x.Name == "WriteEndQueryResult").Single();
            MethodInfo writeValueMethod = writeValueMethod = typeof(IQueryResultWriter).GetMethods().Where(x => x.Name == "WriteValue" && x.GetParameters()[0].ParameterType == typeof(string)).Single();

            // defino el cuerpo del metodo Serialize
            ILGenerator serializeIL = serializeMethod.GetILGenerator();
            serializeIL.Emit(OpCodes.Ldarg_1);                   // push the first parameter
            serializeIL.Emit(OpCodes.Call, writeStartQueryResultMethod);   // llamo al método WriteStartQueryResult del serializador
            serializeIL.Emit(OpCodes.Ldarg_1);                   // push the first parameter
            serializeIL.Emit(OpCodes.Ldstr, "1.0");             // format version
            serializeIL.Emit(OpCodes.Call, writeValueMethod);   // llamo al método WriteStartQueryResult del serializador
            serializeIL.Emit(OpCodes.Ldarg_0);                   // push "this"
            serializeIL.Emit(OpCodes.Ldarg_1);                   // push the first parameter
            serializeIL.Emit(OpCodes.Call, parentSerialize);    // llamo al método base para que imprima el nombre del query

            // obtengo la propiedad resultado
            MethodInfo getResult = parentType.GetMethod("get_Result");

            // obtengo el metodo Serialize del EventResult
            MethodInfo serializeMethodOfEventResult = arrayDeResultados.GetMethod("Serialize", new Type[] { typeof(IQueryResultWriter) });

            // empieza el loop              
            // preparing locals
            var i = serializeIL.DeclareLocal(typeof(int));

            // Preparing labels
            var loopCondition = serializeIL.DefineLabel();
            var loopIterator = serializeIL.DefineLabel();
            var returnLabel = serializeIL.DefineLabel();
            var loopBody = serializeIL.DefineLabel();

            // Writing body
            // i = 0
            serializeIL.Emit(OpCodes.Ldc_I4_0);
            serializeIL.Emit(OpCodes.Stloc, i);

            // jump to loop condition
            serializeIL.Emit(OpCodes.Br_S, loopCondition);

            // begin loop body
            serializeIL.MarkLabel(loopBody);

            serializeIL.Emit(OpCodes.Ldarg_0); // omit if 'knownTypes' is static
            serializeIL.Emit(OpCodes.Callvirt, getResult); // use 'Ldsfld' if 'knownTypes' is static
            serializeIL.Emit(OpCodes.Ldloc, i);
            serializeIL.Emit(OpCodes.Ldelem_Ref);
            serializeIL.Emit(OpCodes.Ldarg_1);                   // push the first parameter
            serializeIL.Emit(OpCodes.Call, serializeMethodOfEventResult);
                        
            serializeIL.MarkLabel(loopIterator);

            // i = i + 1
            serializeIL.Emit(OpCodes.Ldloc, i);
            serializeIL.Emit(OpCodes.Ldc_I4_1);
            serializeIL.Emit(OpCodes.Add);
            serializeIL.Emit(OpCodes.Stloc, i);

            // begin loop condition
            serializeIL.MarkLabel(loopCondition);

            // if (i < knownTypes.Length) jump to loop body
            serializeIL.Emit(OpCodes.Ldloc, i);
            serializeIL.Emit(OpCodes.Ldarg_0); // omit if 'knownTypes' is static
            serializeIL.Emit(OpCodes.Callvirt, getResult); // use 'Ldsfld' if 'knownTypes' is static
            serializeIL.Emit(OpCodes.Ldlen);
            serializeIL.Emit(OpCodes.Conv_I4);
            serializeIL.Emit(OpCodes.Blt_S, loopBody);

            // return
            serializeIL.MarkLabel(returnLabel);

            serializeIL.Emit(OpCodes.Ldarg_1);                   // push the first parameter
            serializeIL.Emit(OpCodes.Call, writeEndQueryResultMethod);   // llamo al método writeEndQueryResult del serializador

            serializeIL.Emit(OpCodes.Ret);
            */

            // tb.DefineMethodOverride(serializeMethod, parentSerialize);
            return tb.CreateType();
        }
    }    
}
