using HtmlAgilityPack;
using imbSCI.Core.reporting;
using imbSCI.Data;
using System;
using System.Collections.Generic;

namespace imbNLP.Toolkit.Documents.GraphAnalysis
{




    public class WebSiteGraphBuilder
    {

        public WebSiteGraph ConstructGraph(WebSiteDocuments webSite, ILogBuilder logger)
        {
            String description = "";

            WebSiteGraph output = new WebSiteGraph(webSite.domain, "");

            Dictionary<String, WebSiteDocument> PagesByPagePath = new Dictionary<string, WebSiteDocument>();
            Dictionary<String, String> RecoveredPagePaths = new Dictionary<string, string>();

            Dictionary<WebSiteDocument, List<WebSiteLink>> InnerLinksByPages = new Dictionary<WebSiteDocument, List<WebSiteLink>>();

            foreach (WebSiteDocument page in webSite.documents)
            {
                String pagePath = "http://" + webSite.domain;

                if (RecoveredPagePaths.ContainsKey(page.path))
                {
                    pagePath = RecoveredPagePaths[page.path];
                }
                else
                {
                    pagePath = pagePath.add(page.path, "/");
                }

                PagesByPagePath.Add(pagePath, page);

                HtmlDocument htmlDoc = new HtmlDocument();

                htmlDoc.LoadHtml(page.HTMLSource);

                HtmlNodeCollection anchorNodes = htmlDoc.DocumentNode.SelectNodes("\\a");

                InnerLinksByPages.Add(page, new List<WebSiteLink>());

                foreach (HtmlNode node in anchorNodes)
                {
                    String href = node.GetAttributeValue("href", "");

                    WebSiteLink link = new WebSiteLink();
                    link.Deploy(webSite.domain, pagePath, href);

                    if (link.isInnerLink)
                    {
                        String safepath = WebSiteDocumentsSetTools.GetSafeFilename(link.fullUrl);
                        if (PagesByPagePath.ContainsKey(safepath))
                        {
                            if (!RecoveredPagePaths.ContainsKey(safepath)) RecoveredPagePaths.Add(safepath, link.fullUrl);
                        }

                        InnerLinksByPages[page].Add(link);
                    }

                }
            }

            foreach (WebSiteDocument page in webSite.documents)
            {
                String pagePath = "http://" + webSite.domain;
                if (RecoveredPagePaths.ContainsKey(page.path))
                {
                    pagePath = RecoveredPagePaths[page.path];
                }
                else
                {
                    pagePath = pagePath.add(page.path, "/");
                }
                output.AddNode(pagePath, 1, WebSiteGraph.NodeType_InternalVisited);
            }



            foreach (WebSiteDocument page in webSite.documents)
            {
                String pagePath = "http://" + webSite.domain;
                if (RecoveredPagePaths.ContainsKey(page.path))
                {
                    pagePath = RecoveredPagePaths[page.path];
                }
                else
                {
                    pagePath = pagePath.add(page.path, "/");
                }

                foreach (WebSiteLink link in InnerLinksByPages[page])
                {
                    if (!output.ContainsNode(link.fullUrl))
                    {
                        output.AddNode(link.fullUrl, 1, WebSiteGraph.NodeType_InternalUnvisited);
                    }

                    output.AddLink(pagePath, link.fullUrl, 1, 0);
                }
            }

            return output;

        }

    }

}