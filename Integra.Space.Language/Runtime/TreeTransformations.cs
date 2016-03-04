//-----------------------------------------------------------------------
// <copyright file="TreeTransformations.cs" company="Integra.Space.Language">
//     Copyright (c) Integra.Space.Language. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Language.Runtime
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Exceptions;
    using Metadata;

    /// <summary>
    /// Tree transformations class
    /// </summary>
    internal sealed class TreeTransformations
    {
        /// <summary>
        /// Execution plan root node
        /// </summary>
        private PlanNode executionPlanRootNode;

        /// <summary>
        /// Initializes a new instance of the <see cref="TreeTransformations"/> class.
        /// </summary>
        /// <param name="executionPlanRootNode">Execution plan root node.</param>
        public TreeTransformations(PlanNode executionPlanRootNode)
        {
            this.executionPlanRootNode = executionPlanRootNode;
        }

        /// <summary>
        /// Transform the execution plan to get it ready to compile.
        /// </summary>
        public void Transform()
        {
            this.AddSourceIdToEventNodes();
            IEnumerable<IGrouping<string, PlanNode>> objects = this.GetValuesFromEvents();
            if (objects.Count() > 0)
            {
                this.InsertGenerationOfLocalObjects(objects);
                this.ReplaceEventsByLocalObject(objects);
            }
        }

        /// <summary>
        /// Replace event branches by access to local object properties
        /// </summary>
        /// <param name="objects">Objects grouped by source.</param>
        private void ReplaceEventsByLocalObject(IEnumerable<IGrouping<string, PlanNode>> objects)
        {
            foreach (IGrouping<string, PlanNode> @object in objects)
            {
                foreach (PlanNode column in @object)
                {
                    column.NodeType = PlanNodeTypeEnum.Property;
                    string propertyName = column.Properties["PropertyName"].ToString();
                    column.Properties.Clear();
                    column.Children.Clear();
                    column.Properties.Add("source", @object.Key);
                    column.Properties.Add("Property", propertyName);
                    column.Children = new List<PlanNode>();

                    PlanNode fromForLambda = new PlanNode();
                    /*fromForLambda.Properties.Add("ParameterName", @object.Key);*/
                    fromForLambda.NodeType = PlanNodeTypeEnum.ObservableFromForLambda;
                    column.Children.Add(fromForLambda);
                }
            }
        }

        /// <summary>
        /// Insert into the execution tree the nodes for local object generation.
        /// </summary>
        /// <param name="objects">Objects grouped by source.</param>
        private void InsertGenerationOfLocalObjects(IEnumerable<IGrouping<string, PlanNode>> objects)
        {
            List<PlanNode> localObjects = new List<PlanNode>();
            foreach (IGrouping<string, PlanNode> @object in objects)
            {
                PlanNode newScopeForSelect = new PlanNode();
                newScopeForSelect.NodeType = PlanNodeTypeEnum.NewScope;

                PlanNode projection = new PlanNode();
                projection.NodeType = PlanNodeTypeEnum.Projection;
                projection.Properties.Add("ProjectionType", PlanNodeTypeEnum.ObservableSelect);
                projection.Properties.Add("DisposeEvents", true);
                projection.Properties.Add("OverrideGetHashCodeMethod", false);
                projection.Children = new List<PlanNode>();

                PlanNode selectNode = new PlanNode();
                selectNode.NodeType = PlanNodeTypeEnum.ObservableSelectForObservableBufferOrSource;
                selectNode.Properties.Add("Source", @object.Key);
                selectNode.Children = new List<PlanNode>();
                selectNode.Children.Add(newScopeForSelect);
                selectNode.Children.Add(projection);

                localObjects.Add(selectNode);

                IEnumerable<PlanNode> replicas = this.CreateACopyOfQueryObjectsAccessors(@object.Distinct(new PropertyNameComparer()));
                foreach (PlanNode column in replicas)
                {
                    PlanNode tupleProjection = new PlanNode();
                    tupleProjection.NodeType = PlanNodeTypeEnum.TupleProjection;
                    tupleProjection.Children = new List<PlanNode>();
                    projection.Children.Add(tupleProjection);

                    PlanNode alias = new PlanNode();
                    alias.NodeType = PlanNodeTypeEnum.Identifier;
                    alias.NodeText = column.Properties["PropertyName"].ToString();
                    alias.NodeType = PlanNodeTypeEnum.Identifier;
                    alias.Properties.Add("Value", column.Properties["PropertyName"].ToString());
                    alias.Properties.Add("DataType", typeof(object).ToString());

                    tupleProjection.Children.Add(alias);

                    PlanNode copy = new PlanNode();
                    copy.Children = column.Children;
                    copy.Column = column.Column;
                    copy.Line = column.Line;
                    copy.NodeText = column.NodeText;
                    copy.NodeType = column.NodeType;
                    column.Properties.ToList().ForEach(x => copy.Properties.Add(x.Key, x.Value));

                    tupleProjection.Children.Add(copy);
                }
            }

            List<PlanNode> parentsWheresEventLock = this.executionPlanRootNode.FindParentNode(PlanNodeTypeEnum.ObservableWhereForEventLock);
            foreach (PlanNode whereEventLockParent in parentsWheresEventLock)
            {
                for (int i = 0; i < whereEventLockParent.Children.Count; i++)
                {
                    PlanNode whereEventLock = whereEventLockParent.Children[i];
                    PlanNode selectNode = localObjects.Where(x => x.Properties["Source"].Equals(whereEventLock.Properties["Source"])).Single();
                    selectNode.Children[0].Children = new List<PlanNode>();
                    selectNode.Children[0].Children.Add(whereEventLock);
                    whereEventLockParent.Children[i] = selectNode;
                }
            }

            localObjects.ToArray();
        }

        /// <summary>
        /// Creates a replica of the list of specified and it's children
        /// </summary>
        /// <param name="objectAccessors">Object accessors execution plan branches.</param>
        /// <returns>Branches replicated.</returns>
        private IEnumerable<PlanNode> CreateACopyOfQueryObjectsAccessors(IEnumerable<PlanNode> objectAccessors)
        {
            List<PlanNode> result = new List<PlanNode>();
            foreach (PlanNode column in objectAccessors)
            {
                PlanNode replica = new PlanNode();
                replica.Column = column.Column;
                replica.Line = column.Line;
                replica.NodeText = column.NodeText;
                replica.NodeType = column.NodeType;
                column.Properties.ToList().ForEach(x => replica.Properties.Add(x.Key, x.Value));

                if (column.Children != null)
                {
                    replica.Children = this.CreateACopyOfQueryObjectsAccessors(column.Children).ToList();
                }

                result.Add(replica);
            }

            return result;
        }

        /// <summary>
        /// Gets and replace the event values branches with the access to the properties of the respective local object.
        /// </summary>
        /// <returns>Objects grouped by source.</returns>
        private IEnumerable<IGrouping<string, PlanNode>> GetValuesFromEvents()
        {
            IEnumerable<IGrouping<string, PlanNode>> objects = this.executionPlanRootNode.FindNode(PlanNodeTypeEnum.ObjectValue, PlanNodeTypeEnum.EventProperty)
                        .GroupBy(x =>
                        {
                            PlanNode identifier = x.FindNode(PlanNodeTypeEnum.Event).First().Children.Where(y => y.NodeType == PlanNodeTypeEnum.Identifier).FirstOrDefault();

                            if (identifier != null)
                            {
                                return identifier.Properties["Value"].ToString();
                            }
                            else
                            {
                                return this.executionPlanRootNode.Children.Where(w => w.NodeType == PlanNodeTypeEnum.ObservableFrom).First().Children.First().Properties["Value"].ToString();
                            }

                            /*if (x.NodeType == PlanNodeTypeEnum.Cast)
                            {
                                // identifier = x.FindNode(SpaceParseTreeNodeTypeEnum.OBJECT).Single().ChildNodes.Where(y => y.Type == SpaceParseTreeNodeTypeEnum.EVENT).First().ChildNodes.Where(y => y.Type == SpaceParseTreeNodeTypeEnum.identifier).FirstOrDefault();
                                identifier = x.FindNode(PlanNodeTypeEnum.Event).First().Children.Where(y => y.NodeType == PlanNodeTypeEnum.Identifier).FirstOrDefault();
                            }
                            else
                            {
                                // identifier = x.ChildNodes.Where(y => y.Type == SpaceParseTreeNodeTypeEnum.EVENT).First().ChildNodes.Where(y => y.Type == SpaceParseTreeNodeTypeEnum.identifier).FirstOrDefault();
                                identifier = x.FindNode(PlanNodeTypeEnum.Event).First().Children.Where(y => y.NodeType == PlanNodeTypeEnum.Identifier).FirstOrDefault();
                            }

                            if (identifier != null)
                            {
                                return identifier.Properties["Value"].ToString();
                            }
                            else
                            {
                                return this.executionPlanRootNode.Children.Where(w => w.NodeType == PlanNodeTypeEnum.ObservableFrom).First().Children.First().Properties["Value"].ToString();
                            }*/
                        });

            if (objects.Count() == 0)
            {
                System.Diagnostics.Debug.WriteLine("No events found in the on condition.");
            }

            return objects;
        }

        /// <summary>
        /// Do things to the syntax tree
        /// </summary>
        private void AddSourceIdToEventNodes()
        {
            IEnumerable<string> sources = this.executionPlanRootNode.FindNode(PlanNodeTypeEnum.ObservableFrom).Select(x => x.Children.First().Properties["Value"].ToString());
            List<PlanNode> sourceRefNodes = this.executionPlanRootNode.FindNode(PlanNodeTypeEnum.Event);
            IEnumerable<string> sourceRefs = null;

            // obtengo referencias vacias, es decir, llamadas a eventos sin referenciar a la fuente. Ej: @event.Mes...
            IEnumerable<PlanNode> emptyRefNodes = sourceRefNodes.Where(x => x.Children[0].Properties["Value"].ToString().Equals(string.Empty));

            if (sources.Count() == 1)
            {
                // obtengo la fuente
                string source = sources.First();

                foreach (PlanNode emptyRef in emptyRefNodes)
                {
                    // transormo las referencias vacias al nombre de la fuente
                    emptyRef.Children[0].Properties["Value"] = source;
                    emptyRef.Children[0].NodeText = source;
                }

                // si hace referencia a una fuente diferente a la del from lanzo una excepcion
                sourceRefs = sourceRefNodes.Select(x => x.Children[0].Properties["Value"].ToString());
                if (!sourceRefs.All(x => x == source))
                {
                    throw new Exceptions.CompilationException(string.Format("Invalid source {0}.", source));
                }
            }
            else
            {
                // si existe alguna referencia vacia, lanzo una excepción
                if (emptyRefNodes.Count() > 0)
                {
                    string place = emptyRefNodes.Select(x => string.Format("line: {0} and column: {1}", x.Line, x.Column)).First();
                    throw new Exceptions.CompilationException(string.Format("You need to specify the source for the event at {0}.", place));
                }

                // verifico que solo se hagan referencia a las fuentes especificadas en el join y with
                sourceRefs = sourceRefNodes.Select(x => x.Children[0].Properties["Value"].ToString());
                foreach (string source in sources)
                {
                    if (!sourceRefs.Contains(source))
                    {
                        throw new Exceptions.CompilationException(string.Format("Invalid source {0}.", source));
                    }
                }
            }
        }

        /// <summary>
        /// Class to compare the property names used within the query 
        /// </summary>
        private class PropertyNameComparer : IEqualityComparer<PlanNode>
        {
            /// <inheritdoc />
            public bool Equals(PlanNode x, PlanNode y)
            {
                if (x.Properties["PropertyName"].Equals(y.Properties["PropertyName"]))
                {
                    return true;
                }

                return false;
            }

            /// <inheritdoc />
            public int GetHashCode(PlanNode obj)
            {
                return obj.Properties["PropertyName"].GetHashCode();
            }
        }
    }
}
