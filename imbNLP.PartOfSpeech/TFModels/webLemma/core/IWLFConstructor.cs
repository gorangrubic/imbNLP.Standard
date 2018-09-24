// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IWLFConstructor.cs" company="imbVeles" >
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

// // using imbMiningContext.TFModels.WLF_ISF;
using imbNLP.PartOfSpeech.flags.token;
using imbNLP.PartOfSpeech.pipeline.machine;
using imbNLP.PartOfSpeech.resourceProviders.core;
using imbNLP.PartOfSpeech.TFModels.webLemma.table;
using imbSCI.Core.reporting;
using System;
using System.Collections.Generic;

namespace imbNLP.PartOfSpeech.TFModels.webLemma.core
{
    /// <summary>
    /// A constructor for Web Lemma Term Table
    /// </summary>
    public interface IWLFConstructor
    {
        //  webLemmaTermTable process(IEnumerable<IPipelineTaskSubject> source, cnt_level document_level, cnt_level term_level, String tableName, ITextResourceResolver parser = null, ILogBuilder logger = null, webLemmaTermTable table = null);

        webLemmaTermTable process(IEnumerable<IPipelineTaskSubject> source, cnt_level document_level, webLemmaTermTable table = null, ITextResourceResolver parser = null,
            ILogBuilder logger = null, Boolean forSingleWebSite = false);

        wlfConstructorSettings settings { get; set; }

        /// <summary>
        /// Describes self in multiple lines. Description contains the most important settings and way of operation
        /// </summary>
        /// <returns></returns>
        List<String> DescribeSelf();
    }
}