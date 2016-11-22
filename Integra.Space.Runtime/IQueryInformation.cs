//-----------------------------------------------------------------------
// <copyright file="IQueryInformation.cs" company="Integra.Space.Language">
//     Copyright (c) Integra.Space.Language. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space
{
    /// <summary>
    /// Query information interface.
    /// </summary>
    public interface IQueryInformation
    {
        /// <summary>
        /// Returns the SpaceQuery child class type.
        /// </summary>
        /// <returns>SpaceQuery child class type.</returns>
        System.Type GetQueryType();

        /// <summary>
        /// Gets the referenced assemblies by the dll.
        /// </summary>
        /// <returns>The referenced assemblies</returns>
        System.Reflection.AssemblyName[] GetReferencedAssemblies();        
    }
}
