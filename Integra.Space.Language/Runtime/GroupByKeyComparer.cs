//-----------------------------------------------------------------------
// <copyright file="GroupByKeyComparer.cs" company="Integra.Space.Language">
//     Copyright (c) Integra.Space.Language. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Language.Runtime
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// Group key comparer class
    /// </summary>
    /// <typeparam name="T">Type to compare</typeparam>
    public class GroupByKeyComparer<T> : IEqualityComparer<T>
    {
        /// <summary>
        /// properties of the order by key
        /// </summary>
        private PropertyInfo[] properties;

        /// <summary>
        /// Initializes a new instance of the <see cref="GroupByKeyComparer{T}"/> class
        /// </summary>
        public GroupByKeyComparer()
        {
            this.properties = typeof(T).GetProperties().ToArray();
        }

        /// <inheritdoc />
        public bool Equals(T x, T y)
        {
            foreach (PropertyInfo p in this.properties)
            {
                var a = p.GetValue(x);
                var b = y.GetType().GetProperty(p.Name).GetValue(y);

                bool areEqual;
                if (a == null)
                {
                    areEqual = a == b;
                }
                else
                {
                    areEqual = a.Equals(b);
                }

                if (!areEqual)
                {
                    return false;
                }
            }

            return true;
        }

        /// <inheritdoc />
        public int GetHashCode(T obj)
        {
            return 0;
        }
    }
}
