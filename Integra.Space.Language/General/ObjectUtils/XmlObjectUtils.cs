//-----------------------------------------------------------------------
// <copyright file="XmlObjectUtils.cs" company="Integra.Space.Language">
//     Copyright (c) Integra.Space.Language. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Language.General.ObjectUtils
{
    using System;
    using System.IO;
    using System.Text;
    using System.Xml.Serialization;

    /// <summary>
    /// XmlObjectUtils class
    /// </summary>
    internal static class XmlObjectUtils
    {
        /// <summary>
        /// Doc go here
        /// </summary>
        /// <param name="obj">object to convert to JSON struct</param>
        /// <returns>object serialized</returns>
        public static string ToXML(this object obj)
        {
            XmlSerializer serializer = new XmlSerializer(obj.GetType());
            StringBuilder reader = new StringBuilder();
            using (StringWriter writer = new StringWriter(reader))
            {
                serializer.Serialize(writer, obj);
            }

            return reader.ToString();
        }

        /// <summary>
        /// Doc go here
        /// </summary>
        /// <param name="type">type of the target object</param>
        /// <param name="xml">string JSON</param>
        /// <returns>object deserialized</returns>
        public static object FromXML(Type type, string xml)
        {
            XmlSerializer serializer = new XmlSerializer(type);
            object writer;
            using (TextReader reader = new StringReader(xml))
            {
                writer = serializer.Deserialize(reader);
            }

            return writer;
        }
    }
}
