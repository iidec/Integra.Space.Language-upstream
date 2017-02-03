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
        internal static List<ProjectionColumn> GetQueryProyection(this PlanNode plan)
        {
            PlanNode projectionNode = plan.FindNode(new PlanNodeTypeEnum[] { PlanNodeTypeEnum.Projection }).Single(x => (PlanNodeTypeEnum)x.Properties["ProjectionType"] == PlanNodeTypeEnum.ObservableSelect && (Type)x.Properties["ParentType"] == typeof(Integra.Space.EventResult));
            List<PlanNode> fromNodes = plan.FindNode(new PlanNodeTypeEnum[] { PlanNodeTypeEnum.ObservableFrom });

            List<ProjectionColumn> result = new List<ProjectionColumn>();

            foreach (PlanNode tuple in projectionNode.Children)
            {
                PlanNode propertyNode = tuple.Children.Last().FindNode(PlanNodeTypeEnum.Property).FirstOrDefault();

                // se valida si se encontraron propiedades, un ejemplo de cuando no encuentra es cuando se hace lo siguiente: select 1 as entero.
                string propertyName = null;
                if (propertyNode != null)
                {
                    propertyName = propertyNode.Properties["Property"].ToString();
                }

                // si no hay propiedades tampoco habra una fuente, por lo tanto, se valida si existe el nodo ObservableFromForLambda.
                PlanNode source = NodesFinder.FindNode(tuple, PlanNodeTypeEnum.ObservableFromForLambda).FirstOrDefault();
                string sourceName = null;
                string sourceAlias = null;
                if (source != null && source.Properties.ContainsKey("SourceName"))
                {
                    sourceAlias = source.Properties["SourceName"].ToString();
                    sourceName = fromNodes.Single(x => x.Children.Single(c => c.NodeType == PlanNodeTypeEnum.Identifier).Properties["Value"].ToString() == sourceAlias).Properties["SourceName"].ToString();
                }

                PlanNode identifierNode = tuple.Children.Single(x => x.NodeType == PlanNodeTypeEnum.Identifier);
                string projectionColumnName = identifierNode.Properties["Value"].ToString();

                Type propertyType = null;

                /*
                if (identifierNode.Properties.ContainsKey("DataType"))
                {
                    propertyType = (Type)identifierNode.Properties["DataType"];
                }

                PlanNode columnValue = tuple.FindNode(new PlanNodeTypeEnum[] { PlanNodeTypeEnum.Cast, PlanNodeTypeEnum.Constant }).SingleOrDefault();*/

                if (tuple.Children[1].NodeType == PlanNodeTypeEnum.Cast || tuple.Children[1].NodeType == PlanNodeTypeEnum.Constant)
                {
                    propertyType = (Type)tuple.Children[1].Properties["DataType"];
                }
                else if (tuple.Children[1].NodeType != PlanNodeTypeEnum.Property)
                {
                    if (tuple.Children[1].Children[1].Properties.ContainsKey("DataType"))
                    {
                        propertyType = (Type)tuple.Children[1].Children[1].Properties["DataType"];
                    }
                }

                result.Add(new ProjectionColumn(propertyName, projectionColumnName, sourceAlias, sourceName, propertyType));
            }

            System.Diagnostics.Contracts.Contract.Ensures(result.Count > 0);

            return result;
        }
    }
}
