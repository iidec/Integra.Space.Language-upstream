//-----------------------------------------------------------------------
// <copyright file="RightExtractedData.cs" company="Ingetra.Vision.EventObject">
//     Copyright (c) Ingetra.Vision.EventObject. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Language.Runtime
{
    /// <summary>
    /// Right extracted data class
    /// </summary>
    public class RightExtractedData : ExtractedEventData
    {
        /// <summary>
        /// Gets the adapter name.
        /// </summary>
        public object _adapter_Name
        {
            get
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Gets the fourth field.
        /// </summary>
        public object _message_1_4
        {
            get
            {
                return (decimal)1;
            }
        }

        /// <summary>
        /// Gets other field.
        /// </summary>
        public object _message_1_43
        {
            get
            {
                return "123456789";
            }
        }
    }
}
