//-----------------------------------------------------------------------
// <copyright file="SpacePermissionsCommandNode.cs" company="Integra.Space.Common">
//     Copyright (c) Integra.Space.Common. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Language
{
    using System.Diagnostics.Contracts;

    /// <summary>
    /// Command action node class.
    /// </summary>
    internal abstract class SpacePermissionsCommandNode : SpaceCommand
    {
        /// <summary>
        /// Space action.
        /// </summary>
        private SpacePermissionsEnum permission;

        /// <summary>
        /// Space object.
        /// </summary>
        private SpaceObjectEnum spaceObject;

        /// <summary>
        /// Space object to assign the permission.
        /// </summary>
        private string toIdentifier;

        /// <summary>
        /// Stream identifier.
        /// </summary>
        private string streamIdentifier;

        /// <summary>
        /// Initializes a new instance of the <see cref="SpacePermissionsCommandNode"/> class.
        /// </summary>
        /// <param name="line">Line of the evaluated sentence.</param>
        /// <param name="column">Column evaluated sentence column.</param>
        /// <param name="nodeText">Text of the actual node.</param>
        /// <param name="permission">Space permission.</param>
        /// <param name="spaceObject">Space object.</param>
        /// <param name="toIdentifier">Space object to assign the permission must be a role or user.</param>
        /// <param name="streamIdentifier">Stream identifier.</param>
        public SpacePermissionsCommandNode(int line, int column, string nodeText, SpacePermissionsEnum permission, SpaceObjectEnum spaceObject, string toIdentifier, string streamIdentifier = "") : base(line, column, nodeText)
        {
            this.permission = permission;
            this.spaceObject = spaceObject;
            this.toIdentifier = toIdentifier;
            this.streamIdentifier = streamIdentifier;
        }

        /// <summary>
        /// Gets the space action.
        /// </summary>
        public SpacePermissionsEnum Permission
        {
            get
            {
                return this.permission;
            }
        }

        /// <summary>
        /// Gets the space object.
        /// </summary>
        public SpaceObjectEnum SpaceObject
        {
            get
            {
                return this.spaceObject;
            }
        }

        /// <summary>
        /// Gets the space object to assign the permission.
        /// </summary>
        public string ToIdentifier
        {
            get
            {
                return this.toIdentifier;
            }
        }

        /// <summary>
        /// Gets the stream identifier.
        /// </summary>
        public string StreamIdentifier
        {
            get
            {
                return this.streamIdentifier;
            }
        }
    }
}
