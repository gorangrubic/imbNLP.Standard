
using System;
using System.Collections.Generic;
using System.Linq;

namespace imbNLP.Toolkit.Documents
{

    /// <summary>
    /// Single web site
    /// </summary>
    public class WebSiteDocuments
    {
        public WebSiteDocuments()
        {
        }

        public WebSiteDocuments(String _domain)
        {
            domain = _domain;
        }

        public String domain { get; set; } = "";

        public List<WebSiteDocument> documents { get; set; } = new List<WebSiteDocument>();

        /// <summary>
        /// Gets the or add.
        /// </summary>
        /// <param name="fullUrl">The full URL.</param>
        /// <returns></returns>
        public WebSiteDocument GetOrAdd(String fullUrl)
        {
            Int32 p = fullUrl.IndexOf(domain);
            if (p > -1)
            {
                fullUrl = fullUrl.Substring(p + domain.Length);
            }
            //            fullUrl = fullUrl.removeStartsWith(domain);
            if (documents.Any(x => x.path == fullUrl))
            {
                return documents.First(x => x.path == fullUrl);
            }
            WebSiteDocument doc = new WebSiteDocument();
            doc.path = fullUrl;
            documents.Add(doc);
            return doc;
        }
    }
}