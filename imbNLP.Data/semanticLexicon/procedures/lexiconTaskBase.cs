// --------------------------------------------------------------------------------------------------------------------
// <copyright file="lexiconTaskBase.cs" company="imbVeles" >
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
namespace imbNLP.Data.semanticLexicon.procedures
{
    using imbSCI.Core.reporting;
    using System.Linq;

    public abstract class lexiconTaskBase
    {
        public lexiconTaskBase()
        {
            state = new lexiconConstructTaskState(this);
        }

        public abstract string taskTitle { get; }

        public abstract string taskInputPath { get; }

        public virtual bool taskSourcePathIsAppRoot
        {
            get
            {
                return false;
            }
        }

        public abstract string taskSourcePath { get; }

        public abstract string taskOutputPath { get; }

        public virtual void stageReset(ILogBuilder response)
        {
            state.taskStateReset(this);
        }

        public virtual void stateSave()
        {
            state.stateSessionTick(this, true);
        }

        public virtual bool iterationAllowParallel
        {
            get { return true; }
        }

        public virtual int iterationTake
        {
            get
            {
                if (iterationAllowParallel)
                {
                    return semanticLexiconManager.manager.constructionSettings.parallelTake;
                }
                else
                {
                    return 1;
                }
            }
        }

        /// <summary>
        /// Stages the parallel execution. // <---- urediti
        /// </summary>
        protected void stageParallelExecution()
        {
        }

        /// <summary>
        /// Stages the singular execution.
        /// </summary>
        protected void stageSingularExecution()
        {
        }

        public virtual void sessionStart(int take, bool __savemodels, bool __debug, bool __verbose, ILogBuilder __response)
        {
            state.taskStateStartSession(this, take, __savemodels, __debug, __verbose, __response);

            running = true;
            while (running)
            {
                state.entryList = state.scheduledTasks.Take(iterationTake, state.taskShadow, state.shadowBuffer).getLineContentList();

                if (!state.entryList.Any())
                {
                    running = false;
                    state.stateSessionTick(this, true);
                    stageComplete(__response);
                    return;
                }

                stageExecute(__response);

                if (state.stateSessionTick(this))
                {
                    running = false;
                    sessionComplete(__response);

                    return;
                }
            }
        }

        public virtual void sessionComplete(ILogBuilder response)
        {
            state.stateSessionTick(this, true);
        }

        protected abstract void stageExecute(ILogBuilder response);

        //public abstract void stageReset(ILogBuilder response);

        // public abstract void stateSave();

        public abstract void stageComplete(ILogBuilder response);

        // public abstract void sessionComplete(ILogBuilder response);

        /// <summary>
        ///
        /// </summary>
        public lexiconConstructTaskState state { get; set; }

        public virtual int stateSavePeriod
        {
            get
            {
                return semanticLexiconManager.manager.constructionSettings.saveAllIterations;
            }
        }

        /*
        public void checkIndex()
        {
            index++;
            indexAbsolute++;

            if ((index >= semanticLexiconManager.manager.constructionSettings.saveAllIterations) || indexAbsolute == 1)
            {
                lastSave = DateTime.Now;

                index = 0;
                semanticLexiconManager.manager.constructor.saveAll();

                semanticLexiconManager.manager.constructor.corpusOperater.file.Refresh();

                double ratio = ((Double)semanticLexiconManager.manager.constructor.corpusOperater.file.Length) / ((Double)semanticLexiconManager.manager.constructor.corpusInputFileSize);
                ratio = 1 - ratio;

                String msg = GetType().Name + " done[" + ratio.ToString("P") +  "]    take[" + indexAbsolute + ":" + takeCount + "]   shadow[" + taskShadow.ToString() + "]";

                semanticLexiconManager.manager.console.output.log(msg);
                semanticLexiconManager.manager.console.response.log("[" + period.TotalMinutes.ToString("#0.0000") + " min for " + semanticLexiconManager.manager.constructionSettings.saveAllIterations+" items ] speed [" + speed.ToString("#0.000" + " n/min]"));

                Console.Title = msg;

                semanticLexiconManager.manager.constructor.corpusOperater.Remove(taskShadow);
                taskShadow.Clear();
            }
        }*/

        /// <summary> </summary>
        public bool running { get; set; }
    }
}