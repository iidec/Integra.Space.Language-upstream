//-----------------------------------------------------------------------
// <copyright file="OrderByKeyComparer.cs" company="Integra.Space.Language">
//     Copyright (c) Integra.Space.Language. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Language.Runtime
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// Order by key comparer class
    /// </summary>
    /// <typeparam name="T">Type to compare</typeparam>
    internal class OrderByKeyComparer<T> : IComparer<T>
    {
        /// <summary>
        /// properties of the order by key
        /// </summary>
        private PropertyInfo[] properties;

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderByKeyComparer{T}"/> class
        /// </summary>
        public OrderByKeyComparer()
        {
            this.properties = typeof(T).GetProperties().ToArray();
        }

        /// <inheritdoc />
        public int Compare(T x, T y)
        {
            int resultIz = 0;
            int resultDer = 0;
            int posicion = x.GetType().GetProperties().Count() + 1;

            foreach (PropertyInfo propIz in this.properties)
            {
                int resultIzAux = 0;
                int resultComparer = 0;

                PropertyInfo propDer = y.GetType().GetProperty(propIz.Name);
                var valIz = propIz.GetValue(x);
                var valDer = propDer.GetValue(y);

                if (valIz == null && valDer == null)
                {
                    continue;
                }

                if (valIz == null && valDer != null)
                {
                    resultComparer = -1;
                    resultIzAux = this.CalcLeftResult(resultComparer, posicion);
                    resultIz += resultIzAux;
                    resultDer += this.CalcRightResult(resultComparer, resultIzAux);

                    /*resultIzAux += posicion * -1;
                    resultIz = resultIzAux;
                    resultDer += resultIzAux + -1;*/
                    continue;
                }

                if (valIz != null && valDer == null)
                {
                    resultComparer = 1;
                    resultIzAux = this.CalcLeftResult(resultComparer, posicion);
                    resultIz += resultIzAux;
                    resultDer += this.CalcRightResult(resultComparer, resultIzAux);

                    /*resultIz += posicion * 1;
                    resultIz = resultIzAux;
                    resultDer += resultIzAux + -1;*/
                    continue;
                }

                if (propIz.PropertyType != propDer.PropertyType)
                {
                    throw new ArgumentException("Invalid 'order by' argument.", propIz.Name);
                }
                else
                {
                    if (propIz.PropertyType.Equals(typeof(string)))
                    {
                        resultComparer = string.Compare(valIz.ToString(), valDer.ToString());
                    }
                    else if (propIz.PropertyType.Equals(typeof(byte)))
                    {
                        resultComparer = ((byte)valIz).CompareTo((byte)valDer);
                    }
                    else if (propIz.PropertyType.Equals(typeof(sbyte)))
                    {
                        resultComparer = ((sbyte)valIz).CompareTo((sbyte)valDer);
                    }
                    else if (propIz.PropertyType.Equals(typeof(short)))
                    {
                        resultComparer = ((short)valIz).CompareTo((short)valDer);
                    }
                    else if (propIz.PropertyType.Equals(typeof(ushort)))
                    {
                        resultComparer = ((ushort)valIz).CompareTo((ushort)valDer);
                    }
                    else if (propIz.PropertyType.Equals(typeof(int)))
                    {
                        resultComparer = ((int)valIz).CompareTo((int)valDer);
                    }
                    else if (propIz.PropertyType.Equals(typeof(uint)))
                    {
                        resultComparer = ((uint)valIz).CompareTo((uint)valDer);
                    }
                    else if (propIz.PropertyType.Equals(typeof(long)))
                    {
                        resultComparer = ((long)valIz).CompareTo((long)valDer);
                    }
                    else if (propIz.PropertyType.Equals(typeof(ulong)))
                    {
                        resultComparer = ((ulong)valIz).CompareTo((ulong)valDer);
                    }
                    else if (propIz.PropertyType.Equals(typeof(float)))
                    {
                        resultComparer = ((float)valIz).CompareTo((float)valDer);
                    }
                    else if (propIz.PropertyType.Equals(typeof(double)))
                    {
                        resultComparer = ((double)valIz).CompareTo((double)valDer);
                    }
                    else if (propIz.PropertyType.Equals(typeof(decimal)))
                    {
                        resultComparer = ((decimal)valIz).CompareTo((decimal)valDer);
                    }
                    else if (propIz.PropertyType.Equals(typeof(char)))
                    {
                        resultComparer = ((char)valIz).CompareTo((char)valDer);
                    }
                    else if (propIz.PropertyType.Equals(typeof(byte?)))
                    {
                        resultComparer = ((byte?)valIz).Value.CompareTo(((byte?)valDer).Value);
                    }
                    else if (propIz.PropertyType.Equals(typeof(sbyte?)))
                    {
                        resultComparer = ((sbyte?)valIz).Value.CompareTo(((sbyte?)valDer).Value);
                    }
                    else if (propIz.PropertyType.Equals(typeof(short?)))
                    {
                        resultComparer = ((short?)valIz).Value.CompareTo(((short?)valDer).Value);
                    }
                    else if (propIz.PropertyType.Equals(typeof(ushort?)))
                    {
                        resultComparer = ((ushort?)valIz).Value.CompareTo(((ushort?)valDer).Value);
                    }
                    else if (propIz.PropertyType.Equals(typeof(int?)))
                    {
                        resultComparer = ((int?)valIz).Value.CompareTo(((int?)valDer).Value);
                    }
                    else if (propIz.PropertyType.Equals(typeof(uint?)))
                    {
                        resultComparer = ((uint?)valIz).Value.CompareTo(((uint?)valDer).Value);
                    }
                    else if (propIz.PropertyType.Equals(typeof(long?)))
                    {
                        resultComparer = ((long?)valIz).Value.CompareTo(((long?)valDer).Value);
                    }
                    else if (propIz.PropertyType.Equals(typeof(ulong?)))
                    {
                        resultComparer = ((ulong?)valIz).Value.CompareTo(((ulong?)valDer).Value);
                    }
                    else if (propIz.PropertyType.Equals(typeof(float?)))
                    {
                        resultComparer = ((float?)valIz).Value.CompareTo(((float?)valDer).Value);
                    }
                    else if (propIz.PropertyType.Equals(typeof(double?)))
                    {
                        resultComparer = ((double?)valIz).Value.CompareTo(((double?)valDer).Value);
                    }
                    else if (propIz.PropertyType.Equals(typeof(decimal?)))
                    {
                        resultComparer = ((decimal?)valIz).Value.CompareTo(((decimal?)valDer).Value);
                    }
                    else if (propIz.PropertyType.Equals(typeof(char?)))
                    {
                        resultComparer = ((char?)valIz).Value.CompareTo(((char?)valDer).Value);
                    }
                    else if (propIz.PropertyType.Equals(typeof(DateTime)))
                    {
                        resultComparer = DateTime.Compare((DateTime)valIz, (DateTime)valDer);
                    }
                    else if (propIz.PropertyType.Equals(typeof(TimeSpan)))
                    {
                        resultComparer = TimeSpan.Compare((TimeSpan)valIz, (TimeSpan)valDer);
                    }
                    else if (propIz.PropertyType.Equals(typeof(object)))
                    {
                        resultComparer = string.Compare(valIz.ToString(), valDer.ToString());
                    }
                    else
                    {
                        throw new ArgumentException("Invalid 'order by' argument.", propIz.Name);
                    }

                    // se actualizan los valores
                    resultIzAux = this.CalcLeftResult(resultComparer, posicion);
                    resultIz += resultIzAux;
                    resultDer += this.CalcRightResult(resultComparer, resultIzAux);
                }

                posicion--;
            }

            if (resultIz == resultDer)
            {
                return 0;
            }
            else if (resultIz < resultDer)
            {
                return -1;
            }
            else
            {
                // si es mayor que 0 retorno 1
                return 1;
            }
        }

        /// <summary>
        /// Get the compare result of the left object.
        /// </summary>
        /// <param name="resultComprarer">Compare function result.</param>
        /// <param name="posicion">Position of the property in the object.</param>
        /// <returns>Left result</returns>
        private int CalcLeftResult(int resultComprarer, int posicion)
        {
            if (resultComprarer == 0)
            {
                return 0;
            }
            else if (resultComprarer < 0)
            {
                return posicion * -1;
            }

            // si es mayor que 0
            return posicion;
        }

        /// <summary>
        /// Get the compare result of the right object.
        /// </summary>
        /// <param name="resultComprarer">Compare function result.</param>
        /// <param name="leftResult">Left result.</param>
        /// <returns>Right result</returns>
        private int CalcRightResult(int resultComprarer, int leftResult)
        {
            if (resultComprarer == 0)
            {
                return 0;
            }
            else
            {
                return leftResult * -1;
            }
        }
    }
}
