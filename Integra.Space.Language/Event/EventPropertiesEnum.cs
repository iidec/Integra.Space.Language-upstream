//-----------------------------------------------------------------------
// <copyright file="EventPropertiesEnum.cs" company="Ingetra.Vision.Event">
//     Copyright (c) Ingetra.Vision.Event. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space
{
    using System;

    /// <summary>
    /// Event properties enumerator
    /// </summary>
    [Serializable]
    public enum EventPropertiesEnum
    {
        /// <summary>
        /// Event adapter property
        /// </summary>
        Adapter = 10,

        /// <summary>
        /// Event agent property
        /// </summary>
        Agent = 20,

        /// <summary>
        /// Event message property
        /// </summary>
        Message = 30,

        /// <summary>
        /// Adapter or agent timestamp property
        /// </summary>
        Timestamp = 40,

        /// <summary>
        /// Adapter machine name property
        /// </summary>
        MachineName = 50,

        /// <summary>
        /// Adapter or agent name property
        /// </summary>
        Name = 60,
    }
}
