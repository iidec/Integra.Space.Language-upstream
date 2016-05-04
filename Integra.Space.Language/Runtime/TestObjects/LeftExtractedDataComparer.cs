//-----------------------------------------------------------------------
// <copyright file="LeftExtractedDataComparer.cs" company="Ingetra.Vision.EventObject">
//     Copyright (c) Ingetra.Vision.EventObject. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Language.Runtime
{
    /// <summary>
    /// Left extracted data class
    /// </summary>
    internal class LeftExtractedDataComparer : LeftExtractedData
    {
        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            RightExtractedData red = (RightExtractedData)obj;
            return this._message_1_2 == red._message_1_2 && this._message_1_32 == red._message_1_32;
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            // Overflow is fine, just wrap
            unchecked
            {
                int hash = (int)2166136261;
                hash = hash * 16777619 ^ this._message_1_2.GetHashCode();
                hash = hash * 16777619 ^ this._message_1_32.GetHashCode();
                return hash;
            }
        }
    }
}
