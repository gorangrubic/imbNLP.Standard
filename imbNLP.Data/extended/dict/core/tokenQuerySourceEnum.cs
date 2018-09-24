// --------------------------------------------------------------------------------------------------------------------
// <copyright file="tokenQuerySourceEnum.cs" company="imbVeles" >
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
namespace imbNLP.Data.extended.dict.core
{
    using System;

    [Flags]
    public enum tokenQuerySourceEnum
    {
        unknown = 0,
        hunspell = 1,
        imb_dictionary = 2,
        imb_elements = 4,
        imb_lexicon = 8,
        imb_morphology = 16,
        imb_alfabetTest = 32,
        imb_namedentities = 64,
        ext_wordnet = 128,
        ext_unitex = 256,
        ext_dict = 512,
        ext_apertium = 1024,
    }
}