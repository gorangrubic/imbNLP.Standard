using imbNLP.Toolkit.Documents.Ranking.Data;
using imbNLP.Toolkit.Space;
using imbSCI.Core.extensions.text;
using imbSCI.Core.reporting;
using imbSCI.DataComplex.special;
using System;
using System.Collections.Generic;

namespace imbNLP.Toolkit.Documents.Ranking.Core
{
public static class ScoreModelEnumConversions
    {
        public static String ToStringCaption(this ScoreModelMetricFactorEnum functionName)
        {
            String score = "";

            switch (functionName)
            {
                case ScoreModelMetricFactorEnum.varianceFreq:
                    score = "VAR"; // entry_stats.varianceFreq;
                    break;
                case ScoreModelMetricFactorEnum.TotalScore:
                    score = "TOTAL"; // entry_stats.TotalScore;
                    break;
                case ScoreModelMetricFactorEnum.standardDeviation:
                    score = "STD"; // entry_stats.standardDeviation;
                    break;
                case ScoreModelMetricFactorEnum.entropyFreq:
                    score = "ENT"; // entry_stats.entropyFreq;
                    break;
                case ScoreModelMetricFactorEnum.avgFreq:
                    score = "AVG"; //entry_stats.avgFreq;
                    break;
                case ScoreModelMetricFactorEnum.Count:
                    score = "DST"; // entry_stats.Count;
                    break;
                case ScoreModelMetricFactorEnum.Ordinal:
                    score = "OFF"; // assignedIDs.Count - assignedIDs.IndexOf(entry.AssignedID);
                    break;

                default:
                    score = functionName.ToString();
                    break;
            }
            return score;
        }
    }
}