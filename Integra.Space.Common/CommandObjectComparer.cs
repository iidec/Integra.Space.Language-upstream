//-----------------------------------------------------------------------
// <copyright file="CommandObjectComparer.cs" company="Integra.Space.Common">
//     Copyright (c) Integra.Space.Common. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Common
{
    using System.Collections.Generic;

    /// <summary>
    /// Command object comparer class.
    /// </summary>
    internal sealed class CommandObjectComparer : EqualityComparer<CommandObject>
    {
        /// <inheritdoc />
        public override bool Equals(CommandObject x, CommandObject y)
        {
            if (x.Name == y.Name && x.SecurableClass == y.SecurableClass && x.DatabaseName == y.DatabaseName && x.SchemaName == y.SchemaName && x.GranularPermission == y.GranularPermission)
            {
                return true;
            }

            return false;
        }

        /// <inheritdoc />
        public override int GetHashCode(CommandObject obj)
        {
            int result = obj.SecurableClass.GetHashCode() + obj.GranularPermission.GetHashCode();

            if (!string.IsNullOrWhiteSpace(obj.DatabaseName))
            {
                result = result + obj.DatabaseName.GetHashCode();
            }

            if (!string.IsNullOrWhiteSpace(obj.SchemaName))
            {
                result = result + obj.SchemaName.GetHashCode();
            }

            return result + obj.Name.GetHashCode();
        }
    }
}
