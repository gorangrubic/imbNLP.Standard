// --------------------------------------------------------------------------------------------------------------------
// <copyright file="multiLanguageEvaluation.cs" company="imbVeles" >
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
namespace imbNLP.Data.evaluate
{
    using imbSCI.Data.data;
    using imbSCI.DataComplex.special;
    using System.Collections.Generic;
    using System.Xml.Serialization;

    /// <summary>
    /// Results of multi language evaluation
    /// </summary>
    public class multiLanguageEvaluation : imbBindable
    {
        public multiLanguageEvaluation()
        {
        }

        public string comment { get; set; } = "";

        /// <summary>
        /// Language that was detected
        /// </summary>
        /// <value>
        /// The result language.
        /// </value>
        public basicLanguageEnum result_language { get; set; } = basicLanguageEnum.unknown;

        /// <summary>
        /// Language score / singleLanguageTokens count
        /// </summary>
        /// <value>
        /// The result ratio.
        /// </value>
        public double result_ratio { get; set; } = 0;

        /// <summary>
        /// Absolute score, without double matches - only words matched by single language is considered here
        /// </summary>
        /// <value>
        /// The language score.
        /// </value>
        [XmlIgnore]
        public instanceCountCollection<basicLanguageEnum> languageScore { get; set; } = new instanceCountCollection<basicLanguageEnum>();

        /// <summary>
        /// Gets or sets the language enums.
        /// </summary>
        /// <value>
        /// The language enums.
        /// </value>
        public List<basicLanguageEnum> languageEnums { get; set; } = new List<basicLanguageEnum>();

        public string languageScoreList { get; set; } = "";

        /// <summary>
        /// Dismissed tokens as they matched more than one language
        /// </summary>
        /// <value>
        /// The multi language tokens.
        /// </value>
        public List<string> multiLanguageTokens { get; set; } = new List<string>();

        /// <summary>
        /// Valid tokens used in the final score evaluation
        /// </summary>
        /// <value>
        /// The single language tokens.
        /// </value>
        public List<string> singleLanguageTokens { get; set; } = new List<string>();

        public List<string> noLanguageTokens { get; set; } = new List<string>();

        /// <summary>
        /// All tested tokens - valid or not
        /// </summary>
        /// <value>
        /// All tested tokens.
        /// </value>
        public List<string> allTestedTokens { get; set; } = new List<string>();

        /// <summary>
        /// All proper content tokens that were extracted from the page content
        /// </summary>
        /// <value>
        /// All content tokens.
        /// </value>
        public List<string> allContentTokens { get; set; } = new List<string>();

        public multiLanguageEvaluationTask task { get; set; }
    }
}