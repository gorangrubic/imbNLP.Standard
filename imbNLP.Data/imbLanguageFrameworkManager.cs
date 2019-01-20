// --------------------------------------------------------------------------------------------------------------------
// <copyright file="imbLanguageFrameworkManager.cs" company="imbVeles" >
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
namespace imbNLP.Data
{
    using imbACE.Core;
    using imbACE.Core.core;
    using imbNLP.Data.basic;
    using imbNLP.Data.config;
    using imbNLP.Data.extended.dict.core;
    using imbSCI.Core.extensions.table;
    using imbSCI.Core.extensions.text;
    using imbSCI.Data;
    using imbSCI.Data.collection.nested;
    using imbSCI.Data.enums;
    using imbSCI.DataComplex.extensions.data.formats;
    using System;
    using System.Data;
    using System.IO;
    using System.Threading.Tasks;

    /// <summary>
    /// Manager klasa treba da omoguci referenciranje i upravljanje statickim resursima
    /// </summary>
    public static class imbLanguageFrameworkManager
    {
        /// <summary>
        /// Returns a basic language object with loaded dictionary file
        /// </summary>
        /// <param name="languageID">The language identifier.</param>
        /// <returns></returns>
        public static basicLanguage GetBasicLanguage(basicLanguageEnum languageID)
        {
            if (languageID == basicLanguageEnum.unknown)
            {
                return null;
            }
            if (basicLanguageRegistry.ContainsKey(languageID))
            {
                basicLanguage output = basicLanguageRegistry[languageID];
                output.checkHuspell();
                return output;
            }
            return null;
        }

        internal static aceEnumDictionary<basicLanguageEnum, basicLanguage> basicLanguageRegistry { get; set; } = new aceEnumDictionary<basicLanguageEnum, basicLanguage>();

        private static builderForLog _log;

        /// <summary>
        /// Main log builder for the language framework
        /// </summary>
        public static builderForLog log
        {
            get
            {
                if (_log == null) _log = new builderForLog(logOutputSpecial.languageEngine);
                return _log;
            }
        }

        /// <summary>
        /// Explores the token.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <param name="metaobject">The metaobject.</param>
        /// <param name="sources">The sources.</param>
        /// <returns></returns>
        public static tokenQuery exploreToken(string token, object metaobject, tokenQuerySourceEnum sources)
        {
            log.log("Token [" + token + "] asks to be explored with [" + sources.ToStringEnumSmart() + "]"); //000 ToString())
            tokenQuery query = new tokenQuery(token, metaobject, sources);

            var srl = sources.getEnumListFromFlags<tokenQuerySourceEnum>();

            foreach (tokenQuerySourceEnum source in srl)
            {
                tokenQueryResponse response = null;
                switch (source)
                {
                    case tokenQuerySourceEnum.imb_alfabetTest:
                        response = languageManagerAlphabet.manager.exploreToken(query);
                        break;

                    case tokenQuerySourceEnum.hunspell:
                        response = languageManagerHunspell.manager.exploreToken(query);
                        break;

                    case tokenQuerySourceEnum.imb_namedentities:
                        response = languageManagerHunspell.manager.exploreToken(query);
                        break;

                    case tokenQuerySourceEnum.imb_elements:
                        response = languageManagerHunspell.manager.exploreToken(query);
                        break;

                    case tokenQuerySourceEnum.imb_dictionary:
                        response = languageManagerHunspell.manager.exploreToken(query);
                        break;

                    case tokenQuerySourceEnum.imb_lexicon:
                        response = languageManagerHunspell.manager.exploreToken(query);
                        break;

                    case tokenQuerySourceEnum.ext_wordnet:
                        response = languageManagerHunspell.manager.exploreToken(query);
                        break;

                    case tokenQuerySourceEnum.ext_unitex:
                        response = languageManagerHunspell.manager.exploreToken(query);
                        break;

                    case tokenQuerySourceEnum.ext_dict:
                        response = languageManagerHunspell.manager.exploreToken(query);
                        break;

                    case tokenQuerySourceEnum.imb_morphology:
                        break;
                }
                if (response.status == tokenQueryResultEnum.dismiss) break;
            }
            return query;
        }

        /// <summary>
        /// 2013C: Ovo je bitno da bude pozvano kako bi uspesno referencirao ovu Biblioteku!! -- > TREBA DA GA POZOVE manager.onApplicationReady()
        /// </summary>
        public static void Prepare()
        {
            aceLog.consoleControl.setAsOutput(log, "lang_mng");

            if (imbNLPDataConfig.settings.DoLoadBasicLanguageDefinitions)
            {

                String hunListPath = appManager.Application.folder_resources.findFile(imbNLPDataConfig.settings.BasicLanguageDefinitionsList, SearchOption.AllDirectories);
                DataTable dt = hunListPath.deserializeDataTable(imbSCI.Data.enums.reporting.dataTableExportEnum.excel);
                //dt.Rows.GetEnumerator
                Parallel.ForEach<DataRow>(dt.Rows.ToList(), (rw) =>
                {
                    basicLanguage bl = new basicLanguage();
                    bl.deploy(rw);

                    if (bl.languageEnum != basicLanguageEnum.unknown)
                    {
                        basicLanguageRegistry[bl.languageEnum] = bl;

                        log.log("Hunspell dictionary entry for [" + bl.languageEnum + "] found");
                    }
                    else
                    {
                        log.log("Hunspell dictionary entry failed [" + bl.languageEnum + "] found");
                    }
                });
            }


            //     basicLanguageRegistry[basicLanguageEnum.english].testBoolean("known", basicLanguageCheck.spellCheck);

            //foreach (DataRow dr in dt.Rows)
            //{
            //    basicLanguage bl = new basicLanguage();
            //    bl.deploy(dr);

            //    if (bl.languageEnum != basicLanguageEnum.unknown)
            //    {
            //        basicLanguageRegistry[bl.languageEnum] = bl;

            //            log.log("Hunspell dictionary entry for [" + bl.languageEnum + "] found");

            //    } else
            //    {
            //        log.log("Hunspell dictionary entry failed [" + bl.languageEnum + "] found");
            //    }
            //}

            //  languageManagerApertium.manager.prepare();

            //languageManagerAlphabet.manager.prepare();
            //  languageManagerDictionary.manager.prepare();
            //  languageManagerElements.manager.prepare();

            //languageManagerHunspell.manager.prepare();

            //  languageManagerLexicon.manager.prepare();
            // languageManagerMorphology.manager.prepare();

            //languageManagerUnitex.manager.prepare();

            //languageManagerWordnet.manager.prepare();
            // languageManagerDict.manager.prepare();

            //languageManagerDBNamedEntities.manager.prepare();

            //semanticLexicon.semanticLexiconManager.manager.prepare();

            aceLog.consoleControl.removeFromOutput(log);
        }
    }
}