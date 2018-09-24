// --------------------------------------------------------------------------------------------------------------------
// <copyright file="contentElementBase.cs" company="imbVeles" >
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
namespace imbNLP.Core.contentStructure.core
{
    #region imbVELES USING

    using imbNLP.Core.contentStructure.collections;
    using imbNLP.Core.contentStructure.interafaces;

    // using Newtonsoft.Json;
    using imbSCI.Core.attributes;
    using imbSCI.Core.extensions.text;
    using imbSCI.Data;
    using imbSCI.Data.data;
    using imbSCI.Data.interfaces;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Xml.Serialization;

    //    using imbCore.core.interfaces;
    //using imbCore.stringTools;

    #endregion imbVELES USING

    [imb(imbAttributeName.collectionDisablePrimaryKey)]
    public abstract class contentElementBase : imbBindable, IObjectWithParent, IObjectWithPath
    {
        public abstract List<Enum> GetFlags();

        /// <summary>
        /// Gets the root.
        /// </summary>
        /// <value>
        /// The root.
        /// </value>
        public object root
        {
            get
            {
                if (parent != null)
                {
                    return parent.root;
                }
                return this;
            }
        }

        #region -----------  match  -------  [Kontent match meta info]

        private contentMatch _match; // = new contentMatch();

        /// <summary>
        /// Kontent match meta info
        /// </summary>
        // [XmlIgnore]
        [Category("contentElementBase")]
        [DisplayName("Match")]
        [Description("Kontent match meta info")]
        [Browsable(false)]
        public contentMatch match
        {
            get
            {
                return _match;
            }
            set
            {
                // Boolean chg = (_match != value);
                _match = value;
                OnPropertyChanged("match");
                // if (chg) {}
            }
        }

        #endregion -----------  match  -------  [Kontent match meta info]

        private string _UID = "";

        #region imbObject Property <String> UID

        /// <summary>
        /// imbControl property UID tipa String
        /// </summary>
        public string UID
        {
            get
            {
                //if (_UID == null) _UID = imbStringGenerators.getRandomString(32);

                return _UID;
            }
            set
            {
                _UID = value;
                OnPropertyChanged("UID");
            }
        }

        #endregion imbObject Property <String> UID

        #region FLAGING

        /// <summary>
        /// izvrsava se od
        /// </summary>
        /// <param name="resources"></param>
        public abstract void primaryFlaging(params object[] resources);

        public abstract void secondaryFlaging(params object[] resources);

        public abstract void generalSemanticsFlaging(params object[] resources);

        public abstract void specialSematicsFlaging(params object[] resources);

        #endregion FLAGING

        #region IContentElement Members

        // private IImbCollections _items;

        /// <summary>
        /// Podešava PREV i NEXT za nlpBase klase -- i dodaje item u svoju items kolekciju
        /// </summary>
        /// <remarks>
        /// Radiće u slučaju da je item član liste kao i da nije. Ako nije dodeliće mu prev kao last
        /// </remarks>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        public IContentElement setItem(IContentElement newItem)
        {
            IContentElement source = this as IContentElement;
            if (newItem == this)
            {
                return newItem;
            }
            if (source == null)
            {
                return null;
            }
            if (!source.items.Contains(newItem))
            {
                source.items.Add(newItem);
            }

            newItem.parent = this as IContentElement;
            source.items.Nest(newItem);

            //   ind = source.items.IndexOf(newItem);

            /*
            if (id > 0)
            {
                newItem.prev = source.items.items[ind - 1] as IContentElement;
                newItem.prev.next = newItem;
            }
            */
            return newItem;
        }

        /// <summary>
        /// Referenca prema parent objektu
        /// </summary>
        object IObjectWithParent.parent
        {
            get { return parent as object; }
        }

        /// <summary>
        /// Putanja objekta
        /// </summary>
        string IObjectWithPath.path
        {
            get
            {
                string __path = "";

                if (parent != null)
                {
                    return parent.path.add(name, "/");
                }
                else
                {
                    return "/" + name;
                }
            }
        }

        /// <summary>
        /// Osnovni ID podatak
        /// </summary>
        public long id
        {
            get
            {
                long _id = -1;

                if (parent != null)
                {
                    if (parent.items != null)
                    {
                        if (parent.items.Contains(this))
                        {
                            //__path = __path + ".items[" + parent.items.IndexOf(this) + "]";

                            _id = parent.items.IndexOf(this);
                        }
                    }
                }
                return _id;
            }
            set
            {
                // throw new NotImplementedException();
            }
        }

        #endregion IContentElement Members

        #region --- _nameAbbrevation ------- skracenica imena

        private string __nameAbbrevation = "";

        /// <summary>
        /// skracenica imena
        /// </summary>
        protected string _nameAbbrevation
        {
            get
            {
                if (string.IsNullOrEmpty(__nameAbbrevation))
                {
                    __nameAbbrevation = GetType().Name.imbGetAbbrevation(3, true);
                }
                return __nameAbbrevation;
            }
        }

        #endregion --- _nameAbbrevation ------- skracenica imena

        #region -----------  name  -------  [ime objekta - ne jedinstveno]

        private string _name; // = new String();

        /// <summary>
        /// ime objekta - ime tipa i ID - ako je objekat deo glavnog niza
        /// </summary>
        // [XmlIgnore]
        [Category("contentElementBase")]
        [DisplayName("name")]
        [Description("ime objekta - ne jedinstveno")]
        // [JsonProperty]
        public string name
        {
            get
            {
                _name = _nameAbbrevation;

                long _id = id;

                if (_id > 0)
                {
                    _name += "[" + _id.ToString() + "]";
                }

                return _name;
            }
            set
            {
                // Boolean chg = (_name != value);
                // _name = value;
                // OnPropertyChanged("name");
                // if (chg) {}
            }
        }

        #endregion -----------  name  -------  [ime objekta - ne jedinstveno]

        #region -----------  isLast  -------  [Da li je reč o poslednjem elementu?]

        /// <summary>
        /// Skup pod-itema
        /// </summary>
        // [XmlIgnore]
        //public abstract IContentCollectionBase items { get; }

        protected IContentCollectionBase _items;
        /// <summary>
        ///
        /// </summary>
        //public abstract IContentCollection items { get; set; }
        //{
        //    get { return _items; }
        //    set { _items = value; }
        //}

        /// <summary>
        /// Da li je reč o poslednjem elementu?
        /// </summary>
        [XmlIgnore]
        [Category("Order")]
        [DisplayName("isLast")]
        [Description("Da li je reč o poslednjem elementu?")]
        public bool isLast
        {
            get { return (next == null); }
        }

        #endregion -----------  isLast  -------  [Da li je reč o poslednjem elementu?]

        #region -----------  isFirst  -------  [Da li je reč o prvom elementu?]

        /// <summary>
        /// Da li je reč o prvom elementu?
        /// </summary>
        [XmlIgnore]
        [Category("Order")]
        [DisplayName("isFirst")]
        [Description("Da li je reč o prvom elementu?")]
        public bool isFirst
        {
            get { return (prev == null); }
        }

        #endregion -----------  isFirst  -------  [Da li je reč o prvom elementu?]

        #region -----------  next  -------  [Referenca prema sledećem slogu]

        private IContentElement _next; // = new nlpSyllable();

        /// <summary>
        /// Referenca prema sledećem slogu
        /// </summary>
        [XmlIgnore]
        public IContentElement next
        {
            get
            {
                /*
                if (_next == null) {
                    if (parent != null)
                    {
                        if (parent.items.Contains(this))
                        {
                            _next
                        }
                    }
                }*/
                return _next;
            }
            set
            {
                _next = value;
                OnPropertyChanged("next");
            }
        }

        #endregion -----------  next  -------  [Referenca prema sledećem slogu]

        #region -----------  prev  -------  [Referenca prema prethodnom slogu]

        private IContentElement _prev; // = new nlpSyllable();

        /// <summary>
        /// Referenca prema prethodnom slogu
        /// </summary>
        [XmlIgnore]
        public IContentElement prev
        {
            get { return _prev; }
            set
            {
                _prev = value;
                OnPropertyChanged("prev");
            }
        }

        #endregion -----------  prev  -------  [Referenca prema prethodnom slogu]

        #region -----------  parent  -------  [referenca prema parent objektu]

        private IContentElement _parent; // = new nlpBase();

        /// <summary>
        /// referenca prema parent objektu
        /// </summary>
        [XmlIgnore]
        public IContentElement parent
        {
            get { return _parent; }
            set
            {
                _parent = value;
                OnPropertyChanged("parent");
            }
        }

        #endregion -----------  parent  -------  [referenca prema parent objektu]

        #region -----------  page  -------  [Referenca prema stranici ]

        /// <summary>
        /// Referenca prema stranici
        /// </summary>
        // [XmlIgnore]
        [Category("contentElementBase")]
        [DisplayName("page")]
        [Description("Referenca prema stranici ")]
        public IContentPage page
        {
            get
            {
                if (_parent == null) return null;
                if (_parent is IContentPage)
                {
                    return _parent as IContentPage;
                }
                return _parent.page;
            }
        }

        #endregion -----------  page  -------  [Referenca prema stranici ]

        #region -----------  sourceContent  -------  [Izvorni content]

        private string _sourceContent = ""; // = new String();

        /// <summary>
        /// Izvorni content
        /// </summary>
        // [XmlIgnore]
        [Category("Content")]
        [DisplayName("sourceContent")]
        [Description("Izvorni content")]
        public string sourceContent
        {
            get { return _sourceContent; }
            set
            {
                _sourceContent = value;
                OnPropertyChanged("sourceContent");
            }
        }

        #endregion -----------  sourceContent  -------  [Izvorni content]

        #region -----------  content  -------  [Originalni sadržaj koji je pronađen u tokenu]

        private string _content = ""; // = new String();

        /// <summary>
        /// Originalni sadržaj koji je pronađen u tokenu
        /// </summary>
        // [XmlIgnore]
        [Category("Content")]
        [DisplayName("content")]
        [Description("Originalni sadržaj koji je pronađen u tokenu")]
        // [JsonProperty]
        public string content
        {
            get { return _content; }
            set
            {
                _content = value;
                OnPropertyChanged("content");
            }
        }

        #endregion -----------  content  -------  [Originalni sadržaj koji je pronađen u tokenu]

        public override string ToString()
        {
            return content;
        }

        private string _spliter = ""; // = new String();

        /// <summary>
        /// Koji je spliter odredio odvajanje tokena
        /// </summary>
        // [XmlIgnore]
        [Category("Content")]
        [DisplayName("spliter")]
        [Description("Koji je spliter odredio odvajanje  tokena/recenice")]
        [imb(imbAttributeName.xmlEntityOutput)]
        // [JsonProperty]
        public string spliter
        {
            get { return _spliter; }
            set
            {
                _spliter = value;
                OnPropertyChanged("spliter");
            }
        }
    }
}