// --------------------------------------------------------------------------------------------------------------------
// <copyright file="pipelineMachine.cs" company="imbVeles" >
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
using imbACE.Core.core;
using imbNLP.PartOfSpeech.flags.token;
using imbNLP.PartOfSpeech.pipeline.core;
using imbSCI.Core.reporting;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace imbNLP.PartOfSpeech.pipeline.machine
{
    /// <summary>
    /// Machine executes the pipelineModule in parallel manner, and returns <see cref="pipelineModelExecutionContext"/>
    /// </summary>
    public class pipelineMachine
    {
        protected ILogBuilder logger { get; set; }

        public pipelineMachine(ILogBuilder _logger = null)
        {
            //model = __model;
            logger = _logger;

            if (logger == null)
            {
                logger = new builderForLog();

                imbSCI.Core.screenOutputControl.logToConsoleControl.setAsOutput(logger as IConsoleControl, "PipeMachine");

                logger.log("Pipeline Machine - autocreated logger");
            }
        }

        public pipelineMachineSettings settings { get; set; } = new pipelineMachineSettings();

        /// <summary>
        /// Number of parallel tasks to perform at once
        /// </summary>
        /// <value>
        /// The n parallel tasks.
        /// </value>
        public Int32 nParallelTasks { get; set; } = 1;

        /// <summary>
        /// Gets or sets a value indicating whether [machine running].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [machine running]; otherwise, <c>false</c>.
        /// </value>
        public Boolean machineRunning { get; set; } = true;

        /// <summary>
        /// The tasktake insanelimit
        /// </summary>
        public const Int32 TASKTAKE_INSANELIMIT = 500;

        /// <summary>
        /// Gets or sets the state.
        /// </summary>
        /// <value>
        /// The state.
        /// </value>
        public pipelineMachineState state { get; set; } = pipelineMachineState.none;

        public const String statusLineOneFormat = "pipeline --> [ _{0, -10}_ ] - W[ _{1, 5:D5}_ ] : R[ _{2, 5:D5}_ ] : F[ _{3, 5:D5}_ ] ---- ";
        public const String statusLineTwoFormat = "  RTime  --> [ _{0, 5:F2}_ min ] - TC[ _{1, 3}_ ] : State [ _{2, -10}_ ] -- Bins E[ _{3, 5:D5}_ ] T[ _{4, 5:D5}_ ] -- ";

        public const String statusLineOneExplainFormat = "pipeline --> [ _{0}_ ] - W[ _{1}_ ] : R[ _{2}_ ] : F[ _{3}_ ] ---- ";
        public const String statusLineTwoExplainFormat = "  RTime  --> [ _{0}_ min ] - TC[ _{1}_ ] : State [ _{2}_ ] -- Bins E[ _{3}_ ] T[ _{4}_ ] -- ";

        protected void statusExplain()
        {
            if (logger != null)
            {
                logger.AppendLine("--- Meaning of status message ----");

                logger.AppendLine(String.Format(statusLineOneExplainFormat, "Model name", "Tasks waiting", "Running", "Finished"));

                logger.AppendLine(String.Format(statusLineTwoExplainFormat, "Minutes running", "N of parallel tasks allowed", "State", "Completed", "Trashed"));

                logger.AppendLine("----------------------------------");
            }
        }

        /// <summary>
        /// Statuses the update.
        /// </summary>
        /// <param name="context">The context.</param>
        protected void statusUpdate(pipelineModelExecutionContext context)
        {
            if (logger != null)
            {
                logger.AppendLine(String.Format(statusLineOneFormat, context.model.name, context.scheduledTasks.Count, context.lastTakeSize, context.finishedTasks.Count));

                logger.AppendLine(String.Format(statusLineTwoFormat, DateTime.Now.Subtract(context.startTime).TotalMinutes, nParallelTasks, state.ToString(), context.exitSubjects.Count, context.trashSubjects.Count));
            }

            imbACE.Services.terminal.aceTerminalInput.doBeepViaConsole();
        }

        /// <summary>
        /// Runs the specified model.
        /// </summary>
        /// <param name="__model">The model.</param>
        /// <param name="paramsForPrimaryTasks">The parameters for primary tasks creation, passed to the model</param>
        /// <returns></returns>
        public pipelineModelExecutionContext run(IPipelineModel __model, params Object[] paramsForPrimaryTasks)
        {
            var output = new pipelineModelExecutionContext(__model);

            var primTasks = __model.createPrimaryTasks(paramsForPrimaryTasks);

            foreach (var pTask in primTasks)
            {
                output.scheduledTasks.Push(pTask);
            }

            statusExplain();

            Task runMasterTask = new Task(() =>
            {
                runSeparate(output);
            });

            machineRunning = true;

            statusUpdate(output);

            runMasterTask.Start();

            while (machineRunning)
            {
                if (output.GetSinceLastStatusUpdate() > settings.StatusReportPeriod)
                {
                    output.lastStatusUpdate = DateTime.Now;

                    statusUpdate(output);
                }

                Thread.Sleep(settings.TickForCheck);

                if (output.scheduledTasks.Count == 0 && output.lastTakeSize == 0)
                {
                    machineRunning = false;
                    statusUpdate(output);
                }
            }

            logger.log("Sorting exit bin into dictionaries (by type and content level)");

            foreach (var item in output.exitSubjects)
            {
                Type t = item.GetType();
                if (!output.exitByType.ContainsKey(t)) output.exitByType.Add(t, new ConcurrentBag<IPipelineTaskSubject>());
                cnt_level level = item.contentLevelType;
                if (!output.exitByLevel.ContainsKey(level)) output.exitByLevel.Add(level, new ConcurrentBag<IPipelineTaskSubject>());
                output.exitByType[t].Add(item);
                output.exitByLevel[level].Add(item);
            }

            logger.log("Exit bin sorted by [" + output.exitByType.Count + "] types and [" + output.exitByLevel.Count + "] levels");

            //imbACE.Services.terminal.aceTerminalInput.doBeepViaConsole(1200, 200, 1);

            return output;
        }

        /// <summary>
        /// Runs the specified model.
        /// </summary>
        /// <param name="output">The output.</param>
        protected void runSeparate(pipelineModelExecutionContext output)
        {
            while (machineRunning)
            {
                output.lastTakeSize = Math.Min(nParallelTasks, output.scheduledTasks.Count);

                List<IPipelineTask> taskTake = new List<IPipelineTask>();

                if (output.lastTakeSize == 0)
                {
                    state = pipelineMachineState.done;
                    break;
                }

                state = pipelineMachineState.preparingTake;

                Int32 c = 0;
                while (taskTake.Count < output.lastTakeSize)
                {
                    c++;
                    IPipelineTask task;

                    if (output.scheduledTasks.TryPop(out task))
                    {
                        taskTake.Add(task);
                    }

                    if (c > TASKTAKE_INSANELIMIT)
                    {
                        logger.log("Popping [" + output.lastTakeSize + "] tasks from scheduled task stack [" + output.scheduledTasks.Count + "] reached insane limit switch [" + c.ToString() + "]");
                        break;
                    }
                }

                state = pipelineMachineState.runningTaskTake;

                if (settings.doUseParallelExecution)
                {
                    Parallel.ForEach(taskTake, (IPipelineTask task) =>
                    {
                        try
                        {
                            task.StartProcess(output);
                        }
                        catch (Exception ex)
                        {
                            logger.log("Pipeline task exception:" + ex.Message);
                            throw;
                        }
                    });
                }
                else
                {
                    foreach (var task in taskTake)
                    {
                        try
                        {
                            task.StartProcess(output);
                        }
                        catch (Exception ex)
                        {
                            logger.log("Pipeline task exception:" + ex.Message);
                            throw;
                        }
                    }
                }

                output.finishedTasks.PushRange(taskTake.ToArray());

                state = pipelineMachineState.preparingTake;

                output.takeCounter++;
            }

            logger.log("Pipeline machine finished [" + output.finishedTasks.Count + "] tasks in model [" + output.model.name + "] after [" + output.takeCounter + "] tasks");
        }
    }
}