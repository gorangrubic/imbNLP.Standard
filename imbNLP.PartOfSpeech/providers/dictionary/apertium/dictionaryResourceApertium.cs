using System;
using System.Collections.Generic;
using System.Linq;

namespace imbNLP.PartOfSpeech.providers.dictionary.apertium
{
    using imbACE.Core;
    using imbACE.Core.core;
    using imbNLP.PartOfSpeech.lexicUnit.tokenGraphs;
    using imbSCI.Core.files.folders;
    using imbSCI.Core.files.search;
    using imbSCI.Core.reporting;

    //using imbNLP.Data.extended.dict.core;
    //using imbNLP.Data.extended.tokenGraphs;
    using imbSCI.Data;
    using System.IO;
    using System.Text.RegularExpressions;
    using System.Xml;

    public abstract class dictionaryResourceBase
    {
        public Boolean isReady { get; protected set; } = false;
    }

    /// <summary>
    /// Stand-alone quering of an Apertium dictionary
    /// </summary>
    /// <seealso cref="imbNLP.Data.extended.apertium.dictionaryResourceBase" />
    public class dictionaryResourceApertium : dictionaryResourceBase
    {
        public const String FORMAT_EXACT_SRB = "\\<l\\>({0})\\<s";
        public const String FORMAT_START_SRB = "\\<l\\>({0})";
        public const String FORMAT_ANYWHERE = "({0})";
        public const String FORMAT_EXACT_ENG = "\\<r\\>({0})\\<s";
        public const String FORMAT_START_ENG = "\\<r\\>({0})";

        public dictionaryResourceSetup settings { get; set; } = new dictionaryResourceSetup();

        /// <summary>
        /// Makes the query.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <param name="scope">The scope.</param>
        /// <param name="side">The side.</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException">
        /// </exception>
        internal String makeQuery(String token, apertiumDictQueryScope scope = apertiumDictQueryScope.startingWith, apertiumDictNeedleSide side = apertiumDictNeedleSide.native)
        {
            String query = "";

            switch (scope)
            {
                case apertiumDictQueryScope.anywhere:
                    query = String.Format(FORMAT_ANYWHERE, token);
                    break;

                case apertiumDictQueryScope.exact:
                    if (side == apertiumDictNeedleSide.native)
                    {
                        query = String.Format(FORMAT_EXACT_SRB, token);
                    }
                    else if (side == apertiumDictNeedleSide.translated)
                    {
                        query = String.Format(FORMAT_EXACT_ENG, token);
                    }
                    else
                    {
                        throw new NotImplementedException();
                    }
                    break;

                case apertiumDictQueryScope.startingWith:
                    if (side == apertiumDictNeedleSide.native)
                    {
                        query = String.Format(FORMAT_START_SRB, token);
                    }
                    else if (side == apertiumDictNeedleSide.translated)
                    {
                        query = String.Format(FORMAT_START_ENG, token);
                    }
                    else
                    {
                        throw new NotImplementedException();
                    }
                    break;
            }
            return query;
        }

        /// <summary>
        /// Queries dictionary using [first with <c>token</c>, if not found, then with alternative tokens] scopes in order of confidence until it gets any translation pair. Order of scopes: <see cref="apertiumDictQueryScope.exact" />, <see cref="apertiumDictQueryScope.startingWith" />, <see cref="apertiumDictQueryScope.anywhere" />
        /// </summary>
        /// <param name="token">The token to translate, in language of the <c>side</c> specified</param>
        /// <param name="alternativeTokens">The alternative tokens, to try with if the <c>token</c> wasn't resolved, in language of the <c>side</c> specified</param>
        /// <param name="side">Side of the dictionary that the search <c>token</c> is from</param>
        /// <param name="includeAnywhere">if set to <c>true</c> it includes <see cref="apertiumDictQueryScope.anywhere"/> scope </param>
        /// <returns></returns>
        public apertiumDictionaryResult queryBestScope(String token, IEnumerable<String> alternativeTokens, apertiumDictNeedleSide side = apertiumDictNeedleSide.native, Boolean includeAnywhere = true)
        {
            var result = query(token, apertiumDictQueryScope.exact, side);

            if (!result.Any())
            {
                result = query(token, apertiumDictQueryScope.startingWith, side);
            }

            if (!result.Any() && includeAnywhere)
            {
                result = query(token, apertiumDictQueryScope.anywhere, side);
            }

            if (result.Any())
            {
                return result;
            }

            List<String> altTokens = alternativeTokens.ToList();

            altTokens.Sort((x, y) => y.Length.CompareTo(x.Length));

            if (!result.Any())
            {
                foreach (String infl in altTokens)
                {
                    result = query(infl, apertiumDictQueryScope.exact, apertiumDictNeedleSide.native);
                    if (result.Any()) break;
                }
            }

            if (!result.Any())
            {
                foreach (String infl in altTokens)
                {
                    result = query(infl, apertiumDictQueryScope.startingWith, apertiumDictNeedleSide.native);
                    if (result.Any()) break;
                }
            }

            if (!result.Any() && includeAnywhere)
            {
                foreach (String infl in altTokens)
                {
                    result = query(infl, apertiumDictQueryScope.anywhere, apertiumDictNeedleSide.native);
                    if (result.Any()) break;
                }
            }

            return result;
        }

        /// <summary>
        /// Queries dictionary using scopes in order of confidence until it gets any translation pair. Order of scopes: <see cref="apertiumDictQueryScope.exact"/>, <see cref="apertiumDictQueryScope.startingWith"/>, <see cref="apertiumDictQueryScope.anywhere"/>
        /// </summary>
        /// <param name="token">The token to translate, in language of the <c>side</c> specified</param>
        /// <param name="side">Side of the dictionary that the search <c>token</c> is from</param>
        ///<param name="includeAnywhere">if set to <c>true</c> it includes <see cref="apertiumDictQueryScope.anywhere"/> scope </param>
        /// <returns></returns>
        public apertiumDictionaryResult queryBestScope(String token, apertiumDictNeedleSide side = apertiumDictNeedleSide.native, Boolean includeAnywhere = true)
        {
            var result = query(token, apertiumDictQueryScope.exact, side);
            if (!result.Any())
            {
                result = query(token, apertiumDictQueryScope.startingWith, side);
            }

            if (!result.Any() && includeAnywhere)
            {
                result = query(token, apertiumDictQueryScope.anywhere, side);
            }

            return result;
        }

        /// <summary>
        /// Queries the dictionary for translation of <c>token</c>, into opposite <c>side</c>
        /// </summary>
        /// <param name="token">The token to translate, in language of the <c>side</c> specified</param>
        /// <param name="scope">The scope of search, the matching rule</param>
        /// <param name="side">Side of the dictionary that the search <c>token</c> is from</param>
        /// <returns></returns>
        public apertiumDictionaryResult query(String token, apertiumDictQueryScope scope = apertiumDictQueryScope.startingWith, apertiumDictNeedleSide side = apertiumDictNeedleSide.native)
        {
            apertiumDictionaryResult output = new apertiumDictionaryResult();
            String query = makeQuery(token, scope, side);

            fileTextSearchResult res = dictionaryOperator.Search(query, true, 50, RegexOptions.IgnoreCase);

            foreach (var pair in res)
            {
                output.addLine(pair.Value);
            }

            return output;
        }

        /// <summary>
        /// Queries the specified tokens.
        /// </summary>
        /// <param name="tokens">The tokens.</param>
        /// <param name="scope">The scope.</param>
        /// <param name="side">The side.</param>
        /// <returns></returns>
        public apertiumDictionaryResult query(IEnumerable<String> tokens, apertiumDictQueryScope scope = apertiumDictQueryScope.startingWith, apertiumDictNeedleSide side = apertiumDictNeedleSide.native)
        {
            apertiumDictionaryResult output = new apertiumDictionaryResult();
            List<String> queryList = new List<string>();
            foreach (String tkn in tokens)
            {
                if (!tkn.isNullOrEmpty())
                {
                    queryList.Add(makeQuery(tkn, scope, side));
                }
            }

            var res = dictionaryOperator.Search(queryList, true, RegexOptions.IgnoreCase);

            foreach (String line in res.getLines(true))
            {
                output.addLine(line);
            }

            return output;
        }

        /// <summary>
        /// Queries for synonyms.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <param name="scope">The scope.</param>
        /// <returns></returns>
        public apertiumDictionaryResult queryForSynonyms(String token, apertiumDictQueryScope scope = apertiumDictQueryScope.startingWith)
        {
            apertiumDictionaryResult output = new apertiumDictionaryResult();
            apertiumDictionaryResult firstStep = query(token, scope, apertiumDictNeedleSide.native);
            var engList = firstStep.GetTranslatedWords();

            output = query(engList, scope, apertiumDictNeedleSide.translated);
            return output;
        }

        /// <summary>
        /// Queries for graph.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <param name="scope">The scope.</param>
        /// <param name="side">The side.</param>
        /// <returns></returns>
        public tokenGraph queryForGraph(String token, apertiumDictQueryScope scope = apertiumDictQueryScope.startingWith, apertiumDictNeedleSide side = apertiumDictNeedleSide.native)
        {
            var queryRes = query(token, scope, side);
            tokenGraph output = new tokenGraph(token);
            if (side == apertiumDictNeedleSide.native)
            {
                output.AddKeyMatches(queryRes, tokenGraphNodeType.word_eng);
            }
            else
            {
                output.AddKeyMatches(queryRes, tokenGraphNodeType.word_srb);
            }
            return output;

            //return (tokenGraph)queryForGraph(new string[] { token }, scope, side);
        }

        /// <summary>
        /// Queries by graph leaf child nodes and populates grand children
        /// </summary>
        /// <param name="graph">The graph.</param>
        /// <param name="scope">The scope.</param>
        /// <param name="side">The side.</param>
        /// <returns></returns>
        public tokenGraphNode queryByGraphNode(tokenGraphNode graph, apertiumDictQueryScope scope = apertiumDictQueryScope.startingWith, apertiumDictNeedleSide side = apertiumDictNeedleSide.native)
        {
            List<tokenGraphNode> nodes = graph.getAllLeafs().getTyped<tokenGraphNode>();
            List<String> tokens = nodes.getNames();
            var queryRes = query(tokens, scope, side);

            foreach (tokenGraphNode node in nodes)
            {
                if (side == apertiumDictNeedleSide.native)
                {
                    node.AddKeyMatches(queryRes, tokenGraphNodeType.word_eng);
                }
                else
                {
                    node.AddValueMatches(queryRes, tokenGraphNodeType.word_srb);
                }
            }

            /*
            tokenGraphNodeType rType = tokenGraphNodeType.word_eng;
            tokenGraphNodeType qType = tokenGraphNodeType.word_srb;

            if (graph.type == tokenGraphNodeType.word_query)
            {
                if (side == apertiumDictNeedleSide.english)
                {
                    rType = tokenGraphNodeType.word_srb;
                    qType = tokenGraphNodeType.word_eng;
                }
            } else if (graph.type == tokenGraphNodeType.word_eng)
            {
                rType = tokenGraphNodeType.word_srb;
                qType = tokenGraphNodeType.word_eng;
            }

            graph.AddKeyValueChildren(queryRes, qType, rType, true);
            */
            return graph;
        }

        /// <summary>
        /// Queries for graph for multiple tokens
        /// </summary>
        /// <param name="tokens">The tokens.</param>
        /// <param name="scope">The scope.</param>
        /// <param name="side">The side.</param>
        /// <returns></returns>
        public tokenGraphSet queryForGraphSet(IEnumerable<String> tokens, apertiumDictQueryScope scope = apertiumDictQueryScope.startingWith, apertiumDictNeedleSide side = apertiumDictNeedleSide.native)
        {
            tokenGraphSet outset = new tokenGraphSet();

            var queryRes = query(tokens, scope, side);

            tokenGraphNodeType nType = tokenGraphNodeType.word_eng;

            if (side == apertiumDictNeedleSide.translated)
            {
                nType = tokenGraphNodeType.word_srb;
            }

            outset.Add(queryRes, nType);

            return outset;
        }

        /// <summary> </summary>
        protected fileTextOperater dictionaryOperator { get; set; }

        /// <summary>
        ///
        /// </summary>
        protected XmlDocument dictionaryXml { get; set; }

        /// <summary>
        /// The switch: if <c>false</c> it will use XmlDocument, if <token></token>
        /// </summary>
        public const Boolean SWITCH_TEXTvsXML_SEARCH = true;

        public ILogBuilder logger { get; set; }

        /// <summary>
        /// Prepares the instance to query the dictionary
        /// </summary>
        /// <param name="_settings">The settings.</param>
        /// <param name="_logger">The logger.</param>
        /// <param name="resourceFolder">The resource folder to search for, if not specified then <see cref="appManager.Application.folder_resources"/> is used.</param>
        /// <returns>True if ready for use</returns>
        public Boolean prepare(dictionaryResourceSetup _settings, ILogBuilder _logger = null, folderNode resourceFolder = null)
        {
            if (_logger != null)
            {
                logger = _logger;
            }
            else
            {
                logger = new builderForLog();
                imbSCI.Core.screenOutputControl.logToConsoleControl.setAsOutput(logger, "Apertium");
            }

            if (resourceFolder == null)
            {
                resourceFolder = appManager.Application.folder_resources;
            }

            settings = _settings;

            String filepath = resourceFolder.findFile(settings.fileNameSearchPattern, SearchOption.AllDirectories);

            if (filepath.isNullOrEmpty())
            {
                logger.log("Dictionary file: [" + settings.fileNameSearchPattern + "] not found");

                return false;
            }
            else
            {
                try
                {
                    if (settings.useTextInsteadOfXML)
                    {
                        dictionaryOperator = new fileTextOperater(filepath);
                    }
                    else
                    {
                        dictionaryXml = new XmlDocument();
                        dictionaryXml.LoadXml(filepath);
                    }
                    logger.log("Dictionary file: [" + filepath + "] found");
                    isReady = true;
                }
                catch (Exception ex)
                {
                    logger.log("Dictionary [" + filepath + "] load failed: " + ex.Message);
                    isReady = false;
                }
            }
            return isReady;
        }
    }
}