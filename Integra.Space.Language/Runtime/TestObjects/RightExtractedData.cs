﻿//-----------------------------------------------------------------------
// <copyright file="RightExtractedData.cs" company="Ingetra.Vision.EventObject">
//     Copyright (c) Ingetra.Vision.EventObject. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Language.Runtime
{
    /// <summary>
    /// Right extracted data class
    /// </summary>
    internal class RightExtractedData : ExtractedEventData
    {
        /// <summary>
        /// Doc goes here
        /// </summary>
        private object message132;

        /// <summary>
        /// Doc goes here
        /// </summary>
        private object message12;

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
        /// Gets or sets the fourth field.
        /// </summary>
        public object _message_1_32
        {
            get
            {
                return (decimal)1;
            }

            set
            {
                this.message132 = value;
            }
        }

        /// <summary>
        /// Gets or sets other field.
        /// </summary>
        public object _message_1_2
        {
            get
            {
                return "9999941616073663_2";
            }

            set
            {
                this.message12 = value;
            }
        }
    }
}
