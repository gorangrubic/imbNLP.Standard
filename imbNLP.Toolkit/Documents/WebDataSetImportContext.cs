using System;
using System.Collections.Generic;

namespace imbNLP.Toolkit.Documents
{
    public class WebDataSetImportContext : List<WebSiteDocumentsSet>
    {





        /// <summary>
        /// Full path to the root folder of the dataset
        /// </summary>
        /// <value>
        /// The data set path.
        /// </value>
        public String dataSetPath { get; set; } = "";

        //  public IEnumerable<WebSiteDocumentsSet> dataset { get; set; }

        public WebDataSetImportContext(String _dataSetPath, IEnumerable<WebSiteDocumentsSet> _dataset)
        {
            dataSetPath = _dataSetPath;

            foreach (WebSiteDocumentsSet ws in _dataset)
            {
                Add(ws);
            }

            //_dataset.ForEach
            //  dataset = _dataset;
        }


        public WebDataSetImportContext()
        {

        }
    }
}