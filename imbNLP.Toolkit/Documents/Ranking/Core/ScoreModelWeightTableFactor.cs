using imbNLP.Toolkit.Documents.Ranking.Data;
using imbNLP.Toolkit.Processing;
using imbSCI.Core.extensions.text;
using imbSCI.Core.reporting;
using imbSCI.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace imbNLP.Toolkit.Documents.Ranking.Core
{


    /// <summary>
    /// Score factor based on a precompiled <see cref="WeightDictionary"/>
    /// </summary>
    /// <seealso cref="imbNLP.Toolkit.Documents.Ranking.Core.ScoreModelFactorBase" />
    public class ScoreModelWeightTableFactor : ScoreModelFactorBase
    {
        public ScoreModelWeightTableFactor()
        {

        }

        public override String GetSignature()
        {
            String output = "WT";

            if (!dictionaryFile.isNullOrEmpty())
            {
                output += "[" + dictionaryFile + "]";
            }

            output += GetWeightSignature();

            return output;
        }


        [XmlIgnore]
        public String _name { get; set; } = "";

        public override string name
        {
            get
            {
                if (_name.isNullOrEmpty())
                {
                    String output = dictionaryFile + weight.ToString("F2");
                    return output;
                }
                return _name;
            }
            set
            {

                _name = value;
            }
        }

        public String dictionaryFile { get; set; } = "";

        [XmlIgnore]
        public WeightDictionary weightDictionary { get; set; }

        [XmlIgnore]
        public List<String> queryTerms { get; set; } = new List<string>();

        public override ScoreModelRequirements CheckRequirements(ScoreModelRequirements requirements = null)
        {
            if (requirements == null) requirements = new ScoreModelRequirements();
            requirements.MayUseSelectedFeatures = true;
            requirements.MayUseTextRender = true;
            requirements.MayUseSpaceModel = true;
            return requirements;
        }

        /// <summary>
        /// Prepares the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="log">The log.</param>
        /// <exception cref="ArgumentException">context</exception>
        public override void Prepare(DocumentSelectResult context, ILogBuilder log)
        {
            /*
             String p_m = WeightDictionary.GetDictionaryFilename(, context.folder);

             if (File.Exists(p_m))
             {
                 //objectSerialization.loadObjectFromXML<WeightDictionary>(p_m, log);

             }
             */
            weightDictionary = WeightDictionary.LoadFile(WeightDictionary.GetDictionaryFilename(dictionaryFile, context.folder), log);

            if (context.spaceModel == null)
            {
                String msg = "Error: TermWeight factor requires SpaceModel declared in the context for operation";
                throw new ArgumentException(msg, nameof(context));
            }



            if (context.query.isNullOrEmpty())
            {
                context.query.QueryTerms = context.query.QueryTerms.Trim();

                List<String> tkns = context.query.QueryTerms.getTokens(true, true, true, false, 4);

                foreach (String tkn in tkns)
                {
                    queryTerms.Add(context.stemmingContext.Stem(tkn));
                }
            }


        }

        /// <summary>
        /// Computes score for given entry
        /// </summary>
        /// <param name="entry">The entry.</param>
        /// <param name="context">The context.</param>
        /// <param name="log">The log.</param>
        /// <returns></returns>
        public override double Score(DocumentSelectResultEntry entry, DocumentSelectResult context, ILogBuilder log)
        {
            Double output = 0;

            foreach (String term in entry.spaceDocument.terms.GetTokens())
            {
                Boolean isOk = true;
                if (context.selectedFeatures != null)
                {
                    if (context.selectedFeatures.Count > 0)
                    {
                        if (!context.selectedFeatures.ContainsKey(term))
                        {
                            isOk = false;
                        }
                    }
                }

                if (isOk)
                {
                    if (queryTerms.Any())
                    {
                        if (queryTerms.Contains(term))
                        {
                            output += weightDictionary.GetValue(term); // TermWeightModel.GetWeight(term, entry.spaceDocument, context.spaceModel);
                        }
                    }
                    else
                    {
                        output += weightDictionary.GetValue(term); // TermWeightModel.GetWeight(term, entry.spaceDocument, context.spaceModel);
                    }
                }
            }
            return output;
        }
    }
}