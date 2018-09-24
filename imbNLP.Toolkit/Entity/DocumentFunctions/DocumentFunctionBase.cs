using System;
using System.Collections.Generic;

namespace imbNLP.Toolkit.Entity.DocumentFunctions
{
    public abstract class DocumentFunctionBase : IDocumentFunction
    {
        public DocumentFunctionKernelType kernel { get; set; } = DocumentFunctionKernelType.singleCycle;

        public abstract Double Compute(TextDocumentLayerCollection document, TextDocumentSet documentSet);

        public abstract void Learn(IEnumerable<TextDocumentSet> documentSets);

    }

}