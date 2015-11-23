//-----------------------------------------------------------------------
// <copyright file="PlanNodeTypeExtensions.cs" company="Integra.Space.Language">
//     Copyright (c) Integra.Space.Language. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Language
{
    /// <summary>
    /// Plan node type enumerable extensions
    /// </summary>
    internal static class PlanNodeTypeExtensions
    {
        /// <summary>
        /// Verify if the node type is group by type.
        /// </summary>
        /// <param name="nodeType">Plan node type to verify.</param>
        /// <returns>True if is group by type, otherwise false</returns>
        public static bool IsGroupBy(this PlanNodeTypeEnum nodeType)
        {
            return PlanNodeTypeEnum.ObservableGroupBy.Equals(nodeType) || PlanNodeTypeEnum.EnumerableGroupBy.Equals(nodeType);
        }

        /// <summary>
        /// Verify if the node type is buffer type.
        /// </summary>
        /// <param name="nodeType">Plan node type to verify.</param>
        /// <returns>True if is buffer type, otherwise false</returns>
        public static bool IsBuffer(this PlanNodeTypeEnum nodeType)
        {
            return PlanNodeTypeEnum.ObservableBuffer.Equals(nodeType) || PlanNodeTypeEnum.ObservableBufferTimeAndSize.Equals(nodeType);
        }

        /// <summary>
        /// Verify if the node type is where type.
        /// </summary>
        /// <param name="nodeType">Plan node type to verify.</param>
        /// <returns>True if is where type, otherwise false</returns>
        public static bool IsWhere(this PlanNodeTypeEnum nodeType)
        {
            return PlanNodeTypeEnum.ObservableWhere.Equals(nodeType);
        }

        /// <summary>
        /// Verify if the node type is select projection type.
        /// </summary>
        /// <param name="nodeType">Plan node type to verify.</param>
        /// <returns>True if is select projection type, otherwise false</returns>
        public static bool IsProjectionOfSelect(this PlanNodeTypeEnum nodeType)
        {
            return PlanNodeTypeEnum.Projection.Equals(nodeType);
        }

        /// <summary>
        /// Verify if the node type is order by projection type.
        /// </summary>
        /// <param name="nodeType">Plan node type to verify.</param>
        /// <returns>True if is select order by type, otherwise false</returns>
        public static bool IsOrderBy(this PlanNodeTypeEnum nodeType)
        {
            return PlanNodeTypeEnum.EnumerableOrderBy.Equals(nodeType) || PlanNodeTypeEnum.EnumerableOrderByDesc.Equals(nodeType);
        }
    }
}
