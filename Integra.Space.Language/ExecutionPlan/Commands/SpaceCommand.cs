//-----------------------------------------------------------------------
// <copyright file="SpaceCommand.cs" company="Integra.Space.Common">
//     Copyright (c) Integra.Space.Common. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Language
{
    using System.Collections.Generic;
    using Common;

    /// <summary>
    /// Command action node class.
    /// </summary>
    internal abstract class SpaceCommand : ISpaceCommand
    {
        /// <summary>
        /// Command action.
        /// </summary>
        private SpaceActionCommandEnum action;

        /// <summary>
        /// Space object type.
        /// </summary>
        private SpaceObjectEnum spaceObjectType;

        /// <summary>
        /// Space object name.
        /// </summary>
        private string objectName;

        /// <summary>
        /// Initializes a new instance of the <see cref="SpaceCommand"/> class.
        /// </summary>
        /// <param name="action">Space command action.</param>
        /// <param name="spaceObjectType">Space object type.</param>
        /// <param name="objectName">Object name.</param>
        public SpaceCommand(SpaceActionCommandEnum action, SpaceObjectEnum spaceObjectType, string objectName)
        {
            this.action = action;
            this.spaceObjectType = spaceObjectType;
            this.objectName = objectName; 
        }

        /// <summary>
        /// Gets command action.
        /// </summary>
        public SpaceActionCommandEnum Action
        {
            get
            {
                return this.action;
            }
        }

        /// <summary>
        /// Gets the space object type.
        /// </summary>
        public SpaceObjectEnum SpaceObjectType
        {
            get
            {
                return this.spaceObjectType;
            }
        }

        /// <summary>
        /// Gets the object name.
        /// </summary>
        public string ObjectName
        {
            get
            {
                return this.objectName;
            }
        }
    }
}
