// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ITextResourceResolver.cs" company="imbVeles" >
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
using System;
using System.Collections.Generic;

namespace imbNLP.PartOfSpeech.resourceProviders.core
{
    using imbNLP.PartOfSpeech.lexicUnit;
    using imbSCI.Core.reporting;

    public interface ITextResourceResolver
    {
        /// <summary>
        /// Main method for token resolution. Returns null on fail
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="logger">The logger.</param>
        /// <returns></returns>
        lexicInflection GetLexicUnit(String key, ILogBuilder logger = null);

        void LoadLexicResource(ILogBuilder output, String resourceFilePath);

        void SaveUsedCache(string localLemmaResourcePath, Boolean clearCache = true);

        /// <summary>
        /// Gets the lemma set for inflection.
        /// </summary>
        /// <param name="inflection">The inflection.</param>
        /// <param name="allInflections">All inflections.</param>
        /// <param name="logger">The logger.</param>
        /// <returns></returns>
        lexicGraphSetWithLemma GetLemmaSetForInflection(String inflection, List<String> allInflections, ILogBuilder logger = null);
    }
}