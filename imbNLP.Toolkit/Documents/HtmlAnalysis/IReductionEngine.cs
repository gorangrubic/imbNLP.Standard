using imbSCI.Core.reporting;
using System;
using System.Collections.Generic;

namespace imbNLP.Toolkit.Documents.HtmlAnalysis
{
    public interface IReductionEngine<T> where T : class
    {


        Double ReduceDataset(IEnumerable<WebSiteDocumentsSet> dataSet, T settings, ILogBuilder logger);
        Double ReduceDatasetCategory(WebSiteDocumentsSet dataSet, T settings, ILogBuilder logger);
        //  Double ReduceDocumentSet(WebSiteDocuments docSet, T settings, ILogBuilder logger);




    }
}