// --------------------------------------------------------------------------------------------------------------------
// <copyright file="webLemmaTermTableSufix.cs" company="imbVeles" >
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
// Project: imbNLP.PartOfSpeech
// Author: Goran Grubic
// ------------------------------------------------------------------------------------------------------------------
// Project web site: http://blog.veles.rs
// GitHub: http://github.com/gorangrubic
// Mendeley profile: http://www.mendeley.com/profiles/goran-grubi2/
// ORCID ID: http://orcid.org/0000-0003-2673-9471
// Email: hardy@veles.rs
// </summary>
// ------------------------------------------------------------------------------------------------------------------
using imbSCI.Core.reporting;
using System;
using System.Collections.Generic;

namespace imbNLP.PartOfSpeech.TFModels.webLemma.table
{
    /// <summary>
    /// Additional meta info on the lemma table
    /// </summary>
    public class webLemmaTermTableSufix
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="webLemmaTermTableSufix"/> class.
        /// </summary>
        public webLemmaTermTableSufix()
        {
        }

        /// <summary>
        /// Recounts the minimum maximum.
        /// </summary>
        /// <param name="lemmas">The lemmas.</param>
        /// <param name="logger">The logger.</param>
        public void recountMinMax(List<webLemmaTerm> lemmas, ILogBuilder logger = null)
        {
            minDF = Double.MaxValue;
            minDSF = Double.MaxValue;
            minTF = Double.MaxValue;
            maxDF = Double.MinValue;
            maxDSF = Double.MinValue;
            maxTF = Double.MinValue;

            foreach (webLemmaTerm term in lemmas)
            {
                checkMinMax(term);
            }
        }

        public void checkMinMax(webLemmaTerm term)
        {
            minDF = Math.Min(term.documentFrequency, minDF);
            minDSF = Math.Min(term.documentSetFrequency, minDSF);
            minTF = Math.Min(term.termFrequency, minTF);
            maxDF = Math.Max(term.documentFrequency, maxDF);
            maxDSF = Math.Max(term.documentSetFrequency, maxDSF);
            maxTF = Math.Max(term.termFrequency, maxTF);
        }

        public Double maxTF { get; set; } = 0;
        public Double minTF { get; set; } = 0;

        public Double maxDF { get; set; } = 0;
        public Double minDF { get; set; } = 0;

        public Double maxDSF { get; set; } = 0;
        public Double minDSF { get; set; } = 0;
    }
}