// --------------------------------------------------------------------------------------------------------------------
// <copyright file="morphRuleMatchSet.cs" company="imbVeles" >
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
namespace imbNLP.Data.semanticLexicon.morphology
{
    using imbNLP.Data.semanticLexicon.explore;
    using System.Collections.Generic;

    /// <summary>
    /// Collection of matched rules and associated models
    /// </summary>
    public class morphRuleMatchSet
    {
        /// <summary> </summary>
        public List<morphRuleMatch> matches { get; protected set; } = new List<morphRuleMatch>();

        public void Add(morphRuleMatch ruleMatch)
        {
            if (ruleMatch != null) matches.Add(ruleMatch);
        }

        /// <summary>
        /// Gets the explore items.
        /// </summary>
        /// <returns></returns>
        public List<termExploreModel> GetExploreItems()
        {
            List<termExploreModel> output = new List<termExploreModel>();
            foreach (morphRuleMatch rmatch in matches)
            {
                output.Add(rmatch.item);
            }
            return output;
        }
    }
}