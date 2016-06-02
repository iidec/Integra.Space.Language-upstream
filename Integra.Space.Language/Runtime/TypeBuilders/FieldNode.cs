//-----------------------------------------------------------------------
// <copyright file="FieldNode.cs" company="Integra.Space.Language">
//     Copyright (c) Integra.Space.Language. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Space.Language.Runtime
{
    using System;

    /// <summary>
    /// Field Node
    /// </summary>
    public class FieldNode
    {
        /// <summary>
        /// Field name.
        /// </summary>
        private string fieldName;

        /// <summary>
        /// Field type.
        /// </summary>
        private Type fieldType;

        /// <summary>
        /// Indicates whether the field is belongs to ON condition.
        /// </summary>
        private int incidenciasEnOnCondition;

        /// <summary>
        /// Initializes a new instance of the <see cref="FieldNode" /> class.
        /// </summary>
        /// <param name="fieldName">Field name.</param>
        /// <param name="fieldType">Field type.</param>
        /// <param name="incidenciasEnOnCondition">Incidences in ON condition.</param>
        public FieldNode(string fieldName, Type fieldType, int incidenciasEnOnCondition)
        {
            this.fieldName = fieldName;
            this.fieldType = fieldType;
            this.incidenciasEnOnCondition = incidenciasEnOnCondition;
        }

        /// <summary>
        /// Gets or sets the field name.
        /// </summary>
        public string FieldName
        {
            get
            {
                return this.fieldName;
            }

            set
            {
                this.fieldName = value;
            }
        }

        /// <summary>
        /// Gets or sets the field type.
        /// </summary>
        public Type FieldType
        {
            get
            {
                return this.fieldType;
            }

            set
            {
                this.fieldType = value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the field belongs to the ON condition.
        /// </summary>
        public int IncidenciasEnOnCondition
        {
            get
            {
                return this.incidenciasEnOnCondition;
            }
        }

        /// <summary>
        /// Gets or sets the field builder of the actual field.
        /// </summary>
        public System.Reflection.Emit.FieldBuilder FieldBuilder { get; set; }
    }
}
