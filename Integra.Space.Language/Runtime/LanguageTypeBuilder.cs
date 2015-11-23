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
    using Event;

    /// <summary>
    /// Language type builder class
    /// </summary>
    public static class LanguageTypeBuilder
    {
        /// <summary>
        /// Create a new object based in the created type.
        /// </summary>
        /// <param name="listOfFields">List of fields.</param>
        /// <returns>The new object.</returns>
        public static object CreateNewObject(List<dynamic> listOfFields)
        {
            return Activator.CreateInstance(CompileResultType(listOfFields));
        }

        /// <summary>
        /// Creates a new type based in the list of fields.
        /// </summary>
        /// <param name="listOfFields">List of fields.</param>
        /// <returns>The created type.</returns>
        public static Type CompileResultType(List<dynamic> listOfFields)
        {
            TypeBuilder tb = GetTypeBuilder();
            ConstructorBuilder constructor = tb.DefineDefaultConstructor(MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName);

            // The list contains a dynamic object with fields FieldName of type string and FieldType of type Type
            foreach (var field in listOfFields)
            {
                CreateProperty(tb, field.FieldName, field.FieldType);
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
        public static Type CompileResultType(List<dynamic> listOfFields, Type parentType)
        {
            TypeBuilder tb = GetTypeBuilder();
            tb.SetParent(parentType);
            ConstructorBuilder constructor = tb.DefineConstructor(MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName, CallingConventions.Standard, parentType.GetProperties().Select(x => x.PropertyType).ToArray());

            var baseConstructor = parentType.GetConstructor(parentType.GetProperties().Select(x => x.PropertyType).ToArray());

            ILGenerator ctorIL = constructor.GetILGenerator();
            ctorIL.Emit(OpCodes.Ldarg_0);                // push "this"
            ctorIL.Emit(OpCodes.Ldarg_1);                // push the 1. parameter
            ctorIL.Emit(OpCodes.Call, baseConstructor);
            ctorIL.Emit(OpCodes.Ret);

            // The list contains a dynamic object with fields FieldName of type string and FieldType of type Type
            foreach (var field in listOfFields)
            {
                CreateProperty(tb, field.FieldName, field.FieldType);
            }

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
        private static void CreateProperty(TypeBuilder tb, string propertyName, Type propertyType)
        {
            FieldBuilder fieldBuilder = tb.DefineField("_" + propertyName, propertyType, FieldAttributes.Private);
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
    }
}