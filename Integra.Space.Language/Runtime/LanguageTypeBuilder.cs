//-----------------------------------------------------------------------
// <copyright file="LanguageTypeBuilder.cs" company="Integra.Space.Language">
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
    /// Language type builder class
    /// </summary>
    public static class LanguageTypeBuilder
    {
        /// <summary>
        /// Create a new object based in the created type.
        /// </summary>
        /// <param name="listOfFields">List of fields.</param>
        /// <param name="parentType">Parent type.</param>
        /// <param name="queryWriter">Query writer</param>
        /// <returns>The new object.</returns>
        public static object CreateNewObject(List<FieldNode> listOfFields, Type parentType, Type queryWriter)
        {
            return Activator.CreateInstance(CompileResultType(listOfFields, parentType));
        }

        /// <summary>
        /// Creates a new type based in the list of fields.
        /// </summary>
        /// <param name="listOfFields">List of fields.</param>
        /// <returns>The created type.</returns>
        public static Type CompileResultType(List<FieldNode> listOfFields)
        {
            TypeBuilder tb = GetTypeBuilder();
            ConstructorBuilder constructor = tb.DefineDefaultConstructor(MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName);

            // creo la lista de campos antes de volver a crear propiedades
            List<Tuple<string, Type, FieldBuilder>> fieldList = new List<Tuple<string, Type, FieldBuilder>>();

            // The list contains a dynamic object with fields FieldName of type string and FieldType of type Type
            foreach (var field in listOfFields)
            {
                CreateProperty(tb, field.FieldName, field.FieldType, fieldList);
            }

            Type objectType = tb.CreateType();
            return objectType;
        }

        /// <summary>
        /// Creates a new type based in the list of fields.
        /// </summary>
        /// <param name="listOfFields">List of fields.</param>
        /// <param name="parentType">Event result type for the projection.</param>
        /// <returns>The created type.</returns>
        public static Type CompileResultType(List<FieldNode> listOfFields, Type parentType)
        {
            TypeBuilder tb = GetTypeBuilder();
            tb.SetParent(parentType);
            ConstructorBuilder constructor = tb.DefineConstructor(MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName, CallingConventions.Standard, parentType.GetProperties().Select(x => x.PropertyType).ToArray());

            var baseConstructor = parentType.GetConstructor(parentType.GetProperties().Select(x => x.PropertyType).ToArray());

            ILGenerator ctorIL = constructor.GetILGenerator();
            ctorIL.Emit(OpCodes.Ldarg_0);                // push "this"
            ctorIL.Emit(OpCodes.Call, baseConstructor);
            ctorIL.Emit(OpCodes.Ret);

            // creo la lista de campos antes de volver a crear propiedades
            List<Tuple<string, Type, FieldBuilder>> fieldList = new List<Tuple<string, Type, FieldBuilder>>();

            // The list contains a dynamic object with fields FieldName of type string and FieldType of type Type
            foreach (var field in listOfFields)
            {
                CreateProperty(tb, field.FieldName, field.FieldType, fieldList);
            }

            // creo el metodo Serialize
            CreateSerializeMethod(tb, parentType, fieldList);

            Type objectType = tb.CreateType();
            return objectType;
        }

        /// <summary>
        /// Gets the type builder.
        /// </summary>
        /// <returns>Type builder.</returns>
        private static TypeBuilder GetTypeBuilder()
        {
            var typeSignature = "SpaceDynamicType_" + DateTime.Now.Millisecond;
            var an = new AssemblyName(typeSignature);
            AssemblyBuilder assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(an, AssemblyBuilderAccess.Run);
            ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule("MainModule");

            // The Type Attributes.Serializable is new
            TypeBuilder tb = moduleBuilder.DefineType(typeSignature, TypeAttributes.Public | TypeAttributes.Class | TypeAttributes.AutoClass | TypeAttributes.AnsiClass | TypeAttributes.BeforeFieldInit | TypeAttributes.AutoLayout | TypeAttributes.Serializable, null);
            
            return tb;
        }

        /// <summary>
        /// Creates a new property for the new type.
        /// </summary>
        /// <param name="tb">The type builder.</param>
        /// <param name="propertyName">The property name.</param>
        /// <param name="propertyType">The property type.</param>
        /// <param name="fieldList">List of fields.</param>
        private static void CreateProperty(TypeBuilder tb, string propertyName, Type propertyType, List<Tuple<string, Type, FieldBuilder>> fieldList)
        {
            string fieldName = "_" + propertyName;
            FieldBuilder fieldBuilder = tb.DefineField(fieldName, propertyType, FieldAttributes.Private);
            fieldList.Add(new Tuple<string, Type, FieldBuilder>(fieldName, propertyType, fieldBuilder));

            PropertyBuilder propertyBuilder = tb.DefineProperty(propertyName, PropertyAttributes.HasDefault, propertyType, null);

            MethodBuilder getPropMthdBldr = tb.DefineMethod("get_" + propertyName, MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig, propertyType, Type.EmptyTypes);
            ILGenerator getIl = getPropMthdBldr.GetILGenerator();

            getIl.Emit(OpCodes.Ldarg_0);
            getIl.Emit(OpCodes.Ldfld, fieldBuilder);
            getIl.Emit(OpCodes.Ret);

            MethodBuilder setPropMthdBldr = tb.DefineMethod("set_" + propertyName, MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig, null, new[] { propertyType });
            ILGenerator setIl = setPropMthdBldr.GetILGenerator();

            Label modifyProperty = setIl.DefineLabel();
            Label exitSet = setIl.DefineLabel();

            setIl.MarkLabel(modifyProperty);
            setIl.Emit(OpCodes.Ldarg_0);
            setIl.Emit(OpCodes.Ldarg_1);
            setIl.Emit(OpCodes.Stfld, fieldBuilder);

            setIl.Emit(OpCodes.Nop);
            setIl.MarkLabel(exitSet);
            setIl.Emit(OpCodes.Ret);

            propertyBuilder.SetGetMethod(getPropMthdBldr);
            propertyBuilder.SetSetMethod(setPropMthdBldr);
        }

        /// <summary>
        /// Create serialize method.
        /// </summary>
        /// <param name="tb">Type builder.</param>
        /// <param name="parentType">Parent type.</param>
        /// <param name="fieldList">List of fields.</param>
        private static void CreateSerializeMethod(TypeBuilder tb, Type parentType, List<Tuple<string, Type, FieldBuilder>> fieldList)
        {
            // obtengo el método Serialize del padre
            MethodInfo parentSerialize = parentType.GetMethod("Serialize", new Type[] { typeof(IQueryResultWriter) });

            // defino el cuerpo que sobreescribirá al cuerpo del método Serialize
            // MethodBuilder serializeMethod = tb.DefineMethod("Serialize", MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName, CallingConventions.Standard, null, new Type[] { typeof(IQueryResultWriter) });
            MethodBuilder serializeMethod = tb.DefineMethod("Serialize", MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.NewSlot | MethodAttributes.Virtual | MethodAttributes.Final, CallingConventions.HasThis, null, new Type[] { typeof(IQueryResultWriter) });

            // defino el parámetro de tipo IQueryResultWriter
            serializeMethod.DefineParameter(1, ParameterAttributes.In, "writer");

            tb.DefineMethodOverride(serializeMethod, parentSerialize);

            // definición del método WriteValue de IQueryResultWriter
            MethodInfo writeStartQueryResultRowMethod = typeof(IQueryResultWriter).GetMethods().Where(x => x.Name == "WriteStartQueryResultRow").Single();
            MethodInfo writeEndQueryResultRowMethod = typeof(IQueryResultWriter).GetMethods().Where(x => x.Name == "WriteEndQueryResultRow").Single();

            // defino el cuerpo del metodo Serialize
            ILGenerator serializeIL = serializeMethod.GetILGenerator();
            serializeIL.Emit(OpCodes.Ldarg_0);                   // push "this"
            serializeIL.Emit(OpCodes.Ldarg_1);                   // push the first parameter
            serializeIL.Emit(OpCodes.Call, parentSerialize);    // llamo al método base para que imprima el nombre del query

            MethodInfo writeValueMethod = null;
            foreach (Tuple<string, Type, FieldBuilder> t in fieldList)
            {
                writeValueMethod = typeof(IQueryResultWriter).GetMethods().Where(x => x.Name == "WriteValue" && x.GetParameters()[0].ParameterType == ToPrimitiveNullable(t.Item2)).Single();

                serializeIL.Emit(OpCodes.Ldarg_1);                   // push the first parameter
                serializeIL.Emit(OpCodes.Ldarg_0);
                serializeIL.Emit(OpCodes.Ldfld, t.Item3);
                serializeIL.Emit(OpCodes.Call, writeValueMethod);   // llamo al método WriteValue del serializador 
            }
            
            serializeIL.Emit(OpCodes.Ret);
        }

        /// <summary>
        /// check if is a numeric type
        /// </summary>
        /// <param name="type">type to check</param>
        /// <returns>true if is numeric type</returns>
        private static Type ToPrimitiveNullable(Type type)
        {
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Boolean:
                case TypeCode.Char:
                case TypeCode.SByte:
                case TypeCode.Byte:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                case TypeCode.Single:
                case TypeCode.Double:
                case TypeCode.Decimal:
                    return typeof(Nullable<>).MakeGenericType(type);
            }

            return type;
        }
    }
}