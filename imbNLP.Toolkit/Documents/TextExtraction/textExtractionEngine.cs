using System;
using System.Linq;
using System.Collections.Generic;
namespace imbNLP.Toolkit.Documents.TextExtraction
{
    using System.Text;
    using System.Xml;
    using System.Xml.XPath;


    /// <summary>
    /// Mehanizam za preuzimanje cistog teksta iz HTML-a
    /// </summary>
    public class textExtractionEngine
    {
        public string deploy(string value, textExtraction_structure mode, textExtractionSetup settings)
        {
            StringBuilder output = new StringBuilder();
            value = value.Trim();
            if (string.IsNullOrEmpty(value)) return "";
            switch (mode)
            {
                case textExtraction_structure.ignore:
                    break;

                case textExtraction_structure.newLine:
                    output.Append(Environment.NewLine);
                    output.Append(value);
                    output.Append(Environment.NewLine);
                    output.Append(Environment.NewLine);
                    break;

                case textExtraction_structure.normal:
                    output.Append(value);
                    output.Append(Environment.NewLine);
                    break;

                case textExtraction_structure.spaceInline:
                    output.Append(value + settings.inlineSpace);
                    break;
            }
            return output.ToString();
        }

        /// <summary>
        /// Proverava da li je prosledjeni node u saglasju sa podesavanjima
        /// </summary>
        /// <param name="source"></param>
        /// <param name="settings"></param>
        /// <returns></returns>
        internal bool checkNode(XPathNavigator source, textExtractionSetup settings)
        {
            switch (source.NodeType)
            {
                case XPathNodeType.Element:
                    string nn = source.Name.ToLower();
                    switch (nn)
                    {
                        case "script":
                            return settings.doExportScripts;
                            break;

                        case "title":
                            return settings.doExportTitle;
                            break;

                        case "style":
                            return settings.doExportStyles;

                            break;

                        default:
                            return true;
                            break;
                    }
                    break;

                case XPathNodeType.Comment:
                    return settings.doExportComments;
                    break;

                case XPathNodeType.Whitespace:
                case XPathNodeType.SignificantWhitespace:
                    return false;
                    break;
            }
            return false;
        }

        public textExtractionSetup checkSettings(textExtractionSetup settings)
        {
            if (settings == null)
            {
                var trs = new textExtractionSetup();

                //var tRecord = resources.getFirstOfType<modelSpiderTestRecord>
                // ILogBuilder pRecordLog = resources.getFirstOfType<ILogBuilder>(false, false, false);
                // crawledPage cpage = resources.getOfType<crawledPage>();

                trs.doExportScripts = false;
                trs.doExportComments = false;
                trs.doExportStyles = false;
                trs.doRetrieveChildren = false;
                trs.doHtmlCleanUp = true;
                trs.doCyrToLatTransliteration = true;
                return trs;
            }
            return settings;
        }

        public string retriveText(IXPathNavigable source, textExtractionSetup settings = null)
        {
            return retriveText(source.CreateNavigator(),settings);
        }

        /// <summary>
        /// 2014c> novi mehanizam za tekstualnu reprezentaciju ucitanog dokumenta
        /// </summary>
        /// <param name="source"></param>
        /// <param name="settings"></param>
        /// <returns></returns>
        public string retriveText(XPathNavigator source, textExtractionSetup settings = null)
        {
            StringBuilder output = new StringBuilder();
            if (source == null)
            {
                return "";
            }
            settings = checkSettings(settings);

            XPathNodeIterator itr = source.SelectDescendants(XPathNodeType.Text, true);
            while (itr.MoveNext())
            {
                switch (itr.Current.NodeType)
                {
                    case XPathNodeType.Text:
                        string inner = itr.Current.Value;
                        if (!string.IsNullOrEmpty(inner))
                        {
                            var subNav = itr.Current.CreateNavigator();

                            if (subNav.MoveToParent())
                            {
                                if (checkNode(subNav,settings))
                                    output.AppendLine(deploySpacing(inner, subNav, settings));
                            }
                            else
                            {
                                if (checkNode(subNav, settings)) output.AppendLine(inner);
                            }
                        }
                        break;

                    default:
                        break;
                }
            }
            string out2 = output.ToString();

            if (settings.doCompressNewLines)
            {
                string nnnl = Environment.NewLine + Environment.NewLine + Environment.NewLine + Environment.NewLine;
                string nnl = Environment.NewLine + Environment.NewLine + Environment.NewLine;
               // out2 = tokenization.blankLineSelector.Replace(out2, nnl);
                while (out2.Contains(nnnl))
                {
                    out2 = out2.Replace(nnnl, nnl);
                }
            }
            return out2;
        }

        /// <summary>
        /// Primenjuje podesavanja spejsinga - simulacija HTML strukture
        /// </summary>
        /// <param name="insert"></param>
        /// <param name="parentTag"></param>
        /// <param name="settings"></param>
        /// <returns></returns>
        internal string deploySpacing(string insert, XPathNavigator parentTag, textExtractionSetup settings)
        {
            string tag = parentTag.Name.ToLower();
            /*
            if (htmlDefinitions.HTMLTags_blockStructureTags.Contains(tag))
                return deploy(insert, settings.spanExtractMode, settings);
            if (htmlDefinitions.HTMLTags_headingTags.Contains(tag))
                return deploy(insert, settings.headingExtractMode, settings);
            if (htmlDefinitions.HTMLTags_tableItemTags.Contains(tag))
                return deploy(insert, settings.tdExtractMode, settings);
                */
            return insert;
        }

        /*

        /// <summary>
        /// 2013A> GLAVNI mehaniza za preuzimanje TXT reprezentacije XmlNoda *podržani i multilevel
        /// </summary>
        /// <param name="source"></param>
        /// <param name="settings"></param>
        /// <returns></returns>
        public string retriveText(XmlNode source, textExtractionSetup settings)
        {
            
            if (source == null)
            {
                return "";
            }

            StringBuilder output = new StringBuilder();
            string tmp = "";
            string tag = source.Name.ToLower();
            string tmpVal = "";
            switch (tag)
            {
                case "meta":
                    switch (settings.metaExtractMode)
                    {
                        case textExtraction_meta.full:

                            tmp = imbXmlCommonTools.getNodeDataSourceMulti(source, "@property =\"", true);
                            tmp += imbXmlCommonTools.getNodeDataSourceMulti(source, "@content @http-equiv", false);
                            tmp += imbXmlCommonTools.getNodeDataSourceMulti(source, "\" ", true) + Environment.NewLine;
                            output.Append(deploy(tmp, settings.metaSpaceExtractMode, settings));
                            break;

                        default:
                        case textExtraction_meta.onlyDescriptionAndKeywords:
                            tmpVal = imbXmlCommonTools.getNodeDataSourceMulti(source, "@property", false).ToLower();
                            switch (tmpVal)
                            {
                                case "keywords":
                                case "desc":
                                case "description":
                                    output.Append(
                                        deploy(
                                            imbXmlCommonTools.getNodeDataSourceMulti(source, "@property=@content", false),
                                            settings.metaSpaceExtractMode, settings));
                                    break;
                            }
                            break;

                        case textExtraction_meta.skip:
                            break;
                    }
                    break;

                case "title":
                    if (settings.doExportTitle)
                    {
                        output.Append(imbXmlCommonTools.getNodeDataSourceMulti(source, "innertext", false));
                    }
                    break;

                default:
                case "p":
                    output.Append(deploy(imbXmlCommonTools.getNodeDataSourceMulti(source, "innertext", false),
                                         settings.pExtractMode, settings));
                    break;

                case "h":
                case "h1":
                case "h2":
                case "h3":
                case "h4":
                case "h5":
                case "h6":
                    output.Append(deploy(imbXmlCommonTools.getNodeDataSourceMulti(source, "innertext", false),
                                         settings.headingExtractMode, settings));
                    break;

                case "td":
                    output.Append(deploy(imbXmlCommonTools.getNodeDataSourceMulti(source, "innertext", false),
                                         settings.tdExtractMode, settings));
                    break;

                case "tr":
                    output.Append(deploy(imbXmlCommonTools.getNodeDataSourceMulti(source, "innertext", false),
                                         settings.trExtractMode, settings));
                    break;

                case "div":
                    output.Append(deploy(imbXmlCommonTools.getNodeDataSourceMulti(source, "innertext", false),
                                         settings.divExtractMode, settings));
                    break;

                case "span":
                    output.Append(deploy(imbXmlCommonTools.getNodeDataSourceMulti(source, "innertext", false),
                                         settings.spanExtractMode, settings));
                    break;

                case "li":
                    output.Append(deploy(imbXmlCommonTools.getNodeDataSourceMulti(source, "innertext", false),
                                        settings.pExtractMode, settings));
                    break;

                case "a":
                    switch (settings.linksExtractMode)
                    {
                        case textExtraction_links.skip:
                            break;

                        default:
                        case textExtraction_links.title:
                            output.Append(deploy(imbXmlCommonTools.getNodeDataSourceMulti(source, "innertext", false),
                                                 settings.aExtractMode, settings));
                            break;

                        case textExtraction_links.titleAndUrl:
                            tmp = imbXmlCommonTools.getNodeDataSourceMulti(source, "innertext", false) +
                                  Environment.NewLine;
                            tmp += imbXmlCommonTools.getNodeDataSourceMulti(source, "@href", false) +
                                   Environment.NewLine;
                            output.Append(deploy(tmp, settings.aExtractMode, settings));
                            break;
                    }
                    break;

                case "script":
                    if (settings.doExportScripts)
                    {
                        output.Append(Environment.NewLine);
                        output.Append(imbXmlCommonTools.getNodeDataSourceMulti(source, "SCRIPT:@language @type @src",
                                                                               true));
                        output.Append(imbXmlCommonTools.getNodeDataSourceMulti(source, "innertext", false) +
                                      Environment.NewLine);
                        output.Append(Environment.NewLine);
                    }

                    break;

                case "#comment":
                    output.Append(deploy(imbXmlCommonTools.getNodeDataSourceMulti(source, "innertext", false),
                                         settings.commentExtractMode, settings));
                    break;
            }

            if (settings.doRetrieveChildren)
            {
                if (source.HasChildNodes)
                {
                    foreach (XmlNode ch in source.ChildNodes)
                    {
                        output.Append(retriveText(ch, settings));
                    }
                }
            }

            string str = output.ToString().Trim();
            if (settings.doHtmlCleanUp) str = str.htmlContentProcess();
            if (settings.doCyrToLatTransliteration) str = str.transliterate();

            // if (settings.insertNewLine) output.Append(Environment.NewLine + Environment.NewLine);

            return output.ToString();
        }
        */
    }

}