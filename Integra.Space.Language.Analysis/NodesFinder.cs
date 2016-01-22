//-----------------------------------------------------------------------
// <copyright file="NodesFinder.cs" company="Integra.Space.Language.Analysis">
//     Copyright (c) Integra.Space.Language.Analysis. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Language.Analysis
{
    using System.Collections.Generic;

    /// <summary>
    /// Nodes finder class.
    /// </summary>
    internal class NodesFinder
    {
        /// <summary>
        /// Find the nodes of the specified type in the execution plan.
        /// </summary>
        /// <param name="plan">Execution plan.</param>
        /// <param name="target">Plan node type to search.</param>
        /// <returns>List of plan nodes</returns>
        internal List<PlanNode> FindNode(PlanNode plan, PlanNodeTypeEnum target)
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
                        foreach (PlanNode planAux in this.FindNode(p, target))
                        {
                            resultList.Add(planAux);
                        }
                    }
                }
            }

            return resultList;
        }
    }
}
