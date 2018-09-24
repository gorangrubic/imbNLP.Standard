using System;
using System.Collections.Generic;

namespace imbNLP.Toolkit.Entity.DocumentFunctions
{
    public interface IDocumentFunction
    {
        DocumentFunctionKernelType kernel { get; set; }

        Double Compute(TextDocumentLayerCollection document, TextDocumentSet documentSet);

        void Learn(IEnumerable<TextDocumentSet> documentSets);


    }

}
