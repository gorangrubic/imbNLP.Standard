// --------------------------------------------------------------------------------------------------------------------
// <copyright file="pipelineTask.cs" company="imbVeles" >
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
using imbNLP.PartOfSpeech.pipeline.core;
using System;
using System.Threading;

namespace imbNLP.PartOfSpeech.pipeline.machine
{
    /// <summary>
    /// One pipeline task
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <seealso cref="imbNLP.PartOfSpeech.pipeline.machine.IPipelineTask" />
    public class pipelineTask<T> : IPipelineTask where T : IPipelineTaskSubject
    {
        /// <summary>
        /// Sets subject, model reference and lifespan
        /// </summary>
        /// <param name="__subject">The subject.</param>
        /// <param name="__model">The model.</param>
        public pipelineTask(T __subject)
        {
            subject = __subject;
        }

        public String GetStringInfo(Boolean full = true)
        {
            String output = GetType().Name;

            output += "<" + subject.GetType().Name + ">";

            if (full)
            {
                output += "[l=" + lifeSpanLeft.ToString("D5") + "]";
            }

            return output;
        }

        /// <summary>
        /// Gets or sets the state.
        /// </summary>
        /// <value>
        /// The state.
        /// </value>
        public pipelineTaskStateEnum state { get; protected set; } = pipelineTaskStateEnum.running;

        public pipelineModelExecutionContext context { get; set; }

        /// <summary>
        /// Starting task execution
        /// </summary>
        /// <param name="__context">Context in which </param>
        public void StartProcess(pipelineModelExecutionContext __context)
        {
            model = __context.model;

            currentNode = model;
            nextNode = model;

            context = __context;

            lifeSpanLeft = model.taskInitialLife;

            state = pipelineTaskStateEnum.running;

            while (state.HasFlag(pipelineTaskStateEnum.running))
            {
                processNextNode();

                Thread.SpinWait(1);
            }
        }

        public const Int32 RETRY = 5;

        /// <summary>
        /// Processes the next node, calculates the next life span, moves> currentNode to lastNode
        /// /// </summary>
        private void processNextNode()
        {
            if (state.HasFlag(pipelineTaskStateEnum.notRunning))
            {
                if (context.RunInDebugMode)
                {
                    context.logger.log("Task [" + this.GetType().Name + "] is in the state [" + state.ToString() + "]");
                }

                return;
            }

            // <---- sliding toward the next node
            lastNode = currentNode;

            currentNode = nextNode;

            // <----- processing the current node

            if (currentNode == null)
            {
                currentNode = model.exitBin;

                if (context.RunInDebugMode)
                {
                    context.logger.log("Task [" + this.GetType().Name + "] has no currentNode --> exitBin [" + state.ToString() + "]");
                }
            }

            Int32 retry = RETRY;
            while (retry > 0)
            {
                try
                {
                    nextNode = currentNode.process(this);
                    retry = 0;
                }
                catch (Exception ex)
                {
                    retry--;
                    if (retry < 1)
                    {
                        imbACE.Services.terminal.aceTerminalInput.doBeepViaConsole(2600, 500, 2);
                        nextNode = model.trashBin;

                        state = pipelineTaskStateEnum.abortingCrash;

                        context.logger.log("Task [" + GetStringInfo(true) + "] crashed at node [" + currentNode.name + "] (" + currentNode.GetType().Name + ") having path: " + currentNode.path);
                        context.logger.log("Exception message [" + ex.Message + "]");

                        context.logger.log(ex.StackTrace);
                        break;
                    }
                }
            }

            if (context.RunInDebugMode)
            {
                if (nextNode != null)
                {
                    context.logger.log("Task [" + this.GetType().Name + "] set to nextNode --> [" + nextNode.name + "]");
                }
                else
                {
                    context.logger.log("Task [" + this.GetType().Name + "] set to nextNode --> null");
                }
            }

            if (currentNode.nodeType.HasFlag(pipelineNodeTypeEnum.bin))
            {
                if (state == pipelineTaskStateEnum.running)
                {
                    state = pipelineTaskStateEnum.finishedInBin;
                    return;
                }
            }

            lifeSpanLeft--;

            if (lifeSpanLeft < 0)
            {
                state = pipelineTaskStateEnum.abortedLifeOut;
                return;
            }

            if (lifeSpanLeft == 0)
            {
                nextNode = model.exitBin;
                state = pipelineTaskStateEnum.abortingLifeOut;
            }
            else if (nextNode == null)
            {
                nextNode = model.exitBin;
            }
        }

        /// <summary>
        /// Gets or sets the model.
        /// </summary>
        /// <value>
        /// The model.
        /// </value>
        public IPipelineModel model { get; protected set; }

        /// <summary>
        /// How many iterations/node moves are left for this task
        /// </summary>
        /// <value>
        /// The life span left.
        /// </value>
        public Int32 lifeSpanLeft { get; set; } = 0;

        /// <summary>
        /// Item that is subject of process
        /// </summary>
        /// <value>
        /// The subject.
        /// </value>
        public T subject { get; set; }

        /// <summary>
        /// Current node where the task is
        /// </summary>
        /// <value>
        /// The current node.
        /// </value>
        public IPipelineNode currentNode { get; protected set; } = null;

        /// <summary>
        /// Next node designated to traverse to
        /// </summary>
        /// <value>
        /// The next node.
        /// </value>
        public IPipelineNode nextNode { get; protected set; } = null;

        /// <summary>
        /// Node that was executing before <see cref="currentNode" />
        /// </summary>
        /// <value>
        /// The last node.
        /// </value>
        public IPipelineNode lastNode { get; protected set; } = null;

        /// <summary>
        /// Item that is subject of process
        /// </summary>
        /// <value>
        /// The subject.
        /// </value>
        IPipelineTaskSubject IPipelineTask.subject
        {
            get { return subject; }
            set { subject = (T)value; }
        }
    }
}