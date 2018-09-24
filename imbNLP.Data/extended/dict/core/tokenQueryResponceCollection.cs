// --------------------------------------------------------------------------------------------------------------------
// <copyright file="tokenQueryResponceCollection.cs" company="imbVeles" >
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
namespace imbNLP.Data.extended.dict.core
{
    using imbNLP.Data.enums.flags;
    using imbSCI.Data;
    using imbSCI.Data.collection.nested;
    using imbSCI.DataComplex.special;
    using System;
    using System.Collections.Generic;

    public class tokenQueryResponceCollection : aceEnumDictionary<tokenQuerySourceEnum, List<tokenQueryResponse>>
    {
        private instanceCountCollection<tokenQueryResultEnum> stats;

        public void process()
        {
            stats = new instanceCountCollection<tokenQueryResultEnum>();
            foreach (tokenQuerySourceEnum key in Keys)
            {
                foreach (tokenQueryResponse res in this[key])
                {
                    stats.AddInstance(res.response, 1);
                    if (res.description.isNullOrEmpty())
                    {
                        description.Add(res.description);
                    }
                    //if (flags.Contains(res)
                }
            }
        }

        private contentTokenFlag _flags;

        /// <summary>
        ///
        /// </summary>
        public contentTokenFlag flags
        {
            get { return _flags; }
            set { _flags = value; }
        }

        private List<String> _description = new List<string>();

        /// <summary>
        ///
        /// </summary>
        public List<String> description
        {
            get { return _description; }
            set { _description = value; }
        }
    }
}