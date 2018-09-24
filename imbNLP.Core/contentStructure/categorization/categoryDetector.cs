// --------------------------------------------------------------------------------------------------------------------
// <copyright file="categoryDetector.cs" company="imbVeles" >
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
namespace imbNLP.Core.contentStructure.categorization
{
    #region imbVELES USING

    using imbNLP.Core.contentStructure.interafaces;
    using imbNLP.Core.contentStructure.tokenizator;
    using imbNLP.Data.basic;
    using System.Text;

    #endregion imbVELES USING

    /// <summary>
    /// Koristi se za kategorizaciju elemenata tokenized kontenta - nakon tokenizacije
    /// </summary>
    public static class categoryDetector
    {
        /// <summary>
        /// Poziva detekciju generickih tipova za recenice, paragrafe i tokene. Odmrzava kolekcije u tokenizedContent
        /// </summary>
        /// <param name="tokenizedContent"></param>
        /// <param name="settings"></param>
        /// <param name="language"></param>
        /// <returns></returns>
        public static string detectGenericTypes(IContentPage tokenizedContent, nlpTokenizatorSettings settings,
                                                basicLanguage language)
        {
            if (settings == null) return "";
            if (tokenizedContent == null) return "";

            StringBuilder sb = new StringBuilder();
            //if (settings.doTokenTypeDetection_basic)
            //{
            //    tokenCategorization.tokenAnalysis(tokenizedContent, settings, language);
            //}

            if (settings.doSentenceDetection)
            {
                blokCategorization.sentenceAnalysis(tokenizedContent, settings, language);
            }

            if (settings.doParagraphDetection)
            {
                blokCategorization.paragraphAnalysis(tokenizedContent, settings, language);
            }

            if (settings.doBlockDetection)
            {
                blokCategorization.blockAnalysis(tokenizedContent, settings, language);
            }
            /*
            tokenizedContent.tokens.unfreeze();
            tokenizedContent.paragraphs.unfreeze();
            tokenizedContent.sentences.unfreeze();
            tokenizedContent.items.unfreeze();
            */
            return sb.ToString();
        }
    }
}