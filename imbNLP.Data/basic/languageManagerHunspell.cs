// --------------------------------------------------------------------------------------------------------------------
// <copyright file="languageManagerHunspell.cs" company="imbVeles" >
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
namespace imbNLP.Data.basic
{
    using imbNLP.Data.extended.dict.core;
    using imbNLP.Data.semanticLexicon.explore;
    using System.IO;

    /// <summary>
    /// Token Query resolver for Hunspell dictionaries
    /// </summary>
    /// <seealso cref="imbNLP.Data.extended.dict.core.tokenQueryResolverBase" />
    public class languageManagerHunspell : tokenQueryResolverBase
    {
        /// <summary> </summary>
        public FileInfo resource { get; protected set; }

        private static languageManagerHunspell _manager;

        /// <summary>
        /// static and autoinitiated object
        /// </summary>
        public static languageManagerHunspell manager
        {
            get
            {
                if (_manager == null)
                {
                    _manager = new languageManagerHunspell();
                }
                return _manager;
            }
        }

        public override bool isReady
        {
            get
            {
                return (resource != null);
            }
        }

        /// <summary>
        /// Explores the token.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns></returns>
        public override tokenQueryResponse exploreToken(tokenQuery query)
        {
            tokenQueryResponse response = new tokenQueryResponse(query, tokenQuerySourceEnum.ext_dict);

            if (basicLanguageTools.testBoolean((basicLanguage)query.language.basic, query.token, basicLanguageCheck.spellCheck))
            {
                response.status = tokenQueryResultEnum.accept;
                //response.suggestions.AddVariation(basicLanguageTools.getSuggestions(query.language.basic, query.token));
                //response.tokenCase.root = basicLanguageTools.getRootBySuggests(response.suggestions);

                response.exploreModel = termExploreProcedures.exploreWithHunspell(new termExploreItem(query.token), query.loger);
            }
            else
            {
                response.status = tokenQueryResultEnum.unknown_token;
            }

            return response;
        }

        public override void prepare()
        {
            //imbLanguageFramework.imbLanguageFrameworkManager.languages.loadItems();

            //basicLanguage serbian = imbLanguageFrameworkManager.getLanguage("sr");
            //serbian.testBoolean("Provera", basicLanguageCheck.spellCheck);
            //openBase.
        }
    }
}