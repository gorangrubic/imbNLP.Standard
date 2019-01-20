// --------------------------------------------------------------------------------------------------------------------
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
// Project:
// Author:
// ------------------------------------------------------------------------------------------------------------------
// Created with imbVeles / imbACE framework
// Project web site: http://blog.veles.rs
// GitHub: http://github.com/gorangrubic
// Mendeley profile: http://www.mendeley.com/profiles/goran-grubi2/
// ORCID ID: http://orcid.org/0000-0003-2673-9471
// Email: hardy@veles.rs
// </summary>
// ------------------------------------------------------------------------------------------------------------------
namespace imbNLP.Project.Plugin
{
    using imbACE.Core.operations;
    using imbACE.Services.console;
    using imbACE.Services.consolePlugins;
    using imbNLP.Project.Dataset;
    using System;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    /// <summary>
    /// Plugin for imbACE console - imbNLPProjectPlugin
    /// </summary>
    /// <seealso cref="imbACE.Services.consolePlugins.aceConsolePluginBase" />
    public class imbNLPProjectPlugin : aceConsolePluginBase
    {

        public WebDocumentsCategory WebCategories { get; set; } = null;

        public WebDomainCategory WebDomains { get; set; } = null;


        /// <summary>
        /// Reference to parent console, containing this plugin instance. This property must stay private.
        /// </summary>
        /// <value>
        /// The parent console.
        /// </value>
        private IAceAdvancedConsole parentConsole { get; set; }

        public imbNLPProjectPlugin(IAceAdvancedConsole __parent) : base(__parent, "imbNLPProjectPlugin", "This is imbACE advanced console plugin for imbNLPProjectPlugin")
        {
            parentConsole = __parent;
        }

        [Display(GroupName = "run", Name = "LoadWebKB", ShortName = "", Description = "Loads WebKB web site datasets")]
        [aceMenuItem(aceMenuItemAttributeRole.ExpandedHelp, "It will load WebKB 7Sectors dataset")]
        /// <summary>Loads WebKB web site datasets</summary>
        /// <remarks><para>It will load WebKB 7Sectors dataset</para></remarks>
        /// <param name="path">Path to 7Secotrs dataset</param>
        /// <seealso cref="aceOperationSetExecutorBase"/>
        public void aceOperation_runLoadWebKB(
            [Description("Path to 7Secotrs dataset")] String path = "G:\\_DOKTORAT_MAIN\\SM03_Datasets\\7sectors")
        {
            //WebKBDatasetAdapter webKBDatasetAdapter = new WebKBDatasetAdapter();
            //WebDocumentsCategory dataset = webKBDatasetAdapter.LoadDataset(path, parentConsole.response);

            //WebDomainCategory rootCategory = dataset.GetDomainCategory();
            //rootCategory.Save(parentConsole.workspace.folder.Add("7Sectors", "WebKB", "Domain category tree"));
        }

        [Display(GroupName = "run", Name = "HarvestODP", ShortName = "", Description = "Harvests the Open Directory Projects")]
        [aceMenuItem(aceMenuItemAttributeRole.ExpandedHelp, "It will scan for subdirectories and extract outer links")]
        /// <summary>Harvests the Open Directory Projects</summary>
        /// <remarks><para>It will scan for subdirectories and extract outer links</para></remarks>
        /// <param name="start">Starting URL</param>
        /// <param name="steps">How deep the harvester is allowed to go</param>
        /// <param name="debug">--</param>
        /// <seealso cref="aceOperationSetExecutorBase"/>
        public void aceOperation_runHarvestODP(
              [Description("Starting URL")] String start = "https://dmoztools.net/Business/",
              [Description("How deep the harvester is allowed to go")] Int32 steps = 3)
        {
            OpenDictionaryHarvester harvester = new OpenDictionaryHarvester(start);
            harvester.DepthLimit = steps;

            harvester.Start(start, parentConsole.response);

            harvester.result.Save(parentConsole.workspace.folder.Add("ODP", "Open Directory Project", "Harvested links").path);
        }

        // at this class you can add properties and nested classes your plugin implementation requires

        // use _imbAceOperationMethod snippet to add ACE Script operations available at this plugin
    }
}