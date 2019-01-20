using imbNLP.Toolkit.Documents.Ranking.Data;
using imbNLP.Toolkit.Space;
using imbSCI.Core.extensions.data;
using imbSCI.Core.extensions.text;
using imbSCI.Core.reporting;
using imbSCI.DataComplex.special;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace imbNLP.Toolkit.Documents.Ranking.Core
{

    //public enum ScoreModelMetricFactorEnum
    //{
    //    varianceFreq,
    //    TotalScore,
    //    standardDeviation,
    //    entropyFreq,
    //    avgFreq,
    //    Count
    //}

    /// <summary>
    /// Factor based on <see cref="instanceCountCollection{T}"/> class, counting textual tokens
    /// </summary>
    /// <seealso cref="imbNLP.Toolkit.Documents.Ranking.Core.ScoreModelFactorBase" />
    public class ScoreModelMetricFactor : ScoreModelFactorBase
    {

        public override String GetSignature()
        {
            String output = functionName.ToStringCaption();  //functionName.ToString().Replace(", ", "_");

          
            output += GetWeightSignature();

            return output;
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="ScoreModelMetricFactor"/> class.
        /// </summary>
        public ScoreModelMetricFactor()
        {

        }

        /// <summary>
        /// Gets or sets the name of the function.
        /// </summary>
        /// <value>
        /// The name of the function.
        /// </value>
        public ScoreModelMetricFactorEnum functionName { get; set; } = ScoreModelMetricFactorEnum.varianceFreq;

        /// <summary>
        /// If true it will base the result on stems, and not on tokens 
        /// </summary>
        /// <value>
        ///   <c>true</c> if [use stems]; otherwise, <c>false</c>.
        /// </value>
        public Boolean useStems { get; set; } = true;


        /// <summary>
        /// The stats by assigned identifier
        /// </summary>
        protected Dictionary<String, instanceCountCollection<String>> statsByAssignedID = new Dictionary<string, instanceCountCollection<string>>();


        protected List<String> assignedIDs { get; set; } = new List<string>();

        [XmlIgnore]
        public String _name { get; set; } = "";

        public override string name
        {
            get
            {
                if (_name.isNullOrEmpty())
                {
                    String output = functionName + weight.ToString("F2");
                    return output;
                }
                return _name;
            }
            set
            {

                _name = value;
            }
        }



        public override ScoreModelRequirements CheckRequirements(ScoreModelRequirements requirements = null)
        {
            if (requirements == null) requirements = new ScoreModelRequirements();
            requirements.MayUseTextRender = true;
            return requirements;
        }

        /// <summary>
        /// Scores the specified entry.
        /// </summary>
        /// <param name="entry">The entry.</param>
        /// <param name="context">The context.</param>
        /// <param name="log">The log.</param>
        /// <returns></returns>
        public override Double Score(DocumentSelectResultEntry entry, DocumentSelectResult context, ILogBuilder log)
        {


            var entry_stats = statsByAssignedID[entry.AssignedID];
            entry_stats.reCalculate(instanceCountCollection<string>.preCalculateTasks.all);

            Double score = 0;

            switch (functionName)
            {
                case ScoreModelMetricFactorEnum.varianceFreq:
                    score = entry_stats.varianceFreq;
                    break;
                case ScoreModelMetricFactorEnum.TotalScore:
                    score = entry_stats.TotalScore;
                    break;
                case ScoreModelMetricFactorEnum.standardDeviation:
                    score = entry_stats.standardDeviation;
                    break;
                case ScoreModelMetricFactorEnum.entropyFreq:
                    score = entry_stats.entropyFreq;
                    break;
                case ScoreModelMetricFactorEnum.avgFreq:
                    score = entry_stats.avgFreq;
                    break;
                case ScoreModelMetricFactorEnum.Count:
                    score = entry_stats.Count;
                    break;
                case ScoreModelMetricFactorEnum.Ordinal:
                    score = assignedIDs.Count - assignedIDs.IndexOf(entry.AssignedID);
                    break;

                default:
                    score = entry_stats.Count;
                    break;
            }


            return score;

        }


        /// <summary>
        /// Prepares the factor by processing the context
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="log">The log.</param>
        public override void Prepare(DocumentSelectResult context, ILogBuilder log)
        {

            statsByAssignedID.Clear();


            foreach (DocumentSelectResultEntry docEntry in context.items)
            {

                instanceCountCollection<string> ft = new instanceCountCollection<string>();

                if (docEntry.type.HasFlag(DocumentSelectEntryType.spaceDocument))
                {
                    SpaceDocumentModel document = docEntry.spaceDocument;
                    foreach (var term in document.terms.GetTokens())
                    {
                        ft.AddInstance(term, document.terms.GetTokenFrequency(term));
                    }
                }
                else if (docEntry.type.HasFlag(DocumentSelectEntryType.textDocument))
                {

                    String content = docEntry.textDocument.content; // document.ToString();

                    List<String> tkns = content.getTokens(true, true, true, false, 4);

                    foreach (String tkn in tkns)
                    {
                        String stem = tkn;
                        if (useStems) stem = context.stemmingContext.Stem(tkn);
                        ft.AddInstance(stem);
                    }

                }
                statsByAssignedID.Add(docEntry.AssignedID, ft);

                assignedIDs.Add(docEntry.AssignedID);

            }

        }


    }
}