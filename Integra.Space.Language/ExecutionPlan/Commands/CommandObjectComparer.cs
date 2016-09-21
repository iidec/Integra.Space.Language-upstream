//-----------------------------------------------------------------------
// <copyright file="CommandObjectComparer.cs" company="Integra.Space.Language">
//     Copyright (c) Integra.Space.Common. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Language
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
            if (x.Name == y.Name && x.SecurableClass == y.SecurableClass)
            {
                return true;
            }

            return false;
        }

        /// <inheritdoc />
        public override int GetHashCode(CommandObject obj)
        {
            return obj.SecurableClass.GetHashCode() + obj.Name.GetHashCode();
        }
    }
}
