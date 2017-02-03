//-----------------------------------------------------------------------
// <copyright file="ConstructionValidator.cs" company="CompanyName">
//     Copyright (c) CompanyName. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Language.Runtime
{
    using System;
    using System.Configuration;
    using System.Linq.Expressions;
    using Exceptions;

    /// <summary>
    /// Observable constructor class
    /// </summary>
    internal static class ConstructionValidator
    {
        /// <summary>
        /// Validate the plan node specified.
        /// </summary>
        /// <param name="nodeType">Plan node type.</param>
        /// <param name="actualNode">Actual execution plan node.</param>
        /// <param name="left">Left expression.</param>
        /// <param name="right">Right expression</param>
        public static void Validate(PlanNodeTypeEnum nodeType, PlanNode actualNode, Expression left, Expression right)
        {
            if (actualNode.Properties.ContainsKey("internallyGenerated") && bool.Parse(actualNode.Properties["internallyGenerated"].ToString()))
            {
                return;
            }

            switch (nodeType)
            {
                case PlanNodeTypeEnum.ObservableBuffer:
                    CreateBufferValidator(actualNode, left, right);
                    break;
                case PlanNodeTypeEnum.ObservableBufferTimeAndSize:
                    CreateBufferTimeAndSizeValidator(actualNode, left, right);
                    break;
                default:
                    throw new CompilationException(string.Format("No es posible validar el nodo {0}.", nodeType.ToString()));
            }
        }

        /// <summary>
        /// Validates the context of the CreateBuffer method in ObservableConstructor class.
        /// </summary>
        /// <param name="actualNode">Actual execution plan node.</param>
        /// <param name="incomingObservable">Incoming observable expression.</param>
        /// <param name="bufferTimeOrSize">Time or size expression for the buffer.</param>
        private static void CreateBufferValidator(PlanNode actualNode, Expression incomingObservable, Expression bufferTimeOrSize)
        {
            if (bufferTimeOrSize.Type.Equals(typeof(DateTime)))
            {
                throw new CompilationException("Error al compilar, el tamaño de la ventana debe ser un espacio de tiempo ('TimeSpan') y no una fecha.");
            }

            if (bufferTimeOrSize.Type.Equals(typeof(TimeSpan)))
            {
                if ((TimeSpan)Expression.Lambda(bufferTimeOrSize).Compile().DynamicInvoke() < TimeSpan.FromSeconds(0) || (TimeSpan)Expression.Lambda(bufferTimeOrSize).Compile().DynamicInvoke() > TimeSpan.FromSeconds(int.Parse(ConfigurationManager.AppSettings["MaxWindowTimeInSeconds"].ToString())))
                {
                    throw new CompilationException("Error al compilar, el tamaño de la ventana debe ser mayor o igual a 1 y menor o igual a " + ConfigurationManager.AppSettings["MaxWindowTimeInSeconds"].ToString() + ".");
                }
            }
        }

        /// <summary>
        /// Validates the context of the CreateBuffer method in ObservableConstructor class.
        /// </summary>
        /// <param name="actualNode">Actual execution plan node.</param>
        /// <param name="incomingObservable">Incoming observable expression.</param>
        /// <param name="bufferTimeAndSize">Time or size expression for the buffer.</param>
        private static void CreateBufferTimeAndSizeValidator(PlanNode actualNode, Expression incomingObservable, Expression bufferTimeAndSize)
        {
            if (bufferTimeAndSize.Type.GetProperty("TimeSpanValue").PropertyType.Equals(typeof(DateTime)))
            {
                throw new CompilationException("Error al compilar, el tamaño de la ventana debe ser un espacio de tiempo ('TimeSpan') y no una fecha.");
            }

            if (((dynamic)Expression.Lambda(bufferTimeAndSize).Compile().DynamicInvoke()).TimeSpanValue < TimeSpan.FromSeconds(0) || ((dynamic)Expression.Lambda(bufferTimeAndSize).Compile().DynamicInvoke()).TimeSpanValue > TimeSpan.FromSeconds(int.Parse(ConfigurationManager.AppSettings["MaxWindowTimeInSeconds"].ToString())))
            {
                throw new CompilationException("Error al compilar, el tamaño de la ventana debe ser mayor o igual a 1 y menor o igual a " + ConfigurationManager.AppSettings["MaxWindowTimeInSeconds"].ToString() + ".");
            }

            if (((dynamic)Expression.Lambda(bufferTimeAndSize).Compile().DynamicInvoke()).IntegerValue < 1 || ((dynamic)Expression.Lambda(bufferTimeAndSize).Compile().DynamicInvoke()).IntegerValue > int.Parse(ConfigurationManager.AppSettings["MaxWindowSize"].ToString()))
            {
                throw new CompilationException("Error al compilar, el tamaño de la ventana debe ser mayor o igual a 1 y menor o igual a " + ConfigurationManager.AppSettings["MaxWindowTimeInSeconds"].ToString() + ".");
            }
        }
    }
}
