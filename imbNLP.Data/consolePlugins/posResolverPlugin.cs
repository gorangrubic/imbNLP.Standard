// --------------------------------------------------------------------------------------------------------------------
// <copyright file="posResolverPlugin.cs" company="imbVeles" >
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
using imbSCI.Data;
using System.Collections.Generic;
using System.ComponentModel;

namespace imbNLP.Data.consolePlugins
{
    using imbACE.Core;
    using imbACE.Core.operations;
    using imbACE.Services.console;
    using imbACE.Services.consolePlugins;
    using imbNLP.PartOfSpeech.lexicUnit;
    using imbNLP.PartOfSpeech.resourceProviders.core;
    using imbNLP.PartOfSpeech.resourceProviders.multitext;
    using imbSCI.Core.extensions.data;
    using imbSCI.Core.files.search;
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.IO;

    /// <summary>
    /// Plugin for imbACE console
    /// </summary>
    /// <seealso cref="imbACE.Services.consolePlugins.aceConsolePluginBase" />
    public class posResolverPlugin : aceConsolePluginBase
    {
        public posResolverPlugin(IAceAdvancedConsole __parent) : base(__parent, "Part-of-Speech resolver", "This plug-in resolves string token or tokens into POS information object")
        {
        }

        [Display(GroupName = "setup", Name = "Multitext", ShortName = "", Description = "Prepares the resolver to work with MULTITEXT v5 lexic resources, default values are set for Serbian language")]
        [aceMenuItem(aceMenuItemAttributeRole.ExpandedHelp, "It will load grammar tag converter specification and lock on designated MULTITEXT dictionary file")]
        /// <summary>Prepares the resolver to work with MULTITEXT v5 lexic resources</summary>
        /// <remarks><para>It will load grammar tag converter specification and lock on designated MULTITEXT lexic resource file</para></remarks>
        /// <param name="grammSpecFilename">Name of Excel file containing conversion specifications</param>
        /// <param name="multitextResource">Morphologic \\ lexic dictionary resource file annotated by MULTITEXT specification</param>
        /// <seealso cref="aceOperationSetExecutorBase"/>
        public void aceOperation_setupMultitext(
            [Description("Name of Excel file containing conversion specifications")] String grammSpecFilename = "sr_multitext_conversion.xlsx",
            [Description("Morphologic \\ lexic dictionary resource file annotated by MULTITEXT specification")] String multitextResource = "srLex_v1.2.mtx")
        {
            String specFilePath = appManager.Application.folder_resources.findFile(grammSpecFilename, SearchOption.AllDirectories);
            String resFilePath = appManager.Application.folder_resources.findFile(multitextResource, SearchOption.AllDirectories);

            if (specFilePath.isNullOrEmpty())
            {
                response.AppendLine("Gramm conversion specification file [ " + grammSpecFilename + "not found in resources");
                return;
            }

            if (resFilePath.isNullOrEmpty())
            {
                response.AppendLine("Resource file [ " + multitextResource + " ] not found in resources");
                return;
            }
            multitext = new multitextResourceParser(resFilePath, specFilePath, output);
        }

        [Display(GroupName = "query", Name = "Inflection", ShortName = "", Description = "Performs query over lexic resource to recover lexicInflection graph")]
        [aceMenuItem(aceMenuItemAttributeRole.ExpandedHelp, "What it will do?")]
        /// <summary>Performs query over lexic resource to recover lexicInflection graph</summary>
        /// <remarks><para>What it will do?</para></remarks>
        /// <param name="searchFor">Word or more words to query resource on</param>
        /// <param name="save">If set true it will save the result in the console workspace</param>
        /// <param name="show">If set true it will show all result graphs</param>
        /// <seealso cref="aceOperationSetExecutorBase"/>
        public List<lexicInflection> aceOperation_queryInflection(
            [Description("Word or more words to query resource on")] String searchFor = "proba",
            [Description("If set true it will save the result in the console workspace")] Boolean save = true,
            [Description("Output format to save results into")] lexicFormatType formatType = lexicFormatType.pathList,
            [Description("If set true it will show all result graphs")] Boolean show = true,
            [Description("Limit for entries to search in the resource")] Int32 searchLimit = -1
            )
        {
            List<lexicInflection> graphs = new List<lexicInflection>();
            List<String> tokens = searchFor.SplitSmart(",", "", true, true);

            var graphSet = multitext.GetLexicInflection(tokens, searchLimit, output, System.Text.RegularExpressions.RegexOptions.IgnoreCase);

            foreach (var tokenpair in graphSet)
            {
                response.AppendLine("Resolved token: _" + tokenpair.Key + "_");

                var graph = tokenpair.Value;

                if (show)
                {
                    var paths = graph.getPaths();

                    foreach (String p in paths)
                    {
                        output.AppendLine(p);
                    }
                }

                if (save)
                {
                }

                graphs.Add(graph);
            }

            return graphs;
        }

        protected multitextResourceParser multitext { get; set; }

        protected resourceConverterForGramaticTags grammTagConverter { get; set; } = new resourceConverterForGramaticTags();

        protected fileTextOperater resourceFileOperater { get; set; }

        protected Boolean isReadyForUse
        {
            get
            {
                return (resourceFileOperater != null);
            }
        }
    }
}