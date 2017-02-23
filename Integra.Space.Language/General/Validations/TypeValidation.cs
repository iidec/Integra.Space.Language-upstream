//-----------------------------------------------------------------------
// <copyright file="TypeValidation.cs" company="Integra.Spaces.Language">
//     Copyright (c) Integra.Spaces.Language. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Language.General.Validations
{
    using System;
    using Integra.Space.Language.Errors;

    using Irony.Interpreter;

    /// <summary>
    /// TypeValidation class
    /// </summary>
    internal sealed class TypeValidation
    {
        /// <summary>
        /// Initializes a new instance of the TypeValidation class
        /// </summary>
        /// <param name="leftType">left type</param>
        /// <param name="rightType">right type</param>
        /// <param name="plan">actual plan</param>
        /// <param name="thread">actual thread</param>
        public TypeValidation(Type leftType, Type rightType, PlanNode plan, ScriptThread thread)
        {
            this.LeftType = leftType;
            this.RightType = rightType;
            this.ConvertLeftNode = false;
            this.ConvertRightNode = false;
            this.Plan = plan;
            this.Thread = thread;
        }

        /// <summary>
        /// Gets or sets the left of the left node
        /// </summary>
        public Type LeftType { get; set; }

        /// <summary>
        /// Gets or sets the type of the right node
        /// </summary>
        public Type RightType { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether convert to left node type
        /// </summary>
        public bool ConvertLeftNode { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether convert to right node type
        /// </summary>
        public bool ConvertRightNode { get; set; }

        /// <summary>
        /// Gets or sets the actual plan
        /// </summary>
        public PlanNode Plan { get; set; }

        /// <summary>
        /// Gets or sets the actual thread
        /// </summary>
        public ScriptThread Thread { get; set; }

        /// <summary>
        /// Gets the selected type to cast
        /// </summary>
        /// <returns>selected type</returns>
        public Type SelectTypeToCast()
        {
            Type selectedType = null;

            try
            {
                if (this.RightType == null || this.LeftType == null)
                {
                    if (this.RightType != null)
                    {
                        selectedType = this.RightType;
                        this.ConvertLeftNode = true;
                    }

                    if (this.LeftType != null)
                    {
                        selectedType = this.LeftType;
                        this.ConvertRightNode = true;
                    }
                }
                else if (this.RightType.ToString().Equals(this.LeftType.ToString()))
                {
                    selectedType = null;
                }
                else
                {
                    if (this.RightType.Equals(typeof(object)))
                    {
                        selectedType = this.LeftType;
                        this.ConvertRightNode = true;
                    }
                    else if (this.LeftType.Equals(typeof(object)))
                    {
                        selectedType = this.RightType;
                        this.ConvertLeftNode = true;
                    }
                    else
                    {
                        if (this.RightType.IsCastableTo(this.LeftType))
                        {
                            selectedType = this.RightType;
                            this.ConvertLeftNode = true;
                        }
                        else if (this.LeftType.IsCastableTo(this.RightType))
                        {
                            selectedType = this.LeftType;
                            this.ConvertRightNode = true;
                        }
                        else
                        {
                            this.Thread.App.Parser.Context.AddParserError(Resources.ParseResults.InvalidTypes(LanguageResultCodes.InvalidTypes, this.LeftType, this.RightType), this.Plan.Line, this.Plan.Column);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                this.Thread.App.Parser.Context.AddParserError(Resources.ParseResults.InvalidTypes(LanguageResultCodes.InvalidTypes, this.LeftType, this.RightType), this.Plan.Line, this.Plan.Column);
            }

            return selectedType;
        }
    }
}
