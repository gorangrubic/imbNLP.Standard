// --------------------------------------------------------------------------------------------------------------------
// <copyright file="pipelineModelConsolePlugin.cs" company="imbVeles" >
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
// Project: imbNLP.PartOfSpeech
// Author: Goran Grubic
// ------------------------------------------------------------------------------------------------------------------
// Project web site: http://blog.veles.rs
// GitHub: http://github.com/gorangrubic
// Mendeley profile: http://www.mendeley.com/profiles/goran-grubi2/
// ORCID ID: http://orcid.org/0000-0003-2673-9471
// Email: hardy@veles.rs
// </summary>
// ------------------------------------------------------------------------------------------------------------------

namespace imbNLP.PartOfSpeech.pipeline.plugin
{
    using imbACE.Core.operations;
    using imbACE.Services.console;
    using imbACE.Services.consolePlugins;
    using System;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// Plugin for imbACE console - pipelineModelConsolePlugin
    /// </summary>
    /// <seealso cref="imbACE.Services.consolePlugins.aceConsolePluginBase" />
    public class pipelineModelConsolePlugin : aceConsolePluginBase
    {
        public pipelineModelConsolePlugin(IAceAdvancedConsole __parent) : base(__parent, "Pipeline Model Console Plugin", "This is imbACE advanced console plugin for pipelineModelConsolePlugin")
        {
        }

        [Display(GroupName = "run", Name = "MCRepo", ShortName = "", Description = "Creates instance of MCRepositorium processing pipeline model and runs it against specified repositorium name")]
        [aceMenuItem(aceMenuItemAttributeRole.ExpandedHelp, "It will create new instance of pipeline: model and machine, and run it.")]
        /// <summary>Creates instance of MCRepositorium processing pipeline model and runs it against specified repositorium name</summary>
        /// <remarks><para>It will create new instance of pipeline: model and machine, and run it.</para></remarks>
        /// <param name="MCRepoName">Name of MCRepository to process</param>

        /// <seealso cref="aceOperationSetExecutorBase"/>
        public void aceOperation_runMCRepo(
            [Description("Name of MCRepository to process")] String MCRepoName = "dev")
        {
            // mcRepoProcessModel model = new mcRepoProcessModel();
            //model.logger = output;
            //model.constructionProcess();

            //pipelineMachine machine = new pipelineMachine(output);

            //machine.run(model, MCRepoName);
        }
    }
}