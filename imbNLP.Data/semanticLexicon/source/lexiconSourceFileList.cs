// --------------------------------------------------------------------------------------------------------------------
// <copyright file="lexiconSourceFileList.cs" company="imbVeles" >
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
namespace imbNLP.Data.semanticLexicon.source
{
    using imbACE.Core;
    using imbACE.Core.core;
    using imbSCI.Core.files.folders;
    using imbSCI.Core.files.search;
    using imbSCI.Core.reporting;
    using imbSCI.Data;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;

    public class lexiconSourceFileList : List<lexiconSourceFile>
    {
        /// <summary>
        /// Returns list of files missing
        /// </summary>
        /// <returns></returns>
        public List<string> checkMissingFiles()
        {
            List<string> output = new List<string>();
            foreach (lexiconSourceFile src in this)
            {
                if (!File.Exists(src.filepath))
                {
                    output.Add(src.filepath);
                }
            }
            return output;
        }

        /// <summary>
        /// Gets the file paths for selected source type. If <c>regexPattern</c> is specified, local path to the resource has to match the regex
        /// </summary>
        /// <param name="source">The source type to query.</param>
        /// <param name="regexPattern">The regex pattern to filter out available resources</param>
        /// <returns>List of paths</returns>
        public string getFilePath(lexiconSourceTypeEnum source, String regexPattern = "")
        {
            return getFilePaths(source, regexPattern).First();
        }

        /// <summary>
        /// Gets the file paths for selected source type. If <c>regexPattern</c> is specified, local path to the resource has to match the regex
        /// </summary>
        /// <param name="source">The source type to query.</param>
        /// <param name="regexPattern">The regex pattern to filter out available resources</param>
        /// <returns>List of paths</returns>
        public List<string> getFilePaths(lexiconSourceTypeEnum source, String regexPattern = "")
        {
            List<string> output = new List<string>();

            foreach (lexiconSourceFile item in this)
            {
                if (source.HasFlag(item.sourceType))
                {
                    Boolean ok = false;
                    if (regexPattern.isNullOrEmpty())
                    {
                        ok = true;
                    }
                    else
                    {
                        ok = Regex.IsMatch(item.filepath, regexPattern);
                    }
                    if (ok) output.Add(item.filepath);
                }
            }

            return output;
        }

        protected void findAndAdd(lexiconSourceTypeEnum type, string pattern, ILogBuilder logger = null)
        {
            folderNode folder = appManager.Application.folder_resources;
            var found = folder.findFiles(pattern);

            if (found.Any())
            {
                foreach (String f in found)
                {
                    String relPath = f.removeStartsWith(folder.path).Trim(Path.PathSeparator);
                    Add(type, relPath);
                    logger.log("Resource found [" + type.ToString() + "] at: " + relPath);
                }
            }
            else
            {
                logger.log("Resource _not found_ for [" + type.ToString() + "]");
            }
        }

        /// <summary>
        /// Sets the default paths to the lexic resources
        /// </summary>
        public void setDefaults()
        {
            Clear();

            aceLog.log("Default resource paths for Lexicon Source file list");

            findAndAdd(lexiconSourceTypeEnum.apertium, "apertium-*.dix", aceLog.loger);

            findAndAdd(lexiconSourceTypeEnum.serbianWordNet, "*wordnet*.csv", aceLog.loger);
            findAndAdd(lexiconSourceTypeEnum.englishWordNet, "*wordnet*.xlsx", aceLog.loger);
            findAndAdd(lexiconSourceTypeEnum.unitexDelaf, "unitex_delaf*.inf", aceLog.loger);
            findAndAdd(lexiconSourceTypeEnum.unitexDelas, "unitex_delacf*.inf", aceLog.loger);
            findAndAdd(lexiconSourceTypeEnum.unitexDelafBig, "unitex_delaf_??.dic", aceLog.loger);
            findAndAdd(lexiconSourceTypeEnum.unitexDelasBig, "unitex_delacf_??.dic", aceLog.loger);
            findAndAdd(lexiconSourceTypeEnum.unitexImmutableBig, "unitex_delaf_immutable.dic", aceLog.loger);

            findAndAdd(lexiconSourceTypeEnum.dictionary, "unitex_delaf_immutable.dic", aceLog.loger);

            findAndAdd(lexiconSourceTypeEnum.multitext, "*.mtx", aceLog.loger);

            //Add(lexiconSourceTypeEnum.apertium, "resources\\apertium_hbs.dix");
            //Add(lexiconSourceTypeEnum.serbianWordNet, "resources\\sr_wordnet.csv");
            //Add(lexiconSourceTypeEnum.englishWordNet, "resources\\wordnet_eng.xlsx");
            //Add(lexiconSourceTypeEnum.unitexDelaf, "resources\\unitex_delaf_sr.inf");
            //Add(lexiconSourceTypeEnum.unitexDelas, "resources\\unitex_delacf_sr.inf");
            //Add(lexiconSourceTypeEnum.unitexDelafBig, "resources\\unitex_delaf.dic");
            //Add(lexiconSourceTypeEnum.unitexDelasBig, "resources\\unitex_delacf.dic");
            //Add(lexiconSourceTypeEnum.unitexImmutableBig, "resources\\unitex_delaf_immutable.dic");
            // Add(lexiconSourceTypeEnum.dictionary, "resources\\recnik_source.csv");
            // Add(lexiconSourceTypeEnum.corpus, "resources\\corpus\\sm_corpus_input.csv");
            // Add(lexiconSourceTypeEnum.domainConcepts, "resources\\LexiconConcepts.xlsx");
        }

        /// <summary>
        /// Gets the file search operter instance
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns></returns>
        public fileTextOperater getOperater(lexiconSourceTypeEnum source)
        {
            lexiconSourceFile sourceFile = this.First(x => x.sourceType == source);
            fileTextOperater output = new fileTextOperater(sourceFile.filepath);

            return output;
        }

        //public DataTable getDataTable(lexiconSourceTypeEnum source)
        //{
        //    lexiconSourceFile sourceFile = this.First(x => x.sourceType == source);
        //    dataTableExportEnum format = sourceFile.filepath.getExportFormatByExtension();

        //    DataTable output = sourceFile.deserialize

        //}

        public void Add(lexiconSourceTypeEnum type, string filepath)
        {
            lexiconSourceFile sfile = new lexiconSourceFile();
            sfile.sourceType = type;
            sfile.filepath = filepath;
            Add(sfile);
        }

        public lexiconSourceFileList()
        {
        }
    }
}