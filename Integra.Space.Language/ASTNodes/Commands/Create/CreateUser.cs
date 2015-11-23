//-----------------------------------------------------------------------
// <copyright file="CreateUser.cs" company="Integra.Space.Language">
//     Copyright (c) Integra.Space.Language. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Language.ASTNodes.Commands.Create
{
    using Integra.Space.Language.ASTNodes.Base;
    using Integra.Space.Language.Resources;
    using Irony.Ast;
    using Irony.Interpreter;
    using Irony.Interpreter.Ast;
    using Irony.Parsing;

    /// <summary>
    /// CreateUser class
    /// </summary>
    internal sealed class CreateUser : AstNodeBase
    {
        /// <summary>
        /// reserved word
        /// </summary>
        private string createOrAlterWord;

        /// <summary>
        /// reserved word
        /// </summary>
        private string userWord;

        /// <summary>
        /// new user name
        /// </summary>
        private AstNodeBase userName;

        /// <summary>
        /// reserved word
        /// </summary>
        private string withWord;

        /// <summary>
        /// reserved word
        /// </summary>
        private string passwordWord;

        /// <summary>
        /// password of the user
        /// </summary>
        private AstNodeBase password;

        /// <summary>
        /// reserved word
        /// </summary>
        private string statusWord;

        /// <summary>
        /// status of the user: disable or enable
        /// </summary>
        private PlanNode status;

        /// <summary>
        /// result planNode
        /// </summary>
        private PlanNode result;

        /// <summary>
        /// First method called
        /// </summary>
        /// <param name="context">Contains the actual context</param>
        /// <param name="treeNode">Contains the tree of the context</param>
        public override void Init(AstContext context, ParseTreeNode treeNode)
        {
            base.Init(context, treeNode);

            this.createOrAlterWord = (string)ChildrenNodes[0].Token.Value;
            this.userWord = (string)ChildrenNodes[1].Token.Value;
            this.userName = AddChild(NodeUseType.Parameter, SR.UserDefinedObjectRole, ChildrenNodes[2]) as AstNodeBase;
            this.withWord = (string)ChildrenNodes[3].Token.Value;
            this.passwordWord = (string)ChildrenNodes[4].Token.Value;
            this.password = AddChild(NodeUseType.Parameter, SR.PasswordRole, ChildrenNodes[6]) as AstNodeBase;
            this.statusWord = (string)ChildrenNodes[8].Token.Value;

            this.status = new PlanNode();
            this.status.Column = ChildrenNodes[10].Token.Location.Column;
            this.status.Line = ChildrenNodes[10].Token.Location.Line;
            this.status.NodeText = ChildrenNodes[10].Token.Text;
            this.status.NodeType = PlanNodeTypeEnum.Status;
            this.status.Properties.Add(SR.StatusProperty, ChildrenNodes[10].Token.Value);

            this.result = new PlanNode();
            this.result.Column = ChildrenNodes[0].Token.Location.Column;
            this.result.Line = ChildrenNodes[0].Token.Location.Line;

            if (this.createOrAlterWord.Equals("create", System.StringComparison.InvariantCultureIgnoreCase))
            {
                this.result.NodeType = PlanNodeTypeEnum.CreateUser;
            }
            else if (this.createOrAlterWord.Equals("alter", System.StringComparison.InvariantCultureIgnoreCase))
            {
                this.result.NodeType = PlanNodeTypeEnum.AlterUser;
            }
        }

        /// <summary>
        /// DoEvaluate
        /// Doc go here 
        /// </summary>
        /// <param name="thread">Thread of the evaluated grammar</param>
        /// <returns>return a plan node</returns>
        protected override object DoEvaluate(ScriptThread thread)
        {
            this.BeginEvaluate(thread);
            PlanNode userNameAux = (PlanNode)this.userName.Evaluate(thread);
            PlanNode passwordAux = (PlanNode)this.password.Evaluate(thread);
            this.EndEvaluate(thread);

            this.result.NodeText = SR.UserNodeText(this.createOrAlterWord, this.userWord, userNameAux.NodeText, this.withWord, this.passwordWord, passwordAux.NodeText, this.statusWord, this.status.NodeText);

            this.result.Children = new System.Collections.Generic.List<PlanNode>();
            this.result.Children.Add(userNameAux);
            this.result.Children.Add(passwordAux);
            this.result.Children.Add(this.status);

            return this.result;
        }
    }
}
