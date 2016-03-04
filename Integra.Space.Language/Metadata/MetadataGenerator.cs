//-----------------------------------------------------------------------
// <copyright file="MetadataGenerator.cs" company="Integra.Space.Language">
//     Copyright (c) Integra.Space.Language. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Language.Metadata
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Integra.Space.Language.Exceptions;
    using Integra.Space.Language.Runtime;
    using Irony.Parsing;

    /// <summary>
    /// Query metadata generator class
    /// </summary>
    internal class MetadataGenerator
    {
        /// <summary>
        /// Get the metadata from the parse tree
        /// </summary>
        /// <param name="parseTreeNode">Parse tree root node</param>
        /// <returns>Metadata tree structure</returns>
        public SpaceMetadataTreeNode GenerateMetadata(SpaceParseTreeNode parseTreeNode)
        {
            SpaceMetadataTreeNode root = new SpaceMetadataTreeNode(SpaceMetadataTreeNodeTypeEnum.Query);

            if (parseTreeNode == null)
            {
                return root;
            }

            root.ChildNodes = new List<SpaceMetadataTreeNode>();

            // se obtienen las fuentes
            SpaceParseTreeNode sourceDefinition = parseTreeNode.FindNode(SpaceParseTreeNodeTypeEnum.SOURCE_DEFINITION).First();
            List<SpaceParseTreeNode> sources = sourceDefinition.FindNode(SpaceParseTreeNodeTypeEnum.FROM, SpaceParseTreeNodeTypeEnum.JOIN_SOURCE, SpaceParseTreeNodeTypeEnum.WITH);

            SpaceMetadataTreeNode metadataSources = new SpaceMetadataTreeNode(SpaceMetadataTreeNodeTypeEnum.Sources);
            metadataSources.ChildNodes = new List<SpaceMetadataTreeNode>();
            root.ChildNodes.Add(metadataSources);

            foreach (SpaceParseTreeNode source in sources)
            {
                SpaceMetadataTreeNode metadataSource = new SpaceMetadataTreeNode(SpaceMetadataTreeNodeTypeEnum.Source);
                metadataSource.Value = source.FindNode(SpaceParseTreeNodeTypeEnum.ID_OR_ID_WITH_ALIAS).SingleOrDefault().ChildNodes.Last().TokenValue;
                metadataSources.ChildNodes.Add(metadataSource);
            }

            // se obtienen las propiedades utilizadas de las fuentes definidas
            SpaceMetadataTreeNode objectPrpertiesUsed = new SpaceMetadataTreeNode(SpaceMetadataTreeNodeTypeEnum.SourcePropertiesUsed);
            objectPrpertiesUsed.ChildNodes = new List<SpaceMetadataTreeNode>();
            root.ChildNodes.Add(objectPrpertiesUsed);

            this.GetObjectPropertiesUsedInTheBranch(parseTreeNode, objectPrpertiesUsed, root, false);

            // se obtiene la ventana de la consulta
            SpaceParseTreeNode applyWindow = parseTreeNode.FindNode(SpaceParseTreeNodeTypeEnum.APPLY_WINDOW).SingleOrDefault();

            if (applyWindow != null)
            {
                SpaceParseTreeNode windowValue = applyWindow.ChildNodes.Last();
                SpaceMetadataTreeNode metadataApplyWindow = new SpaceMetadataTreeNode(SpaceMetadataTreeNodeTypeEnum.Window);
                metadataApplyWindow.Value = windowValue.TokenValue;
                metadataApplyWindow.ValueDataType = windowValue.TokenValueDataType;
                root.ChildNodes.Add(metadataApplyWindow);
            }

            // se obtiene el agrupamiento     
            SpaceParseTreeNode groupBy = parseTreeNode.FindNode(SpaceParseTreeNodeTypeEnum.GROUP_BY_OP).SingleOrDefault();
            if (groupBy != null)
            {
                SpaceMetadataTreeNode metadataGroupBy = new SpaceMetadataTreeNode(SpaceMetadataTreeNodeTypeEnum.GroupBy);
                metadataGroupBy.ChildNodes = this.CreateMetadataColumnNodes(groupBy.FindNode(SpaceParseTreeNodeTypeEnum.VALUES_WITH_ALIAS), null);
                root.ChildNodes.Add(metadataGroupBy);
            }

            // se obtine la proyección 
            SpaceMetadataTreeNode metadataSelect = new SpaceMetadataTreeNode(SpaceMetadataTreeNodeTypeEnum.Select);
            metadataSelect.ChildNodes = new List<SpaceMetadataTreeNode>();
            root.ChildNodes.Add(metadataSelect);

            SpaceParseTreeNode select = parseTreeNode.FindNode(SpaceParseTreeNodeTypeEnum.SELECT).SingleOrDefault();
            metadataSelect.ChildNodes = this.CreateMetadataColumnNodes(select.FindNode(SpaceParseTreeNodeTypeEnum.VALUES_WITH_ALIAS), root.ChildNodes.Where(x => x.Type == SpaceMetadataTreeNodeTypeEnum.GroupBy).FirstOrDefault());

            SpaceParseTreeNode top = select.FindNode(SpaceParseTreeNodeTypeEnum.TOP).SingleOrDefault();
            if (top != null)
            {
                SpaceMetadataTreeNode metadataTop = new SpaceMetadataTreeNode(SpaceMetadataTreeNodeTypeEnum.Top);
                metadataTop.Value = top.ChildNodes.Last().TokenValue;
                metadataTop.ValueDataType = top.ChildNodes.Last().TokenValueDataType;
                metadataSelect.ChildNodes.Add(metadataTop);
            }

            // se obtiene el ordenamiento     
            SpaceParseTreeNode orderBy = parseTreeNode.FindNode(SpaceParseTreeNodeTypeEnum.ORDER_BY).SingleOrDefault();
            if (orderBy != null)
            {
                SpaceMetadataTreeNode metadataOrderBy = new SpaceMetadataTreeNode(SpaceMetadataTreeNodeTypeEnum.OrderBy);
                metadataOrderBy.ChildNodes = this.CreateMetadataColumnNodes(orderBy.FindNode(SpaceParseTreeNodeTypeEnum.LIST_OF_VALUES), root.ChildNodes.Where(x => x.Type == SpaceMetadataTreeNodeTypeEnum.Select).First());
                root.ChildNodes.Add(metadataOrderBy);
            }

            /************************ JOIN  ************************/
            SpaceParseTreeNode join = parseTreeNode.FindNode(SpaceParseTreeNodeTypeEnum.JOIN).FirstOrDefault();

            if (join != null)
            {
                SpaceMetadataTreeNode metadataJoin = new SpaceMetadataTreeNode(SpaceMetadataTreeNodeTypeEnum.Join);
                metadataJoin.ChildNodes = new List<SpaceMetadataTreeNode>();
                root.ChildNodes.Add(metadataJoin);

                // tipo de join
                SpaceMetadataTreeNode metadataJoinType = new SpaceMetadataTreeNode(SpaceMetadataTreeNodeTypeEnum.JoinType);
                metadataJoinType.Value = join.FindNode(SpaceParseTreeNodeTypeEnum.JOIN_TYPE).First().ChildNodes[0].TokenValue;
                metadataJoin.ChildNodes.Add(metadataJoinType);

                // condición de emparejamiento del join
                SpaceParseTreeNode on = parseTreeNode.FindNode(SpaceParseTreeNodeTypeEnum.ON).SingleOrDefault();

                if (on != null)
                {
                    SpaceMetadataTreeNode metadataOn = new SpaceMetadataTreeNode(SpaceMetadataTreeNodeTypeEnum.On);
                    metadataOn.ChildNodes = new List<SpaceMetadataTreeNode>();
                    metadataJoin.ChildNodes.Add(metadataOn);

                    this.GetObjectPropertiesUsedInTheBranch(on, metadataOn, root, true);
                }

                // se obtiene el timeout del join
                SpaceParseTreeNode timeout = parseTreeNode.FindNode(SpaceParseTreeNodeTypeEnum.TIMEOUT).SingleOrDefault();

                if (timeout != null)
                {
                    SpaceParseTreeNode timeoutValue = timeout.ChildNodes.Last();
                    SpaceMetadataTreeNode metadataTimeout = new SpaceMetadataTreeNode(SpaceMetadataTreeNodeTypeEnum.Timeout);
                    metadataTimeout.Value = timeoutValue.TokenValue;
                    metadataTimeout.ValueDataType = timeoutValue.TokenValueDataType;
                    metadataJoin.ChildNodes.Add(metadataTimeout);
                }

                // se obtiene el timeout del join
                SpaceParseTreeNode eventLifeTime = parseTreeNode.FindNode(SpaceParseTreeNodeTypeEnum.EVENT_LIFE_TIME).SingleOrDefault();

                if (eventLifeTime != null)
                {
                    SpaceParseTreeNode eventLifeTimeValue = eventLifeTime.ChildNodes.Last();
                    SpaceMetadataTreeNode metadataEventLifeTime = new SpaceMetadataTreeNode(SpaceMetadataTreeNodeTypeEnum.EventLifeTime);
                    metadataEventLifeTime.Value = eventLifeTimeValue.TokenValue;
                    metadataEventLifeTime.ValueDataType = eventLifeTimeValue.TokenValueDataType;
                    metadataJoin.ChildNodes.Add(metadataEventLifeTime);
                }
            }

            return root;
        }

        /// <summary>
        /// Converts the irony parse tree to space parse tree.
        /// </summary>
        /// <param name="externalPaserTreeNode">Parse tree node to concert.</param>
        /// <returns>Space parse tree.</returns>
        public SpaceParseTreeNode ConvertIronyParseTree(ParseTreeNode externalPaserTreeNode)
        {
            if (externalPaserTreeNode == null)
            {
                throw new Exception("Parse tree node cannot be null.");
            }

            SpaceParseTreeNode root = this.SelectSpaceParseTreeNode(externalPaserTreeNode);

            if (externalPaserTreeNode.ChildNodes != null)
            {
                root.ChildNodes = new List<SpaceParseTreeNode>();

                foreach (ParseTreeNode node in externalPaserTreeNode.ChildNodes)
                {
                    root.ChildNodes.Add(this.ConvertIronyParseTree(node));
                }
            }

            return root;
        }

        /// <summary>
        /// Gets the properties used of the specified sources in the incoming brach of the parse three.
        /// </summary>
        /// <param name="branch">Branch of the parse tree containing the properties we want.</param>
        /// <param name="parentNodeForProperties">Metadata node where the properties will be placed as child nodes.</param>
        /// <param name="root">Metadata root node</param>
        /// <param name="getByUses">Indicates whether the properties will obtained by usage or by occurrence.</param>
        private void GetObjectPropertiesUsedInTheBranch(SpaceParseTreeNode branch, SpaceMetadataTreeNode parentNodeForProperties, SpaceMetadataTreeNode root, bool getByUses)
        {
            IEnumerable<IGrouping<string, SpaceParseTreeNode>> objects = branch.FindNode(SpaceParseTreeNodeTypeEnum.OBJECT, SpaceParseTreeNodeTypeEnum.EVENT_PROPERTY_VALUE)
                        .GroupBy(x =>
                        {
                            SpaceParseTreeNode identifier = x.FindNode(SpaceParseTreeNodeTypeEnum.EVENT).First().ChildNodes.Where(y => y.Type == SpaceParseTreeNodeTypeEnum.identifier).FirstOrDefault();

                            if (identifier != null)
                            {
                                return identifier.TokenValue;
                            }
                            else
                            {
                                return root.ChildNodes.Where(w => w.Type == SpaceMetadataTreeNodeTypeEnum.Sources).First().ChildNodes.First().Value.ToString();
                            }
                        });

            if (objects.Count() == 0)
            {
                throw new CompilationException("No events found in the on condition.");
            }

            // aqui la O grande indica que es un bloque pesado
            foreach (IGrouping<string, SpaceParseTreeNode> @object in objects)
            {
                SpaceMetadataTreeNode source = new SpaceMetadataTreeNode(SpaceMetadataTreeNodeTypeEnum.Source);
                source.Value = @object.Key;
                source.ChildNodes = new List<SpaceMetadataTreeNode>();
                parentNodeForProperties.ChildNodes.Add(source);

                foreach (SpaceParseTreeNode column in @object)
                {
                    string columnName = string.Empty;
                    Type columnType = typeof(object);

                    columnName = this.GetColumnName(column);

                    if (getByUses)
                    {
                        SpaceMetadataTreeNode metadataColumn = new SpaceMetadataTreeNode(SpaceMetadataTreeNodeTypeEnum.Column);
                        metadataColumn.Value = columnName;
                        metadataColumn.ValueDataType = columnType;
                        source.ChildNodes.Add(metadataColumn);
                    }
                    else
                    {
                        if (!source.ChildNodes.Any(x => x.Value.ToString().Equals(columnName)))
                        {
                            SpaceMetadataTreeNode metadataColumn = new SpaceMetadataTreeNode(SpaceMetadataTreeNodeTypeEnum.Column);
                            metadataColumn.Value = columnName;
                            metadataColumn.ValueDataType = columnType;
                            source.ChildNodes.Add(metadataColumn);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Generates the column name of the specify event property.
        /// </summary>
        /// <param name="node">Event property tree.</param>
        /// <returns>Column name.</returns>
        private string GetColumnName(SpaceParseTreeNode node)
        {
            string columnName = string.Empty;

            foreach (SpaceParseTreeNode nodesOfEventDefinition in node.ChildNodes)
            {
                if (nodesOfEventDefinition.Type.Equals(SpaceParseTreeNodeTypeEnum.OBJECT) || nodesOfEventDefinition.Type.Equals(SpaceParseTreeNodeTypeEnum.EVENT_PROPERTY_VALUE))
                {
                    columnName += this.GetColumnName(nodesOfEventDefinition);
                }
                else if (nodesOfEventDefinition.Type.Equals(SpaceParseTreeNodeTypeEnum.OBJECT_ID_OR_NUMBER))
                {
                    columnName += "_";
                    foreach (SpaceParseTreeNode y in nodesOfEventDefinition.ChildNodes)
                    {
                        columnName += y.TokenValue;
                    }
                }
                else if (!nodesOfEventDefinition.Type.Equals(SpaceParseTreeNodeTypeEnum.EVENT))
                {
                    columnName += string.Format("_{0}", nodesOfEventDefinition.TokenValue);
                }
            }

            return columnName;
        }

        /// <summary>
        /// Gets the space columns of a branch from a space parse tree nodes list.
        /// </summary>
        /// <param name="columnNodes">List of parse tree nodes.</param>
        /// <param name="previousStep">Previous step in the query. group by -> select -> order by.</param>
        /// <returns>Space parse tree node columns.</returns>
        private List<SpaceMetadataTreeNode> CreateMetadataColumnNodes(List<SpaceParseTreeNode> columnNodes, SpaceMetadataTreeNode previousStep)
        {
            List<SpaceMetadataTreeNode> metadataColumns = new List<SpaceMetadataTreeNode>();
            foreach (SpaceParseTreeNode valueOrvalueWithAlias in columnNodes)
            {
                SpaceMetadataTreeNode column = new SpaceMetadataTreeNode(SpaceMetadataTreeNodeTypeEnum.Column);
                column.Value = valueOrvalueWithAlias.ChildNodes.Last().TokenValue;
                metadataColumns.Add(column);

                SpaceParseTreeNode cast = valueOrvalueWithAlias.ChildNodes.First().FindNode(SpaceParseTreeNodeTypeEnum.EXPLICIT_CAST).SingleOrDefault();

                if (cast == null)
                {
                    SpaceParseTreeNode @event = valueOrvalueWithAlias.ChildNodes.First().FindNode(SpaceParseTreeNodeTypeEnum.EVENT).SingleOrDefault();

                    if (@event == null)
                    {
                        SpaceParseTreeNode value = valueOrvalueWithAlias.ChildNodes.First().FindNode(SpaceParseTreeNodeTypeEnum.EVENT, SpaceParseTreeNodeTypeEnum.identifier, SpaceParseTreeNodeTypeEnum.GROUP_KEY_VALUE, SpaceParseTreeNodeTypeEnum.number, SpaceParseTreeNodeTypeEnum.@string, SpaceParseTreeNodeTypeEnum.datetimeValue, SpaceParseTreeNodeTypeEnum.constantBool, SpaceParseTreeNodeTypeEnum.constantNull).SingleOrDefault();
                        if (value.Type.Equals(SpaceParseTreeNodeTypeEnum.GROUP_KEY_VALUE) && previousStep != null)
                        {
                            column.ValueDataType = previousStep.ChildNodes.Where(x => x.Value.Equals(value.ChildNodes.First().TokenValue)).First().ValueDataType;
                        }
                        else if (value.Type.Equals(SpaceParseTreeNodeTypeEnum.identifier) && previousStep != null)
                        {
                            column.ValueDataType = previousStep.ChildNodes.Where(x => x.Value.Equals(value.TokenValue)).First().ValueDataType;
                        }
                        else if (value.Type.Equals(SpaceParseTreeNodeTypeEnum.EVENT))
                        {
                            column.ValueDataType = value.TokenValueDataType;
                        }
                    }
                    else
                    {
                        column.ValueDataType = typeof(object);
                    }
                }
                else
                {
                    column.ValueDataType = Type.GetType(cast.ChildNodes.First().TokenValue);
                }
            }

            return metadataColumns;
        }

        /// <summary>
        /// Selects the space parse tree node type from the external parse tree node type given.
        /// </summary>
        /// <param name="externalParseTreeNode">Parse tree node with external type.</param>
        /// <returns>Space parse tree node.</returns>
        private SpaceParseTreeNode SelectSpaceParseTreeNode(ParseTreeNode externalParseTreeNode)
        {
            SpaceParseTreeNode result = null;
            string nodeType = externalParseTreeNode.Term.Name;

            if (Enum.IsDefined(typeof(SpaceParseTreeNodeTypeEnum), nodeType))
            {
                result = new SpaceParseTreeNode((SpaceParseTreeNodeTypeEnum)Enum.Parse(typeof(SpaceParseTreeNodeTypeEnum), nodeType));
                if (externalParseTreeNode.Token != null)
                {
                    result.Token = externalParseTreeNode.Token.Text;
                    result.TokenValueDataType = externalParseTreeNode.Token.Value.GetType();
                    result.TokenValue = externalParseTreeNode.Token.ValueString;
                }
            }
            else
            {
                throw new Exception(string.Format("{0} is an invalid node type, space parse tree node not found.", nodeType));
            }

            return result;
        }
    }
}
