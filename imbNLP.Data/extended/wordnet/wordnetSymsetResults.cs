// --------------------------------------------------------------------------------------------------------------------
// <copyright file="wordnetSymsetResults.cs" company="imbVeles" >
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
    using imbNLP.Data.semanticLexicon.explore;
    using imbSCI.Core.extensions.data;
    using imbSCI.DataComplex.special;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Result of wordnetSymsetQuery
    /// </summary>
    /// <seealso cref="aceCommonTypes.collection.special.translationTableMulti{System.String, System.String}" />
    public class wordnetSymsetResults : translationTableMulti<String, String>
    {
        private Dictionary<String, termExploreModel> _models = new Dictionary<String, termExploreModel>();

        /// <summary>
        ///
        /// </summary>
        public Dictionary<String, termExploreModel> models
        {
            get
            {
                //if (_models == null)_models = new Dictionary<String, termExploreModel>();
                return _models;
            }
            protected set { _models = value; }
        }

        public Int32 Count
        {
            get
            {
                return entries.Count;
            }
        }

        public List<String> GetEnglish()
        {
            return GetValues();
        }

        public List<String> GetSymsetCodes()
        {
            return GetKeys();
        }

        public List<String> GetForEnglish(String word)
        {
            return GetByValue(word);
        }

        public List<String> GetForSymsetCode(String word)
        {
            return GetByKey(word);
        }

        public List<String> GetAll()
        {
            List<String> output = new List<string>();
            output.AddRange(GetEnglish());
            output.AddRange(GetSymsetCodes());
            return output;
        }

        public override List<String> GetByValue(String needle)
        {
            List<String> output = new List<String>();
            foreach (var pair in entries)
            {
                if (String.Equals(pair.Value, needle, StringComparison.CurrentCultureIgnoreCase))
                {
                    output.AddUnique(pair.Value);
                }
            }
            return output;
        }

        internal void Add(string code, string eng)
        {
            throw new NotImplementedException();
        }

        public override List<String> GetByKey(String needle)
        {
            List<String> output = new List<String>();
            foreach (var pair in entries)
            {
                if (String.Equals(pair.Key, needle, StringComparison.CurrentCultureIgnoreCase))
                {
                    output.AddUnique(pair.Key);
                }
            }
            return output;
        }
    }
}