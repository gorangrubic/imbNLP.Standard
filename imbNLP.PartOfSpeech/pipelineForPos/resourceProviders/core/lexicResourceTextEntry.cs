// --------------------------------------------------------------------------------------------------------------------
// <copyright file="lexicResourceTextEntry.cs" company="imbVeles" >
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
namespace imbNLP.PartOfSpeech.resourceProviders.core
{
    /// <summary>
    /// Basic resource entry - from a lexic resource in text file format
    /// </summary>
    public class lexicResourceTextEntry
    {
        /// <summary>
        /// Original source line that was found in the source file
        /// </summary>
        /// <value>
        /// The entry source line.
        /// </value>
        public string sourceLine { get; set; } = "";

        /// <summary>
        /// The lemma form matching the <see cref="tokenForm"/>
        /// </summary>
        /// <value>
        /// The lemma form.
        /// </value>
        public string lemmaForm { get; set; } = "";

        /// <summary>
        /// The token form, as it was matched in the source (inflected form)
        /// </summary>
        /// <value>
        /// The token form.
        /// </value>
        public string tokenForm { get; set; } = "";

        /// <summary>
        /// Unparsed grammatical case information, for the tokenForm
        /// </summary>
        /// <value>
        /// The gram information.
        /// </value>
        public string gramInfo { get; set; } = "";

        /// <summary>
        /// Everything found in the entry line, that wasn-t already separated into other properties
        /// </summary>
        /// <value>
        /// The extra information.
        /// </value>
        public string extraInformation { get; set; } = "";
    }
}