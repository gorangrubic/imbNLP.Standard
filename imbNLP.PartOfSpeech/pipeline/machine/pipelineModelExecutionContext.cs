// --------------------------------------------------------------------------------------------------------------------
// <copyright file="pipelineModelExecutionContext.cs" company="imbVeles" >
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
using imbSCI.Core;
using imbSCI.Core.reporting;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace imbNLP.PartOfSpeech.pipeline.machine
{
    /// <summary>
    /// Result of an pipeline machine processing
    /// </summary>
    public class pipelineModelExecutionContext
    {
        public ILogBuilder logger { get; set; }

        public DateTime startTime { get; set; }

        /// <summary>
        /// Collection of custom data properties, accessable from tasks
        /// </summary>
        /// <value>
        /// The custom data properties.
        /// </value>
        protected PropertyCollection customDataProperties { get; set; } = new PropertyCollection();

        private Object customDataEntryLock = new Object();

        /// <summary>
        /// Gets the custom data property.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public T GetCustomDataProperty<T>(String key)
        {
            if (!customDataProperties.ContainsKey(key))
            {
                lock (customDataEntryLock)
                {
                    if (!customDataProperties.ContainsKey(key))
                    {
                        customDataProperties.Add(key, default(T));
                    }
                }
            }

            Object output = customDataProperties[key];
            return (T)output;
        }

        private Object GetAndChangeCustomInt32Lock = new Object();

        /// <summary>
        /// Updates custom data property of type Int32 and returns new value
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="change">The change.</param>
        /// <returns></returns>
        public Int32 GetAndChangeCustomDataProperty(String key, Int32 change)
        {
            if (!customDataProperties.ContainsKey(key))
            {
                lock (GetAndChangeCustomInt32Lock)
                {
                    if (!customDataProperties.ContainsKey(key))
                    {
                        customDataProperties.Add(key, 0);
                    }
                }
            }

            Int32 output = 0;
            lock (GetAndChangeCustomInt32Lock)
            {
                output = (Int32)customDataProperties[key];
                output = output + change;
                customDataProperties[key] = output;
            }
            return output;
        }

        private Object SetCustomDataLock = new Object();

        /// <summary>
        /// Gets the custom data property.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public void SetCustomDataProperty<T>(String key, T newValue)
        {
            lock (SetCustomDataLock)
            {
                if (!customDataProperties.ContainsKey(key))
                {
                    customDataProperties.Add(key, newValue);
                }
                customDataProperties[key] = newValue;
            }
        }

        public DateTime lastStatusUpdate { get; set; }

        /// <summary>
        /// Gets the since last status update.
        /// </summary>
        /// <returns></returns>
        public Double GetSinceLastStatusUpdate()
        {
            return DateTime.Now.Subtract(lastStatusUpdate).TotalSeconds;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="pipelineModelExecutionContext"/> class.
        /// </summary>
        /// <param name="__model">The model.</param>
        public pipelineModelExecutionContext(IPipelineModel __model)
        {
            model = __model;
            model.exitBin.SetRoute(exitSubjects);
            model.trashBin.SetRoute(trashSubjects);

            startTime = DateTime.Now;
            lastStatusUpdate = DateTime.Now;

            logger = new builderForLog();
            screenOutputControl.logToConsoleControl.setAsOutput(logger as IConsoleControl, "ExContext");
        }

        public Int32 currentTake { get; set; } = 0;

        public Int32 takeCounter { get; set; } = 0;

        public Int32 lastTakeSize { get; set; }

        public Boolean RunInDebugMode { get; set; } = false;

        /// <summary>
        /// Gets or sets the scheduled tasks.
        /// </summary>
        /// <value>
        /// The scheduled tasks.
        /// </value>
        public ConcurrentStack<IPipelineTask> scheduledTasks { get; protected set; } = new ConcurrentStack<IPipelineTask>();

        /// <summary>
        /// Gets or sets the scheduled tasks.
        /// </summary>
        /// <value>
        /// The scheduled tasks.
        /// </value>
        public ConcurrentStack<IPipelineTask> finishedTasks { get; protected set; } = new ConcurrentStack<IPipelineTask>();

        public String GetSignature()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("# Model: " + model.name);

            sb.AppendLine("## Exit bin - Exit by type");
            foreach (var exitByTypePair in exitByType)
            {
                sb.AppendLine(String.Format("{0,-20} : {1,20}", exitByTypePair.Key.Name, exitByTypePair.Value.Count));
            }
            sb.AppendLine("## Exit bin - Exit by level");
            foreach (var exitByTypePair in exitByLevel)
            {
                sb.AppendLine(String.Format("{0,-20} : {1,20}", exitByTypePair.Key.ToString(), exitByTypePair.Value.Count));
            }

            return sb.ToString();
        }

        public Dictionary<Type, ConcurrentBag<IPipelineTaskSubject>> exitByType { get; protected set; } = new Dictionary<Type, ConcurrentBag<IPipelineTaskSubject>>();

        public Dictionary<cnt_level, ConcurrentBag<IPipelineTaskSubject>> exitByLevel { get; protected set; } = new Dictionary<cnt_level, ConcurrentBag<IPipelineTaskSubject>>();

        /// <summary>
        /// Subjects that were completed
        /// </summary>
        /// <value>
        /// The exit subjects.
        /// </value>
        public ConcurrentBag<IPipelineTaskSubject> exitSubjects { get; protected set; } = new ConcurrentBag<IPipelineTaskSubject>();

        /// <summary>
        /// Subjects that were trashed
        /// </summary>
        /// <value>
        /// The trash subjects.
        /// </value>
        public ConcurrentBag<IPipelineTaskSubject> trashSubjects { get; protected set; } = new ConcurrentBag<IPipelineTaskSubject>();

        /// <summary>
        /// pipeline model
        /// </summary>
        /// <value>
        /// The model.
        /// </value>
        public IPipelineModel model { get; protected set; }
    }
}