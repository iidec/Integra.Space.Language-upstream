//-----------------------------------------------------------------------
// <copyright file="SpaceModuleBuilder.cs" company="Integra.Space.Language">
//     Copyright (c) Integra.Space.Language. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Compiler
{
    using System.Reflection.Emit;

    /// <summary>
    /// Space module builder class.
    /// </summary>
    internal class SpaceModuleBuilder
    {
        /// <summary>
        /// Assembly builder.
        /// </summary>
        private AssemblyBuilder asmBuilder;

        /// <summary>
        /// Initializes a new instance of the <see cref="SpaceModuleBuilder"/> class.
        /// </summary>
        /// <param name="asmBuilder">Assembly builder.</param>
        public SpaceModuleBuilder(AssemblyBuilder asmBuilder)
        {
            this.asmBuilder = asmBuilder;
        }

        /// <summary>
        /// Creates the module builder.
        /// </summary>
        /// <returns>The module builder created.</returns>
        public ModuleBuilder CreateModuleBuilder()
        {
            ModuleBuilder moduleBuilder = this.asmBuilder.DefineDynamicModule("SpaceMainModule", this.asmBuilder.GetName().Name + "." + SpaceAssemblyBuilder.FILEEXTENSION);
            return moduleBuilder;
        }
    }
}
