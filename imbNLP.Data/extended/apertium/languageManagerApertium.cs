// --------------------------------------------------------------------------------------------------------------------
// <copyright file="languageManagerApertium.cs" company="imbVeles" >
//
// Copyright (C) 2018 imbVeles
//
// This program is free software: you can redistribute it and/or modify
// it under the +terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see http://www.gnu.org/licenses/.
// </copyright>
// <summary>
// Project: imbNLP.Data
// Author: Goran Grubic
// ------------------------------------------------------------------------------------------------------------------
// Project web site: http://blog.veles.rs
// GitHub: http://github.com/gorangrubic
// Mendeley profile: http://www.mendeley.com/profiles/goran-grubi2/
// ORCID ID: http://orcid.org/0000-0003-2673-9471
// Email: hardy@veles.rs
// </summary>
// ------------------------------------------------------------------------------------------------------------------
namespace imbNLP.Data.extended.apertium
{
    using imbNLP.Data.extended.dict.core;
    using imbNLP.PartOfSpeech.lexicUnit.tokenGraphs;
    using imbNLP.PartOfSpeech.providers.dictionary.apertium;
    using imbSCI.Core.files.search;
    using imbSCI.Data;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Xml;

    /// <summary>
    /// Manager for quering Apertium dictionary
    /// </summary>
    /// <seealso cref="tokenQueryResolverBase" />
    public class languageManagerApertium : tokenQueryResolverBase
    {
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
                    query = String.Format(dictionaryResourceApertium.FORMAT_ANYWHERE, token);
                    break;

                case apertiumDictQueryScope.exact:
                    if (side == apertiumDictNeedleSide.native)
                    {
                        query = String.Format(dictionaryResourceApertium.FORMAT_EXACT_SRB, token);
                    }
                    else if (side == apertiumDictNeedleSide.translated)
                    {
                        query = String.Format(dictionaryResourceApertium.FORMAT_EXACT_ENG, token);
                    }
                    else
                    {
                        throw new NotImplementedException();
                    }
                    break;

                case apertiumDictQueryScope.startingWith:
                    if (side == apertiumDictNeedleSide.native)
                    {
                        query = String.Format(dictionaryResourceApertium.FORMAT_START_SRB, token);
                    }
                    else if (side == apertiumDictNeedleSide.translated)
                    {
                        query = String.Format(dictionaryResourceApertium.FORMAT_START_ENG, token);
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
        /// Queries the specified token
        /// </summary>
        /// <param name="token">The token.</param>
        /// <param name="scope">The scope.</param>
        /// <param name="side">The side.</param>
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

        private fileTextOperater _dictionaryOperator;

        /// <summary> </summary>
        public fileTextOperater dictionaryOperator
        {
            get
            {
                return _dictionaryOperator;
            }
            protected set
            {
                _dictionaryOperator = value;
            }
        }

        private static languageManagerApertium _manager;

        /// <summary>
        /// Default manager for apertium dictionary
        /// </summary>
        public static languageManagerApertium manager
        {
            get
            {
                if (_manager == null)
                {
                    _manager = new languageManagerApertium();
                }
                return _manager;
            }
        }

        private XmlDocument _dictionaryXml;

        /// <summary>
        ///
        /// </summary>
        public XmlDocument dictionaryXml
        {
            get { return _dictionaryXml; }
            set { _dictionaryXml = value; }
        }

        /// <summary>
        /// The switch: if <c>false</c> it will use XmlDocument, if <token></token>
        /// </summary>
        public const Boolean SWITCH_TEXTvsXML_SEARCH = true;

        public override bool isReady
        {
            get
            {
                if (true)
                {
                    return (dictionaryOperator != null);
                }
                else
                {
                    return (dictionaryXml != null);
                }
            }
        }

        public override tokenQueryResponse exploreToken(tokenQuery query)
        {
            tokenQueryResponse response = new tokenQueryResponse(query, tokenQuerySourceEnum.ext_apertium);

            return response;
        }

        public override void prepare()
        {
            if (SWITCH_TEXTvsXML_SEARCH)
            {
                dictionaryOperator = semanticLexicon.semanticLexiconManager.manager.settings.sourceFiles.getOperater(semanticLexicon.source.lexiconSourceTypeEnum.apertium);
            }
            else
            {
                String filepath = semanticLexicon.semanticLexiconManager.manager.settings.sourceFiles.getFilePaths(semanticLexicon.source.lexiconSourceTypeEnum.apertium).First();
                dictionaryXml = new XmlDocument();
                dictionaryXml.LoadXml(filepath);
            }
            imbLanguageFrameworkManager.log.log("Apertium Serbian dictionary activated:" + isReady.ToString());
        }
    }
}