using imbNLP.Toolkit.Core;
using imbNLP.Toolkit.Processing;
using imbNLP.Toolkit.Space;
using imbSCI.Core.reporting;
using imbSCI.Data;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace imbNLP.Toolkit.Weighting.Global
{
    /// <summary>
    /// 
    /// </summary>
    public enum FunctionResultTypeEnum
    {
        numeric,
        numericVectorForMultiClass,
    }

    /// <summary>
    /// Global function
    /// </summary>
    /// <seealso cref="imbNLP.Toolkit.Weighting.Global.IGlobalElement" />
    public abstract class GlobalElementBase : IDescribe, IGlobalElement
    {
        [XmlIgnore]
        public String shortName { get; set; } = "global";

        [XmlIgnore]
        public String description { get; set; } = "";

        public abstract void PrepareTheModel(SpaceModel space);

        public abstract Double GetElementFactor(string term, SpaceModel space, SpaceLabel label = null);

        public WeightDictionaryEntry GetElementFactorEntry(string term, SpaceModel space, SpaceLabel label = null)
        {
            WeightDictionaryEntry output = new WeightDictionaryEntry(term, 0);

            switch (resultType)
            {
                case FunctionResultTypeEnum.numeric:
                    output = new WeightDictionaryEntry(term, GetElementFactor(term, space, label));
                    break;
                case FunctionResultTypeEnum.numericVectorForMultiClass:
                    Double[] vec = new double[space.labels.Count];
                    Int32 c = 0;
                    foreach (SpaceLabel lb in space.labels)
                    {
                        vec[c] = GetElementFactor(term, space, lb);
                        c++;
                    }
                    output = new WeightDictionaryEntry(term, vec);
                    //                    output.AddEntry(term, vec);
                    break;
            }
            return output;

        }

        public Boolean IsEnabled { get; set; } = true;

        public virtual FunctionResultTypeEnum resultType { get { return FunctionResultTypeEnum.numeric; } }


        /// <summary>
        /// Builds dictionary of global element factors
        /// </summary>
        /// <param name="terms">The terms.</param>
        /// <param name="space">The space.</param>
        /// <param name="label">The label.</param>
        /// <returns></returns>
        public WeightDictionary GetElementFactors(IEnumerable<String> terms, SpaceModel space, SpaceLabel label = null)
        {
            WeightDictionary output = new WeightDictionary();


            switch (resultType)
            {
                case FunctionResultTypeEnum.numeric:
                    output.nDimensions = 1;
                    break;
                case FunctionResultTypeEnum.numericVectorForMultiClass:
                    output.nDimensions = space.labels.Count;
                    break;
            }


            foreach (String term in terms)
            {
                output.entries.Add(GetElementFactorEntry(term, space, label));


            }

            return output;
        }

        public virtual void Describe(ILogBuilder logger)
        {
            logger.AppendPair("Schema", shortName, true, "\t\t\t");
            if (!description.isNullOrEmpty())
            {
                logger.AppendComment(description);
            }
            String str_dataType = "Data type";
            switch (resultType)
            {
                case FunctionResultTypeEnum.numeric:
                    logger.AppendPair(str_dataType, "Rational number", true, "\t\t\t");
                    break;
                case FunctionResultTypeEnum.numericVectorForMultiClass:
                    logger.AppendPair(str_dataType, "n-dimensional vector", true, "\t\t\t");
                    logger.AppendComment("where n=|C|, C are classes");
                    break;
            }
        }
    }

}