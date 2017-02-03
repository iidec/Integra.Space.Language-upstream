//-----------------------------------------------------------------------
// <copyright file="SpaceAssemblyBuilder.cs" company="Integra.Space.Language">
//     Copyright (c) Integra.Space.Language. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Compiler
{
    using System;
    using System.Reflection;
    using System.Reflection.Emit;

    /// <summary>
    /// Space assembly builder class.
    /// </summary>
    internal class SpaceAssemblyBuilder
    {
        /// <summary>
        /// File extension of the assembly.
        /// </summary>
        public const string FILEEXTENSION = "dll";

        /// <summary>
        /// Assembly name.
        /// </summary>
        private string assemblySignature;

        /// <summary>
        /// Initializes a new instance of the <see cref="SpaceAssemblyBuilder"/> class.
        /// </summary>
        /// <param name="assemblySignature">Assembly name.</param>
        public SpaceAssemblyBuilder(string assemblySignature)
        {
            this.assemblySignature = assemblySignature;
        }

        /// <summary>
        /// Creates the assembly builder.
        /// </summary>
        /// <returns>The assembly builder created.</returns>
        public AssemblyBuilder CreateAssemblyBuilder()
        {
            AssemblyName an = new AssemblyName(this.assemblySignature);
            AssemblyBuilder assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(an, AssemblyBuilderAccess.RunAndSave);
            return assemblyBuilder;
        }

        /// <summary>
        /// Creates the assembly builder.
        /// </summary>
        /// <param name="domain">App domain for the new assembly.</param>
        /// <returns>The assembly builder created.</returns>
        public AssemblyBuilder CreateAssemblyBuilder(AppDomain domain)
        {
            AssemblyName an = new AssemblyName(this.assemblySignature);
            AssemblyBuilder assemblyBuilder = domain.DefineDynamicAssembly(an, AssemblyBuilderAccess.RunAndSave);
            return assemblyBuilder;
        }
    }
}
