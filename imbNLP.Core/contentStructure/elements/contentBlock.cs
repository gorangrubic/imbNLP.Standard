// --------------------------------------------------------------------------------------------------------------------
// <copyright file="contentBlock.cs" company="imbVeles" >
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
// Project: imbNLP.Core
// Author: Goran Grubic
// ------------------------------------------------------------------------------------------------------------------
// Project web site: http://blog.veles.rs
// GitHub: http://github.com/gorangrubic
// Mendeley profile: http://www.mendeley.com/profiles/goran-grubi2/
// ORCID ID: http://orcid.org/0000-0003-2673-9471
// Email: hardy@veles.rs
// </summary>
// ------------------------------------------------------------------------------------------------------------------
namespace imbNLP.Core.contentStructure.elements
{
    #region imbVELES USING

    using imbNLP.Core.contentExtensions;
    using imbNLP.Core.contentStructure.collections;
    using imbNLP.Core.contentStructure.core;
    using imbNLP.Core.contentStructure.interafaces;
    using imbNLP.Data.enums;
    using imbNLP.Data.enums.flags;
    using imbSCI.Core.attributes;
    using imbSCI.Core.extensions.data;
    using imbSCI.Data;
    using imbSCI.Data.interfaces;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Xml.Serialization;

    #endregion imbVELES USING

    /// <summary>
    /// 2013c: LowLevel resurs
    /// </summary>
    [imb(imbAttributeName.xmlNodeTypeName, "b")]
    [imb(imbAttributeName.xmlEntityOutput, "title,flags")]
    public class contentBlock : contentElementBase, IContentBlock
    {
        public IEnumerator GetEnumerator()
        {
            if (items == null) return null;

            return items.GetEnumerator();
        }

        public int indexOf(IObjectWithChildSelector child)
        {
            if (items == null) return -1;

            return items.IndexOf(child as IContentParagraph);
        }

        public int Count()
        {
            if (items == null) return 0;

            return items.Count;
        }

        /// <summary>
        /// Gets the <see cref="System.Object"/> with the specified key.
        /// </summary>
        /// <value>
        /// The <see cref="System.Object"/>.
        /// </value>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public object this[int key]
        {
            get
            {
                if (items == null) return null;

                return items[key];
            }
        }

        /// <summary>
        /// Gets the <see cref="System.Object"/> with the specified child name.
        /// </summary>
        /// <value>
        /// The <see cref="System.Object"/>.
        /// </value>
        /// <param name="childName">Name of the child.</param>
        /// <returns></returns>
        public object this[string childName]
        {
            get
            {
                foreach (IContentElement ch in items)
                {
                    if (ch.name == childName)
                    {
                        return ch;
                    }
                }
                return null;
            }
        }

        public contentBlock()
        {
            items = new contentParagraphCollection();
        }

        public bool ContainsOneOrMore(contentRelationType qRelation, int limit, params contentBlockFlag[] _flgs)
        {
            return items.Query<contentBlockFlag>(contentRelationQueryType.gatherFlags, qRelation, this, limit).ContainsOneOrMore(_flgs);
        }

        public bool ContainsAll(contentRelationType qRelation, int limit, params contentBlockFlag[] _flgs)
        {
            return items.Query<contentBlockFlag>(contentRelationQueryType.gatherFlags, qRelation, this, limit).ContainsAll(_flgs);
        }

        public bool ContainsOnly(contentRelationType qRelation, int limit, params contentBlockFlag[] _flgs)
        {
            return items.Query<contentBlockFlag>(contentRelationQueryType.gatherFlags, qRelation, this, limit).ContainsOnly(_flgs);
        }

        #region -----------  title  -------  [block title - dobijen analizom sadrzaja]

        private string _title = ""; // = new String();

        /// <summary>
        /// block title - dobijen analizom sadrzaja
        /// </summary>
        // [XmlIgnore]
        [Category("contentBlock")]
        [DisplayName("title")]
        [Description("block title - dobijen analizom sadrzaja")]
        [imb(imbAttributeName.xmlEntityOutput)]
        public string title
        {
            get { return _title; }
            set
            {
                // Boolean chg = (_title != value);
                _title = value;
                OnPropertyChanged("title");
                // if (chg) {}
            }
        }

        #endregion -----------  title  -------  [block title - dobijen analizom sadrzaja]

        #region --- flags ------- flagovi za blok

        private contentBlockFlag _flags; //= new contentBlockFlags();

        /// <summary>
        /// flagovi za blok
        /// </summary>
        public contentBlockFlag flags
        {
            get { return _flags; }
            set
            {
                _flags = value;
                OnPropertyChanged("flags");
            }
        }

        #endregion --- flags ------- flagovi za blok

        #region IContentBlock Members

        IContentCollectionBase IContentElement.items
        {
            get { return _items; }
        }

        public override void primaryFlaging(params object[] resources)
        {
            foreach (var t in items) t.primaryFlaging(resources);
            //items.ForEach<IContentElement>(x => x.primaryFlaging(resources));
        }

        public override void secondaryFlaging(params object[] resources)
        {
            foreach (var t in items) t.secondaryFlaging(resources);
            //items.ForEach<IContentElement>(x => x.secondaryFlaging(resources));
        }

        public override void generalSemanticsFlaging(params object[] resources)
        {
            // throw new NotImplementedException();
        }

        public override void specialSematicsFlaging(params object[] resources)
        {
            // throw new NotImplementedException();
        }

        public override List<Enum> GetFlags()
        {
            List<Enum> output = new List<Enum>();

            foreach (var fl in flags.getEnumListFromFlags())
            {
                output.Add(fl as Enum);
            }

            return output;
        }

        #endregion IContentBlock Members

        #region -----------  paragraphs  -------  [Blokovi sadrzaja]

        //private contentParagraphCollection _items = new contentParagraphCollection();

        /// <summary>
        /// Blokovi sadrzaja
        /// </summary>
        [XmlIgnore]
        [Category("nlpContent")]
        [DisplayName("paragraphs")]
        [Description("Blokovi sadrzaja")]
        public new contentParagraphCollection items
        {
            get
            {
                if (_items == null) _items = new contentParagraphCollection();

                return _items as contentParagraphCollection;
            }
            set
            {
                // Boolean chg = (_paragraphs != value);
                _items = value;
                OnPropertyChanged("items");
                // if (chg) {}
            }
        }

        #endregion -----------  paragraphs  -------  [Blokovi sadrzaja]
    }
}