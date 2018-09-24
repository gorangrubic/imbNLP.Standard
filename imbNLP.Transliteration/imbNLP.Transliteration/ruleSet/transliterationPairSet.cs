// --------------------------------------------------------------------------------------------------------------------
// <copyright file="transliterationPairSet.cs" company="imbVeles" >
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
using System.Collections.Generic;
using System.Linq;

namespace imbNLP.Transliteration.ruleSet
{
    using System;
    using System.Text;
    using System.Text.RegularExpressions;

    /// <summary>
    /// Single alphabet-2-alphabet mapping
    /// </summary>
    [Serializable]
    public class transliterationPairSet
    {
        protected List<transliterationPairEntry> fromAtoB = new List<transliterationPairEntry>();
        protected List<transliterationPairEntry> fromBtoA = new List<transliterationPairEntry>();

        /// <summary>
        /// Indicating if the transliteration table is filled with definitions, i.e. loaded
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is initiated; otherwise, <c>false</c>.
        /// </value>
        public Boolean IsInitiated
        {
            get
            {
                if (fromAtoB.Any()) return true;
                return false;
            }
        }

        /// <summary>
        /// Gets or sets the language identifier for A side
        /// </summary>
        /// <value>
        /// The language A identifier.
        /// </value>
        public String lang_A_id { get; set; } = "";

        /// <summary>
        /// Gets or sets the language identifier for B side
        /// </summary>
        /// <value>
        /// The language B identifier.
        /// </value>
        public String lang_B_id { get; set; } = "";

        /// <summary>
        /// Gets or sets a value indicating whether pairs for uppercase letters are automatically built
        /// </summary>
        /// <value>
        ///   <c>true</c> if [automatic capital case]; otherwise, <c>false</c>.
        /// </value>
        public Boolean auto_capital_case { get; set; } = false;

        public String comment { get; set; } = "";

        /// <summary>
        /// Adds new definition pair
        /// </summary>
        /// <param name="A">a.</param>
        /// <param name="B">The b.</param>
        public void Add(String A, String B)
        {
            var entry = new transliterationPairEntry(A, B);
            fromAtoB.Add(entry);
            fromBtoA.Add(entry.GetInversed());
        }

        /// <summary>
        /// Converts from alphabet A to B
        /// </summary>
        /// <param name="input">The input text that contains letters in alphabet A</param>
        /// <returns>transliterated form</returns>
        public String ConvertFromAtoB(String input)
        {
            String output = input;
            if (input == null) return "";
            foreach (var en in fromAtoB)
            {
                output = output.Replace(en.A, en.B);
            }
            return output;
        }

        /// <summary>
        /// Converts from alphabet B to A
        /// </summary>
        /// <param name="input">The input text that contains letters in alphabet B</param>
        /// <returns>transliterated form</returns>
        public String ConvertFromBtoA(String input)
        {
            String output = input;
            if (input == null) return "";
            foreach (var en in fromBtoA)
            {
                output = output.Replace(en.A, en.B);
            }
            return output;
        }

        /// <summary>
        /// Saves the transliteration pair set into string form
        /// </summary>
        /// <returns></returns>
        public String SaveToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine(String.Format(transliteration.FORMAT_PARAMETER, nameof(lang_A_id), lang_A_id));
            sb.AppendLine(String.Format(transliteration.FORMAT_PARAMETER, nameof(lang_B_id), lang_B_id));
            sb.AppendLine(String.Format(transliteration.FORMAT_PARAMETER, nameof(auto_capital_case), auto_capital_case));
            sb.AppendLine(String.Format(transliteration.FORMAT_PARAMETER, nameof(comment), comment));

            for (int i = 0; i < fromAtoB.Count; i++)
            {
                sb.Append(fromAtoB[i].ConvertToEntry());
                if (i != fromAtoB.Count) sb.Append(transliteration.DEF_PAIR_SEPARATOR);
            }
            return sb.ToString();
        }

        /// <summary>
        /// Populates the pairs from string form
        /// </summary>
        /// <param name="input">The input.</param>
        public void LoadFromString(String input)
        {
            // loading parameters
            var par = transliteration.regex_paramSelector.Matches(input);
            foreach (Match m in par)
            {
                String[] parParts = m.Value.Split(new string[] { transliteration.DEF_PARAM_SEPARATOR }, StringSplitOptions.RemoveEmptyEntries);
                if (parParts.Length == 2)
                {
                    String partTwo = parParts[1].Trim(';', '"');
                    switch (parParts[0].ToLower())
                    {
                        case "lang_a_id":
                            lang_A_id = partTwo;
                            break;

                        case "lang_b_id":
                            lang_B_id = partTwo;
                            break;

                        case "auto_capital_case":
                            if (partTwo == "1") auto_capital_case = true;
                            if (partTwo.ToLower() == "true") auto_capital_case = true;
                            if (partTwo == "yes") auto_capital_case = true;
                            break;

                        case "comment":
                            comment = partTwo;
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(input), "Parameter name [" + parParts[0] + "] unknown, declaration [" + m.Value + "]");
                            break;
                    }
                }
                else
                {
                    throw new ArgumentOutOfRangeException(nameof(input), "Parameter declaration [" + m.Value + "] is not in proper format");
                }
            }

            // loading definitions

            var mch = transliteration.regex_pairSelector.Matches(input);
            foreach (Match m in mch)
            {
                transliterationPairEntry entry = new transliterationPairEntry();
                entry.ConvertFromEntry(m.Value);
                fromAtoB.Add(entry);
                fromBtoA.Add(entry.GetInversed());

                if (auto_capital_case)
                {
                    transliterationPairEntry entry_capital = new transliterationPairEntry(entry.A.ToUpper(), entry.B.ToUpper());
                    fromAtoB.Add(entry_capital);
                    fromBtoA.Add(entry_capital.GetInversed());
                }
            }

            auto_capital_case = false;
        }
    }
}