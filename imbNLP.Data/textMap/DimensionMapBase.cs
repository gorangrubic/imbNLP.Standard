// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DimensionMapBase.cs" company="imbVeles" >
//
// Copyright (C) 2018 imbVeles
//
// This program is free software: you can redistribute it and/or modify
// it under the +terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see http://www.gnu.org/licenses/.
// </copyright>
// <summary>
// Project: imbNLP.Data
// Author: Goran Grubic
// ------------------------------------------------------------------------------------------------------------------
// Project web site: http://blog.veles.rs
// GitHub: http://github.com/gorangrubic
// Mendeley profile: http://www.mendeley.com/profiles/goran-grubi2/
// ORCID ID: http://orcid.org/0000-0003-2673-9471
// Email: hardy@veles.rs
// </summary>
// ------------------------------------------------------------------------------------------------------------------
namespace imbNLP.Data.textMap
{
    using System.Collections.Concurrent;
    using System.Xml;
    using System.Xml.Schema;
    using System.Xml.Serialization;

    /// <summary>
    /// Foundation of DimensionMap data structure
    /// </summary>
    /// <seealso cref="aceCommonTypes.sciDataStructures.data.package.IDataPackageItem" />
    public abstract class DimensionMapBase : IXmlSerializable, IDimensionMap
    {
        /// <summary>
        /// Constructor used by serialization, do not use this
        /// </summary>
        protected DimensionMapBase()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DimensionMapBase"/> class, setting dimension name and parent
        /// </summary>
        /// <param name="__dimensionName">Name of the dimension.</param>
        /// <param name="__parent">The parent.</param>
        protected DimensionMapBase(string __dimensionName, IDimensionMap __parent = null)
        {
            dimensionName = __dimensionName;
            parent = __parent;
        }

        /// <summary>
        /// Map entries of the DimensionMap. Do not
        /// </summary>
        /// <value>
        /// The entries.
        /// </value>
        protected ConcurrentBag<DimensionMapEntry> entries { get; set; } = new ConcurrentBag<DimensionMapEntry>();

        /// <summary>
        /// Hook (universal ID, path, hash...) resolution that retrieve instance - the hooked/mapped object
        /// </summary>
        /// <param name="hook">The hook String</param>
        /// <returns>Mapped object</returns>
        protected abstract object ResolveHook(string hook);

        /// <summary>
        /// Gets the hook (universal ID, path, hash...) for specified instance - the hooked object
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <returns>Universal ID to be used for mapping</returns>
        protected abstract string GetHook(object instance);

        protected DimensionMapQueryResult SelectByHook(string hook, DimensionMapQueryTakeMode takeMode = DimensionMapQueryTakeMode.selectInnerShadow)
        {
            DimensionMapQueryResult output = new DimensionMapQueryResult(this);

            return output;
        }

        [XmlIgnore]
        protected IDimensionMap parent { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [do use parent surface].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [do use parent surface]; otherwise, <c>false</c>.
        /// </value>
        [XmlIgnore]
        public bool doUseParentSurface
        {
            get
            {
                return (parent != null);
            }
        }

        /// <summary>
        /// If <c>true</c> it will allow <see cref="surface"/> to set <see cref="parent"/> surface value on Set
        /// </summary>
        /// <value>
        ///   <c>true</c> if [do write to parent surface]; otherwise, <c>false</c>.
        /// </value>
        public bool doWriteToParentSurface { get; protected set; } = false;

        private string _surface = "";

        /// <summary>
        /// Textual representation that is mapped by hooks -- not used if <see cref="doUseParentSurface"/>
        /// </summary>
        /// <value>
        /// The map surface.
        /// </value>
        public string surface
        {
            get
            {
                if (doUseParentSurface)
                {
                    return parent.surface;
                }
                return _surface;
            }
            protected set
            {
                if (doUseParentSurface)
                {
                    if (doWriteToParentSurface)
                    {
                        //parent.surface = value;
                    }
                }
                else
                {
                    _surface = value;
                }
            }
        }

        /// <summary>
        /// Name of this dimension
        /// </summary>
        /// <value>
        /// The name of the dimension.
        /// </value>
        public string dimensionName { get; protected set; }

        /// <summary>
        /// To be called before item is saved
        /// </summary>
        public abstract void OnBeforeSave();

        /// <summary>
        /// To be called after item is loaded
        /// </summary>
        public abstract void OnLoaded();

        /// <summary>
        /// XML serialization called at end of <see cref="WriteXml(XmlWriter)"/> - override this to add support for additional data of your inheriting class
        /// </summary>
        /// <param name="writer">The writer.</param>
        protected virtual void WriteXmlAppendix(XmlWriter writer)
        {
        }

        /// <summary>
        /// XML deserialization called at end of <see cref="ReadXml(XmlReader)"/> - override this to add support for additional data of your inheriting class
        /// </summary>
        /// <param name="reader">The reader.</param>
        protected virtual void ReadXmlAppendix(XmlReader reader)
        {
        }

        /// <summary>
        /// This method is reserved and should not be used. When implementing the IXmlSerializable interface, you should return null (Nothing in Visual Basic) from this method, and instead, if specifying a custom schema is required, apply the <see cref="T:System.Xml.Serialization.XmlSchemaProviderAttribute" /> to the class.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Xml.Schema.XmlSchema" /> that describes the XML representation of the object that is produced by the <see cref="M:System.Xml.Serialization.IXmlSerializable.WriteXml(System.Xml.XmlWriter)" /> method and consumed by the <see cref="M:System.Xml.Serialization.IXmlSerializable.ReadXml(System.Xml.XmlReader)" /> method.
        /// </returns>
        public XmlSchema GetSchema()
        {
            return (null);
        }

        /// <summary>
        /// Serializes data of this dimension
        /// </summary>
        /// <param name="writer">The <see cref="T:System.Xml.XmlWriter" /> stream to which the object is serialized.</param>
        public virtual void WriteXml(XmlWriter writer)
        {
            writer.Settings.Indent = true;

            writer.WriteElementString(nameof(dimensionName), dimensionName);
            if (!doUseParentSurface)
            {
                writer.WriteElementString(nameof(surface), surface);
            }

            writer.WriteStartElement(nameof(entries));

            foreach (DimensionMapEntry entry in entries)
            {
                writer.WriteStartElement(nameof(entry));

                writer.WriteAttributeString(nameof(entry.start), entry.start.ToString());
                writer.WriteAttributeString(nameof(entry.end), entry.end.ToString());
                writer.WriteAttributeString(nameof(entry.hook), entry.hook);

                writer.WriteEndElement();
            }

            writer.WriteEndElement();

            WriteXmlAppendix(writer);
        }

        /// <summary>
        /// De-serializes data of this dimension
        /// </summary>
        /// <param name="reader">The <see cref="T:System.Xml.XmlReader" /> stream from which the object is deserialized.</param>
        public virtual void ReadXml(XmlReader reader)
        {
            dimensionName = reader.ReadElementString(nameof(dimensionName));

            //reader.ReadElementContentAsString(nameof(surface));
            if (reader.LocalName == (nameof(surface)))
            {
                _surface = reader.ReadElementString(nameof(surface));
            }

            reader.ReadStartElement(nameof(entries));

            DimensionMapEntry entry;

            do
            {
                entry = new DimensionMapEntry();
                if (reader.MoveToAttribute(nameof(entry.start))) entry.start = reader.ReadContentAsInt();
                if (reader.MoveToAttribute(nameof(entry.end))) entry.end = reader.ReadContentAsInt();
                if (reader.MoveToAttribute(nameof(entry.hook)))
                {
                    entry.hook = reader.ReadContentAsString();
                    reader.MoveToElement();
                }
                entries.Add(entry);
            } while (reader.ReadToNextSibling(nameof(entry)));

            reader.ReadEndElement();

            ReadXmlAppendix(reader);
        }
    }
}