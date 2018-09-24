// --------------------------------------------------------------------------------------------------------------------
// <copyright file="cnt_level.cs" company="imbVeles" >
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

namespace imbNLP.PartOfSpeech.flags.token
{
    /// <summary>
    /// Container level
    /// </summary>
    public enum cnt_level
    {
        none,

        /// <summary>
        /// The mc repo: set of site
        /// </summary>
        mcRepo,

        /// <summary>
        /// The mc site: set of pages
        /// </summary>
        mcSite,

        /// <summary>
        /// The mc webpage:
        /// </summary>
        mcPage,

        /// <summary>
        /// The mc block: block with set of mcTokenStreams
        /// </summary>
        mcBlock,

        /// <summary>
        /// The mc token stream: stream of tokens, not necessarily a sentence
        /// </summary>
        mcTokenStream,

        /// <summary>
        /// The mc token
        /// </summary>
        mcToken,

        /// <summary>
        /// Chunk constructed
        /// </summary>
        mcChunk,

        /// <summary>
        /// The mc subtoken: hypothetical use
        /// </summary>
        mcSubtoken,
    }
}