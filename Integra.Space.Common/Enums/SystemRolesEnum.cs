//-----------------------------------------------------------------------
// <copyright file="SystemRolesEnum.cs" company="Integra.Space.Common">
//     Copyright (c) Integra.Space.Common. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Common
{
    /// <summary>
    /// Space role enumerator.
    /// </summary>
    internal enum SystemRolesEnum
    {
        /// <summary>
        /// System reader role. Can read all the streams of the system.
        /// </summary>
        SysReader = 1,

        /// <summary>
        /// System administrator role.
        /// </summary>
        SysAdmin = 2,

        /// <summary>
        /// Schema creator role. Can create schemas in the system.
        /// </summary>
        SchemaCreator = 4
    }
}
