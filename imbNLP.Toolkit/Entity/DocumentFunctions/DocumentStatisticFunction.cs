using imbSCI.Core.extensions.text;
using imbSCI.DataComplex.special;
using System;
using System.Collections.Generic;

namespace imbNLP.Toolkit.Entity.DocumentFunctions
{

    /// <summary>
    /// Document function based on textual token statistics in documents
    /// </summary>
    /// <seealso cref="imbNLP.Toolkit.Entity.DocumentFunctions.DocumentFunctionBase" />
    public abstract class DocumentStatisticFunction : DocumentFunctionBase
    {
        protected Dictionary<String, Dictionary<String, instanceCountCollection<String>>> stats = new Dictionary<string, Dictionary<string, instanceCountCollection<string>>>();

        public override void Learn(IEnumerable<TextDocumentSet> documentSets)
        {
            foreach (TextDocumentSet docSet in documentSets)
            {
                stats.Add(docSet.name, new Dictionary<String, instanceCountCollection<String>>());

                foreach (TextDocumentLayerCollection document in docSet)
                {
                    String content = document.ToString();

                    List<String> tkns = content.getTokens(true, true, true, false, 4);
                    instanceCountCollection<string> ft = new instanceCountCollection<string>();
                    ft.AddInstanceRange(tkns);

                    ft.reCalculate(instanceCountCollection<string>.preCalculateTasks.all);
                    stats[docSet.name].Add(document.name, ft);
                }
            }
        }
    }

}