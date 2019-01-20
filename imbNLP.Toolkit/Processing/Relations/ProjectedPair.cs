using imbNLP.Toolkit.Documents.Ranking;
using imbNLP.Toolkit.Documents.Ranking.Data;
using imbNLP.Toolkit.Space;
using imbSCI.Core.reporting;
using System;
using System.Collections.Generic;

namespace imbNLP.Toolkit.Processing.Relations
{
    public static class ProjectionTools
    {

        public static TokenFrequencyAndScoreDictionary ProjectPrimaryTermsToScores(ProjectionDictionary projection, DocumentSelectResult scores, ILogBuilder logger)
        {
            var scoreByAssignedID = scores.GetByAssignedID(logger);


            TokenFrequencyAndScoreDictionary tokenFrequencyAndScoreDictionary = new TokenFrequencyAndScoreDictionary();

            foreach (var pair in projection)
            {
                DocumentSelectResultEntry entry = null; //drmContext.items.FirstOrDefault(x => x.AssignedID == pair.Key);

                if (scoreByAssignedID.ContainsKey(pair.Key))
                {
                    entry = scoreByAssignedID[pair.Key];
                }


                if (entry != null)
                {
                    Double score = entry.score;
                    tokenFrequencyAndScoreDictionary.Add(pair.Value.primary.terms, score);
                }
            }

            return tokenFrequencyAndScoreDictionary;
        }

    }

    public class ProjectionDictionary : Dictionary<String, ProjectionPair>
    {


    }

    public class ProjectionPair
    {

        public String AssignedID { get; set; } = "";

        public SpaceDocumentModel primary { get; set; }

        public SpaceDocumentModel secondary { get; set; }

    }


}
