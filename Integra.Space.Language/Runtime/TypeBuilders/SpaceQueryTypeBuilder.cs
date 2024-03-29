﻿//-----------------------------------------------------------------------
// <copyright file="SpaceQueryTypeBuilder.cs" company="Integra.Space.Language">
//     Copyright (c) Integra.Space.Language. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Language.Runtime
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Reflection.Emit;

    /// <summary>
    /// Space query type builder class.
    /// </summary>
    internal class SpaceQueryTypeBuilder : SpaceTypeBuilder
    {
        /// <summary>
        /// Query identifier.
        /// </summary>
        private string queryId;

        /// <summary>
        /// Lambda expression of the function to create.
        /// </summary>
        private LambdaExpression function;

        /// <summary>
        /// Initializes a new instance of the <see cref="SpaceQueryTypeBuilder"/> class.
        /// </summary>
        /// <param name="asmBuilder">Assembly builder.</param>
        /// <param name="queryId">Query identifier.</param>
        /// <param name="function">Lambda expression of the function to create.</param>
        public SpaceQueryTypeBuilder(AssemblyBuilder asmBuilder, string queryId, LambdaExpression function) : base(asmBuilder, "SpaceQuery_" + queryId, typeof(SpaceQuery))
        {
            this.queryId = queryId;
            this.function = function;
        }

        /// <inheritdoc />
        public override Type CreateNewType()
        {
            TypeBuilder typeBuilder = this.CreateType();
            this.CreateConstructor(typeBuilder);

            this.CreateMethodForFunction(typeBuilder);
            Type newType = typeBuilder.CreateType();

            return newType;
        }

        /// <summary>
        /// Create and save a new assembly.
        /// </summary>
        /// <returns>The new assembly created.</returns>
        public Assembly CreateNewAssembly()
        {
            TypeBuilder typeBuilder = this.CreateType();
            this.CreateConstructor(typeBuilder);

            this.CreateMethodForFunction(typeBuilder);
            Type newType = typeBuilder.CreateType();

            // Create a new information type.
            QueryInformationTypeBuilder qitb = new QueryInformationTypeBuilder(this.AsmBuilder, this.queryId, newType);
            qitb.CreateNewType();

            /*string assemblyPath = this.SaveAssembly(this.AsmBuilder);
            Assembly assemblyReloaded = Assembly.LoadFile(assemblyPath);*/

            return newType.Assembly;
        }

        /// <inheritdoc />
        protected override void CreateConstructor(TypeBuilder typeBuilder)
        {
            // creo el constructor de la clase
            ConstructorBuilder constructor = typeBuilder.DefineConstructor(MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName, CallingConventions.Standard, Type.EmptyTypes);

            // llamo al constructor base
            ConstructorInfo baseConstructor = this.ParentType.GetConstructors()[0];

            // creo el cuerpo de la llamada al constructor base
            ILGenerator ctorIL = constructor.GetILGenerator();

            ctorIL.Emit(OpCodes.Ldarg_0);                   // push "this"
            ctorIL.Emit(OpCodes.Ldstr, this.queryId);       // igualo el identificador del query al primer parámetro del constructor
            ctorIL.Emit(OpCodes.Call, baseConstructor);     // llamo al constructor base

            ctorIL.Emit(OpCodes.Ret);
        }

        /// <summary>
        /// Saves the created assembly in a file.
        /// </summary>
        /// <param name="asmBuilder">Assembly builder</param>
        /// <returns>The assembly path.</returns>
        private string SaveAssembly(AssemblyBuilder asmBuilder)
        {
            string assemblyFileName = asmBuilder.GetName().Name + SpaceAssemblyBuilder.FILEEXTENSION;
            asmBuilder.Save(assemblyFileName); // , PortableExecutableKinds.PE32Plus, ImageFileMachine.IA64);

            string assemblyDirectoryPath = System.IO.Path.Combine(Environment.CurrentDirectory, "TempQueryAssemblies");
            System.IO.Directory.CreateDirectory(assemblyDirectoryPath);
            string assemblyPath = System.IO.Path.Combine(assemblyDirectoryPath, assemblyFileName);

            if (System.IO.File.Exists(assemblyPath))
            {
                try
                {
                    System.IO.File.Delete(assemblyPath);
                }
                catch (Exception ex)
                {
                    throw new Exception(string.Format("Cannot delete the assembly '{0}'.", assemblyFileName), ex);
                }
            }

            System.IO.File.Move(System.IO.Path.Combine(Environment.CurrentDirectory, assemblyFileName), assemblyPath);

            return assemblyPath;
        }

        /// <inheritdoc />
        private void CreateMethodForFunction(TypeBuilder typeBuilder)
        {
            List<Type> parameterTypes = new List<Type>();
            foreach (ParameterExpression p in this.function.Parameters)
            {
                parameterTypes.Add(p.Type);
            }

            MethodBuilder mainFunction = typeBuilder.DefineMethod("MainFunction", MethodAttributes.Public | MethodAttributes.Static, this.function.ReturnType, parameterTypes.ToArray());
            this.function.CompileToMethod(mainFunction);
        }
    }
}
