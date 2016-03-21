//-----------------------------------------------------------------------
// <copyright file="RightExtractedDataComparer.cs" company="Ingetra.Vision.EventObject">
//     Copyright (c) Ingetra.Vision.EventObject. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Language.Runtime
{
    /// <summary>
    /// Right extracted data class
    /// </summary>
    public class RightExtractedDataComparer : RightExtractedData
    {
        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            LeftExtractedData led = (LeftExtractedData)obj;
            return this._message_1_2 == led._message_1_2 && this._message_1_32 == led._message_1_32;
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
