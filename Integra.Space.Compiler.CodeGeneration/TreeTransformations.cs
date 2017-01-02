//-----------------------------------------------------------------------
// <copyright file="TreeTransformations.cs" company="Integra.Space.Language">
//     Copyright (c) Integra.Space.Language. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Compiler
{
    using Language;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection.Emit;

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
        /// Dictionary of specific extracted data types for each source.
        /// </summary>
        private Dictionary<string, Type> dictionaryOfTypes;

        /// <summary>
        /// Source type factory.
        /// </summary>
        ISourceTypeFactory sourceTypeFactory;

        /// <summary>
        /// Assembly builder.
        /// </summary>
        private AssemblyBuilder asmBuilder;

        /// <summary>
        /// Source list.
        /// </summary>
        private IEnumerable<Source> sources;

        /// <summary>
        /// Initializes a new instance of the <see cref="TreeTransformations"/> class.
        /// </summary>
        /// <param name="asmBuilder">Assembly builder.</param>
        /// <param name="executionPlanRootNode">Execution plan root node.</param>
        /// <param name="sourceTypeFactory">Source type factory to get the type of the sources specified at the query.</param>
        public TreeTransformations(AssemblyBuilder asmBuilder, PlanNode executionPlanRootNode, ISourceTypeFactory sourceTypeFactory)
        {
            this.asmBuilder = asmBuilder;
            this.executionPlanRootNode = executionPlanRootNode;
            this.sourceTypeFactory = sourceTypeFactory;

            this.sources = this.executionPlanRootNode.FindNode(PlanNodeTypeEnum.ObservableFrom).Select(x =>
            {
                string alias = null;
                if (x.Properties.ContainsKey("SourceAlias"))
                {
                    alias = x.Properties["SourceAlias"].ToString();
                }

                return new Source(x.Properties["SourceName"].ToString(), alias);
            });
        }

        /// <summary>
        /// Transform the execution plan to get it ready to compile.
        /// </summary>
        public void Transform()
        {
            this.AddSourceIdToObservableFromForLambdaNodes();
            IEnumerable<IGrouping<string, PlanNode>> objects = this.GetPropertyNodesGroupedBySource(this.executionPlanRootNode);

            if (objects.Count() > 0)
            {
                this.CreateExtractedEventDataSpecificTypes(objects);
                this.InsertGenerationOfLocalObjects(objects);
                this.ModifyOnConditionBranch();
            }
        }

        /// <summary>
        /// Modifies the 'on' condition branch.
        /// </summary>
        private void ModifyOnConditionBranch()
        {
            PlanNode branch = this.executionPlanRootNode.FindNode(PlanNodeTypeEnum.On).SingleOrDefault();

            // esta condición aplica para las consultas hacia una sola fuente.
            if (branch == null)
            {
                return;
            }

            IEnumerable<IGrouping<string, PlanNode>> objects = branch.FindNode(PlanNodeTypeEnum.ObservableFromForLambda)
                .GroupBy(x => x.FindNode(PlanNodeTypeEnum.ObservableFromForLambda).Single().Properties["SourceName"].ToString());

            IEnumerable<PlanNode> sources = this.executionPlanRootNode.FindNode(PlanNodeTypeEnum.ObservableFrom).Select(x => x.Children.Single());

            int position = 0;
            foreach (PlanNode source in sources)
            {
                IGrouping<string, PlanNode> group = objects.Single(x => x.Key == source.Properties["Value"].ToString());
                foreach (PlanNode node in group)
                {
                    if (!node.Properties.ContainsKey("ParameterPosition"))
                    {
                        node.Properties.Add("ParameterPosition", position);
                    }
                }

                position++;
            }
        }

        /// <summary>
        /// Creates the specific extracted event data types for each source.
        /// </summary>
        /// <param name="objects">Objects used in the query grouped by source.</param>
        private void CreateExtractedEventDataSpecificTypes(IEnumerable<IGrouping<string, PlanNode>> objects)
        {
            this.dictionaryOfTypes = new Dictionary<string, Type>();
            List<FieldNode> fieldList;
            int position = 0;
            ExtractedEventDataTypeBuilder eedtb;
            Type newType = null;
            foreach (IGrouping<string, PlanNode> grupo in objects)
            {
                Source source = this.sources.Where(x => x.Alias == grupo.Key || x.Name == grupo.Key).Single();
                fieldList = new List<FieldNode>();
                foreach (PlanNode @object in grupo.Distinct(new PropertyNameComparer()))
                {
                    Type sourceType = this.sourceTypeFactory.GetSourceType(new Common.CommandObject(Common.SystemObjectEnum.Source, source.Name, Common.PermissionsEnum.None, false));
                    Type propType = sourceType.GetProperty(@object.Properties["Property"].ToString()).PropertyType;
                    fieldList.Add(new FieldNode(@object.Properties["Property"].ToString(), this.ConvertToNullable(propType), 0));
                }

                eedtb = new ExtractedEventDataTypeBuilder(this.asmBuilder, fieldList, position);
                newType = eedtb.CreateNewType();

                this.dictionaryOfTypes.Add(grupo.Key, newType);
                position++;
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
                PlanNode newScopeForSelect = new PlanNode(@object.First().Line, @object.First().Column, @object.First().NodeText);
                newScopeForSelect.NodeType = PlanNodeTypeEnum.NewScope;

                PlanNode projection = new PlanNode(@object.First().Line, @object.First().Column, @object.First().NodeText);
                projection.NodeType = PlanNodeTypeEnum.Projection;
                projection.Properties.Add("ProjectionType", PlanNodeTypeEnum.ObservableExtractedEventData);
                projection.Properties.Add("ParentType", typeof(ExtractedEventData));
                projection.Properties.Add("DisposeEvents", true);
                projection.Properties.Add("OverrideGetHashCodeMethod", false);
                projection.Children = new List<PlanNode>();

                PlanNode selectNode = new PlanNode(@object.First().Line, @object.First().Column, @object.First().NodeText);
                selectNode.NodeType = PlanNodeTypeEnum.ObservableSelectForObservableBufferOrSource;
                selectNode.Properties.Add("SourceName", @object.Key);
                selectNode.Children = new List<PlanNode>();
                selectNode.Children.Add(newScopeForSelect);
                selectNode.Children.Add(projection);

                localObjects.Add(selectNode);

                // se hace el where para filtrar la propiedad SystemTimestamp ya que la clase ExtractedEventData ya tiene dicha propiedad y solo tiene metodo get
                IEnumerable<PlanNode> replicas = this.CreateACopyOfQueryObjectsAccessors(@object.Where(x => (x.NodeType == PlanNodeTypeEnum.Property && !x.Properties["Property"].ToString().Equals("SystemTimestamp"))).Distinct(new PropertyNameComparer()));
                IEnumerable<IGrouping<string, PlanNode>> objectsInOnCondition = this.GetPropertyNodesGroupedBySource(this.executionPlanRootNode.FindNode(PlanNodeTypeEnum.On).FirstOrDefault());

                if (objectsInOnCondition != null && objectsInOnCondition.Count() > 0)
                {
                    projection.Properties["ProjectionType"] = PlanNodeTypeEnum.ObservableExtractedEventDataComparer;
                    projection.Properties["OverrideGetHashCodeMethod"] = true;

                    // esto solo servirá para casos con dos fuentes unicamente
                    projection.Properties["ParentType"] = this.dictionaryOfTypes[@object.Key];
                    projection.Properties.Add("OtherSourceType", this.dictionaryOfTypes.Where(x => x.Key != @object.Key).FirstOrDefault().Value);

                    IGrouping<string, PlanNode> aux = objectsInOnCondition.FirstOrDefault(x => x.Key == @object.Key);

                    // en el observable constructor se valida que todos los valores puestos en el oncondition no son constantes
                    if (aux != null)
                    {
                        foreach (PlanNode objectInOn in aux)
                        {
                            foreach (PlanNode column in replicas)
                            {
                                if (!column.Properties.ContainsKey("IncidenciasEnOn"))
                                {
                                    column.Properties.Add("IncidenciasEnOn", 0);
                                }

                                if (objectInOn.Properties["Property"].Equals(column.Properties["Property"]))
                                {
                                    column.Properties["IncidenciasEnOn"] = int.Parse(column.Properties["IncidenciasEnOn"].ToString()) + 1;
                                }
                            }
                        }
                    }
                }

                foreach (PlanNode column in replicas)
                {
                    PlanNode tupleProjection = new PlanNode(column.Line, column.Column, column.NodeText);
                    tupleProjection.NodeType = PlanNodeTypeEnum.TupleProjection;
                    tupleProjection.Children = new List<PlanNode>();
                    projection.Children.Add(tupleProjection);

                    PlanNode alias = new PlanNode(column.Line, column.Column, column.NodeText);
                    alias.NodeType = PlanNodeTypeEnum.Identifier;
                    alias.NodeText = column.Properties["Property"].ToString();
                    alias.NodeType = PlanNodeTypeEnum.Identifier;
                    alias.Properties.Add("Value", column.Properties["Property"].ToString());
                    alias.Properties.Add("DataType", typeof(object));

                    tupleProjection.Children.Add(alias);

                    PlanNode copy = new PlanNode(column.Line, column.Column, column.NodeText);
                    copy.Children = column.Children;
                    copy.Column = column.Column;
                    copy.Line = column.Line;
                    copy.NodeText = column.NodeText;
                    copy.NodeType = column.NodeType;
                    column.Properties.ToList().ForEach(x => copy.Properties.Add(x.Key, x.Value));

                    tupleProjection.Children.Add(copy);
                }
            }

            List<PlanNode> parentsOfFromNodes = this.executionPlanRootNode.FindParentNode(PlanNodeTypeEnum.ObservableFrom);

            foreach (PlanNode parentOfFromNode in parentsOfFromNodes)
            {
                PlanNode fromNode = parentOfFromNode.Children[0];
                PlanNode selectNode = localObjects.Where(x => x.Properties["SourceName"].Equals(fromNode.Properties["SourceAlias"])).Single();
                selectNode.Children[0].Children = new List<PlanNode>();
                selectNode.Children[0].Children.Add(fromNode);
                parentOfFromNode.Children[0] = selectNode;
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
                PlanNode replica = new PlanNode(column.Line, column.Column, column.NodeText);
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
        /// <param name="branch">Branch to get the values of the events.</param>
        /// <returns>Objects grouped by source.</returns>
        private IEnumerable<IGrouping<string, PlanNode>> GetPropertyNodesGroupedBySource(PlanNode branch)
        {
            // la propiedad SystemTimestamp se filtra porque ExtractedEventData ya la contiene
            IEnumerable<IGrouping<string, PlanNode>> objects = branch.FindNode(PlanNodeTypeEnum.Property).Where(x => x.Children.Single().Properties.ContainsKey("SourceName"))
                .Where(x => !(x.Properties.ContainsKey("InternalUse") && bool.Parse(x.Properties["InternalUse"].ToString())))
                .GroupBy(x => x.FindNode(PlanNodeTypeEnum.ObservableFromForLambda).Single().Properties["SourceName"].ToString());
            
            return objects;
        }

        /// <summary>
        /// Do things to the syntax tree
        /// </summary>
        private void AddSourceIdToObservableFromForLambdaNodes()
        {
            // IEnumerable<string> sources = this.executionPlanRootNode.FindNode(PlanNodeTypeEnum.ObservableFrom).Select(x => x.Children.First().Properties["Value"].ToString());
            List<PlanNode> sourceRefNodes = this.executionPlanRootNode.FindNode(PlanNodeTypeEnum.Property)
                .Where(x => !(x.Properties.ContainsKey("InternalUse") && bool.Parse(x.Properties["InternalUse"].ToString())))
                .Select(x => x.Children.Single()).ToList(); // new List<PlanNode>(); // this.executionPlanRootNode.FindNode(PlanNodeTypeEnum.Event);
            IEnumerable<string> sourceRefs = null;

            // obtengo referencias vacias, es decir, llamadas a eventos sin referenciar a la fuente. Ej: @event.Mes...
            IEnumerable<PlanNode> emptyRefNodes = sourceRefNodes.Where(x => !x.Properties.ContainsKey("SourceName") || x.Properties["SourceName"].ToString().Equals(string.Empty));
            if (this.sources.Count() == 1)
            {
                // obtengo la fuente
                string source = this.sources.First().Alias;

                foreach (PlanNode emptyRef in emptyRefNodes)
                {
                    // transormo las referencias vacias al nombre de la fuente
                    emptyRef.Properties.Add("SourceName", source);
                    emptyRef.NodeText = source;
                }

                // si hace referencia a una fuente diferente a la del from lanzo una excepcion
                sourceRefs = sourceRefNodes.Select(x => x.Properties["SourceName"].ToString());
                if (!sourceRefs.All(x => x == source))
                {
                    throw new Language.Exceptions.CompilationException(string.Format("Invalid source {0}.", source));
                }
            }
            else
            {
                // si existe alguna referencia vacia, lanzo una excepción
                if (emptyRefNodes.Count() > 0)
                {
                    string place = emptyRefNodes.Select(x => string.Format("line: {0} and column: {1}", x.Line, x.Column)).First();
                    throw new Language.Exceptions.CompilationException(string.Format("You need to specify the source for the event at {0}.", place));
                }

                // verifico que solo se hagan referencia a las fuentes especificadas en el join y with
                sourceRefs = sourceRefNodes.Select(x => x.Properties["SourceName"].ToString());
                foreach (string source in this.sources.Select(x => x.Alias))
                {
                    if (!sourceRefs.Contains(source))
                    {
                        throw new Language.Exceptions.CompilationException(string.Format("Invalid source {0}.", source));
                    }
                }
            }

            PlanNode selectForGroupBy = this.executionPlanRootNode.FindNode(PlanNodeTypeEnum.EnumerableSelectForGroupBy).SingleOrDefault();

            if(selectForGroupBy != null)
            {
                selectForGroupBy.Children[1].FindNode(new PlanNodeTypeEnum[] { PlanNodeTypeEnum.EnumerableSum, PlanNodeTypeEnum.EnumerableMin, PlanNodeTypeEnum.EnumerableMax }, PlanNodeTypeEnum.ObservableFromForLambda).ForEach(x =>
                {
                    if (x.Properties.ContainsKey("SourceName"))
                    {
                        x.Properties.Remove("SourceName");
                    }
                });
            }

            PlanNode enumerableOrderBy = this.executionPlanRootNode.FindNode(PlanNodeTypeEnum.EnumerableOrderBy, PlanNodeTypeEnum.EnumerableOrderByDesc).SingleOrDefault();

            if(enumerableOrderBy != null)
            {
                enumerableOrderBy.Children[1].FindNode(PlanNodeTypeEnum.ObservableFromForLambda).ForEach(x =>
                {
                    if (x.Properties.ContainsKey("SourceName"))
                    {
                        x.Properties.Remove("SourceName");
                    }
                });
            }
        }

        /// <summary>
        /// Try to convert the no null-able type to a null-able type
        /// </summary>
        /// <param name="tipo">Type to convert</param>
        /// <returns>Converted type</returns>
        private Type ConvertToNullable(Type tipo)
        {
            if (tipo.IsValueType)
            {
                if (tipo.IsGenericType && tipo.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    return tipo;
                }
                else
                {
                    return typeof(Nullable<>).MakeGenericType(tipo);
                }
            }
            else
            {
                return tipo;
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
                if (x.Properties["Property"].Equals(y.Properties["Property"]))
                {
                    return true;
                }

                return false;
            }

            /// <inheritdoc />
            public int GetHashCode(PlanNode obj)
            {
                return obj.Properties["Property"].GetHashCode();
            }
        }

        /// <summary>
        /// Source node.
        /// </summary>
        private class Source
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="Source"/> class.
            /// </summary>
            /// <param name="name"></param>
            /// <param name="alias"></param>
            public Source(string name, string alias)
            {
                this.Name = name;
                this.Alias = alias;
            }

            public string Alias { get; private set; }

            public string Name { get; private set; }
        }
    }
}
