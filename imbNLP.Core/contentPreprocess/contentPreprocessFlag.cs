// --------------------------------------------------------------------------------------------------------------------
// <copyright file="contentPreprocessFlag.cs" company="imbVeles" >
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
namespace imbNLP.Core.contentPreprocess
{
    using System;

    /// <summary>
    /// Flagovi - contentPreprocessFlag - izvrsavaju se pre nego sto se stvore recenice
    /// </summary>
    [Flags]
    public enum contentPreprocessFlag
    {
        none = 0,

        /// <summary>
        /// sve quote karaktere ce zameniti za standardizocanu quote formu
        /// </summary>
        quoteStandardization = 1,

        /// <summary>
        /// Akronime sa . pretvara u velika slova, bez ..
        /// </summary>
        acronimStandardization = 2,

        /// <summary>
        /// Svako pominjanje godine u odredjenom rednom formatu i/ili padezu predvara u YYYY. format
        /// </summary>
        yearOrdinal = 4,

        /// <summary>
        /// standardizuje ispis imena
        /// </summary>
        titleStandardize = 8,

        /// <summary>
        /// Standardizuje nacin na koji je deo recenice zagradjen sa ({[ ili drugim zagradama
        /// </summary>
        enbraceStandardize = 16,

        /// <summary>
        /// Standardizuje nacin na koji je iskazano nabrajanje liste u recenici
        /// </summary>
        enlistStandardize = 32,

        deentitize = 64,

        /// <summary>
        /// The international standards format
        /// </summary>
        internationalStandardsFormat = 128,

        standard = 256
    }
}