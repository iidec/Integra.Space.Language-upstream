//-----------------------------------------------------------------------
// <copyright file="SpaceTypeBuilder.cs" company="Integra.Space.Language">
//     Copyright (c) Integra.Space.Language. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Language.Runtime
{
    using System;
    using System.Reflection;
    using System.Reflection.Emit;

    /// <summary>
    /// Space type builder interface
    /// </summary>
    internal abstract class SpaceTypeBuilder
    {        
        /// <summary>
        /// Name of the type that going to be created.
        /// </summary>
        private string typeSignature;

        /// <summary>
        /// Parent type of the new type.
        /// </summary>
        private Type parentType;

        /// <summary>
        /// Assembly builder.
        /// </summary>
        private AssemblyBuilder asmBuilder;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="SpaceTypeBuilder"/> class.
        /// </summary>
        /// <param name="asmBuilder">Assembly builder.</param>
        /// <param name="typeSignature">Name of the type that going to be created.</param>
        /// <param name="parentType">Parent type of the new type.</param>
        public SpaceTypeBuilder(AssemblyBuilder asmBuilder, string typeSignature, Type parentType)
        {
            this.asmBuilder = asmBuilder;
            this.typeSignature = typeSignature;
            this.parentType = parentType;
        }

        /// <summary>
        /// Gets the assembly builder.
        /// </summary>
        protected AssemblyBuilder AsmBuilder
        {
            get
            {
                return this.asmBuilder;
            }
        }

        /// <summary>
        /// Gets the name of the type that going to be created.
        /// </summary>
        protected string TypeSignature
        {
            get
            {
                return this.typeSignature;
            }
        }

        /// <summary>
        /// Gets the parent type of the new type.
        /// </summary>
        protected Type ParentType
        {
            get
            {
                return this.parentType;
            }
        }

        /// <summary>
        /// Creates the new type based on the specified parameters.
        /// </summary>
        /// <returns>The new type created.</returns>
        public abstract System.Type CreateNewType();
        
        /// <summary>
        /// Creates the type builder.
        /// </summary>
        /// <returns>The type builder created.</returns>
        protected TypeBuilder CreateType()
        {
            TypeBuilder tb = this.asmBuilder.GetDynamicModule("SpaceMainModule").DefineType(this.TypeSignature, TypeAttributes.Public | TypeAttributes.Class | TypeAttributes.AutoClass | TypeAttributes.AnsiClass | TypeAttributes.BeforeFieldInit | TypeAttributes.AutoLayout | TypeAttributes.Serializable, this.parentType);
            return tb;
        }

        /// <summary>
        /// Creates the constructor of the new class.
        /// </summary>
        /// <param name="typeBuilder">Type builder.</param>
        protected virtual void CreateConstructor(TypeBuilder typeBuilder)
        {
            ConstructorBuilder constructor = typeBuilder.DefineConstructor(MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName, CallingConventions.Standard, new Type[] { });
            ConstructorInfo baseConstructor = this.ParentType.GetConstructor(new Type[] { });

            ILGenerator ctorIL = constructor.GetILGenerator();
            ctorIL.Emit(OpCodes.Ldarg_0);                // push "this"
            ctorIL.Emit(OpCodes.Call, baseConstructor);
            ctorIL.Emit(OpCodes.Ret);
        }

        /// <summary>
        /// Creates a property.
        /// </summary>
        /// <param name="typeBuilder">Type builder.</param>
        /// <param name="field">Field data.</param>
        protected virtual void CreateProperty(TypeBuilder typeBuilder, FieldNode field)
        {
            string propertyName = field.FieldName;
            Type propertyType = field.FieldType;
            string fieldName = "_" + propertyName;
            FieldBuilder fieldBuilder = typeBuilder.DefineField(fieldName, propertyType, FieldAttributes.Private);
            field.FieldBuilder = fieldBuilder;

            PropertyBuilder propertyBuilder = typeBuilder.DefineProperty(propertyName, PropertyAttributes.HasDefault, propertyType, null);

            MethodBuilder getPropMthdBldr = typeBuilder.DefineMethod("get_" + propertyName, MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig, propertyType, Type.EmptyTypes);
            ILGenerator getIl = getPropMthdBldr.GetILGenerator();

            getIl.Emit(OpCodes.Ldarg_0);
            getIl.Emit(OpCodes.Ldfld, fieldBuilder);
            getIl.Emit(OpCodes.Ret);

            MethodBuilder setPropMthdBldr = typeBuilder.DefineMethod("set_" + propertyName, MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig, null, new[] { propertyType });
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
