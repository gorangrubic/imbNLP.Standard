// --------------------------------------------------------------------------------------------------------------------
// <copyright file="contentChunk.cs" company="imbVeles" >
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
namespace imbNLP.Core.contentStructure.metaElements
{
    #region imbVELES USING

    using imbNLP.Core.contentStructure.collections;
    using imbNLP.Core.contentStructure.core;
    using imbNLP.Core.contentStructure.interafaces;
    using imbNLP.Data.enums;
    using imbSCI.Data.interfaces;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;

    #endregion imbVELES USING

    /// <summary>
    /// Jedna veza izmedju više tokena
    /// </summary>
    public class contentChunk : contentElementBase, IContentChunk
    {
        public override List<Enum> GetFlags()
        {
            List<Enum> output = new List<Enum>();

            return output;
        }

        public IEnumerator GetEnumerator()
        {
            if (items == null) return null;

            return items.GetEnumerator();
        }

        public int indexOf(IObjectWithChildSelector child)
        {
            if (items == null) return -1;

            return items.IndexOf(child as IContentChunk);
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

        #region IContentChunk Members

        /// <summary>
        /// izvrsava se od
        /// </summary>
        /// <param name="resources"></param>
        public override void primaryFlaging(params object[] resources)
        {
            throw new NotImplementedException();
        }

        public override void secondaryFlaging(params object[] resources)
        {
            throw new NotImplementedException();
        }

        public override void generalSemanticsFlaging(params object[] resources)
        {
            throw new NotImplementedException();
        }

        public override void specialSematicsFlaging(params object[] resources)
        {
            throw new NotImplementedException();
        }

        #endregion IContentChunk Members

        #region --- items ------- kolekcija elemenata koji cine chunk

        // private contentChunkCollection _items = new contentChunkCollection();

        /// <summary>
        /// kolekcija elemenata koji cine chunk
        /// </summary>
        public new contentChunkCollection items
        {
            get
            {
                if (_items == null) _items = new contentChunkCollection();
                return _items as contentChunkCollection;
            }
            set
            {
                _items = value;
                OnPropertyChanged("items");
            }
        }

        #endregion --- items ------- kolekcija elemenata koji cine chunk

        #region -----------  chunkType  -------  [Tip chunka]

        private nlpChunkType _chunkType; // = new nlpChunkType();

        /// <summary>
        /// Tip chunka
        /// </summary>
        // [XmlIgnore]
        [Category("nlpChunk")]
        [DisplayName("chunkType")]
        [Description("Tip chunka")]
        public nlpChunkType chunkType
        {
            get { return _chunkType; }
            set
            {
                _chunkType = value;
                OnPropertyChanged("chunkType");
            }
        }

        IContentCollectionBase IContentElement.items
        {
            get
            {
                return items;
            }
        }

        #endregion -----------  chunkType  -------  [Tip chunka]

        //#region -----------  tokens  -------  [Kolekcija tokena koji cine chunk]
        //private contentElementCollection _tokens = new contentElementCollection();
        ///// <summary>
        ///// Kolekcija tokena koji cine chunk
        ///// </summary>
        //// [XmlIgnore]
        //[XmlIgnore]

        //[Category("nlpChunk")]
        //[DisplayName("tokens")]
        //[Description("Kolekcija tokena koji cine chunk")]
        //public contentElementCollection tokens
        //{
        //    get
        //    {
        //        return _tokens;
        //    }
        //    set
        //    {
        //        _tokens = value;
        //        OnPropertyChanged("tokens");
        //    }
        //}
        //#endregion
    }
}