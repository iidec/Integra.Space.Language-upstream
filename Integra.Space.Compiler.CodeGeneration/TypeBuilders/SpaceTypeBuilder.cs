//-----------------------------------------------------------------------
// <copyright file="SpaceTypeBuilder.cs" company="Integra.Space.Language">
//     Copyright (c) Integra.Space.Language. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Compiler
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Reflection.Emit;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// Space type builder interface
    /// </summary>
    internal abstract class SpaceTypeBuilder
    {
        /// <summary>
        /// Properties setters.
        /// </summary>
        private List<MethodBuilder> propertiesSetters;

        /// <summary>
        /// Finds of the new type.
        /// </summary>
        private IEnumerable<FieldNode> fields;

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
        public SpaceTypeBuilder(AssemblyBuilder asmBuilder, string typeSignature, Type parentType, IEnumerable<FieldNode> fields)
        {
            Contract.Assert(asmBuilder != null);
            Contract.Assert(!string.IsNullOrWhiteSpace(typeSignature));
            Contract.Assert(parentType != null);
            Contract.Assert(fields != null);

            this.asmBuilder = asmBuilder;
            this.typeSignature = typeSignature;
            this.parentType = parentType;
            this.fields = fields;

            this.propertiesSetters = new List<MethodBuilder>();
        }

        /// <summary>
        /// Gets the properties setters of this type.
        /// </summary>
        protected IEnumerable<MethodBuilder> PropertiesSetters
        {
            get
            {
                return this.propertiesSetters;
            }
        }

        /// <summary>
        /// Gets the fields for the new type.
        /// </summary>
        protected IEnumerable<FieldNode> Fields
        {
            get
            {
                return this.fields;
            }
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
            try
            {
                TypeBuilder tb = this.asmBuilder.GetDynamicModule("SpaceMainModule").DefineType(this.TypeSignature, TypeAttributes.Public | TypeAttributes.Class | TypeAttributes.AutoClass | TypeAttributes.AnsiClass | TypeAttributes.BeforeFieldInit | TypeAttributes.AutoLayout | TypeAttributes.Serializable, this.parentType);
                return tb;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(string.Format("Error al crear el tipo: {0} que hereda de. {1}.", this.typeSignature, this.parentType.Name));
                throw new Exception(string.Format("Error al crear el tipo: {0} que hereda de. {1}.", this.typeSignature, this.parentType.Name), e);
            }
        }

        /// <summary>
        /// Creates the constructor of the new class.
        /// </summary>
        /// <param name="typeBuilder">Type builder.</param>
        protected virtual void CreateConstructor(TypeBuilder typeBuilder)
        {
            Type[] fieldTypes = this.fields.Select(x => x.FieldType).ToArray();

            // var emiter = Sigil.NonGeneric.Emit.BuildConstructor(fieldTypes, typeBuilder, MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName, CallingConventions.Standard);
            // emiter.LoadArgument(0);
            ConstructorBuilder constructor = typeBuilder.DefineConstructor(MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName, CallingConventions.Standard, fieldTypes);

            int fieldCount = fieldTypes.Count();
            List<ParameterBuilder> paramList = new List<ParameterBuilder>();
            for (int i = 1; i <= fieldCount; i++)
            {
                paramList.Add(constructor.DefineParameter(i, ParameterAttributes.Optional | ParameterAttributes.HasDefault, this.fields.ElementAt(i-1).FieldName));
            }
            
            // creo el IL generator para el constructor actual
            ILGenerator ctorIL = constructor.GetILGenerator();

            ConstructorInfo baseConstructor = null;
            ConstructorInfo constructorInfo = this.ParentType.GetConstructors().Single();
            int parameterCount = constructorInfo.GetParameters().Count();
            if (parameterCount == 0)
            {
                baseConstructor = this.ParentType.GetConstructor(new Type[] { });
                ctorIL.Emit(OpCodes.Ldarg_0); // push "this"
                ctorIL.Emit(OpCodes.Call, baseConstructor);
            }
            else
            {
                Type[] types = constructorInfo.GetParameters().Select(x => x.ParameterType).ToArray();
                baseConstructor = this.ParentType.GetConstructor(types);

                ctorIL.Emit(OpCodes.Ldarg_0); // push "this"

                foreach(var param in baseConstructor.GetParameters())
                {
                    ParameterBuilder paramInfo = paramList.SingleOrDefault(x => x.Name == param.Name);
                    ctorIL.Emit(OpCodes.Ldarg, paramInfo.Position);
                }

                ctorIL.Emit(OpCodes.Call, baseConstructor);
            }
            
            foreach (string propertyName in this.fields.Select(x => x.FieldName))
            {
                ParameterInfo param = constructorInfo.GetParameters().SingleOrDefault(x => x.Name == propertyName);

                if (param != null)
                {
                    MethodBuilder setter = this.propertiesSetters.SingleOrDefault(x => x.Name == "set_" + propertyName);
                    if (setter != null)
                    {
                        ctorIL.Emit(OpCodes.Ldarg_0); // push "this"
                        ctorIL.Emit(OpCodes.Ldarg, param.Position);
                        ctorIL.Emit(OpCodes.Call, setter);
                    }
                }
            }

            ctorIL.Emit(OpCodes.Ret);
        }

        /// <summary>
        /// Creates a property.
        /// </summary>
        /// <param name="typeBuilder">Type builder.</param>
        /// <param name="field">Field data.</param>
        /// <param name="privateSetters">A value indicating wheter the setters of all properties are private.</param>
        protected virtual void CreateProperty(TypeBuilder typeBuilder, FieldNode field, bool privateSetters)
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

            propertyBuilder.SetGetMethod(getPropMthdBldr);

            MethodAttributes protectionLevel = MethodAttributes.Public;

            if(privateSetters)
            {
                protectionLevel = MethodAttributes.Private;
            }

            MethodBuilder setPropMthdBldr = typeBuilder.DefineMethod("set_" + propertyName, protectionLevel | MethodAttributes.SpecialName | MethodAttributes.HideBySig, null, new[] { propertyType });
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

            propertyBuilder.SetSetMethod(setPropMthdBldr);
            this.propertiesSetters.Add(setPropMthdBldr);
        }
    }
}
