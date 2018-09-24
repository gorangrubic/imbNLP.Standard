// --------------------------------------------------------------------------------------------------------------------
// <copyright file="huspellFileFormatType.cs" company="imbVeles" >
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
using imbSCI.Core.attributes;

namespace imbNLP.Data.basic.enums
{
    /// <summary>
    /// Tipovi file formata za huspell
    /// </summary>
    [imb(imbAttributeName.menuCommandTitle, "Huspell language files")]
    [imb(imbAttributeName.menuHelp, "Supported file formats for huspell")]
    [imb(imbAttributeName.menuIcon, "big51")]
    public enum huspellFileFormatType
    {
        [imb(imbAttributeName.menuCommandTitle, "Huspell AFFIx")]
        [imb(imbAttributeName.menuHelp, "Huspell AFFix file")]
        [imb(imbAttributeName.menuIcon, "big50")]
        [imb(imbAttributeName.fileFormatExtensions, "aff")]
        huspellAffix,

        [imb(imbAttributeName.menuCommandTitle, "Huspell Dict")]
        [imb(imbAttributeName.menuHelp, "Huspell Dict file")]
        [imb(imbAttributeName.menuIcon, "big50")]
        [imb(imbAttributeName.fileFormatExtensions, "dic")]
        huspellDict,
    }
}