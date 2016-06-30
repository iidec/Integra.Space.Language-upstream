//-----------------------------------------------------------------------
// <copyright file="PipelineCommandContext.cs" company="Integra.Space.Language">
//     Copyright (c) Integra.Space.Language. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Language.CommandContext
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// Command context class.
    /// </summary>
    internal class PipelineCommandContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PipelineCommandContext"/> class.
        /// </summary>
        /// <param name="action">Space command action.</param>
        /// <param name="spaceObjects">List of space objects.</param>
        /// <param name="permissionList">List of permissions.</param>
        public PipelineCommandContext(SpaceActionCommandEnum action, List<Tuple<string, SpaceObjectEnum, bool?>> spaceObjects, List<Tuple<SpacePermissionsEnum, SpaceObjectEnum, string>> permissionList)
        {
            Contract.Assert(spaceObjects != null && spaceObjects.Count > 0);
            Contract.Assert(permissionList != null && permissionList.Count > 0);

            this.Action = action;
            this.PermissionList = permissionList;
            this.SpaceObjects = spaceObjects;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PipelineCommandContext"/> class.
        /// </summary>
        /// <param name="action">Space command action.</param>
        /// <param name="spaceObjects">List of space objects.</param>
        public PipelineCommandContext(SpaceActionCommandEnum action, List<Tuple<string, SpaceObjectEnum, bool?>> spaceObjects)
        {
            Contract.Assert(spaceObjects != null && spaceObjects.Count > 0);

            this.Action = action;
            this.SpaceObjects = spaceObjects;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PipelineCommandContext"/> class.
        /// </summary>
        /// <param name="action">Space command action.</param>
        /// <param name="spaceObjects">List of space objects.</param>
        /// <param name="query">Space query.</param>
        public PipelineCommandContext(SpaceActionCommandEnum action, List<Tuple<string, SpaceObjectEnum, bool?>> spaceObjects, string query)
        {
            Contract.Assert(spaceObjects != null && spaceObjects.Count > 0);

            this.Action = action;
            this.SpaceObjects = spaceObjects;
            this.Query = query;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PipelineCommandContext"/> class.
        /// </summary>
        /// <param name="action">Space command action.</param>
        /// <param name="spaceObjects">List of space objects.</param>
        /// <param name="userOptions">List of options.</param>
        public PipelineCommandContext(SpaceActionCommandEnum action, List<Tuple<string, SpaceObjectEnum, bool?>> spaceObjects, List<SpaceUserOption> userOptions)
        {
            Contract.Assert(spaceObjects != null && spaceObjects.Count > 0);
            Contract.Assert(userOptions != null && userOptions.Count > 0);

            this.Action = action;
            this.UserOptions = userOptions;
            this.SpaceObjects = spaceObjects;
        }

        /// <summary>
        /// Gets the space action.
        /// </summary>
        public SpaceActionCommandEnum Action { get; private set; }

        /// <summary>
        /// Gets the list of permissions.
        /// </summary>
        public List<Tuple<SpacePermissionsEnum, SpaceObjectEnum, string>> PermissionList { get; private set; }

        /// <summary>
        /// Gets the space objects in the command.
        /// </summary>
        public List<Tuple<string, SpaceObjectEnum, bool?>> SpaceObjects { get; private set; }

        /// <summary>
        /// Gets the query of the stream.
        /// </summary>
        public string Query { get; private set; }

        /// <summary>
        /// Gets the user options specified in the command.
        /// </summary>
        public List<SpaceUserOption> UserOptions { get; private set; }
    }
}
