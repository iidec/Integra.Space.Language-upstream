//-----------------------------------------------------------------------
// <copyright file="NodesFinder.cs" company="Integra.Space.Language">
//     Copyright (c) Integra.Space.Language. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Language.Runtime
{
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
        /// <param name="target">Plan node type to search.</param>
        /// <returns>List of plan nodes</returns>
        internal static List<PlanNode> FindNode(this PlanNode plan, PlanNodeTypeEnum target)
        {
            List<PlanNode> resultList = new List<PlanNode>();

            if (plan == null)
            {
                return resultList;
            }

            List<PlanNode> children = plan.Children;

            if (children != null)
            {
                foreach (PlanNode p in children)
                {
                    if (p.NodeType == target)
                    {
                        resultList.Add(p);
                    }

                    if (p.Children != null)
                    {
                        foreach (PlanNode planAux in FindNode(p, target))
                        {
                            resultList.Add(planAux);
                        }
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
    }
}
