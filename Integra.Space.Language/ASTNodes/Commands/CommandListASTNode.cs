//-----------------------------------------------------------------------
// <copyright file="CommandListASTNode.cs" company="Integra.Space.Language">
//     Copyright (c) Integra.Space.Language. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Language.ASTNodes.Commands
{
    using System.Collections.Generic;
    using Integra.Space.Language.ASTNodes.Base;
    using Irony.Ast;
    using Irony.Interpreter;
    using Irony.Interpreter.Ast;
    using Irony.Parsing;

    /// <summary>
    /// Permission list AST node class.
    /// </summary>
    internal class CommandListASTNode : StatementListNode
    {
        /// <summary>
        /// First method called
        /// </summary>
        /// <param name="context">Contains the actual context</param>
        /// <param name="treeNode">Contains the tree of the context</param>
        public override void Init(AstContext context, ParseTreeNode treeNode)
        {
            base.Init(context, treeNode);
        }

        /// <summary>
        /// DoEvaluate
        /// Doc go here
        /// </summary>
        /// <param name="thread">Thread of the evaluated grammar</param>
        /// <returns>return a plan node</returns>
        protected override object DoEvaluate(ScriptThread thread)
        {
            List<BatchNode> resultList = new List<BatchNode>();
            thread.CurrentNode = this;
            Binding b1 = thread.Bind("Database", BindingRequestFlags.Write | BindingRequestFlags.ExistingOrNew);
            b1.SetValueRef(thread, null);

            this.GetResultCommandList(resultList, thread);
            thread.CurrentNode = this;

            return resultList.ToArray();
        }

        /// <summary>
        /// Get the result command list.
        /// </summary>
        /// <param name="resultList">Resultant command list.</param>
        /// <param name="thread">Script thread.</param>
        private void GetResultCommandList(List<BatchNode> resultList, ScriptThread thread)
        {
            BatchNode batch = new BatchNode();

            foreach (AstNodeBase child in this.GetChildNodes())
            {
                var evaluatedChild = child.Evaluate(thread);
                if (evaluatedChild.GetType().IsArray)
                {
                    // los comandos de permisos son los que devuelven un array
                    // porque los comandos de permisos generan multiple objetos comando, uno por cada permiso.
                    batch.Commands.AddRange((IEnumerable<SystemCommand>)evaluatedChild);
                }
                else
                {
                    // si el comando es go, agrego el conjunto de comandos como un batch al resultado y creo un nuevo batch
                    if (evaluatedChild is GoCommandNode)
                    {
                        batch.Go = (GoCommandNode)evaluatedChild;
                        resultList.Add(batch);
                        batch = new BatchNode();
                    }
                    else
                    {
                        batch.Commands.Add((SystemCommand)evaluatedChild);
                    }
                }
            }

            // si la lista de comandos del batch no esta vacía quiere decir que no se especificó un 'go' explicitamente, por lo tanto, 
            // agrego el batch al la lista de resultado.
            if (batch.Commands.Count > 0)
            {
                resultList.Add(batch);
            }
        }
    }
}
