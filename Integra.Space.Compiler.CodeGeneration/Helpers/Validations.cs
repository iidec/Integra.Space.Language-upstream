//-----------------------------------------------------------------------
// <copyright file="Validations.cs" company="Integra.Space.Language">
//     Copyright (c) Integra.Space.Language. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Compiler
{
    using System;

    /// <summary>
    /// Custom validations
    /// </summary>
    internal static class Validations
    {
        /// <summary>
        /// Check if is a numeric type
        /// </summary>
        /// <param name="type">type to check</param>
        /// <returns>true if is numeric type</returns>
        public static bool IsNumericType(this Type type)
        {
            if (type == null)
            {
                return false;
            }

            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Byte:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.SByte:
                case TypeCode.Single:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                    return true;
            }

            if (type.Equals(typeof(float)))
            {
                return true;
            }

            return false;
        }
    }
}
