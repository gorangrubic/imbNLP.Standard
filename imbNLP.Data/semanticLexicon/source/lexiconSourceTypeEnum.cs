// --------------------------------------------------------------------------------------------------------------------
// <copyright file="lexiconSourceTypeEnum.cs" company="imbVeles" >
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
namespace imbNLP.Data.semanticLexicon.source
{
    using System;

    [Flags]
    public enum lexiconSourceTypeEnum
    {
        none,
        serbianWordNet = 1,
        apertium = 2,

        /// <summary>
        /// The unitex delaf
        /// </summary>
        unitexDelaf = 4,

        corpus = 8,
        dictionary = 16,

        /// <summary>
        /// The unitex DELACF - compounds
        /// </summary>
        unitexDelas = 32,

        unitexDelafBig = 64,

        unitexDelasBig = 128,

        unitexImmutableBig = 256,
        englishWordNet = 512,
        domainConcepts = 1024,

        /// <summary>
        /// The MULTITEXT morphosyntactic resouce file format for inflected forms and lemmas
        /// </summary>
        multitext = 2048,

        /// <summary>
        /// Specification for MULTITEXT resource conversion
        /// </summary>
        multitextSpec = 4096,
    }
}