//-----------------------------------------------------------------------
// <copyright file="InfoResult.cs" company="Integra.Space.Language">
//     Copyright (c) Integra.Space.Language. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Common
{
    public class InfoResult : ResultBase
    {
        public InfoResult(int code, string message) : base(code, message, ResultType.Info)
        {
        }
    }
}
