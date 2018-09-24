// --------------------------------------------------------------------------------------------------------------------
// <copyright file="transliterationPairEntry.cs" company="imbVeles" >
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

namespace imbNLP.Transliteration.ruleSet
{
    using System;

    /// <summary>
    /// Single entry of transliteration rules
    /// </summary>
    [Serializable]
    public class transliterationPairEntry
    {
        public transliterationPairEntry()
        {
        }

        public transliterationPairEntry(String _A, String _B)
        {
            A = _A;
            B = _B;
        }

        /// <summary>
        /// Letter or bi-graph that should be transliterated to <see cref="B"/>
        /// </summary>
        /// <value>
        /// a.
        /// </value>
        public String A { get; set; } = "";

        /// <summary>
        /// Letter or bi-graph <see cref="A"/> should be transliterated to.
        /// </summary>
        /// <value>
        /// The b.
        /// </value>
        public String B { get; set; } = "";

        /// <summary>
        /// Returns an inversed definition
        /// </summary>
        /// <returns></returns>
        public transliterationPairEntry GetInversed()
        {
            transliterationPairEntry tmp = new transliterationPairEntry();
            tmp.B = A;
            tmp.A = B;
            return tmp;
        }

        /// <summary>
        /// Converts from entry string definition
        /// </summary>
        /// <param name="defLine">The definition line.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// defLine - Definition line should contain only A:B, no spaces and no other symbols
        /// or
        /// defLine - Definition [" + defLine + "] is not in proper format
        /// </exception>
        public void ConvertFromEntry(String defLine)
        {
            if (defLine.Contains(" ") || defLine.Contains(transliteration.DEF_PAIR_SEPARATOR))
            {
                throw new ArgumentOutOfRangeException(nameof(defLine), "Definition line should contain only A:B, no spaces and no other symbols");
            }

            String[] members = defLine.Split(new string[] { transliteration.DEF_MEMBER_SEPARATOR }, StringSplitOptions.RemoveEmptyEntries);
            if (members.Length == 2)
            {
                A = members[0];
                B = members[1];
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(defLine), "Definition [" + defLine + "] is not in proper format");
            }
        }

        /// <summary>
        /// Converts to entry definition string
        /// </summary>
        /// <returns></returns>
        public String ConvertToEntry()
        {
            return String.Format(transliteration.FORMAT_PAIR, A, B);
        }
    }
}