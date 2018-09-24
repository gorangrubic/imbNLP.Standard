// --------------------------------------------------------------------------------------------------------------------
// <copyright file="contentTokenSubjectRenderMode.cs" company="imbVeles" >
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

namespace imbNLP.PartOfSpeech.pipelineForPos.render
{
    /// <summary>
    /// The way content token subject is rendered
    /// </summary>
    public enum contentTokenSubjectRenderMode
    {
        none,

        /// <summary>
        /// Putting out the current form of the token
        /// </summary>
        currentForm,

        /// <summary>
        /// Putting out the initial form of the token
        /// </summary>
        initialForm,

        /// <summary>
        /// The position type tag form: |A V|N|Part|Conj|
        /// </summary>
        posTypeTagForm,

        /// <summary>
        /// The position type and gram tag form: [Amspf:Nmsps]|ADJ[fs1f]|
        /// </summary>
        posTypeAndGramTagForm,

        /// <summary>
        /// The flags form: |phoneOfficeNeedle|symbol|number phone phoneNumber|
        /// </summary>
        flagsForm,

        /// <summary>
        /// The flags form: |dat_business.phoneOfficeNeedle|tkn_contains.symbols|tkn_contains.number dat_business.phone dat_business.phoneNumber|
        /// </summary>
        flagsFullForm,

        /// <summary>
        /// The descriptive form: |"kompanijom":"kompanija":N,common,f,s,instrumental:lowerCase,letter,onlyLetters|
        /// </summary>
        descriptive,

        /// <summary>
        /// The lemma form: clean lemmatic form
        /// </summary>
        lemmaForm,

        xmlModelOfMCDocumentElement,
    }
}