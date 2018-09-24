using imbNLP.PartOfSpeech.analysis;
using imbNLP.PartOfSpeech.providers.dictionary.apertium;
using imbNLP.PartOfSpeech.TFModels.semanticCloud;
using imbSCI.Core.files.folders;
using imbSCI.Core.reporting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace imbNLP.PartOfSpeech.TFModels.semanticCloudWeaver
{
    /// <summary>
    /// Performs additional node linking, on already constructed <see cref="lemmaSemanticCloud"/>
    /// </summary>
    public class lemmaSemanticWeaver
    {
        private dictionaryResourceApertium _apertium = null;

        /// <summary>
        /// Settings for cloud waveing when <see cref="useSimilarity"/> is on
        /// </summary>
        /// <value>
        /// The similar words.
        /// </value>
        public wordSimilarityComponent similarWords { get; set; } = new wordSimilarityComponent();

        [XmlAttribute]
        public Boolean useSimilarity { get; set; } = false;

        public const Int32 LINK_OF_SIMILARWORDS = 10;
        public const Int32 LINK_OF_DICTIONARYSYNONIMS = 11;

        /// <summary>
        /// Processes the specified cloud.
        /// </summary>
        /// <param name="cloud">The cloud.</param>
        /// <param name="logger">The logger.</param>
        /// <returns></returns>
        public lemmaSemanticWeaverResult Process(lemmaSemanticCloud cloud, ILogBuilder logger)
        {
            Int32 c = cloud.links.Count;
            lemmaSemanticWeaverResult weaverResult = new lemmaSemanticWeaverResult(cloud);

            wordSimilarityResultSet output = new wordSimilarityResultSet();

            List<String> words = cloud.nodes.Select(x => x.name).ToList();

            if (useSimilarity)
            {
                output = similarWords.GetResult(words);
                foreach (var pair in output)
                {
                    var link = cloud.GetLink(pair.wordA, pair.wordB);
                    if (link == null)
                    {
                        cloud.AddLink(pair.wordA, pair.wordB, pair.score, LINK_OF_SIMILARWORDS);
                    }
                }
                weaverResult.linkRatioAfterWS = cloud.GetLinkPerNodeRatio();
                weaverResult.similarWords = output;
            }

            /* NOT WORKING ---- TEMPORARLY DISABLED
            if (useDictionary)
            {
                Stack<String> wordsToTest = new Stack<string>();
                words.ForEach(x => wordsToTest.Push(x));

                while (wordsToTest.Any())
                {
                    String word = wordsToTest.Pop();
                    apertiumDictionaryResult result = apertium.queryForSynonyms(word, apertiumDictQueryScope.exact);

                    var synonims = result.GetNativeWords();

                    var synonimNodes = wordsToTest.Where(x => synonims.Contains(x));

                    foreach (String syn in synonimNodes)
                    {
                        wordsToTest.RemoveAll(x=>x==syn);
                        cloud.AddLink(word, syn, 1, LINK_OF_DICTIONARYSYNONIMS);

                        weaverResult.appertiumNotes.Add(word + " -> " + syn);
                    }
                    weaverResult.linkRatioAfterDS = cloud.GetLinkPerNodeRatio();
                }
            }*/

            if (c != cloud.links.Count)
            {
                if (logger != null) logger.log("Weaver created [" + (cloud.links.Count - c) + "] new links in the cloud [" + cloud.className + "]");
            }

            return weaverResult;
        }

        private Object resourceLock = new Object();

        [XmlIgnore]
        public dictionaryResourceApertium apertium
        {
            get
            {
                if (_apertium == null)
                {
                    lock (resourceLock)
                    {
                        if (_apertium == null)
                        {
                            _apertium = new dictionaryResourceApertium();
                            _apertium.prepare(apertiumSettings);
                        }
                    }
                }
                return _apertium;
            }
            set { _apertium = value; }
        }

        /// <summary>
        /// Settings for <see cref="apertium"/>
        /// </summary>
        /// <value>
        /// The apertium settings.
        /// </value>
        public dictionaryResourceSetup apertiumSettings { get; set; } = new dictionaryResourceSetup();

        [XmlAttribute]
        public Boolean useDictionary { get; set; } = false;

        //  public appe

        public lemmaSemanticWeaver()
        {
        }

        /// <summary>
        /// Prepares apertium. It is not necessery to call this method, if the weaver is used within <see cref="imbACE.Core.application.IAceApplicationBase"/>
        /// </summary>
        /// <param name="resourceFolder">The resource folder.</param>
        /// <param name="logger">The logger.</param>
        public void prepare(folderNode resourceFolder, ILogBuilder logger)
        {
            _apertium = new dictionaryResourceApertium();
            if (_apertium.prepare(apertiumSettings, logger, resourceFolder))
            {
            }
            else
            {
            }
        }

        /// <summary>
        /// Returns description on the settings
        /// </summary>
        /// <returns></returns>
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("# Semantic weaver component");
            sb.AppendLine(" > Performs additional node linking, using word similarity and dictionary of synonims");

            if (useSimilarity)
            {
                sb.AppendLine(similarWords.ToString());
            }

            if (useDictionary)
            {
                sb.AppendLine(" > It will use bilingual dictionary to find synonims by callback quering of the dictionary");
            }

            return sb.ToString();
        }
    }
}