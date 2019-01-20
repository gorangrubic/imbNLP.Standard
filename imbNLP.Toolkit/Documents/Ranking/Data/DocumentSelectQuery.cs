using imbSCI.Core.reporting.render;
using System;

namespace imbNLP.Toolkit.Documents.Ranking.Data
{
    /// <summary>
    /// Defines one query instance, used for document ranking, score computation and selection
    /// </summary>
    public class DocumentSelectQuery
    {


        public void Describe(ITextRender output)
        {
            output.AppendHeading("Document selection query", 2);
            //  if (ApplyDomainLevelLimits) output.AppendLabel("Apply limit on domain level");
            output.AppendPair("Size limit", SizeLimit, true, "");
            output.AppendPair("Trashold limit", TrasholdLimit, true, "");
            output.AppendPair("Query Terms", QueryTerms, true, "");
        }

        /// <summary>
        /// New instance, used by xml serialization
        /// </summary>
        public DocumentSelectQuery()
        {

        }

        public String PrecompiledScoresFilename { get; set; } = "";

        /// <summary>
        /// If the result limit should be applied at level of web site and not for the complete set
        /// </summary>
        /// <value>
        ///   <c>true</c> if [apply domain level limits]; otherwise, <c>false</c>.
        /// </value>
      //  public Boolean ApplyDomainLevelLimits { get; set; } = true;


        public DocumentSelectQueryOptions options { get; set; } = DocumentSelectQueryOptions.ApplyDomainLevelLimits;

        public String QueryTerms { get; set; } = "";

        /// <summary>
        /// Target size of selected result set, 0 disables result size limit
        /// </summary>
        /// <value>
        /// The size of the result.
        /// </value>
        public Int32 SizeLimit { get; set; } = 0;

        /// <summary>
        /// Minimum score value that a document must have to be included in the result. If 0.0 - trashold criterion is disabled
        /// </summary>
        /// <value>
        /// The trashold limit.
        /// </value>
        public Double TrasholdLimit { get; set; } = 0.0;
    }
}
