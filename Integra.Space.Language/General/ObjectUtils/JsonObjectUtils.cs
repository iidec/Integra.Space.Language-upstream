//-----------------------------------------------------------------------
// <copyright file="JsonObjectUtils.cs" company="Integra.Space.Language">
//     Copyright (c) Integra.Space.Language. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Language.General.ObjectUtils
{
    using System;
    using System.Web.Script.Serialization;

    /// <summary>
    /// JSONObjectUtils class
    /// </summary>
    internal static class JsonObjectUtils
    {
        /// <summary>
        /// Doc go here
        /// </summary>
        /// <param name="obj">object to convert to JSON struct</param>
        /// <returns>object serialized</returns>
        public static string ToJSON(this object obj)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            return serializer.Serialize(obj);
        }

        /// <summary>
        /// Doc go here
        /// </summary>
        /// <param name="obj">object to convert to JSON struct</param>
        /// <param name="recursionDepth">doc go here</param>
        /// <returns>object serialized</returns>
        public static string ToJSON(this object obj, int recursionDepth)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            serializer.RecursionLimit = recursionDepth;
            return serializer.Serialize(obj);
        }

        /// <summary>
        /// Doc go here
        /// </summary>
        /// <param name="type">type of the target object</param>
        /// <param name="json">string JSON</param>
        /// <returns>object deserialized</returns>
        public static object FromJSON(Type type, string json)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            return serializer.Deserialize(json, type);
        }
    }
}
