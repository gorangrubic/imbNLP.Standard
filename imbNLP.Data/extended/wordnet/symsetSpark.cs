// --------------------------------------------------------------------------------------------------------------------
// <copyright file="symsetSpark.cs" company="imbVeles" >
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
namespace imbNLP.Data.extended.wordnet
{
    using imbSCI.Data.data;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Model of single Wordnet eng term query expansion
    /// </summary>
    /// <seealso cref="aceCommonTypes.primitives.imbBindable" />
    public class symsetSpark : imbBindable
    {
        public symsetSpark()
        {
        }

        private List<String> _serbian;

        /// <summary> </summary>
        public List<String> serbian
        {
            get
            {
                return _serbian;
            }
            set
            {
                _serbian = value;
                OnPropertyChanged("serbian");
            }
        }

        private List<String> _english;

        /// <summary> </summary>
        public List<String> english
        {
            get
            {
                return _english;
            }
            set
            {
                _english = value;
                OnPropertyChanged("english");
            }
        }

        private String _englishRootWord;

        /// <summary>the word used to extract the spark</summary>
        public String englishRootWord
        {
            get
            {
                return _englishRootWord;
            }
            set
            {
                _englishRootWord = value;
                OnPropertyChanged("englishRootWord");
            }
        }

        private String _symsetCode;

        /// <summary> </summary>
        public String symsetCode
        {
            get
            {
                return _symsetCode;
            }
            set
            {
                _symsetCode = value;
                OnPropertyChanged("symsetCode");
            }
        }
    }
}