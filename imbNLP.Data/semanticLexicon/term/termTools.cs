// --------------------------------------------------------------------------------------------------------------------
// <copyright file="termTools.cs" company="imbVeles" >
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
namespace imbNLP.Data.semanticLexicon.term
{
    using imbNLP.Data.semanticLexicon.explore;
    using imbSCI.Core.extensions.text;
    using imbSCI.Core.reporting;
    using imbSCI.Data;
    using imbSCI.Data.enums.reporting;
    using imbSCI.DataComplex.extensions.data.formats;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;

    public static class termTools
    {
        public static Regex wordSplit = new Regex("([\\w]*)[\\s:\\.,]*", RegexOptions.Multiline);

        private static termExplorer _explorer;

        /// <summary>
        /// static and autoinitiated object
        /// </summary>
        public static termExplorer explorer
        {
            get
            {
                if (_explorer == null)
                {
                    _explorer = new termExplorer();
                }
                return _explorer;
            }
        }

        public static termSpark getExpandedSpark(this string token, int expansion = 1, ILogBuilder loger = null, bool debug = true, semanticLexiconManager manager = null)
        {
            if (manager == null) manager = semanticLexiconManager.manager;
            termSpark spark = null;

            if (expansion == 0)
            {
                spark = new termSpark(token);
                spark.weight = 1;
                return spark;
            }

            termGraph tg = new termGraph(token);
            string prefix = "lex";

            tg.expand(expansion);

            //if (debug)
            //{
            //    String tree = tg.ToStringTreeview();
            //    tree.saveStringToFile(manager.constructor.projectFolderStructure["logs"].pathFor("exp_" + expansion + "_" + token + ".txt"));
            //}

            spark = tg.getSpark();

            if (debug)
            {
                DataTable dt = spark.GetDataTable("exp_" + expansion + "_" + token, null, true);
                dt.serializeDataTable(dataTableExportEnum.csv, "", manager.constructor.projectFolderStructure["logs"]);
            }

            //if (tokens.Remove(spark.lemma);
            spark.weight = 1;

            return spark;
        }

        /// <summary>
        /// Transforms series of tokens into wparks
        /// </summary>
        /// <param name="tokens">The tokens.</param>
        /// <param name="expansion">The expansion.</param>
        /// <param name="loger">The loger.</param>
        /// <returns></returns>
        public static List<termSpark> getSparks(this List<string> tokens, int expansion = 1, ILogBuilder loger = null, bool debug = true)
        {
            List<string> output = new List<string>();
            List<termSpark> sparks = new List<termSpark>();
            if (!tokens.Any()) return sparks;
            StringBuilder sb = new StringBuilder();
            string qt = "start";
            int tc = tokens.Count();
            int i = 0;
            int ci = 0;
            int cl = tc / 10;

            while (!qt.isNullOrEmpty())
            {
                i++;
                ci++;
                if (tokens.Any())
                {
                    qt = tokens.First();
                    tokens.Remove(qt);
                }
                else
                {
                    qt = null;
                    break;
                }

                termSpark spark = getExpandedSpark(qt, expansion, loger, debug);

                foreach (var it in spark.terms)
                {
                    if (tokens.Remove(it.Key))
                    {
                        spark.AFreqPoints++;
                        spark.weight = spark.weight + it.Value.weight;
                    }
                }

                if (loger != null)
                {
                    sb.Append("[" + qt + "] " + tokens.Count().imbGetPercentage(tc, 2));

                    if (ci > cl)
                    {
                        ci = 0;
                        loger.Append(sb.ToString());
                        sb.Clear();
                    }
                }

                if (spark.lemma != null)
                {
                    sparks.Add(spark);
                }
            }
            return sparks;
        }
    }
}