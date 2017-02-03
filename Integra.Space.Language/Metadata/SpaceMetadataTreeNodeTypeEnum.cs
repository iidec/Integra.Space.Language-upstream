//-----------------------------------------------------------------------
// <copyright file="SpaceMetadataTreeNodeTypeEnum.cs" company="Integra.Space.Common">
//     Copyright (c) Integra.Space.Common. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Language.Metadata
{
    /// <summary>
    /// Metadata tree node type
    /// </summary>
    public enum SpaceMetadataTreeNodeTypeEnum
    {
        /// <summary>
        /// query node type
        /// </summary>
        Query = 0,

        /// <summary>
        /// sources node type
        /// </summary>
        Sources = 1,

        /// <summary>
        /// source node type
        /// </summary>
        Source = 2,

        /// <summary>
        /// Group by node type
        /// </summary>
        GroupBy = 3,

        /// <summary>
        /// Column node type
        /// </summary>
        Column = 4,

        /// <summary>
        /// Select node type
        /// </summary>
        Select = 5,

        /// <summary>
        /// Top node type
        /// </summary>
        Top = 6,

        /// <summary>
        /// Order by node type
        /// </summary>
        OrderBy = 7,

        /// <summary>
        /// Window node type
        /// </summary>
        Window = 8,

        /// <summary>
        /// On node type
        /// </summary>
        On = 9,

        /// <summary>
        /// Timeout node type
        /// </summary>
        Timeout = 10,

        /// <summary>
        /// Event life time node type
        /// </summary>
        EventLifeTime = 11,

        /// <summary>
        /// Source properties used type
        /// </summary>
        SourcePropertiesUsed = 12,

        /// <summary>
        /// Join node type
        /// </summary>
        Join = 13,

        /// <summary>
        /// Join type node type
        /// </summary>
        JoinType = 14
    }
}
