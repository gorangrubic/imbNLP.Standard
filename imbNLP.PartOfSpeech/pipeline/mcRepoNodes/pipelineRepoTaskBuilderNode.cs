// --------------------------------------------------------------------------------------------------------------------
// <copyright file="pipelineRepoTaskBuilderNode.cs" company="imbVeles" >
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
using System;
using System.Collections.Generic;
using System.Linq;

namespace imbNLP.PartOfSpeech.pipeline.mcRepoNodes
{
    using imbACE.Core;
    using imbACE.Core.core.exceptions;
    using imbMiningContext;
    using imbMiningContext.MCDocumentStructure;
    using imbMiningContext.MCRepository;
    using imbMiningContext.MCWebSite;
    using imbNLP.PartOfSpeech.pipeline.core;
    using imbNLP.PartOfSpeech.pipeline.machine;
    using imbNLP.PartOfSpeech.pipelineForPos.subject;
    using imbSCI.Core.files.fileDataStructure;
    using imbSCI.Core.files.folders;
    using imbSCI.Data.data.sample;

    /// <summary>
    /// Task builder node. If the task is not for it, it will forward it to <see cref="IPipelineNode.next"/>,
    /// </summary>
    /// <remarks>
    /// <para>if task is processed and new tasks were fed into <see cref="pipelineModelExecutionContext"/> it will forward the processed task to the <see cref="IPipelineNode.forward"/></para>
    /// </remarks>
    /// <seealso cref="imbNLP.PartOfSpeech.pipeline.core.pipelineNodeRegular{imbNLP.PartOfSpeech.pipeline.machine.pipelineTaskSubjectContentToken}" />
    public class pipelineRepoTaskBuilderNode : pipelineNodeRegular<pipelineTaskMCRepoSubject>
    {
        // protected imbMCManager repoManager { get; set; }

        protected samplingSettings takeSetup { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="pipelineRepoTaskBuilderNode" /> class.
        /// </summary>
        /// <param name="__takeSetup">The web site sample take setup</param>
        public pipelineRepoTaskBuilderNode(samplingSettings __takeSetup)
        {
            _nodeType = pipelineNodeTypeEnum.taskBuilder;

            takeSetup = __takeSetup;
            //repoManager = new imbMCManager();
            SetLabel();
        }

        /// <summary>
        /// Task builder for <see cref="imbMCRepository"/> level of subject. Sends to next if task is not with <see cref="pipelineTaskMCRepoSubject"/>
        /// </summary>
        /// <param name="task">The task.</param>
        /// <returns></returns>
        public override IPipelineNode process(IPipelineTask task)
        {
            pipelineTask<pipelineTaskMCRepoSubject> realTask = task as pipelineTask<pipelineTaskMCRepoSubject>;
            if (realTask == null)
            {
                return next;
            }

            pipelineTaskMCRepoSubject realSubject = realTask.subject;

            folderNode repoFolder = appManager.Application.folder_resources.Add(imbMCManager.MCRepo_DefaultDirectoryName, "MC Repositories", "Root directory with repositories of Crawled Web content");

            imbMCRepository repo = realSubject.MCRepoName.LoadDataStructure<imbMCRepository>(repoFolder, task.context.logger);
            imbMCDocumentRepositorium docRepo = new imbMCDocumentRepositorium();
            docRepo.webRepository = repo;
            realSubject.mcElement = docRepo;
            realSubject.MCRepo = repo;

            List<imbMCWebSite> websites = repo.GetAllWebSites(task.context.logger, takeSetup);
            List<imbMCWebSite> ws = new List<imbMCWebSite>();

            //try
            //{
            //    repo.siteTable.Clear();

            //    repo.CheckSiteTable(task.context.logger);

            //    if (realSubject.WebSiteSample.Any())
            //    {
            //        foreach (String w in realSubject.WebSiteSample)
            //        {
            //            var iws = websites.FirstOrDefault(x => w.Contains(x.name));  //repo.GetWebSite(new domainAnalysis(w), false, task.context.logger);
            //            if (iws != null)
            //            {
            //                task.context.logger.log(this.name + " Web site [ _" + w + "_ ] added to the pipeline: [" + repo.name + "]");
            //                websites.Add(iws);
            //            }
            //            else
            //            {
            //                task.context.logger.log(this.name + " Web site [ _" + w + "_] not found in the repo: [" + repo.name + "]");
            //            }
            //        }
            //    }
            //    else
            //    {
            //    }
            //} catch (Exception ex)
            //{
            //    throw new aceGeneralException("Failed to recover web sites from the repository", ex, this, "Failed to load sites from repository: " + ex.Message);
            //}

            if (!websites.Any())
            {
                task.context.logger.log(this.name + " Failed --- no web sites loaded");
            }
            else
            {
            }

            List<String> needle = new List<string>();
            realSubject.MCSiteTargets.ForEach(x => needle.Add(pipelineSubjectTools.GetCleanCaseName(x)));

            List<String> urls = new List<string>();

            foreach (imbMCWebSite site in websites)
            {
                String sName = pipelineSubjectTools.GetCleanCaseName(site.domainInfo.urlProper);

                Boolean ok = true;

                if (realSubject.MCSiteTargets.Any())
                {
                    if (!needle.Contains(sName))
                    {
                        ok = false;

#if DEBUG
                        //Console.WriteLine("Site refused [" + sName + "]");

#endif
                    }
                }

                if (urls.Contains(sName)) ok = false;

                if (ok)
                {
                    pipelineTaskMCSiteSubject mCSiteSubject = new pipelineTaskMCSiteSubject();
                    mCSiteSubject.MCSite = site;

                    imbMCDocumentSet docSet = new imbMCDocumentSet();

                    docRepo.Add(docSet);
                    mCSiteSubject.mcElement = docSet;
                    mCSiteSubject.contentLevelType = flags.token.cnt_level.mcSite;
                    mCSiteSubject.name = sName;
                    mCSiteSubject.parent = realSubject;
                    realSubject.Add(mCSiteSubject);

                    urls.Add(mCSiteSubject.name);

                    pipelineTask<pipelineTaskMCSiteSubject> taskForSite = new pipelineTask<pipelineTaskMCSiteSubject>(mCSiteSubject);

                    task.context.scheduledTasks.Push(taskForSite);
                }
            }

            if (urls.Count < needle.Count)
            {
                urls.ForEach(x => needle.Remove(x));

                if (needle.Any())
                {
                    String nd = "";
                    needle.ForEach(x => nd += x + " ");

                    throw new aceScienceException("Some sites are not found in the MC Repository!! [" + nd + "]", null, realSubject, "Sites not loaded [" + nd + "]", this);
                }
            }

            return forward;
        }
    }
}