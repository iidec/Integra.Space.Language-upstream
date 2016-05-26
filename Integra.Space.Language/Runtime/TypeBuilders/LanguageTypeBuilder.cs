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
        /// Count of objects generated.
        /// </summary>
        private static volatile int countObjectsGenerated = 0;

        /// <summary>
        /// Create a new object based in the created type.
        /// </summary>
        /// <param name="listOfFields">List of fields.</param>
        /// <param name="parentType">Parent type.</param>
        /// <param name="queryWriter">Query writer</param>
        /// <returns>The new object.</returns>
        public static object CreateNewObject(List<FieldNode> listOfFields, Type parentType, Type queryWriter)
        {
            return Activator.CreateInstance(CompileResultType(listOfFields));
        }

        /// <summary>
        /// Creates a new type based in the list of fields.
        /// </summary>
        /// <param name="listOfFields">List of fields.</param>
        /// <param name="overrideGetHashCodeMethod">Flag that indicates whether override the GetHashCode method of the new type.</param>
        /// <returns>The created type.</returns>
        public static Type CompileResultType(List<FieldNode> listOfFields, bool overrideGetHashCodeMethod = false)
        {
            TypeBuilder tb = GetTypeBuilder(string.Empty);
            ConstructorBuilder constructor = tb.DefineDefaultConstructor(MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName);

            // creo la lista de campos antes de volver a crear propiedades
            List<Tuple<string, Type, FieldBuilder, int>> fieldList = new List<Tuple<string, Type, FieldBuilder, int>>();

            // The list contains a dynamic object with fields FieldName of type string and FieldType of type Type
            foreach (var field in listOfFields)
            {
                CreateProperty(tb, field, fieldList);
            }

            Type objectType = tb.CreateType();
            return objectType;
        }

        /// <summary>
        /// Creates a type derived of type ExtractedEventData.
        /// </summary>
        /// <param name="listOfFields">List of fields</param>
        /// <param name="isSecondSource">Is left source type.</param>
        /// <returns>New type.</returns>
        public static Type CompileExtractedEventDataSpecificTypeForJoin(List<FieldNode> listOfFields, bool isSecondSource)
        {
            TypeBuilder tb = CreateTypeBuilder("EXTRACTED", listOfFields, typeof(ExtractedEventData), false, false, null, false, null);
            Type objectType = tb.CreateType();
            return objectType;
        }

        /// <summary>
        /// Doc goes here.
        /// </summary>
        /// <param name="listOfFields">List of fields for hash code.</param>
        /// <param name="parentType">Parent type</param>
        /// <param name="typeOfTheOtherSource">Type of the other source, left or right source.</param>
        /// <param name="isSecondSource">Is left source type.</param>
        /// <param name="onCondition">Lambda expression of the on condition.</param>
        /// <returns>New type.</returns>
        public static Type CompileExtractedEventDataComparerTypeForJoin(List<FieldNode> listOfFields, Type parentType, Type typeOfTheOtherSource, bool isSecondSource, System.Linq.Expressions.LambdaExpression onCondition)
        {
            TypeBuilder tb = CreateTypeBuilder("COMPARER", listOfFields, parentType, true, true, typeOfTheOtherSource, isSecondSource, onCondition);
            Type objectType = tb.CreateType();
            return objectType;
        }

        /// <summary>
        /// Creates a new type based in the list of fields. The constructor of the parent type must be 0 arity. Note: Settings.StyleCop was edited because Stylecop don't know the word arity ¬¬
        /// </summary>
        /// <param name="listOfFields">List of fields.</param>
        /// <returns>The created type.</returns>
        public static Type CompileResultType(List<FieldNode> listOfFields)
        {
            TypeBuilder tb = CreateTypeBuilder("RESULT", listOfFields, typeof(EventResult), false, false, null, false, null);
            Type objectType = tb.CreateType();
            return objectType;
        }

        /// <summary>
        /// Creates the type builder of the new type to create.
        /// </summary>
        /// <param name="typeSufixId">Identifier of the type.</param>
        /// <param name="listOfFields">List of fields.</param>
        /// <param name="parentType">Parent type.</param>
        /// <param name="overrideGetHashCodeMethod">Override GetHashCode method flag</param>
        /// <param name="overrideEquals">Override Equals method flag.</param>
        /// <param name="typeOtherSource">Type of the other source.</param>
        /// <param name="isSecondSource">Is left source flag.</param>
        /// <param name="onCondition">Lambda expression of the on condition.</param>
        /// <returns>Type builder for the actual type.</returns>
        private static TypeBuilder CreateTypeBuilder(string typeSufixId, List<FieldNode> listOfFields, Type parentType, bool overrideGetHashCodeMethod, bool overrideEquals, Type typeOtherSource, bool isSecondSource, System.Linq.Expressions.LambdaExpression onCondition)
        {
            TypeBuilder tb = GetTypeBuilder(typeSufixId);
            tb.SetParent(parentType);
            ConstructorBuilder constructor = null;
            ConstructorInfo baseConstructor = null;

            if (parentType.Equals(typeof(EventResult)))
            {
                constructor = tb.DefineConstructor(MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName, CallingConventions.Standard, parentType.GetProperties().Select(x => x.PropertyType).ToArray());
                baseConstructor = parentType.GetConstructor(parentType.GetProperties().Select(x => x.PropertyType).ToArray());
            }
            else
            {
                constructor = tb.DefineConstructor(MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName, CallingConventions.Standard, new Type[] { });
                baseConstructor = parentType.GetConstructor(new Type[] { });
            }

            ILGenerator ctorIL = constructor.GetILGenerator();
            ctorIL.Emit(OpCodes.Ldarg_0);                // push "this"
            ctorIL.Emit(OpCodes.Call, baseConstructor);
            ctorIL.Emit(OpCodes.Ret);

            // creo la lista de campos antes de volver a crear propiedades
            List<Tuple<string, Type, FieldBuilder, int>> fieldList = new List<Tuple<string, Type, FieldBuilder, int>>();

            // si no es null quiere decir que se esta creando el objeto para comparar llaves en el join
            if (onCondition == null)
            {
                PropertyInfo[] parentProperties = parentType.GetProperties();

                // The list contains a dynamic object with fields FieldName of type string and FieldType of type Type
                foreach (var field in listOfFields)
                {
                    /*if (!parentProperties.Select(x => x.Name).Contains(field.FieldName))
                    {
                        
                    }*/

                    CreateProperty(tb, field, fieldList);
                }
            }

            if (parentType.Equals(typeof(EventResult)))
            {
                // creo el metodo Serialize
                CreateSerializeMethod(tb, parentType, fieldList);
            }

            if (overrideGetHashCodeMethod)
            {
                CreateGetHashCodeMethod(tb, listOfFields, parentType);
            }

            if (overrideEquals)
            {
                CreateEqualsMethod(tb, listOfFields, parentType, typeOtherSource, isSecondSource, onCondition);
            }

            return tb;
        }

        /// <summary>
        /// Gets the type builder.
        /// </summary>
        /// <param name="sufix">Identifier of the type.</param>
        /// <returns>Type builder.</returns>
        private static TypeBuilder GetTypeBuilder(string sufix)
        {
            var typeSignature = string.Format("SpaceDynamicType_{0}_{1}", sufix, countObjectsGenerated++);
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
        /// <param name="field">Field node.</param>
        /// <param name="fieldList">List of fields.</param>
        private static void CreateProperty(TypeBuilder tb, FieldNode field, List<Tuple<string, Type, FieldBuilder, int>> fieldList)
        {
            string propertyName = field.FieldName;
            Type propertyType = field.FieldType;
            string fieldName = "_" + propertyName;
            FieldBuilder fieldBuilder = tb.DefineField(fieldName, propertyType, FieldAttributes.Private);
            fieldList.Add(new Tuple<string, Type, FieldBuilder, int>(fieldName, propertyType, fieldBuilder, field.IncidenciasEnOnCondition));

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
        private static void CreateSerializeMethod(TypeBuilder tb, Type parentType, List<Tuple<string, Type, FieldBuilder, int>> fieldList)
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
            foreach (Tuple<string, Type, FieldBuilder, int> t in fieldList)
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
        /// Create serialize method.
        /// </summary>
        /// <param name="tb">Type builder.</param>
        /// <param name="fieldList">List of fields.</param>
        /// <param name="parentType">Parent type to get the properties.</param>
        /// <returns>CompareSameObject method builder.</returns>
        private static MethodBuilder CreateCompareSameObjectMethod(TypeBuilder tb, List<FieldNode> fieldList, Type parentType)
        {
            // defino el cuerpo que sobreescribirá al cuerpo del método Serialize
            MethodBuilder compareSameTypeMethod = tb.DefineMethod("CompareSameType", MethodAttributes.Public | MethodAttributes.Static, typeof(bool), new Type[] { parentType, parentType });

            // defino el cuerpo del metodo Serialize
            ILGenerator compareSameTypeIL = compareSameTypeMethod.GetILGenerator();

            compareSameTypeIL.Emit(OpCodes.Nop);
            Label pushFalse = compareSameTypeIL.DefineLabel();

            /*MethodInfo writeLineMethodInfo = typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) });
            compareSameTypeIL.Emit(OpCodes.Ldstr, "-----> Llamó al Compare same!! ");
            compareSameTypeIL.Emit(OpCodes.Call, writeLineMethodInfo);*/

            bool first = true;

            foreach (FieldNode t in fieldList)
            {
                for (int i = 0; i < t.IncidenciasEnOnCondition; i++)
                {
                    if (first)
                    {
                        compareSameTypeIL.Emit(OpCodes.Ldarg_0); // push this
                        compareSameTypeIL.Emit(OpCodes.Call, parentType.GetMethod("get_" + t.FieldName));

                        compareSameTypeIL.Emit(OpCodes.Ldarg_1); // push the first parameter
                        compareSameTypeIL.Emit(OpCodes.Call, parentType.GetMethod("get_" + t.FieldName));

                        compareSameTypeIL.Emit(OpCodes.Ceq);
                        compareSameTypeIL.Emit(OpCodes.Brfalse, pushFalse);

                        first = false;
                    }
                    else
                    {
                        compareSameTypeIL.Emit(OpCodes.Ldarg_0); // push this
                        compareSameTypeIL.Emit(OpCodes.Call, parentType.GetMethod("get_" + t.FieldName));

                        compareSameTypeIL.Emit(OpCodes.Ldarg_1); // push the first parameter
                        compareSameTypeIL.Emit(OpCodes.Call, parentType.GetMethod("get_" + t.FieldName));

                        compareSameTypeIL.Emit(OpCodes.Ceq);
                        compareSameTypeIL.Emit(OpCodes.Brfalse, pushFalse);
                    }
                }
            }

            /*MethodInfo getTypeMethodInfo = typeof(object).GetMethod("GetType");
            MethodInfo writeMethodInfo = typeof(Console).GetMethod("Write", new Type[] { typeof(object) });
            MethodInfo writeLineMethodInfo = typeof(Console).GetMethod("WriteLine", new Type[] { typeof(object) });

            compareSameTypeIL.Emit(OpCodes.Ldstr, "-----> [CST] Type ");
            compareSameTypeIL.Emit(OpCodes.Call, writeLineMethodInfo);
            compareSameTypeIL.Emit(OpCodes.Ldarg_0); // push the first parameter
            compareSameTypeIL.Emit(OpCodes.Callvirt, getTypeMethodInfo);
            compareSameTypeIL.Emit(OpCodes.Call, writeLineMethodInfo);

            compareSameTypeIL.Emit(OpCodes.Ldstr, "-----> [CST] Parameter Type ");
            compareSameTypeIL.Emit(OpCodes.Call, writeLineMethodInfo);
            compareSameTypeIL.Emit(OpCodes.Ldarg_1); // push the first parameter
            compareSameTypeIL.Emit(OpCodes.Callvirt, getTypeMethodInfo);
            compareSameTypeIL.Emit(OpCodes.Call, writeLineMethodInfo);*/

            compareSameTypeIL.Emit(OpCodes.Ldc_I4_1);
            compareSameTypeIL.Emit(OpCodes.Ret);

            compareSameTypeIL.MarkLabel(pushFalse);
            compareSameTypeIL.Emit(OpCodes.Ldc_I4_0);
            compareSameTypeIL.Emit(OpCodes.Ret);

            return compareSameTypeMethod;
        }

        /// <summary>
        /// Create serialize method.
        /// </summary>
        /// <param name="tb">Type builder.</param>
        /// <param name="fieldList">List of fields.</param>
        /// <param name="parentType">Parent type to get the properties.</param>
        private static void CreateGetHashCodeMethod(TypeBuilder tb, List<FieldNode> fieldList, Type parentType)
        {
            // obtengo el método Serialize del padre
            MethodInfo parentGetHashCode = parentType.GetMethod("GetHashCode");

            // defino el cuerpo que sobreescribirá al cuerpo del método Serialize
            MethodBuilder getHashCodeMethod = tb.DefineMethod("GetHashCode", MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.NewSlot | MethodAttributes.Virtual | MethodAttributes.Final, CallingConventions.HasThis, typeof(int), Type.EmptyTypes);
            tb.DefineMethodOverride(getHashCodeMethod, parentGetHashCode);

            // defino el cuerpo del metodo Serialize
            ILGenerator getHashCodeIL = getHashCodeMethod.GetILGenerator();

            MethodInfo getHashCodeMethodOfField = typeof(object).GetMethod("GetHashCode");
            bool first = true;

            /*MethodInfo writeLineMethodInfo = typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) });
            getHashCodeIL.Emit(OpCodes.Ldstr, "-----> Llamó al Hashcode!! ");
            getHashCodeIL.Emit(OpCodes.Call, writeLineMethodInfo);*/

            foreach (FieldNode t in fieldList)
            {
                if (first)
                {
                    getHashCodeIL.Emit(OpCodes.Ldc_I4, 2166136261);
                    first = false;
                }

                for (int i = 0; i < t.IncidenciasEnOnCondition; i++)
                {
                    getHashCodeIL.Emit(OpCodes.Ldc_I4, 16777619);
                    getHashCodeIL.Emit(OpCodes.Mul);

                    getHashCodeIL.Emit(OpCodes.Ldarg_0);

                    getHashCodeIL.Emit(OpCodes.Call, parentType.GetMethod("get_" + t.FieldName));
                    getHashCodeIL.Emit(OpCodes.Callvirt, getHashCodeMethodOfField);
                    getHashCodeIL.Emit(OpCodes.Xor);
                }
            }

            /*MethodInfo getTypeMethodInfo = typeof(object).GetMethod("GetType");
            MethodInfo writeMethodInfo = typeof(Console).GetMethod("Write", new Type[] { typeof(object) });
            MethodInfo writeLineMethodInfo = typeof(Console).GetMethod("WriteLine", new Type[] { typeof(object) });

            getHashCodeIL.Emit(OpCodes.Ldstr, "-----> [GH] Value ");
            getHashCodeIL.Emit(OpCodes.Call, writeLineMethodInfo);            
            getHashCodeIL.Emit(OpCodes.Ldarg_0); // push the first parameter
            getHashCodeIL.Emit(OpCodes.Callvirt, getTypeMethodInfo);
            getHashCodeIL.Emit(OpCodes.Call, writeLineMethodInfo);*/

            getHashCodeIL.Emit(OpCodes.Ret);
        }

        /// <summary>
        /// Doc goes here.
        /// </summary>
        /// <param name="tb">Type builder</param>
        /// <param name="fieldList">List of fields.</param>
        /// <param name="parentType">Parent type.</param>
        /// <param name="typeOtherSource">Type of the other source.</param>
        /// <param name="isSecondSource">Is left source flag.</param>
        /// <param name="onCondition">Lambda expression of the on condition.</param>
        private static void CreateEqualsMethod(TypeBuilder tb, List<FieldNode> fieldList, Type parentType, Type typeOtherSource, bool isSecondSource, System.Linq.Expressions.LambdaExpression onCondition)
        {
            // obtengo el método Serialize del padre
            MethodInfo parentEquals = parentType.GetMethod("Equals", new Type[] { typeof(object) });

            // defino el cuerpo que sobreescribirá al cuerpo del método Serialize
            MethodBuilder equalsMethod = tb.DefineMethod("Equals", MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.NewSlot | MethodAttributes.Virtual | MethodAttributes.Final, CallingConventions.HasThis, typeof(bool), new Type[] { typeof(object) });
            tb.DefineMethodOverride(equalsMethod, parentEquals);

            ILGenerator equalsIL = equalsMethod.GetILGenerator();
            equalsIL.Emit(OpCodes.Nop);

            Label sameObjectComparer = equalsIL.DefineLabel();
            Label endOfMethod = equalsIL.DefineLabel();

            MethodInfo getTypeMethodInfo = typeof(object).GetMethod("GetType");
            MethodInfo getBaseTypeMethodInfo = typeof(Type).GetProperty("BaseType").GetGetMethod();

            // coloco en la pila de evaluación el tipo de la clase padre
            MethodInfo getTypeFromHandleMethod = typeof(Type).GetMethod("GetTypeFromHandle");
            equalsIL.Emit(OpCodes.Ldtoken, parentType);
            equalsIL.Emit(OpCodes.Call, getTypeFromHandleMethod);

            // obtengo el tipo de la clase actual
            equalsIL.Emit(OpCodes.Ldarg_1); // push the first parameter
            equalsIL.Emit(OpCodes.Callvirt, getTypeMethodInfo);
            equalsIL.Emit(OpCodes.Callvirt, getBaseTypeMethodInfo);

            // llamo el metodo Inequality de la clase Type
            MethodInfo inequalityMethod = typeof(Type).GetMethod("op_Inequality");
            equalsIL.Emit(OpCodes.Call, inequalityMethod);

            // si son iguales salta a la etiqueta especificada
            equalsIL.Emit(OpCodes.Brfalse, sameObjectComparer);

            MethodBuilder evaluateOnCondition = null;
            MethodInfo writeMethodInfo = typeof(System.Diagnostics.Debug).GetMethod("Write", new Type[] { typeof(object) });
            MethodInfo writeLineMethodInfo = typeof(System.Diagnostics.Debug).GetMethod("WriteLine", new Type[] { typeof(object) });

            /*equalsIL.Emit(OpCodes.Ldstr, "-----> Llamó al equals!! ");
            equalsIL.Emit(OpCodes.Call, writeLineMethodInfo);*/

            if (!isSecondSource)
            {
                /*equalsIL.Emit(OpCodes.Ldstr, "-----> [EQ] Izquierda ");
                equalsIL.Emit(OpCodes.Call, writeLineMethodInfo);

                equalsIL.Emit(OpCodes.Ldstr, "-----> [EQ] Type ");
                equalsIL.Emit(OpCodes.Call, writeLineMethodInfo);
                equalsIL.Emit(OpCodes.Ldarg_0); // push the first parameter
                equalsIL.Emit(OpCodes.Callvirt, getTypeMethodInfo);
                equalsIL.Emit(OpCodes.Call, writeLineMethodInfo);

                equalsIL.Emit(OpCodes.Ldstr, "-----> [EQ] Argument Type ");
                equalsIL.Emit(OpCodes.Call, writeLineMethodInfo);
                equalsIL.Emit(OpCodes.Ldarg_1); // push the first parameter
                equalsIL.Emit(OpCodes.Callvirt, getTypeMethodInfo);
                equalsIL.Emit(OpCodes.Call, writeLineMethodInfo);*/

                equalsIL.Emit(OpCodes.Ldarg_0); // push "this"
                equalsIL.Emit(OpCodes.Ldarg_1); // push the first parameter
                equalsIL.Emit(OpCodes.Castclass, typeOtherSource);

                evaluateOnCondition = tb.DefineMethod("EvaluateOnCondition", MethodAttributes.Public | MethodAttributes.Static, typeof(bool), new[] { parentType, typeOtherSource });
                onCondition.CompileToMethod(evaluateOnCondition);
            }
            else
            {
                /*equalsIL.Emit(OpCodes.Ldstr, "----->[EQ] Derecha ");
                equalsIL.Emit(OpCodes.Call, writeLineMethodInfo);

                equalsIL.Emit(OpCodes.Ldstr, "-----> [EQ] Type ");
                equalsIL.Emit(OpCodes.Call, writeLineMethodInfo);
                equalsIL.Emit(OpCodes.Ldarg_0); // push the first parameter
                equalsIL.Emit(OpCodes.Callvirt, getTypeMethodInfo);
                equalsIL.Emit(OpCodes.Call, writeLineMethodInfo);

                equalsIL.Emit(OpCodes.Ldstr, "-----> [EQ] Argument Type ");
                equalsIL.Emit(OpCodes.Call, writeLineMethodInfo);
                equalsIL.Emit(OpCodes.Ldarg_1); // push the first parameter
                equalsIL.Emit(OpCodes.Callvirt, getTypeMethodInfo);
                equalsIL.Emit(OpCodes.Call, writeLineMethodInfo);*/

                equalsIL.Emit(OpCodes.Ldarg_1); // push the first parameter
                equalsIL.Emit(OpCodes.Castclass, typeOtherSource);
                equalsIL.Emit(OpCodes.Ldarg_0); // push "this"

                evaluateOnCondition = tb.DefineMethod("EvaluateOnCondition", MethodAttributes.Public | MethodAttributes.Static, typeof(bool), new[] { typeOtherSource, parentType });
                onCondition.CompileToMethod(evaluateOnCondition);
            }

            equalsIL.Emit(OpCodes.Call, evaluateOnCondition);
            equalsIL.Emit(OpCodes.Br_S, endOfMethod);

            MethodBuilder sameObjectComparerMethodBuilder = CreateCompareSameObjectMethod(tb, fieldList, parentType);
            equalsIL.MarkLabel(sameObjectComparer);

            equalsIL.Emit(OpCodes.Ldarg_0);
            equalsIL.Emit(OpCodes.Ldarg_1);
            equalsIL.Emit(OpCodes.Call, sameObjectComparerMethodBuilder);

            equalsIL.MarkLabel(endOfMethod);
            equalsIL.Emit(OpCodes.Ret);
        }

        /// <summary>
        /// check if is a numeric type
        /// </summary>
        /// <param name="type">type to check</param>
        /// <returns>true if is numeric type</returns>
        private static Type ToPrimitiveNullable(Type type)
        {
            if (Nullable.GetUnderlyingType(type) != null)
            {
                return type;
            }
            else
            {
                if (type.Equals(typeof(TimeSpan)))
                {
                    return typeof(Nullable<>).MakeGenericType(type);
                }

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
                    case TypeCode.DateTime:
                        return typeof(Nullable<>).MakeGenericType(type);
                    default:
                        return type;
                }
            }
        }
    }
}