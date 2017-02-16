//-----------------------------------------------------------------------
// <copyright file="SuccessResult.cs" company="Integra.Space.Language">
//     Copyright (c) Integra.Space.Language. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Common
{
    public class SuccessResult : ResultBase
    {
        public SuccessResult(int code, string message) : base(code, message, ResultType.Success)
        {
        }
    }
}
