// --------------------------------------------------------------------------------------------------------------------
// <copyright file="tokenization.cs" company="imbVeles" >
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
// Project: imbNLP.Core
// Author: Goran Grubic
// ------------------------------------------------------------------------------------------------------------------
// Project web site: http://blog.veles.rs
// GitHub: http://github.com/gorangrubic
// Mendeley profile: http://www.mendeley.com/profiles/goran-grubi2/
// ORCID ID: http://orcid.org/0000-0003-2673-9471
// Email: hardy@veles.rs
// </summary>
// ------------------------------------------------------------------------------------------------------------------
namespace imbNLP.Core.contentStructure.tokenizator
{
    #region imbVELES USING

    using System.Text.RegularExpressions;

    #endregion imbVELES USING

    public static class tokenization
    {
        //public static Regex numbersFormatedExpr = new Regex(@"\d[-\\/:.|_]", RegexOptions.Compiled);

        //  public static Regex letterExpr = new Regex(@"\A[a-zA-Z]", RegexOptions.Compiled);

        public const string sentenceEnd_normal = ".";
        public const string sentenceEnd_exclamation = "!";
        public const string sentenceEnd_question = "?";
        public const string sentenceEnd_arrowRight = ">";
        public const string sentenceEnd_arrowLeft = "<";
        public const string sentenceEnd_listStart = ":";
        public const string sentenceEnd_listStart2 = ".:";
        public const string sentenceEnd_listItemEnd = ";";
        public const string sentenceEnd_listItemEnd_listEnd = ";...";

        public const string sentenceEnd_notFinished = "...";

        public const string sentenceEnd_notFinished2 = "....";

        public const string sentenceInsert_Prefix = "Start. ";
        public const string sentenceInsert_Sufix = "End. ";

        /// <summary>
        /// Uzima elemente recenice> prvo slovo, sadrzaj i punktaciju kojom se zavrsava recenica
        /// </summary>
        public static Regex sentenceElements = new Regex(@"([A-Q]?)(.*)\w([\.!\?>:;]*)\Z");

        public static Regex blankLineSelector = new Regex(@"([\s]*\n+)");

        /// <summary>
        /// selektuje samo reci
        /// </summary>
        public static Regex wordSelectClean = new Regex(@"(?:[\w\-\+\\\*\~]+)", RegexOptions.Compiled);

        /// <summary>
        /// Selektuje rec ali i punkptaciju
        /// </summary>
        public static Regex wordSelect = new Regex("(?:[\\w\\.,\":\\-\\+\\\\\\*\\~]+)", RegexOptions.Compiled);

        public static Regex wordPunct = new Regex(@"[,.;:!?]", RegexOptions.Compiled);

        /// <summary>
        /// Selektuje sadrzaj koji je uokviren u zagradama - a same zagrade izostavlja u grupi 1
        /// </summary>
        public static Regex methodParameters = new Regex(@"\((.*[^()])\)");

        public static Regex sentenceSplit = new Regex(@"[.;!?]\s*[A-Z0-9]", RegexOptions.Compiled);

        public static Regex sentenceInner = new Regex(@"[.;!?](\s.*)[A-Z]", RegexOptions.Compiled);
        public static Regex punctOnly = new Regex(@"[^.;!?]", RegexOptions.Compiled);
        public static Regex punctAtEndOfString = new Regex(@"[.;!?]\Z", RegexOptions.Compiled);
        public static Regex tokenSelectForWords = new Regex(@"(?:[\w\.]+)", RegexOptions.Compiled);
        public static Regex tokenSelectForPunctation = new Regex(@"(?:[^\w\s\.])", RegexOptions.Compiled);

        public static Regex tokenSelectForWordsAndPunctation = new Regex(@"(?:[\w\.]+)|(?:[^\w\s\.])",
                                                                         RegexOptions.Compiled);

        public static Regex paragraphSplit = new Regex(@"\n", RegexOptions.Compiled);
        public static Regex whiteSpace = new Regex(@"\s", RegexOptions.Compiled);
        public static Regex cleanSentence = new Regex(@"(?:\w).*(\s)", RegexOptions.Compiled);
        public static Regex numericSelect = new Regex(@"[\d]+", RegexOptions.Compiled);
        public static Regex lettersSelect = new Regex(@"[^\W\d]+", RegexOptions.Compiled);
        public static Regex isNumericStart = new Regex(@"\A[\d]+", RegexOptions.Compiled);
        public static Regex isLetterStart = new Regex(@"\A[\w]+", RegexOptions.Compiled);

        public static Regex emailExpr =
            new Regex(@"^([0-9a-zA-Z]([-.\w]*[0-9a-zA-Z])*@([0-9a-zA-Z][-\w]*[0-9a-zA-Z]\.)+[a-zA-Z]{2,9})$",
                      RegexOptions.Compiled);

        public static Regex numbersCleanExpr = new Regex(@"\d", RegexOptions.Compiled);
        public static Regex numbersFormatedExpr = new Regex(@"\d[-\\/:.|_]", RegexOptions.Compiled);
        public static Regex firstLetterWord = new Regex(@"\A[a-zA-Z]+", RegexOptions.Compiled);
        public static Regex wordWithCapitalStart = new Regex(@"\A[A-Z][a-z]+", RegexOptions.Compiled);
        public static Regex selectPunctation = new Regex(@"[^\w\s]");
        public static Regex selectLettersOnly = new Regex(@"[^\W\d]");
        public static Regex numberOrdinal = new Regex(@"\A([\d]+\.)", RegexOptions.Compiled);
        public static Regex numberFormatSymbols = new Regex(@"[-\\/:.|_]", RegexOptions.Compiled);

        public static Regex selectCaseChanges = new Regex(@"(?:(?<=[a-z])(?=[A-Z]))|(?:(?<=[A-Z])(?=[a-z]))",
                                                          RegexOptions.Compiled);

        public static Regex selectLetterVsNumberChanges =
            new Regex(@"(?:(?<=[a-zA-Z])(?=[\d]))|(?:(?<=[\d])(?=[a-zA-Z]))", RegexOptions.Compiled);

        public static Regex selectLetterToOtherChanges =
            new Regex(@"(?:(?<=[a-zA-Z])(?=[^a-zA-Z\s]))|(?:(?<=[^a-zA-Z])(?=[a-zA\s]))", RegexOptions.Compiled);

        public static Regex samoSlovaITacke = new Regex(@"[^\W\d]+\.");
        public static Regex samoRec = new Regex(@"[\w]+");
    }
}