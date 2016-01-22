//-----------------------------------------------------------------------
// <copyright file="IQueryResultWriter.cs" company="Ingetra.Space.Event">
//     Copyright (c) Ingetra.Space.Event. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space
{
    using System;

    /// <summary>
    /// Query Result Writer.
    /// </summary>
    public interface IQueryResultWriter
    {
        /// <summary>
        /// Serialize a byte[] value.
        /// </summary>
        /// <param name="value">Value to serialize.</param>
        void WriteValue(byte[] value);

        /// <summary>
        /// Serialize a TimeSpan value.
        /// </summary>
        /// <param name="value">Value to serialize.</param>
        void WriteValue(TimeSpan value);

        /// <summary>
        /// Serialize a unique identifier value.
        /// </summary>
        /// <param name="value">Value to serialize.</param>
        void WriteValue(Guid value);

        /// <summary>
        /// Serialize a char[] value.
        /// </summary>
        /// <param name="value">Value to serialize.</param>
        void WriteValue(char[] value);

        /// <summary>
        /// Serialize a boolean value.
        /// </summary>
        /// <param name="value">Value to serialize.</param>
        void WriteValue(bool value);

        /// <summary>
        /// Serialize a char value.
        /// </summary>
        /// <param name="value">Value to serialize.</param>
        void WriteValue(char value);

        /// <summary>
        /// Serialize a 8 bits integer value.
        /// </summary>
        /// <param name="value">Value to serialize.</param>
        void WriteValue(sbyte value);

        /// <summary>
        /// Serialize a byte value.
        /// </summary>
        /// <param name="value">Value to serialize.</param>
        void WriteValue(byte value);

        /// <summary>
        /// Serialize a short value.
        /// </summary>
        /// <param name="value">Value to serialize.</param>
        void WriteValue(short value);

        /// <summary>
        /// Serialize a  value.
        /// </summary>
        /// <param name="value">Value to serialize.</param>
        void WriteValue(ushort value);

        /// <summary>
        /// Serialize a integer value.
        /// </summary>
        /// <param name="value">Value to serialize.</param>
        void WriteValue(int value);

        /// <summary>
        /// Serialize a unit value.
        /// </summary>
        /// <param name="value">Value to serialize.</param>
        void WriteValue(uint value);

        /// <summary>
        /// Serialize a long value.
        /// </summary>
        /// <param name="value">Value to serialize.</param>
        void WriteValue(long value);

        /// <summary>
        /// Serialize a 64 bits integer without sign value.
        /// </summary>
        /// <param name="value">Value to serialize.</param>
        void WriteValue(ulong value);

        /// <summary>
        /// Serialize a float value.
        /// </summary>
        /// <param name="value">Value to serialize.</param>
        void WriteValue(float value);

        /// <summary>
        /// Serialize a double value.
        /// </summary>
        /// <param name="value">Value to serialize.</param>
        void WriteValue(double value);

        /// <summary>
        /// Serialize a decimal value.
        /// </summary>
        /// <param name="value">Value to serialize.</param>
        void WriteValue(decimal value);

        /// <summary>
        /// Serialize a DateTime value.
        /// </summary>
        /// <param name="value">Value to serialize.</param>
        void WriteValue(DateTime value);

        /// <summary>
        /// Serialize a string value.
        /// </summary>
        /// <param name="value">Value to serialize.</param>
        void WriteValue(string value);
    }
}
