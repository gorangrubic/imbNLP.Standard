// --------------------------------------------------------------------------------------------------------------------
// <copyright file="preprocess.cs" company="imbVeles" >
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
namespace imbNLP.Core.contentPreprocess
{
    #region imbVELES USING

    using imbSCI.Data;
    using System.Text.RegularExpressions;

    #endregion imbVELES USING

    public static class preprocess
    {
        /// <summary>
        /// Regex select standardsFormatting : [A-Z]{1,5}[\s\-\:]*[\d]{2,5}[\d\:]*
        /// </summary>
        /// <remarks>
        /// <para>For text: example text</para>
        /// <para>Selects: ex</para>
        /// </remarks>
        public static Regex _select_standardsFormatting = new Regex(@"[A-Z]{1,5}[\s\-\:]*[\d]{2,5}[\d\:]*",
                                                                    RegexOptions.Compiled);

        /// <summary>
        /// Regex select titleAllCapital : [\""][\s]{0,2}([A-Z]|[a-z]{1,}[\s\-]{0,2}){1,6}[\s]{0,2}[\""]
        /// </summary>
        /// <remarks>
        /// <para>For text: " BIN METAL "</para>
        /// <para>Selects: " BIN METAL "</para>
        /// </remarks>
        public static Regex _select_titleAllCapital =
            new Regex(@"[\""][\s]{0,2}([A-Z\d]{1,}[\s\-]{0,2}){1,6}[\s]{0,2}[\""]", RegexOptions.Compiled);

        /// <summary>
        /// Regex select titleFirstCapital : [\""][\s]{0,2}([A-Z]|[a-z]{1,}[\s\-]{0,2}){1,6}[\s]{0,2}[\""]
        /// </summary>
        /// <remarks>
        /// <para>For text: example text</para>
        /// <para>Selects: ex</para>
        /// </remarks>
        /// Regex pattern = new Regex(@"[\""][\s]{0,2}([A-Z]|[a-z]{1,}[\s\-]{0,2}){1,6}[\s]{0,2}[\""]", RegexOptions.Multiline);
        public static Regex _select_titleFirstCapital =
            new Regex(@"[\""][\s]{0,2}([A-Z\d]|[a-z\d]{1,}[\s\-]{0,2}){1,6}[\s]{0,2}[\""]", RegexOptions.Compiled);

        public static Regex _select_acronimWithDots = new Regex(@"([\w]{1}\.{1}){2,}", RegexOptions.Compiled);

        public static Regex _select_acronimWithDotsAndSpaces = new Regex(@"([\w]{1}\.{1}\s{1}){2,}",
                                                                         RegexOptions.Compiled);

        /// <summary>
        /// Regex select yearOrdinalInGramarCase : [\d]{4}[-\s]{0,3}[a-z]{1,3}
        /// </summary>
        /// <remarks>
        /// <para>For text: izradili smo 1985te godine, osnovana 1991 - te godine</para>
        /// <para>Selects: 1985te, 1991 - te</para>
        /// </remarks>
        public static Regex _select_yearOrdinalInGramarCase = new Regex(@"[\d]{4}[-\s]{0,3}[a-z]{1,3}",
                                                                        RegexOptions.Compiled);

        /// <summary>
        /// Regex select yearDigitsOnly : ([\d]{4})
        /// </summary>
        /// <remarks>
        /// <para>For text: example text</para>
        /// <para>Selects: ex</para>
        /// </remarks>
        public static Regex _select_yearDigitsOnly = new Regex(@"([\d]{4})", RegexOptions.Compiled);

        /// <summary>
        /// Regex select enbraceAllTypes : [\(]([A-Za-z\d\w,;:\-\s\b]{2,})*[\)]|[\[]([A-Za-z\d\w,;:\-\s\b]{2,})*[\]]|[\{]([A-Za-z\d\w,;:\-\s\b]{2,})*[\}]|[\<]([A-Za-z\d\w,;:\-\s\b]{2,})*[\>]
        /// </summary>
        /// <remarks>
        /// Regex pattern = new Regex(@"[\(]([A-Za-z\d\w,;:\-\s\b]{2,})*[\)]|[\[]([A-Za-z\d\w,;:\-\s\b]{2,})*[\]]|[\{]([A-Za-z\d\w,;:\-\s\b]{2,})*[\}]|[\<]([A-Za-z\d\w,;:\-\s\b]{2,})*[\>]", RegexOptions.Multiline | RegexOptions.ExplicitCapture);
        /// <para>For text: example text</para>
        /// <para>Selects: ex</para>
        /// </remarks>
        public static Regex _select_enbraceAllTypes =
            new Regex(
                @"[\(]([A-Za-z\d\w,;:\-\s\b]{2,})*[\)]|[\[]([A-Za-z\d\w,;:\-\s\b]{2,})*[\]]|[\{]([A-Za-z\d\w,;:\-\s\b]{2,})*[\}]|[\<]([A-Za-z\d\w,;:\-\s\b]{2,})*[\>]",
                RegexOptions.Compiled);

        /// <summary>
        /// Match Evaluation for standardsFormatting : _select_standardsFormatting
        /// </summary>
        /// <param name="m">Match with value to process</param>
        /// <returns>For m.value "something" returns "SOMETHING"</returns>
        private static string _replace_standardsFormatting(Match m)
        {
            string output = m.Value.Replace("-", "");
            output = output.Replace(" ", "");

            return output.ToUpper();
        }

        /// <summary>
        /// Match Evaluation for titleAllCapital : _select_titleAllCapital
        /// </summary>
        /// <param name="m">Match with value to process</param>
        /// <returns>For m.value " BIN-METAL " returns "BIN METAL"</returns>
        private static string _replace_titleAllCapital(Match m)
        {
            string output = m.Value.Replace("\"", "");
            output = output.Replace("-", " ");
            output = output.Trim();

            output = output.ToUpper();
            output = output.ensureStartsWith("\"");
            output = output.ensureEndsWith("\"");
            return output;
        }

        /// <summary>
        /// Match Evaluation for enbraceAllTypes : _select_enbraceAllTypes
        /// </summary>
        /// <param name="m">Match with value to process</param>
        /// <returns>For m.value "something" returns "SOMETHING"</returns>
        private static string _replace_enbraceAllTypes(Match m)
        {
            if (m.Success)
            {
                string output = m.Value.Trim(" {[<()>]} ".ToCharArray());

                output = "(" + output + ")";

                //String output = m.Value.Replace(".", "");
                //output = output.Replace(" ", "");

                return output;
            }
            else
            {
                return "";
            }
        }

        /// <summary>
        /// Match Evaluation for yearOrdinalInGramarCase : _select_yearOrdinalInGramarCase
        /// </summary>
        /// <param name="m">Match with value to process</param>
        /// <returns>For m.value "1985te" returns "."</returns>
        private static string _replace_yearOrdinalInGramarCase(Match m)
        {
            string output = m.Value;

            var ms = _select_yearDigitsOnly.Match(m.Value);
            if (ms.Success)
            {
                output = ms.Value;
            }

            output = output.ensureEndsWith(".");

            return output;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        private static string _replace_acronimWithDots(Match m)
        {
            if (m.Success)
            {
                string output = m.Value.Replace(".", "");
                output = output.Replace(" ", "");

                return output.ToUpper();
            }
            return "";
        }

        public static string process(string content, contentPreprocessFlag _flags)
        {
            contentPreprocessFlag flags = _flags;
            var flist = _flags.getEnumListFromFlags();
            if (string.IsNullOrEmpty(content)) return "";

            string output = content;
            string tmp = "";
            foreach (contentPreprocessFlag _flag in flist)
            {
                switch (_flag)
                {
                    case contentPreprocessFlag.quoteStandardization:
                        output = output.Replace("„", "\"");
                        output = output.Replace(",,", "\"");
                        output = output.Replace("''", "\"");
                        output = output.Replace("``", "\"");
                        break;

                    case contentPreprocessFlag.acronimStandardization:
                        output = _select_acronimWithDots.Replace(output, _replace_acronimWithDots);
                        output = _select_acronimWithDotsAndSpaces.Replace(output, _replace_acronimWithDots);
                        break;

                    case contentPreprocessFlag.yearOrdinal:
                        output = _select_yearOrdinalInGramarCase.Replace(output, _replace_yearOrdinalInGramarCase);
                        break;

                    case contentPreprocessFlag.enbraceStandardize:
                        // output = _select_enbraceAllTypes.Replace(output, _replace_enbraceAllTypes);
                        break;

                    case contentPreprocessFlag.deentitize:
                        //output = output.imbHtmlDecode();
                        break;

                    case contentPreprocessFlag.internationalStandardsFormat:
                        output = _select_standardsFormatting.Replace(output, _replace_standardsFormatting);
                        break;
                }
            }

            /// drugi prolaz --
            foreach (contentPreprocessFlag _flag in flist)
            {
                // logSystem.log("Processing: " + _flag.ToString(), logType.Notification);
                switch (_flag)
                {
                    case contentPreprocessFlag.titleStandardize:
                        output = _select_titleAllCapital.Replace(output, _replace_titleAllCapital);
                        output = _select_titleFirstCapital.Replace(output, _replace_titleAllCapital);
                        break;
                }
            }

            return output;
        }
    }
}