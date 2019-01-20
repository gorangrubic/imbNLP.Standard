using imbSCI.Core.reporting;
using imbSCI.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace imbNLP.Toolkit.Documents
{



    /// <summary>
    /// Selected set of WebSiteDocuments
    /// </summary>
    [Serializable]
    public class WebSiteDocumentsSet : List<WebSiteDocuments>
    {

        /// <summary>
        /// Counts total number of documents in the set
        /// </summary>
        /// <returns></returns>
        public Int32 CountDocumentsTotal()
        {
            Int32 output = 0;
            foreach (WebSiteDocuments site in this)
            {
                output += site.documents.Count;
            }
            return output;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WebSiteDocumentsSet"/> class.
        /// </summary>
        public WebSiteDocumentsSet()
        {

        }

        public WebSiteDocumentsSet WeakClone()
        {
            WebSiteDocumentsSet output = new WebSiteDocumentsSet();
            output.name = name;
            output.description = description;
            output.datasetSourceName = datasetSourceName;

            foreach (WebSiteDocuments site in this)
            {
                output.Add(site.WeakClone());
            }

            return output;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WebSiteDocumentsSet"/> class.
        /// </summary>
        /// <param name="_name">The name.</param>
        /// <param name="_desc">The desc.</param>
        public WebSiteDocumentsSet(String _name, String _desc)
        {
            name = _name;
            description = _desc;
        }


        /// <summary>
        /// Assigns default identifiers to documents
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="minPageCount">The minimum page count.</param>
        public void AssignID(ILogBuilder logger)
        {
            foreach (WebSiteDocuments site in this)
            {
                foreach (WebSiteDocument page in site.documents)
                {
                    page.AssignedID = WebSiteDocumentsSetTools.GetPageURL(page, site); //WebSiteDocumentsSetTools.GetUrlSignature(site.domain + page.path);
                }
            }
        }



        /// <summary>
        /// Removes the empty documents, and document sets with less pages than <c>minPageCount</c>
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="minPageCount">The minimum page count.</param>
        public void RemoveEmptyDocuments(ILogBuilder logger, Int32 minPageCount = 1, Int32 maxPageCount = -1)
        {
            if (logger != null) logger.log(":: Removing empty documents from [" + name + "] dataset category");

            List<WebSiteDocuments> removeSiteList = new List<WebSiteDocuments>();

            foreach (WebSiteDocuments site in this)
            {
                List<WebSiteDocument> removeList = new List<WebSiteDocument>();

                Int32 pc = 0;

                foreach (WebSiteDocument page in site.documents)
                {
                    if (maxPageCount > 0)
                    {
                        pc++;
                        if (pc > maxPageCount)
                        {
                            removeList.Add(page);
                        }
                    }

                    if (page.HTMLSource.isNullOrEmpty())
                    {
                        removeList.Add(page);
                    }
                }

                if (removeList.Any())
                {
                    foreach (var rem in removeList)
                    {
                        site.documents.Remove(rem);
                    }
                    if (logger != null) logger.log(":: [" + site.domain + "] pages [" + removeList.Count + "] removed");
                }

                if (site.documents.Count < minPageCount)
                {
                    removeSiteList.Add(site);
                }
            }

            if (removeSiteList.Any())
            {
                foreach (var rem in removeSiteList)
                {
                    this.Remove(rem);
                }
                if (logger != null) logger.log(":: [" + name + "] had [" + removeSiteList.Count + "] sites removed, as they had less than [" + minPageCount + "] pages.");
            }


        }

        /// <summary>
        /// Name of the set, also serves as category identification
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public String name { get; set; } = "";

        /// <summary>
        /// Optional description
        /// </summary>
        /// <value>
        /// The description.
        /// </value>
        public String description { get; set; } = "";

        /// <summary>
        /// Optional info on dataset source
        /// </summary>
        /// <value>
        /// The name of the dataset source.
        /// </value>
        public String datasetSourceName { get; set; }



    }

}