
using imbSCI.Core.math;
using imbSCI.Core.reporting;
using imbSCI.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace imbNLP.Toolkit.Documents
{

    public static class WebSiteDocumentsSetTools
    {

        public const Int32 filenameMaxLength = 100;

        public static String GetSafeFilename(String input)
        {
            if (input.Length > filenameMaxLength)
            {
                input = input.Substring(0, filenameMaxLength) + md5.GetMd5Hash(input).Substring(0, 8);
            }
            return input;
        }

    }

    /// <summary>
    /// Selected set of WebSiteDocuments
    /// </summary>
    public class WebSiteDocumentsSet : List<WebSiteDocuments>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WebSiteDocumentsSet"/> class.
        /// </summary>
        public WebSiteDocumentsSet()
        {

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
        /// Removes the empty documents, and document sets with less pages than <c>minPageCount</c>
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="minPageCount">The minimum page count.</param>
        public void RemoveEmptyDocuments(ILogBuilder logger, Int32 minPageCount = 1)
        {
            logger.log(":: Removing empty documents from [" + name + "] dataset category");

            List<WebSiteDocuments> removeSiteList = new List<WebSiteDocuments>();

            foreach (WebSiteDocuments site in this)
            {
                List<WebSiteDocument> removeList = new List<WebSiteDocument>();

                foreach (WebSiteDocument page in site.documents)
                {
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
                    logger.log(":: [" + site.domain + "] pages [" + removeList.Count + "] removed");
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
                logger.log(":: [" + name + "] had [" + removeSiteList.Count + "] sites removed, as they had less than [" + minPageCount + "] pages.");
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