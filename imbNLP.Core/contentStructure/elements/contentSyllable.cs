// --------------------------------------------------------------------------------------------------------------------
// <copyright file="contentSyllable.cs" company="imbVeles" >
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

    using imbNLP.Core.contentStructure.collections;
    using imbNLP.Core.contentStructure.core;
    using imbNLP.Core.contentStructure.interafaces;
    using imbNLP.Core.contentStructure.tokenizator;
    using imbNLP.Data.enums;
    using imbSCI.Data.interfaces;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Text.RegularExpressions;

    #endregion imbVELES USING

    /// <summary>
    /// Slog iliti subtoken
    /// </summary>
    public class contentSyllable : contentElementBase, IContentSyllable
    {
        public IEnumerator GetEnumerator()
        {
            if (items == null) return null;

            return items.GetEnumerator();
        }

        public int indexOf(IObjectWithChildSelector child)
        {
            if (items == null) return -1;

            return items.IndexOf(child as IContentToken);
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

        public contentSyllable()
        {
        }

        public contentSyllable(string __content, IContentToken __parent, nlpTokenizatorSettings settings)
        {
            content = __content;
            parent = __parent;
            //sourceContent = __content;

            Match mv = settings.vowelLastRegex.Match(content);
            spliter = mv.Value;

            type = nlpSyllableType.unknown;

            if (tokenization.isNumericStart.IsMatch(content))
            {
                type = nlpSyllableType.numeric;
            }
            else if (tokenization.isLetterStart.IsMatch(content))
            {
                if (settings.syllabLengthLimit == -1)
                {
                    type = nlpSyllableType.regular;
                }
                else
                {
                    if (content.Length > settings.syllabLengthLimit)
                    {
                        type = nlpSyllableType.irregular;
                    }
                    else
                    {
                        type = nlpSyllableType.regular;
                    }
                }
            }
            else
            {
                if (content.Length > 0)
                {
                    type = nlpSyllableType.symbol;
                }
            }
        }

        #region IContentSyllable Members

        public override void primaryFlaging(params object[] resources)
        {
            //   throw new NotImplementedException();
        }

        public override void secondaryFlaging(params object[] resources)
        {
            //   throw new NotImplementedException();
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
            //foreach (var fl in flags)
            //{
            //    output.Add(fl as Enum);
            //}

            //foreach (var fl in detectionFlags)
            //{
            //    output.Add(fl as Enum);
            //}
            return output;
        }

        /// <summary>
        /// Syllable nema pod iteme
        /// </summary>
        // [XmlIgnore]
        public new IContentCollectionBase items
        {
            get { return null; }
        }

        #endregion IContentSyllable Members

        #region -----------  type  -------  [Tip sloga]

        private nlpSyllableType _type = nlpSyllableType.unknown; // = new nlpSyllableType();

        /// <summary>
        /// Tip sloga
        /// </summary>
        // [XmlIgnore]
        [Category("nlpSyllable")]
        [DisplayName("type")]
        [Description("Tip sloga")]
        public nlpSyllableType type
        {
            get { return _type; }
            set
            {
                _type = value;
                OnPropertyChanged("type");
            }
        }

        #endregion -----------  type  -------  [Tip sloga]
    }
}