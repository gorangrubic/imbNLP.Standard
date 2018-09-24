// --------------------------------------------------------------------------------------------------------------------
// <copyright file="pos_type.cs" company="imbVeles" >
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
namespace imbNLP.PartOfSpeech.flags.basic
{
    public enum pos_type
    {
        none,

        /// <summary>
        /// Noun
        /// </summary>
        N,

        /// <summary>
        /// Adjective
        /// </summary>
        A,

        /// <summary>
        /// Verb
        /// </summary>
        V,

        /// <summary>
        /// The preposition, or adposition
        /// </summary>
        PREP,

        CONJ,
        INT,
        NUM,

        /// <summary>
        /// Proverb
        /// </summary>
        PRO,

        ADV,

        /// <summary>
        /// Adposition
        /// </summary>
       // ADP = PREP,
        ABB,

        PREF,
        NUMnumerical,
        PAR,
        TEMP,

        PUNCT,

        /// <summary>
        /// Residual
        /// </summary>
        RES,
    }
}