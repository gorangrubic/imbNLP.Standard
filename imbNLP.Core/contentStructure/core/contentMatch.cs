// --------------------------------------------------------------------------------------------------------------------
// <copyright file="contentMatch.cs" company="imbVeles" >
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

    using imbNLP.Core.contentStructure.interafaces;
    using imbSCI.Core.extensions.text;
    using imbSCI.Data.data;
    using System;
    using System.Text.RegularExpressions;

    #endregion imbVELES USING

    /// <summary>
    /// Interni REGEX MATCH
    /// </summary>
    public class contentMatch : imbBindable
    {
        #region --- name ------- jedinstveno ime

        private string _name;

        /// <summary>
        /// jedinstveno ime
        /// </summary>
        public string name
        {
            get
            {
                if (string.IsNullOrEmpty(_name))
                {
                    if (match != null)
                    {
                        _name = match.Index + "_" + match.Length + "_" + associatedKey.toStringSafe("nokey") + "_" +
                                match.Value;
                    }
                }
                return _name;
            }
            set
            {
                _name = value;
                OnPropertyChanged("name");
            }
        }

        #endregion --- name ------- jedinstveno ime

        public contentMatch(Enum _key, Match _match)
        {
            associatedKey = _key;
            match = _match;
        }

        #region --- associatedKey ------- kljuc koji je dodeljen

        private Enum _associatedKey;

        /// <summary>
        /// kljuc koji je dodeljen
        /// </summary>
        public Enum associatedKey
        {
            get { return _associatedKey; }
            set
            {
                _associatedKey = value;
                OnPropertyChanged("associatedKey");
            }
        }

        #endregion --- associatedKey ------- kljuc koji je dodeljen

        #region --- match ------- Bindable property

        private Match _match;

        /// <summary>
        /// Bindable property
        /// </summary>
        public Match match
        {
            get { return _match; }
            set
            {
                _match = value;
                OnPropertyChanged("match");
            }
        }

        #endregion --- match ------- Bindable property

        #region --- element ------- instanca content elementa koji je rezultat ovog matcha

        private IContentElement _element;

        /// <summary>
        /// instanca content elementa koji je rezultat ovog matcha
        /// </summary>
        public IContentElement element
        {
            get { return _element; }
            set
            {
                _element = value;
                OnPropertyChanged("element");
            }
        }

        #endregion --- element ------- instanca content elementa koji je rezultat ovog matcha
    }
}