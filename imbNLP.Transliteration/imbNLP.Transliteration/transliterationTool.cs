// --------------------------------------------------------------------------------------------------------------------
// <copyright file="transliterationTool.cs" company="imbVeles" >
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
// Project: imbNLP.Transliteration
// Author: Goran Grubic
// ------------------------------------------------------------------------------------------------------------------
// Project web site: http://blog.veles.rs
// GitHub: http://github.com/gorangrubic
// Mendeley profile: http://www.mendeley.com/profiles/goran-grubi2/
// ORCID ID: http://orcid.org/0000-0003-2673-9471
// Email: hardy@veles.rs
// </summary>
// ------------------------------------------------------------------------------------------------------------------
using imbNLP.Transliteration.ruleSet;
using System;

namespace imbNLP.Transliteration
{
    /// <summary>
    /// Extension methods for end use
    /// </summary>
    public static class transliterationTool
    {
        /// <summary>
        /// Transliterates the <c>inputString</c> according to specified language identifier for the source.
        /// </summary>
        /// <remarks></remarks>
        /// <param name="inputString">The input string, to be transliterated</param>
        /// <param name="id_of_source">The identifier of the source string</param>
        /// <param name="inverse">Should the transliteration be in opposite direction?</param>
        /// <returns>Transliterated text</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">id_of_source - Transliteration definition not found for [" + id_of_source + "]</exception>
        public static String transliterate(this String inputString, String id_of_source = "sr_cyr", Boolean inverse = false)
        {
            String output = inputString;

            transliterationPairSet pairSet = ruleSet.transliteration.GetTransliterationPairSet(id_of_source);

            if (pairSet == null)
            {
                throw new ArgumentOutOfRangeException(nameof(id_of_source), "Transliteration definition not found for [" + id_of_source + "]");
            }
            if (inverse)
            {
                output = pairSet.ConvertFromBtoA(inputString);
            }
            else
            {
                output = pairSet.ConvertFromAtoB(inputString);
            }
            return output;
        }
    }
}