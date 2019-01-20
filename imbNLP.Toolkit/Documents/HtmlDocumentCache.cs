using HtmlAgilityPack;
using imbSCI.Core.data.cache;
using imbSCI.Core.data.systemWatch;
using System;
using System.Collections.Generic;

namespace imbNLP.Toolkit.Documents
{
    /// <summary>
    /// Cache registry, used to speed up creation of HTML DOM from the HTML source
    /// </summary>
    public class HtmlDocumentCache
    {

        public HtmlDocumentCache()
        {
            memoryWatch.Deploy();

        }

        protected MemoryWatch memoryWatch { get; set; } = new MemoryWatch();

        public CacheServiceProviderStats stats { get; set; } = new CacheServiceProviderStats();

        /// <summary>
        /// Default instance of document cache
        /// </summary>
        /// <value>
        /// The default document cache.
        /// </value>
        public static HtmlDocumentCache DefaultDocumentCache { get; set; } = new HtmlDocumentCache();


        protected Dictionary<String, HtmlDocument> DocumentRegistry { get; set; } = new Dictionary<string, HtmlDocument>();



        public void Dispose()
        {
            DocumentRegistry.Clear();

        }


        protected String currentDataSetID { get; set; } = "";

        /// <summary>
        /// Switches to dataset.
        /// </summary>
        /// <param name="datasetID">The dataset identifier.</param>
        public void SwitchToDataset(String datasetID)
        {
            if (currentDataSetID != datasetID)
            {
                Dispose();
                currentDataSetID = datasetID;
            }
        }


        private Object AddDictionaryLock = new Object();


        /// <summary>
        /// Gets or sets a value indicating whether this instance is enabled.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is enabled; otherwise, <c>false</c>.
        /// </value>
        public Boolean IsEnabled { get; set; } = true;

        /// <summary>
        /// Gets the document by the ID, if it's not already loaded into HTML DOM it will use the html source provided to load it and store into registry
        /// </summary>
        /// <param name="AssociatedID">The associated identifier.</param>
        /// <param name="html">The HTML.</param>
        /// <returns></returns>
        public HtmlDocument GetDocument(String AssociatedID, String html)
        {
            HtmlDocument output = null;

            stats.GetCalls++;

            MemoryWatchDirective memEval = memoryWatch.Evaluate();

            if (memEval == MemoryWatchDirective.flush)
            {
                IsEnabled = false;
                Dispose();
                IsEnabled = true;
            }


            if (IsEnabled && DocumentRegistry.ContainsKey(AssociatedID))
            {
                stats.GetFromMemory++;
                output = DocumentRegistry[AssociatedID];
            }
            else
            {
                output = new HtmlDocument();
                output.OptionOutputUpperCase = true;


                output.LoadHtml(html);

                stats.GetFromFile++;

                if (memEval == MemoryWatchDirective.normal)
                {

                    if (IsEnabled)
                    {
                        if (!DocumentRegistry.ContainsKey(AssociatedID))
                        {
                            lock (AddDictionaryLock)
                            {
                                if (!DocumentRegistry.ContainsKey(AssociatedID))
                                {
                                    stats.SetCalls++;
                                    DocumentRegistry.Add(AssociatedID, output);

                                }
                            }
                        }
                    }
                }
                else
                {
                    stats.SetPrevented++;
                }
            }

            return output;
        }


    }
}