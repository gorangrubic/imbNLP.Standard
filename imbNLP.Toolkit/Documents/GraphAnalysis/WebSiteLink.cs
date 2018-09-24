using imbSCI.Data;
using System;

namespace imbNLP.Toolkit.Documents.GraphAnalysis
{

    public class WebSiteLink
    {
        public WebSiteLink() { }

        public Boolean isInnerLink { get; set; } = false;

        public String fullUrl { get; set; } = "";

        public void Deploy(String domain, String hostPagePath, String href)
        {
            if (href.StartsWith("http"))
            {
                if (href.Contains(domain))
                {
                    isInnerLink = true;
                }
                else
                {
                    isInnerLink = false;
                }
                fullUrl = href;
            }
            else if (href.StartsWith("/"))
            {
                fullUrl = "http://" + domain + href;
            }
            else
            {
                if (hostPagePath.StartsWith("http"))
                {
                    fullUrl = hostPagePath.add(href, "/");
                }
                else
                {
                    fullUrl = "http://" + domain;
                    fullUrl = fullUrl.add(hostPagePath, "/");
                    fullUrl = fullUrl.add(href, "/");
                }
            }
        }

    }

}