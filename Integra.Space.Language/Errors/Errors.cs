//-----------------------------------------------------------------------
// <copyright file="Errors.cs" company="Integra.Space.Language">
//     Copyright (c) Integra.Space.Language. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Language.Errors
{    
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Irony.Interpreter;

    /// <summary>
    /// Errors class
    /// </summary>
    internal sealed class Errors
    {
        /// <summary>
        /// Initializes a new instance of the Errors class
        /// </summary>
        /// <param name="thread">thread of the analysis</param>
        public Errors(ScriptThread thread)
        {
            this.Thread = thread;
        }

        /// <summary>
        /// Gets or sets the thread
        /// </summary>
        public ScriptThread Thread { get; set; }

        /// <summary>
        /// Add the error to the error list
        /// </summary>
        /// <param name="error">Error node</param>
        public void AlmacenarError(ErrorNode error)
        {
            if (this.Thread != null)
            {
                Binding b = this.Thread.Bind("errorList", BindingRequestFlags.Write | BindingRequestFlags.ExistingOrNew);
                List<ErrorNode> errorList = (List<ErrorNode>)b.GetValueRef(this.Thread);

                if (errorList == null)
                {
                    b.SetValueRef(this.Thread, new List<ErrorNode>());
                    errorList = (List<ErrorNode>)b.GetValueRef(this.Thread);
                }

                errorList.Add(error);
            }
        }
    }
}
