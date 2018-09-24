// --------------------------------------------------------------------------------------------------------------------
// <copyright file="contentStructureImportExportTools.cs" company="imbVeles" >
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
namespace imbNLP.Core.contentStructure.display
{
    #region imbVELES USING

    using imbNLP.Core.contentStructure.interafaces;
    using imbSCI.Core.reporting.render;

    #endregion imbVELES USING

    public static class contentStructureImportExportTools
    {
        /// <summary>
        /// Generiše izveštaj o contentPage objektu - vraca String Builder objekat
        /// </summary>
        /// <param name="sb"></param>
        /// <returns></returns>
        public static ITextRender makeReport(this IContentElement element, ITextRender sb = null,
                                                  bool autoSaveAndOpen = true)
        {
            //if (sb == null)
            //{
            //    var hsb = new reportHtmlDocument("Report on: " + element.getTypeSignature(),
            //                                     reportOutputFormatName.htmlReport,
            //                                     reportFlag.filenameSmallLetters, reportFlag.filenameInsertTimestamp,
            //                                     reportFlag.headerInsertDescription,
            //                                     reportFlag.footerInsertOutputPath, reportFlag.footerInsertDocumentation,
            //                                     reportFlag.headerInsertTitle, reportFlag.footerInsertTimestamp,
            //                                     reportFlag.openOutput);

            //    hsb.description = "About content element: " + element.getTypology().displayDescription;
            //    sb = hsb;
            //    if (autoSaveAndOpen)
            //    {
            //        String path = hsb.saveOutput();
            //        run.startApplication(externalTool.firefox, path);
            //    }
            //}

            if (element is IContentPage)
            {
                IContentPage _pageElement = element as IContentPage;

                // _pageElement.items.makeReport(sb, 20);

                //_pageElement.paragraphs.makeReport(sb, 20);
                //_pageElement.sentences.makeReport(sb, 20);
                //_pageElement.tokens.makeReport(sb, 20);
                //_pageElement.chunks.makeReport(sb, 20);
            }

            return sb;
        }

        ///// <summary>
        ///// HTML grafički prikaz IContentElement objekata -- za lakši vizuelni pregled dobijenog XML rezultata
        ///// </summary>
        ///// <param name="element"></param>
        ///// <param name="report"></param>
        ///// <param name="autoSaveAndOpen"></param>
        ///// <returns></returns>
        //public static reportHtmlDocument exportStructureToHtml(this IContentElement element,
        //                                                       reportHtmlDocument report = null,
        //                                                       bool autoSaveAndOpen = true)
        //{
        //    if (report == null)
        //    {
        //        report = new reportHtmlDocument("htmlTokenization ", reportOutputFormatName.htmlReport,
        //                                        reportFlag.filenameSmallLetters, reportFlag.filenameInsertTimestamp,
        //                                        reportFlag.headerInsertDescription,
        //                                        reportFlag.footerInsertOutputPath, reportFlag.footerInsertDocumentation,
        //                                        reportFlag.headerInsertTitle, reportFlag.footerInsertTimestamp,
        //                                        reportFlag.openOutput);

        //        report.description = "Tokenization";
        //    }

        //    if (autoSaveAndOpen) report.saveOutput();
        //    return report;
        //}

        ///// <summary>
        ///// XML export dobijenog rezultaat
        ///// </summary>
        ///// <param name="element"></param>
        ///// <param name="xmlDoc"></param>
        ///// <param name="autoSaveAndOpen"></param>
        ///// <returns></returns>
        //public static reportXmlDocument exportStructureToXml(this IContentElement element,
        //                                                     reportXmlDocument xmlDoc = null,
        //                                                     Boolean autoSaveAndOpen = true)
        //{
        //    if (element == null) return xmlDoc;
        //    if (xmlDoc == null)
        //    {
        //        xmlDoc = new reportXmlDocument("tokenized_content", reportXmlFlag.insertXmlDeclaration);
        //    }

        //    xmlDoc = element.makeXml(xmlDoc);

        //    if (autoSaveAndOpen) xmlDoc.saveOutput();

        //    return xmlDoc;
        //}
    }
}