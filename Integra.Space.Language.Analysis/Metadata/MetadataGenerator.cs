using Integra.Space.Language.Analysis.Metadata.MetadataNodes;
using Integra.Space.Language.Exceptions;
using Irony.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Integra.Space.Language.Analysis
{
    internal class MetadataGenerator
    {
        private SpaceMetadataTreeNode root = null;

        public SpaceMetadataTreeNode GenerateMetadata(SpaceParseTreeNode parseTreeNode)
        {
            root = new SpaceMetadataTreeNode(MetadataTreeNodeTypeEnum.Query);
            root.ChildNodes = new List<SpaceMetadataTreeNode>();

            // se obtienen las fuentes
            SpaceParseTreeNode sourceDefinition = parseTreeNode.FindNode(SpaceParseTreeNodeTypeEnum.SOURCE_DEFINITION).First();
            List<SpaceParseTreeNode> sources = sourceDefinition.FindNode(SpaceParseTreeNodeTypeEnum.FROM, SpaceParseTreeNodeTypeEnum.JOIN_SOURCE, SpaceParseTreeNodeTypeEnum.WITH);

            SpaceMetadataTreeNode metadataSources = new SpaceMetadataTreeNode(MetadataTreeNodeTypeEnum.Sources);
            metadataSources.ChildNodes = new List<SpaceMetadataTreeNode>();
            root.ChildNodes.Add(metadataSources);

            foreach (SpaceParseTreeNode source in sources)
            {
                SpaceMetadataTreeNode metadataSource = new SpaceMetadataTreeNode(MetadataTreeNodeTypeEnum.Source);
                metadataSource.Value = source.FindNode(SpaceParseTreeNodeTypeEnum.ID_OR_ID_WITH_ALIAS).SingleOrDefault().ChildNodes.Last().TokenValue;
                metadataSources.ChildNodes.Add(metadataSource);
            }
            
            // se obtienen las propiedades utilizadas de las fuentes definidas
            SpaceMetadataTreeNode objectPrpertiesUsed = new SpaceMetadataTreeNode(MetadataTreeNodeTypeEnum.SourcePropertiesUsed);
            objectPrpertiesUsed.ChildNodes = new List<SpaceMetadataTreeNode>();
            root.ChildNodes.Add(objectPrpertiesUsed);

            this.GetObjectPropertiesUsedInTheBranch(parseTreeNode, objectPrpertiesUsed, false);

            // se obtiene la ventana de la consulta
            SpaceParseTreeNode applyWindow = parseTreeNode.FindNode(SpaceParseTreeNodeTypeEnum.APPLY_WINDOW).SingleOrDefault();

            if (applyWindow != null)
            {
                SpaceParseTreeNode windowValue = applyWindow.ChildNodes.Last();
                SpaceMetadataTreeNode metadataApplyWindow = new SpaceMetadataTreeNode(MetadataTreeNodeTypeEnum.Window);
                metadataApplyWindow.Value = windowValue.TokenValue;
                metadataApplyWindow.ValueDataType = windowValue.TokenValueDataType;
                root.ChildNodes.Add(metadataApplyWindow);
            }

            // se obtiene el agrupamiento     
            SpaceParseTreeNode groupBy = parseTreeNode.FindNode(SpaceParseTreeNodeTypeEnum.GROUP_BY_OP).SingleOrDefault();
            if (groupBy != null)
            {
                SpaceMetadataTreeNode metadataGroupBy = new SpaceMetadataTreeNode(MetadataTreeNodeTypeEnum.GroupBy);
                metadataGroupBy.ChildNodes = this.CreateMetadataColumnNodes(groupBy.FindNode(SpaceParseTreeNodeTypeEnum.VALUES_WITH_ALIAS), null);
                root.ChildNodes.Add(metadataGroupBy);
            }

            // se obtine la proyección 
            SpaceMetadataTreeNode metadataSelect = new SpaceMetadataTreeNode(MetadataTreeNodeTypeEnum.Select);
            metadataSelect.ChildNodes = new List<SpaceMetadataTreeNode>();
            root.ChildNodes.Add(metadataSelect);

            SpaceParseTreeNode select = parseTreeNode.FindNode(SpaceParseTreeNodeTypeEnum.SELECT).SingleOrDefault();
            metadataSelect.ChildNodes = this.CreateMetadataColumnNodes(select.FindNode(SpaceParseTreeNodeTypeEnum.VALUES_WITH_ALIAS), root.ChildNodes.Where(x => x.Type == MetadataTreeNodeTypeEnum.GroupBy).FirstOrDefault());

            SpaceParseTreeNode top = select.FindNode(SpaceParseTreeNodeTypeEnum.TOP).SingleOrDefault();
            if (top != null)
            {
                SpaceMetadataTreeNode metadataTop = new SpaceMetadataTreeNode(MetadataTreeNodeTypeEnum.Top);
                metadataTop.Value = top.ChildNodes.Last().TokenValue;
                metadataTop.ValueDataType = top.ChildNodes.Last().TokenValueDataType;
                metadataSelect.ChildNodes.Add(metadataTop);
            }

            // se obtiene el ordenamiento     
            SpaceParseTreeNode orderBy = parseTreeNode.FindNode(SpaceParseTreeNodeTypeEnum.ORDER_BY).SingleOrDefault();
            if (orderBy != null)
            {
                SpaceMetadataTreeNode metadataOrderBy = new SpaceMetadataTreeNode(MetadataTreeNodeTypeEnum.OrderBy);
                metadataOrderBy.ChildNodes = this.CreateMetadataColumnNodes(orderBy.FindNode(SpaceParseTreeNodeTypeEnum.LIST_OF_VALUES), root.ChildNodes.Where(x => x.Type == MetadataTreeNodeTypeEnum.Select).First());
                root.ChildNodes.Add(metadataOrderBy);
            }

            /************************ JOIN  ************************/
            SpaceParseTreeNode join = parseTreeNode.FindNode(SpaceParseTreeNodeTypeEnum.JOIN).FirstOrDefault();

            if (join != null)
            {
                SpaceMetadataTreeNode metadataJoin = new SpaceMetadataTreeNode(MetadataTreeNodeTypeEnum.Join);
                metadataJoin.ChildNodes = new List<SpaceMetadataTreeNode>();
                root.ChildNodes.Add(metadataJoin);

                // tipo de join
                SpaceMetadataTreeNode metadataJoinType = new SpaceMetadataTreeNode(MetadataTreeNodeTypeEnum.JoinType);
                metadataJoinType.Value = join.FindNode(SpaceParseTreeNodeTypeEnum.JOIN_TYPE).First().ChildNodes[0].TokenValue;
                metadataJoin.ChildNodes.Add(metadataJoinType);

                // condición de emparejamiento del join
                SpaceParseTreeNode on = parseTreeNode.FindNode(SpaceParseTreeNodeTypeEnum.ON).SingleOrDefault();

                if (on != null)
                {
                    SpaceMetadataTreeNode metadataOn = new SpaceMetadataTreeNode(MetadataTreeNodeTypeEnum.On);
                    metadataOn.ChildNodes = new List<SpaceMetadataTreeNode>();
                    metadataJoin.ChildNodes.Add(metadataOn);

                    this.GetObjectPropertiesUsedInTheBranch(on, metadataOn, true);
                    /*IEnumerable<IGrouping<string, SpaceParseTreeNode>> objects = on.FindNode(SpaceParseTreeNodeTypeEnum.OBJECT, SpaceParseTreeNodeTypeEnum.EXPLICIT_CAST)
                        .GroupBy(x =>
                        {
                            if (x.Type == SpaceParseTreeNodeTypeEnum.EXPLICIT_CAST)
                            {
                                return x.FindNode(SpaceParseTreeNodeTypeEnum.OBJECT).Single().ChildNodes.Where(y => y.Type == SpaceParseTreeNodeTypeEnum.EVENT).First().ChildNodes.Where(y => y.Type == SpaceParseTreeNodeTypeEnum.identifier).First().TokenValue;
                            }
                            else
                            {
                                return x.ChildNodes.Where(y => y.Type == SpaceParseTreeNodeTypeEnum.EVENT).First().ChildNodes.Where(y => y.Type == SpaceParseTreeNodeTypeEnum.identifier).First().TokenValue;
                            }
                        });

                    if (objects.Count() == 0)
                    {
                        throw new CompilationException("No events found in the on condition.");
                    }
                    
                    // aqui la O grande indica que es un bloque pesado
                    foreach (IGrouping<string, SpaceParseTreeNode> @object in objects)
                    {
                        SpaceMetadataTreeNode onKey = new SpaceMetadataTreeNode(MetadataTreeNodeTypeEnum.OnKey);
                        onKey.Value = @object.Key;
                        onKey.ChildNodes = new List<SpaceMetadataTreeNode>();
                        metadataOn.ChildNodes.Add(onKey);

                        foreach (SpaceParseTreeNode column in @object)
                        {
                            SpaceParseTreeNode aux = column;
                            string columnName = string.Empty;
                            Type columnType = typeof(object);

                            if (column.Type == SpaceParseTreeNodeTypeEnum.EXPLICIT_CAST)
                            {
                                columnType = Type.GetType(column.ChildNodes.First().TokenValue);
                                aux = aux.FindNode(SpaceParseTreeNodeTypeEnum.OBJECT).First();
                            }

                            foreach (SpaceParseTreeNode nodesOfEventDefinition in aux.ChildNodes)
                            {
                                if (nodesOfEventDefinition.Type.Equals(SpaceParseTreeNodeTypeEnum.OBJECT_ID_OR_NUMBER) || nodesOfEventDefinition.Type.Equals(SpaceParseTreeNodeTypeEnum.EVENT))
                                {
                                    columnName += "_";
                                    foreach (SpaceParseTreeNode y in nodesOfEventDefinition.ChildNodes)
                                    {
                                        columnName += y.TokenValue;
                                    }
                                }
                                else
                                {
                                    columnName += string.Format("_{0}", nodesOfEventDefinition.TokenValue);
                                }
                            }

                            SpaceMetadataTreeNode metadataColumn = new SpaceMetadataTreeNode(MetadataTreeNodeTypeEnum.Column);
                            metadataColumn.Value = columnName;
                            metadataColumn.ValueDataType = columnType;
                            onKey.ChildNodes.Add(metadataColumn);
                        }
                    }*/
                }

                // se obtiene el timeout del join
                SpaceParseTreeNode timeout = parseTreeNode.FindNode(SpaceParseTreeNodeTypeEnum.TIMEOUT).SingleOrDefault();

                if (timeout != null)
                {
                    SpaceParseTreeNode timeoutValue = timeout.ChildNodes.Last();
                    SpaceMetadataTreeNode metadataTimeout = new SpaceMetadataTreeNode(MetadataTreeNodeTypeEnum.Timeout);
                    metadataTimeout.Value = timeoutValue.TokenValue;
                    metadataTimeout.ValueDataType = timeoutValue.TokenValueDataType;
                    metadataJoin.ChildNodes.Add(metadataTimeout);
                }

                // se obtiene el timeout del join
                SpaceParseTreeNode eventLifeTime = parseTreeNode.FindNode(SpaceParseTreeNodeTypeEnum.EVENT_LIFE_TIME).SingleOrDefault();

                if (eventLifeTime != null)
                {
                    SpaceParseTreeNode eventLifeTimeValue = eventLifeTime.ChildNodes.Last();
                    SpaceMetadataTreeNode metadataEventLifeTime = new SpaceMetadataTreeNode(MetadataTreeNodeTypeEnum.EventLifeTime);
                    metadataEventLifeTime.Value = eventLifeTimeValue.TokenValue;
                    metadataEventLifeTime.ValueDataType = eventLifeTimeValue.TokenValueDataType;
                    metadataJoin.ChildNodes.Add(metadataEventLifeTime);
                }
            }

            Console.ReadLine();

            return root;
        }

        private void GetObjectPropertiesUsedInTheBranch(SpaceParseTreeNode branch, SpaceMetadataTreeNode parentNodeForProperties, bool byUses)
        {
            IEnumerable<IGrouping<string, SpaceParseTreeNode>> objects = branch.FindNode(SpaceParseTreeNodeTypeEnum.OBJECT, SpaceParseTreeNodeTypeEnum.EXPLICIT_CAST)
                        .GroupBy(x =>
                        {
                            SpaceParseTreeNode identifier = null;

                            if (x.Type == SpaceParseTreeNodeTypeEnum.EXPLICIT_CAST)
                            {
                                identifier = x.FindNode(SpaceParseTreeNodeTypeEnum.OBJECT).Single().ChildNodes.Where(y => y.Type == SpaceParseTreeNodeTypeEnum.EVENT).First().ChildNodes.Where(y => y.Type == SpaceParseTreeNodeTypeEnum.identifier).FirstOrDefault();
                            }
                            else
                            {
                                identifier = x.ChildNodes.Where(y => y.Type == SpaceParseTreeNodeTypeEnum.EVENT).First().ChildNodes.Where(y => y.Type == SpaceParseTreeNodeTypeEnum.identifier).FirstOrDefault();                                
                            }

                            if (identifier != null)
                            {
                                return identifier.TokenValue;
                            }
                            else
                            {
                                return root.ChildNodes.Where(w => w.Type == MetadataTreeNodeTypeEnum.Sources).First().ChildNodes.First().Value.ToString();
                            }
                        });

            if (objects.Count() == 0)
            {
                throw new CompilationException("No events found in the on condition.");
            }

            // aqui la O grande indica que es un bloque pesado
            foreach (IGrouping<string, SpaceParseTreeNode> @object in objects)
            {
                SpaceMetadataTreeNode source = new SpaceMetadataTreeNode(MetadataTreeNodeTypeEnum.Source);
                source.Value = @object.Key;
                source.ChildNodes = new List<SpaceMetadataTreeNode>();
                parentNodeForProperties.ChildNodes.Add(source);

                foreach (SpaceParseTreeNode column in @object)
                {
                    SpaceParseTreeNode aux = column;
                    string columnName = string.Empty;
                    Type columnType = typeof(object);

                    if (column.Type == SpaceParseTreeNodeTypeEnum.EXPLICIT_CAST)
                    {
                        columnType = Type.GetType(column.ChildNodes.First().TokenValue);
                        aux = aux.FindNode(SpaceParseTreeNodeTypeEnum.OBJECT).First();
                    }

                    foreach (SpaceParseTreeNode nodesOfEventDefinition in aux.ChildNodes)
                    {
                        if (nodesOfEventDefinition.Type.Equals(SpaceParseTreeNodeTypeEnum.OBJECT_ID_OR_NUMBER))
                        {
                            columnName += "_";
                            foreach (SpaceParseTreeNode y in nodesOfEventDefinition.ChildNodes)
                            {
                                columnName += y.TokenValue;
                            }
                        }
                        else if(!nodesOfEventDefinition.Type.Equals(SpaceParseTreeNodeTypeEnum.EVENT))
                        {
                            columnName += string.Format("_{0}", nodesOfEventDefinition.TokenValue);
                        }
                    }

                    if (byUses)
                    {
                        SpaceMetadataTreeNode metadataColumn = new SpaceMetadataTreeNode(MetadataTreeNodeTypeEnum.Column);
                        metadataColumn.Value = columnName;
                        metadataColumn.ValueDataType = columnType;
                        source.ChildNodes.Add(metadataColumn);
                    }
                    else
                    {
                        if(!source.ChildNodes.Any(x => x.Value.ToString().Equals(columnName)))
                        {
                            SpaceMetadataTreeNode metadataColumn = new SpaceMetadataTreeNode(MetadataTreeNodeTypeEnum.Column);
                            metadataColumn.Value = columnName;
                            metadataColumn.ValueDataType = columnType;
                            source.ChildNodes.Add(metadataColumn);
                        }
                    }                    
                }
            }
        }
        

        private List<SpaceMetadataTreeNode> CreateMetadataColumnNodes(List<SpaceParseTreeNode> columnNodes, SpaceMetadataTreeNode previousStep)
        {
            List<SpaceMetadataTreeNode> metadataColumns = new List<SpaceMetadataTreeNode>();
            foreach (SpaceParseTreeNode valueOrvalueWithAlias in columnNodes)
            {
                SpaceMetadataTreeNode column = new SpaceMetadataTreeNode(MetadataTreeNodeTypeEnum.Column);
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
                        if (value.Type.Equals(SpaceParseTreeNodeTypeEnum.identifier) && previousStep != null)
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

        public SpaceParseTreeNode ConvertIronyParseTree(ParseTreeNode ptNode)
        {
            if (ptNode == null)
            {
                throw new Exception("Parse tree node cannot be null.");
            }

            SpaceParseTreeNode root = this.SelectSpaceParseTreeNode(ptNode);

            if (ptNode.ChildNodes != null)
            {
                root.ChildNodes = new List<SpaceParseTreeNode>();

                foreach (ParseTreeNode node in ptNode.ChildNodes)
                {
                    root.ChildNodes.Add(this.ConvertIronyParseTree(node));
                }
            }

            return root;
        }

        private SpaceParseTreeNode SelectSpaceParseTreeNode(ParseTreeNode ptNode)
        {
            SpaceParseTreeNode result = null;
            string nodeType = ptNode.Term.Name;

            if (Enum.IsDefined(typeof(SpaceParseTreeNodeTypeEnum), nodeType))
            {
                result = new SpaceParseTreeNode((SpaceParseTreeNodeTypeEnum)Enum.Parse(typeof(SpaceParseTreeNodeTypeEnum), nodeType));
                if (ptNode.Token != null)
                {
                    result.Token = ptNode.Token.Text;
                    result.TokenValueDataType = ptNode.Token.Value.GetType();
                    result.TokenValue = ptNode.Token.ValueString;
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
