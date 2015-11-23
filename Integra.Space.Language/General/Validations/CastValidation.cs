//-----------------------------------------------------------------------
// <copyright file="CastValidation.cs" company="Integra.Space.Language">
//     Copyright (c) Integra.Space.Language. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Language.General.Validations
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// CastValidation class
    /// </summary>
    internal static class CastValidation
    {
        /// <summary>
        /// Dictionary of numeric types
        /// </summary>
        private static Dictionary<Type, List<Type>> dict = new Dictionary<Type, List<Type>>() 
        {
            { 
                typeof(decimal), new List<Type> 
                { 
                    typeof(sbyte), typeof(byte), typeof(short), typeof(ushort), typeof(int), typeof(uint), typeof(long), typeof(ulong), typeof(char) 
                } 
            },
            { 
                typeof(double), new List<Type> 
                { 
                    typeof(sbyte), typeof(byte), typeof(short), typeof(ushort), typeof(int), typeof(uint), typeof(long), typeof(ulong), typeof(char), typeof(float) 
                } 
                },
            { 
                typeof(float), new List<Type> 
                { 
                    typeof(sbyte), typeof(byte), typeof(short), typeof(ushort), typeof(int), typeof(uint), typeof(long), typeof(ulong), typeof(char), typeof(float) 
                } 
                },
            { 
                typeof(ulong), new List<Type> 
                { 
                    typeof(byte), typeof(ushort), typeof(uint), typeof(char) 
                } 
                },
            { 
                typeof(long), new List<Type> 
                { 
                    typeof(sbyte), typeof(byte), typeof(short), typeof(ushort), typeof(int), typeof(uint), typeof(char) 
                } 
                },
            { 
                typeof(uint), new List<Type> 
                { 
                    typeof(byte), typeof(ushort), typeof(char) 
                } 
                },
            { 
                typeof(int), new List<Type> 
                { 
                    typeof(sbyte), typeof(byte), typeof(short), typeof(ushort), typeof(char) 
                } 
                },
            { 
                typeof(ushort), new List<Type> 
                { 
                    typeof(byte), typeof(char) 
                } 
                },
            { 
                typeof(short), new List<Type> 
                { 
                    typeof(byte) 
                } 
                }
        };

        /// <summary>
        /// Doc go here
        /// </summary>
        /// <param name="from">original type</param>
        /// <param name="to">target type</param>
        /// <returns>true if cast is valid</returns>
        public static bool IsCastableTo(this Type from, Type to)
        {
            if (to.IsAssignableFrom(from))
            {
                return true;
            }

            if (dict.ContainsKey(to) && dict[to].Contains(from))
            {
                return true;
            }

            bool casteable = from.GetMethods(BindingFlags.Public | BindingFlags.Static)
                              .Any(
                                  m => m.ReturnType == to &&
                                       (m.Name == "op_Implicit" ||
                                        m.Name == "op_Explicit"));
            return casteable;
        }
    }
}
