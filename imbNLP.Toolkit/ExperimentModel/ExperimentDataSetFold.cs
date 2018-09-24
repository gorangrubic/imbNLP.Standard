using System;
using System.Linq;
using System.Collections.Generic;
using imbNLP.Toolkit.Documents;
using imbNLP.Toolkit.ExperimentModel.CrossValidation;
using imbNLP.Toolkit.Space;
using imbSCI.Core.reporting;

namespace imbNLP.Toolkit.ExperimentModel
{

    /// <summary>
    /// One experimental "fold" with training and testing documents.
    /// </summary>
    /// <seealso cref="System.Collections.Generic.List{imbNLP.Toolkit.Documents.WebSiteDocumentsSet}" />
    public class ExperimentDataSetFold : List<WebSiteDocumentsSet>
    {
        public ExperimentDataSetFold() { }

        /// <summary>
        /// Copies the label names.
        /// </summary>
        /// <param name="input">The input.</param>
        public void CopyLabelNames(IEnumerable<WebSiteDocumentsSet> input)
        {
            foreach (WebSiteDocumentsSet s in input)
            {
                WebSiteDocumentsSet tmp = new WebSiteDocumentsSet();
                tmp.name = s.name;
                tmp.description = s.description;
                Add(tmp);
            }
        }

        /// <summary>
        /// Identified of the fold
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string name { get; set; } = "";
    }

}