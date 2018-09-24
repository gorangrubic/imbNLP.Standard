// --------------------------------------------------------------------------------------------------------------------
// <copyright file="nlpTokenizator.cs" company="imbVeles" >
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
    using imbNLP.Core.contentStructure.interafaces;
    using imbNLP.Data.enums;

    /// <summary>
    /// Vrši GENERIC level analizu - sa analizu prema semantičkom modelu koristi se meaningEngine
    /// </summary>
    public static class nlpTokenizator
    {
        //public static void prepareContentToTokenize()

        /// <summary>
        /// Univerzalni poziv za tokenizaciju. U zavisnosti od tipa T odabrace najbolji tokenizator
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="content"></param>
        /// <param name="settings"></param>
        /// <param name="semantics"></param>
        /// <returns></returns>
        public static IContentPage tokenizeContent<T>(T content, nlpTokenizatorSettings settings)
        {
            tokenizatorBase tkn;
            IContentPage result = null;

            switch (settings.tknType)
            {
                //case tokenizationType.htmlTokenization:
                //    var htmlTkn = new htmlTokenizator(settings);
                //    return htmlTkn.tokenizeContent(content as HtmlDocument);
                //   // page.tokenizedContent = nlpTokenizator.tokenizeContent<XmlDocument>(page.xmlDocument, _crawlerAgentContext.AgentSettings.tknSettings);
                //    break;
                case tokenizationType.textTokenization:
                    var textTkn = new plainTextTokenizator(settings);

                    // var textTkn = new defaultTokenizator(settings);
                    return textTkn.tokenizeContent(content as string, settings.doBlockDetection);
                    //page.tokenizedContent = nlpTokenizator.tokenizeContent(page.textContent, _crawlerAgentContext.AgentSettings.tknSettings);
                    break;
            }

            return result;
        }
    }
}