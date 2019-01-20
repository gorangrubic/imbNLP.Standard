using imbNLP.Toolkit.Documents;
using System;
using System.Collections.Generic;

namespace imbNLP.Toolkit.Entity.DocumentFunctions
{
    public interface IDocumentFunction
    {
        DocumentFunctionKernelType kernel { get; set; }

        Double Compute(TextDocumentLayerCollection document, String parentID);

        void Learn(IEnumerable<TextDocumentSet> documentSets);


    }

}
