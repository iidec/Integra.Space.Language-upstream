//-----------------------------------------------------------------------
// <copyright file="ProcessorBase.cs" company="Integra.Space.Language">
//     Copyright (c) Integra.Space.Language. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Language.Public
{
    /// <summary>
    /// A class that encapsulate the parser calls.
    /// </summary>
    public class ProcessorBase
    {
        /// <summary>
        /// Parse the command.
        /// </summary>
        /// <param name="command">Space command.</param>
        /// <returns>A value indicating whether the command was parsed successfully.</returns>
        public bool ProcessCommand(string command)
        {
            CommandParser cp = new CommandParser(command, new TestRuleValidator());
            ParseContext context = cp.Evaluate();

            if (context.HasErrors())
            {
                return false;
            }
            else
            {
                foreach (var batch in context.Payload)
                {
                    if (batch.HasErrors())
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Doc goes here
        /// </summary>
        private class TestRuleValidator : IGrammarRuleValidator
        {
            /// <inheritdoc />
            public bool IsAnAllowedFunctionality(EQLFunctionalityEnum functionality)
            {
                return true;
            }
        }
    }
}
