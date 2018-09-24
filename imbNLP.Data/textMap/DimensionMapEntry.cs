// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DimensionMapEntry.cs" company="imbVeles" >
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
namespace imbNLP.Data.textMap
{
    /// <summary>
    /// Map entry describing the starting (character position at <see cref="DimensionMapBase.surface"/> string) and ending position of the hooked/mapped entry
    /// </summary>
    public class DimensionMapEntry
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DimensionMapEntry"/> class.
        /// </summary>
        public DimensionMapEntry()
        {
        }

        /// <summary>
        /// Gets or sets the hook.
        /// </summary>
        /// <value>
        /// The hook.
        /// </value>
        public string hook { get; set; }

        public int start { get; set; }
        public int end { get; set; }
    }
}