
using imbNLP.Toolkit.Documents.WebExtensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace imbNLP.Toolkit.Documents
{

    /// <summary>
    /// Single web site
    /// </summary>
    [Serializable]
    public class WebSiteDocuments
    {
        public WebSiteDocuments()
        {
        }

        public WebSiteDocuments(String _domain)
        {
            domain = _domain;
        }

        /// <summary>
        /// Gets or sets the extensions.
        /// </summary>
        /// <value>
        /// The extensions.
        /// </value>
        public WebSiteDocumentExtensions extensions { get; set; } = new WebSiteDocumentExtensions();

        public WebSiteDocuments WeakClone()
        {
            WebSiteDocuments output = new WebSiteDocuments(domain);
            output.documents.AddRange(documents);
            extensions.graph = extensions.graph;
            return output;
        }


        /// <summary>
        /// Gets or sets the domain.
        /// </summary>
        /// <value>
        /// The domain.
        /// </value>
        public String domain { get; set; } = "";

        /// <summary>
        /// Gets or sets the documents.
        /// </summary>
        /// <value>
        /// The documents.
        /// </value>
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