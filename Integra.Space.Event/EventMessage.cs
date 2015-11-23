//-----------------------------------------------------------------------
// <copyright file="EventMessage.cs" company="Ingetra.Vision.Common">
//     Copyright (c) Ingetra.Vision.Common. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Integra.Vision.Event
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml.Serialization;

    /// <summary>
    /// Message class
    /// </summary>
    [Serializable]
    public sealed class EventMessage : IEnumerable<MessageSection>, ICollection<MessageSection>, IEnumerable
    {
        /// <summary>
        /// List of elements
        /// </summary>
        private List<MessageSection> messageSectionList;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventMessage"/> class
        /// </summary>
        public EventMessage()
        {
            this.messageSectionList = new List<MessageSection>();
        }
        
        /// <summary>
        /// Gets the number of sections of the message
        /// </summary>
        public int Count
        {
            get { return this.messageSectionList.Count; }
        }

        /// <inheritdoc />
        public bool IsReadOnly
        {
            get { return true; }
        }

        /// <summary>
        /// Get the message element by a specific identifier.
        /// </summary>
        /// <param name="identifier">Message element identifier</param>
        /// <returns>Message element</returns>
        public MessageSection this[int identifier]
        {
            get
            {
                return this.messageSectionList.Find(x => x.Identifier == identifier);
            }
        }

        /// <summary>
        /// Get the message element by a specific name.
        /// </summary>
        /// <param name="name">Message element name</param>
        /// <returns>Message element</returns>
        public MessageSection this[string name]
        {
            get
            {
                return this.messageSectionList.Find(x => x.Name == name);
            }
        }

        /// <inheritdoc />
        public IEnumerator<MessageSection> GetEnumerator()
        {
            return this.messageSectionList.GetEnumerator();
        }

        /// <inheritdoc />
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)this.messageSectionList).GetEnumerator();
        }

        /// <inheritdoc />
        public void Add(MessageSection item)
        {
            this.messageSectionList.Add(item);
        }

        /// <inheritdoc />
        public void Clear()
        {
            this.messageSectionList.Clear();
        }

        /// <inheritdoc />
        public bool Contains(MessageSection item)
        {
            return this.messageSectionList.Contains(item);
        }

        /// <summary>
        /// Doc goes here
        /// </summary>
        /// <param name="sectionName">Name of the section</param>
        /// <returns>Whether the section exists or not</returns>
        public bool Contains(string sectionName)
        {
            return this.messageSectionList.Exists(x => x.Name == sectionName);
        }

        /// <summary>
        /// Doc goes here
        /// </summary>
        /// <param name="sectionIdentifier">Identifier of the section</param>
        /// <returns>Whether the section exists or not</returns>
        public bool Contains(int sectionIdentifier)
        {
            return this.messageSectionList.Exists(x => x.Identifier == sectionIdentifier);
        }

        /// <inheritdoc />
        public void CopyTo(MessageSection[] array, int arrayIndex)
        {
            this.messageSectionList.CopyTo(array, arrayIndex);
        }

        /// <inheritdoc />
        public bool Remove(MessageSection item)
        {
            return this.messageSectionList.Remove(item);
        }
    }
}
