//-----------------------------------------------------------------------
// <copyright file="ErrorResult.cs" company="Integra.Space.Language">
//     Copyright (c) Integra.Space.Language. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Common
{
    public class ErrorResult : ResultBase
    {
        public ErrorResult(int code, string message) : base(code, message, ResultType.Warning)
        {
        }
    }
}
