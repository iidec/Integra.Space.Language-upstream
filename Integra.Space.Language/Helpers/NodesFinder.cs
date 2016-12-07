//-----------------------------------------------------------------------
// <copyright file="NodesFinder.cs" company="Integra.Space.Language">
//     Copyright (c) Integra.Space.Language. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Language
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Nodes finder class.
    /// </summary>
    internal static class NodesFinder
    {
        /// <summary>
        /// Find the nodes of the specified type in the execution plan.
        /// </summary>
        /// <param name="plan">Execution plan.</param>
        /// <param name="branchesToOmit">Branches to omit in the path.</param>
        /// <param name="types">Plan node type to search.</param>
        /// <returns>List of plan nodes</returns>
        internal static List<PlanNode> FindNode(this PlanNode plan, IEnumerable<PlanNodeTypeEnum> branchesToOmit, params PlanNodeTypeEnum[] types)
        {
            List<PlanNode> resultList = new List<PlanNode>();

            if (plan == null)
            {
                return resultList;
            }

            if (branchesToOmit.Contains(plan.NodeType))
            {
                return resultList;
            }

            if (types.Contains(plan.NodeType))
            {
                resultList.Add(plan);
                return resultList;
            }

            List<PlanNode> children = plan.Children;

            if (children != null)
            {
                foreach (PlanNode p in children)
                {
                    foreach (PlanNode planAux in FindNode(p, branchesToOmit, types))
                    {
                        resultList.Add(planAux);
                    }
                }
            }

            return resultList;
        }

        /// <summary>
        /// Find the nodes of the specified type in the execution plan.
        /// </summary>
        /// <param name="plan">Execution plan.</param>
        /// <param name="types">Plan node type to search.</param>
        /// <returns>List of plan nodes</returns>
        internal static List<PlanNode> FindNode(this PlanNode plan, params PlanNodeTypeEnum[] types)
        {
            List<PlanNode> resultList = new List<PlanNode>();

            if (plan == null)
            {
                return resultList;
            }

            if (types.Contains(plan.NodeType))
            {
                resultList.Add(plan);
                return resultList;
            }

            List<PlanNode> children = plan.Children;

            if (children != null)
            {
                foreach (PlanNode p in children)
                {
                    foreach (PlanNode planAux in FindNode(p, types))
                    {
                        resultList.Add(planAux);
                    }
                }
            }

            return resultList;
        }

        /// <summary>
        /// Find the parent nodes of the specified type in the execution plan.
        /// </summary>
        /// <param name="plan">Execution plan.</param>
        /// <param name="types">Plan node type to search.</param>
        /// <returns>List of plan nodes</returns>
        internal static List<PlanNode> FindParentNode(this PlanNode plan, params PlanNodeTypeEnum[] types)
        {
            List<PlanNode> resultList = new List<PlanNode>();

            if (plan == null)
            {
                return resultList;
            }

            List<PlanNode> children = plan.Children;

            if (children != null)
            {
                if (plan.Children.Any(x => types.Contains(x.NodeType)))
                {
                    resultList.Add(plan);
                }

                foreach (PlanNode p in children)
                {
                    foreach (PlanNode planAux in FindParentNode(p, types))
                    {
                        resultList.Add(planAux);
                    }
                }
            }

            return resultList;
        }

        /// <summary>
        /// Find the nodes of the specified type in the execution plan.
        /// </summary>
        /// <param name="plan">Execution plan.</param>
        /// <param name="types">Plan node type to search.</param>
        /// <returns>List of plan nodes</returns>
        internal static List<SpaceParseTreeNode> FindNode(this SpaceParseTreeNode plan, params SpaceParseTreeNodeTypeEnum[] types)
        {
            List<SpaceParseTreeNode> resultList = new List<SpaceParseTreeNode>();

            if (plan == null)
            {
                return resultList;
            }

            if (types.Contains(plan.Type))
            {
                resultList.Add(plan);
                return resultList;
            }

            List<SpaceParseTreeNode> children = plan.ChildNodes;

            if (children != null)
            {
                foreach (SpaceParseTreeNode p in children)
                {
                    foreach (SpaceParseTreeNode planAux in FindNode(p, types))
                    {
                        resultList.Add(planAux);
                    }
                }
            }

            return resultList;
        }

        /// <summary>
        /// Gets the columns of the projection of the query.
        /// </summary>
        /// <param name="plan">Execution plan of the query.</param>
        /// <returns>List of columns of the projection of the query.</returns>
        internal static List<Tuple<string, Type>> GetQueryProyection(this PlanNode plan)
        {
            PlanNode projectionNode = plan.FindNode(new PlanNodeTypeEnum[] { PlanNodeTypeEnum.Projection }).Single(x => (PlanNodeTypeEnum)x.Properties["ProjectionType"] == PlanNodeTypeEnum.ObservableSelect && (Type)x.Properties["ParentType"] == typeof(Integra.Space.EventResult));

            List<Tuple<string, Type>> result = new List<Tuple<string, Type>>();

            foreach (PlanNode tuple in projectionNode.Children)
            {
                PlanNode identifierNode = tuple.Children.Single(x => x.NodeType == PlanNodeTypeEnum.Identifier);
                string propertyName = identifierNode.Properties["Value"].ToString();
                Type propertyType = (Type)identifierNode.Properties["DataType"];

                /*PlanNode columnValue = tuple.FindNode(new PlanNodeTypeEnum[] { PlanNodeTypeEnum.Cast, PlanNodeTypeEnum.Constant }).SingleOrDefault();*/

                if (tuple.Children[1].NodeType == PlanNodeTypeEnum.Cast || tuple.Children[1].NodeType == PlanNodeTypeEnum.Constant)
                {
                    propertyType = (Type)tuple.Children[1].Properties["DataType"];
                }

                result.Add(Tuple.Create(propertyName, propertyType));
            }

            System.Diagnostics.Contracts.Contract.Ensures(result.Count > 0);

            return result;
        }
    }
}
