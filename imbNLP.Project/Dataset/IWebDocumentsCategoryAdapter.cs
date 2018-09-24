
using imbSCI.Core.files.folders;
using imbSCI.Core.reporting;
using imbSCI.Data.collection.graph;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace imbNLP.Project.Dataset
{
    /// <summary>
    /// Dataset repository loader and saver
    /// </summary>
    public interface IWebDocumentsCategoryAdapter
    {
        void SaveDataset(WebDocumentsCategory dataset, String path, WebDomainCategoryFormatOptions options, ILogBuilder logger = null);
        WebDocumentsCategory LoadDataset(String path, WebDomainCategoryFormatOptions options, ILogBuilder logger = null);
        String description { get; set; }
        Boolean Probe(String path, ILogBuilder logger = null);

    }
}