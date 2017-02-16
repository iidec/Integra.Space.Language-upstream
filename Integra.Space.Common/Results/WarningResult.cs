//-----------------------------------------------------------------------
// <copyright file="WarningResult.cs" company="Integra.Space.Language">
//     Copyright (c) Integra.Space.Language. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Common
{
    public class WarningResult : ResultBase
    {
        public WarningResult(int code, string message) : base(code, message, ResultType.Warning)
        {
        }
    }
}
