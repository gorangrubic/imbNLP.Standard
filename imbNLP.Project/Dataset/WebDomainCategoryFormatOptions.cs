using imbACE.Network.tools;
using imbSCI.Core;
using imbSCI.Core.extensions.io;
using imbSCI.Core.files.folders;
using imbSCI.Data;
using imbSCI.Data.collection.graph;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace imbNLP.Project.Dataset
{

    [Flags]
    public enum WebDomainCategoryFormatOptions
    {
        none = 0,
        saveAggregate = 1,
        saveReadmeFile = 2,
        saveGraphAtRoot = 4,
        normalizeDomainname = 8,
        saveDomainList = 16,
        /// <summary>
        /// The lazy loading - when loading <see cref="WebDocumentsCategory"/>, it will just index files, the content will be loaded once requested
        /// </summary>
        lazyLoading = 32
    }

}