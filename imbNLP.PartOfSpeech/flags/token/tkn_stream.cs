// --------------------------------------------------------------------------------------------------------------------
// <copyright file="tkn_stream.cs" company="imbVeles" >
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

namespace imbNLP.PartOfSpeech.flags.token
{
    [Flags]
    public enum tkn_stream
    {
        none = 0,

        sentence = 1 << 0,

        subsentence = 1 << 1,

        singleStream = 1 << 10,
        multiStream = 1 << 11,

        // <----- case

        properCase = 1 << 2,

        allCapsCase = 1 << 3,

        regularEndPunctaction = 1 << 4,

        noEndPunctaction = 1 << 5,

        regularEnd = 1 << 20,

        exclamationEnd = 1 << 21,

        questionEnd = 1 << 22,

        enumeration = 1 << 30,

        enbraced = 1 << 40,

        /// <summary>
        /// Between quotation marks \", ', ``,
        /// </summary>
        quoted = 1 << 41,

        /// <summary>
        /// The brackets: ( [ {
        /// </summary>
        brackets = 1 << 42,

        /// <summary>
        /// The between comma: ,
        /// </summary>
        betweenComma = 1 << 43,

        // <---- title role
        titleRole = 1 << 50,

        titleAllCaps = allCapsCase | titleRole,

        titleOpenEnd = noEndPunctaction | titleRole,

        titleProperEnd = regularEndPunctaction | titleRole,

        titleForEnumeration = enumeration | titleRole,

        // <----- sentence

        sentenceProperCase = sentence | properCase,

        sentenceRegularEnd = sentenceProperCase | regularEnd,

        sentenceEclamationEnd = sentenceProperCase | exclamationEnd,

        sentenceQuestionEnd = sentenceProperCase | questionEnd,
    }
}