//-----------------------------------------------------------------------
// <copyright file="InsertNode.cs" company="Integra.Space.Language">
//     Copyright (c) Integra.Space.Language. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Language
{
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Insert node class.
    /// </summary>
    internal class InsertNode : DMLCommand
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InsertNode"/> class.
        /// </summary>
        /// <param name="commandObject">Source to insert the values.</param>
        /// <param name="columnsWithValues">Columns with their respective values.</param>
        /// <param name="line">Line of the command.</param>
        /// <param name="column">Column at the line.</param>
        /// <param name="commandText">Text of the command.</param>
        public InsertNode(Common.CommandObject commandObject, Dictionary<string, object> columnsWithValues, int line, int column, string commandText) : base(Common.ActionCommandEnum.Insert, commandObject, line, column, commandText)
        {
            this.ColumnsWithValues = columnsWithValues;
            List<Common.CommandObject> listOfColumns = new List<Common.CommandObject>();
            foreach (KeyValuePair<string, object> columnWithValue in columnsWithValues)
            {
                listOfColumns.Add(new Common.CommandObject(Common.SystemObjectEnum.SourceColumn, commandObject.DatabaseName, commandObject.SchemaName, commandObject.Name, columnWithValue.Key, Common.PermissionsEnum.None, false));
            }

            this.CommandObjects.Concat(listOfColumns);
        }

        /// <summary>
        /// Gets the columns with values of the insert command.
        /// </summary>
        public Dictionary<string, object> ColumnsWithValues { get; private set; }
    }
}
