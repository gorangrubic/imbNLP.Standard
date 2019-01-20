using imbNLP.Toolkit.Documents;
using imbNLP.Toolkit.Documents.Ranking;
using imbNLP.Toolkit.Documents.Ranking.Core;
using imbNLP.Toolkit.Entity.DocumentFunctions;
using imbNLP.Toolkit.Planes.Core;
using imbNLP.Toolkit.Processing;
using imbNLP.Toolkit.Space;
using imbNLP.Toolkit.Vectors;
using imbNLP.Toolkit.Weighting;
using imbSCI.Core.reporting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace imbNLP.Toolkit.Entity
{

    /// <summary>
    /// OVO MORA DA SE REFORMIŠE
    /// </summary>
    /// <seealso cref="imbNLP.Toolkit.Planes.Core.PlaneMethodFunctionBase" />
    /// <seealso cref="imbNLP.Toolkit.Entity.IEntityPlaneFunction" />
    public class DocumentFilterFunction : PlaneMethodFunctionBase, IEntityPlaneFunction, IHasProceduralRequirements
    {
        private Boolean _isEnabled = true;

        public void Learn(IEnumerable<TextDocumentSet> documentSets)
        {

            function.Learn(documentSets);
        }


        /// <summary>
        /// Queries factors for preprocessing requirements
        /// </summary>
        /// <param name="requirements">The requirements.</param>
        /// <returns></returns>
        public ScoreModelRequirements CheckRequirements(ScoreModelRequirements requirements = null)
        {

            if (requirements == null) requirements = new ScoreModelRequirements();

            switch (function.kernel)
            {
                case DocumentFunctionKernelType.iterative:
                    break;
                case DocumentFunctionKernelType.singleCycle:
                    break;
            }

            requirements.MayUseTextRender = true;

            //render.CheckRequirements(requirements);



            //model.Deploy();
            //model.CheckRequirements(requirements);


            return requirements;
        }

        /// <summary>
        /// Filters the document set by creation descending rank, scored by <see cref="function"/> and taking first <see cref="limit"/> web pages
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns></returns>
        public TextDocumentSet FilterDocumentSet(TextDocumentSet input)
        {
            Int32 iterations = 1;
            TextDocumentSet output = new TextDocumentSet(input.name);
            output.AddRange(input);

            if (function.kernel == DocumentFunctionKernelType.iterative)
            {
                iterations = limit;
                output.Clear();
            }

            for (int itc = 0; itc < iterations; itc++)
            {

                Dictionary<TextDocumentLayerCollection, Double> docVsScore = new Dictionary<TextDocumentLayerCollection, double>();

                foreach (TextDocumentLayerCollection textDocument in input)
                {
                    docVsScore.Add(textDocument, function.Compute(textDocument, input.name));
                }

                List<KeyValuePair<TextDocumentLayerCollection, double>> sorted = docVsScore.OrderByDescending(x => x.Value).ToList();


                if (function.kernel == DocumentFunctionKernelType.singleCycle)
                {
                    if (sorted.Count > limit)
                    {
                        output.Clear();
                        Int32 c = 0;
                        foreach (var p in sorted)
                        {
                            output.Add(p.Key);
                            c++;
                            if (c >= limit) break;
                        }
                    }
                }
                else
                {
                    var p = sorted.First();
                    output.Add(p.Key);
                    input.Remove(p.Key);
                }
            }



            return output;
        }

        public override void Describe(ILogBuilder logger)
        {
            if (IsEnabled)
            {
                logger.AppendLine("Document filter enabled");
                logger.AppendPair("Ranking function", function.GetType().Name, true, "\t\t\t");
                logger.AppendPair("Select top", limit, true, "\t\t\t");
            }
            else
            {
                logger.AppendLine("Document filter disabled");
            }
        }

        public DocumentFilterFunction()
        {

        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is enabled.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is enabled; otherwise, <c>false</c>.
        /// </value>
        public Boolean IsEnabled
        {
            get
            {

                if (function == null) return false;
                if (limit == 0) return false;

                return _isEnabled;
            }
            set { _isEnabled = value; }
        }

        public Int32 limit { get; set; } = 5;

        public IDocumentFunction function { get; set; }

    }

}