// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MultiDimensionMapBase.cs" company="imbVeles" >
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
    /// Collection of <see cref="DimensionMap"/>s, sharing the same <see cref="DimensionMap.surface"/> form
    /// </summary>
    /// <seealso cref="aceCommonTypes.sciDataStructures.data.textMap.IDimensionMap" />
    public class MultiDimensionMapBase : IDimensionMap
    {
        public MultiDimensionMapBase()
        {
        }

        public void Save(string path)
        {
        }

        public void Load(string path)
        {
        }

        /// <summary>
        /// Textual representation that is mapped by hooks -- not used if <see cref="doUseParentSurface"/>
        /// </summary>
        /// <value>
        /// The map surface.
        /// </value>
        public string surface { get; set; }

        public string dimensionName { get; set; } = "main";

        public void OnBeforeSave()
        {
        }

        public void OnLoaded()
        {
        }
    }
}