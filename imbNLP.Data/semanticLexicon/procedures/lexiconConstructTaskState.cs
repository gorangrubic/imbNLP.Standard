// --------------------------------------------------------------------------------------------------------------------
// <copyright file="lexiconConstructTaskState.cs" company="imbVeles" >
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
    using imbSCI.Core.extensions.io;
    using imbSCI.Core.files;
    using imbSCI.Core.files.folders;
    using imbSCI.Core.files.search;
    using imbSCI.Core.reporting;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Xml.Serialization;

    public class lexiconConstructTaskState
    {
        /// <summary>
        ///
        /// </summary>
        [XmlIgnore]
        public string shadow_filepath { get; set; }

        /// <summary>
        ///
        /// </summary>
        [XmlIgnore]
        public string failedTasks_filepath { get; set; }

        /// <summary>
        ///
        /// </summary>
        [XmlIgnore]
        public string scheduledTasks_filepath { get; set; }

        /// <summary>
        ///
        /// </summary>
        [XmlIgnore]
        public string processedTasks_filepath { get; set; }

        [XmlIgnore]
        public string state_filepath { get; private set; }

        /// <summary>
        /// Brise sve sto je imao snimljeno o stageu
        /// </summary>
        /// <param name="task">The task.</param>
        public void taskStateReset(lexiconTaskBase task)
        {
            taskInitDateTime = DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString();
            taskSessionStarts = new List<string>();

            pathPrefix = task.taskTitle.getCleanFilePath();
            setPaths();

            folder.deleteFiles(pathPrefix + "*.*");

            inputFileCheck(task, true);
            File.Copy(inputPath, scheduledTasks_filepath);

            connectWithFiles();
            stateSave();
        }

        /// <summary>
        /// Priprema ga za start nove sesije
        /// </summary>
        /// <param name="task">The task.</param>
        /// <param name="take">The take.</param>
        /// <param name="__savemodels">if set to <c>true</c> [savemodels].</param>
        /// <param name="__debug">if set to <c>true</c> [debug].</param>
        /// <param name="__verbose">if set to <c>true</c> [verbose].</param>
        /// <param name="response">The response.</param>
        public void taskStateStartSession(lexiconTaskBase task, int take, bool __savemodels, bool __debug, bool __verbose, ILogBuilder __response)
        {
            pathPrefix = task.taskTitle.getCleanFilePath();
            setPaths();

            debug = __debug;
            verbose = __verbose;
            saveModel = __savemodels;

            takeCount = take;

            taskSessionStarts.Add(DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString());
            index = 0;
            indexAbsolute = 0;
            lastSave = DateTime.Now;
            response = __response;
            timeSessionStart = DateTime.Now;

            connectWithFiles();
            taskShadow.AddRange(failedTasks.TakeAll().getLineContentList());

            stateSave();
        }

        /// <summary>
        ///
        /// </summary>
        public List<string> entryList { get; set; }

        /// <summary>
        /// When session started
        [XmlIgnore]                    /// </summary>
        public DateTime timeSessionStart { get; private set; }

        /// <summary>
        /// Bindable property
        [XmlIgnore]                            /// </summary>
        public ILogBuilder response { get; private set; }

        private void setPaths()
        {
            folder = semanticLexiconManager.manager.constructor.projectFolderStructure[lexiconConstructorProjectFolder.stages];

            state_filepath = folder.pathFor(pathPrefix + "_state.xml");
            failedTasks_filepath = folder.pathFor(pathPrefix + "_failedTasks.txt");
            scheduledTasks_filepath = folder.pathFor(pathPrefix + "_scheduledTasks.txt");
            processedTasks_filepath = folder.pathFor(pathPrefix + "_processedTasks.txt");
            shadow_filepath = folder.pathFor(pathPrefix + "_shadow.txt");
        }

        private void connectWithFiles()
        {
            #region creating state lists

            failedTasks = new fileTextOperater(failedTasks_filepath, true);
            scheduledTasks = new fileTextOperater(scheduledTasks_filepath, true);

            processedTasks = new List<string>();
            taskShadow = new List<string>();

            if (!File.Exists(processedTasks_filepath)) File.WriteAllLines(processedTasks_filepath, new string[] { });
            if (!File.Exists(shadow_filepath)) File.WriteAllLines(shadow_filepath, new string[] { });
            processedTasks = processedTasks_filepath.openFileToList(true);
            taskShadow = processedTasks_filepath.openFileToList(true);

            #endregion creating state lists
        }

        public bool stateSessionTick(lexiconTaskBase task, bool forceRefresh = false)
        {
            index++;
            indexAbsolute++;

            if ((index >= task.stateSavePeriod) || forceRefresh)
            {
                var totalTime = DateTime.Now.Subtract(timeSessionStart).TotalMinutes;
                TimeSpan period = DateTime.Now.Subtract(lastSave);
                speedCurrent = ((double)index) / period.TotalMinutes;
                speedAverage = ((double)indexAbsolute / (double)totalTime);
                index = 0;

                scheduledTasks.file.Refresh();
                currentRatio = 1 - ((double)scheduledTasks.file.Length / ((double)inputFileSize));

                statusReport = task.taskTitle + "[" + currentRatio.ToString("P") + "]  session[" + indexAbsolute + ":" + takeCount + "] rt[" + totalTime.ToString("#0,0") + " min]  speed[" + speedCurrent.ToString("#0.00") + " n/min] avg[" + speedCurrent.ToString("#0.00") + " n/min] ";

                Console.Title = statusReport;
                response.log(statusReport);

                stateSave();
            }

            if (indexAbsolute > takeCount) return true;

            return false;
        }

        [XmlIgnore]
        public string statusReport { get; private set; } = "";

        /// <summary>
        ///
        /// </summary>
        public double speedCurrent { get; set; } = 0;

        /// <summary>
        ///
        /// </summary>
        public double speedAverage { get; set; } = 0;

        /// <summary>
        /// Ciklicni save
        /// </summary>
        public void stateSave()
        {
            objectSerialization.saveObjectToXML(this, state_filepath);

            taskShadow.AddRange(shadowBuffer);
            processedTasks.AddRange(processedBuffer);

            scheduledTasks.Remove(processedBuffer);
            failedTasks.Append(failedBuffer);

            taskShadow.saveContentOnFilePath(shadow_filepath);
            processedTasks.saveContentOnFilePath(processedTasks_filepath);

            shadowBuffer.Clear();
            processedBuffer.Clear();
            failedBuffer.Clear();

            semanticLexiconManager.manager.constructor.saveAll();
            lastSave = DateTime.Now;
        }

        /// <summary>
        /// Loaded version
        /// </summary>
        public lexiconConstructTaskState()
        {
            /*
            setPaths();

            connectWithFiles();
            stateSave();*/
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="lexiconConstructTaskState"/> class.
        /// </summary>
        /// <param name="task">The task.</param>
        public lexiconConstructTaskState(lexiconTaskBase task)
        {
            pathPrefix = task.taskTitle.getCleanFilePath();
            /*
            setPaths();

            inputFileCheck(task);
            connectWithFiles();
            stateSave();*/
        }

        /// <summary>
        ///
        /// </summary>
        public string taskInitDateTime { get; set; } = "";

        /// <summary>
        ///
        /// </summary>
        public List<string> taskSessionStarts { get; set; } = new List<string>();

        /// <summary>
        /// Number of terms to process
        /// </summary>
        [XmlIgnore]
        public int takeCount { get; set; }

        /// <summary>
        ///
        /// </summary>
        [XmlIgnore]
        public folderNode folder { get; protected set; }

        public string pathPrefix { get; set; }

        [XmlIgnore]
        public string inputPath { get; private set; }

        private void inputFileCheck(lexiconTaskBase task, bool deleteFile = false)
        {
            inputPath = semanticLexiconManager.manager.constructor.projectFolderStructure.pathFor(task.taskInputPath);
            if (deleteFile) File.Delete(inputPath);

            string s_path = "";
            if (task.taskSourcePathIsAppRoot)
            {
                s_path = task.taskSourcePath;
            }
            else
            {
                s_path = semanticLexiconManager.manager.constructor.projectFolderStructure.pathFor(task.taskSourcePath);
            }

            if (File.Exists(s_path))
            {
                //corpusInputFilepath = projectDirectory.FullName.add("corpus_input.csv", "\\");
                if (File.Exists(inputPath))
                {
                    semanticLexiconManager.manager.constructor.output.AppendLine("Corpus input file found: " + inputPath);
                }
                else
                {
                    File.Copy(s_path, inputPath);
                    semanticLexiconManager.manager.constructor.output.AppendLine("Corpus input file [" + s_path + "] copied: " + inputPath);
                }
            }

            FileInfo fi = new FileInfo(inputPath);
            if (fi.Exists) inputFileSize = fi.Length;
        }

        /// <summary>
        ///
        /// </summary>
        [XmlIgnore]
        public DateTime lastSave { get; set; }

        /// <summary> </summary>
        [XmlIgnore]
        public bool verbose { get; set; }

        /// <summary> </summary>

        public long inputFileSize { get; set; } = 0;

        /// <summary>
        ///
        /// </summary>
        [XmlIgnore]
        public bool saveModel { get; set; }

        /// <summary>
        ///
        /// </summary>
        [XmlIgnore]
        public bool debug { get; set; }

        /// <summary> </summary>
        [XmlIgnore]
        public int indexAbsolute { get; protected set; } = 0;

        /// <summary> </summary>
        [XmlIgnore]
        public int index { get; protected set; } = 0;

        /// <summary>
        ///
        /// </summary>
        public double currentRatio { get; set; } = 0;

        /// <summary>
        ///
        /// </summary>
        [XmlIgnore]
        public List<string> shadowBuffer { get; set; } = new List<string>();

        /// <summary>
        ///
        /// </summary>
        [XmlIgnore]
        public List<string> processedBuffer { get; set; } = new List<string>();

        /// <summary>
        ///
        /// </summary>
        [XmlIgnore]
        public List<string> failedBuffer { get; set; } = new List<string>();

        /// <summary>
        ///
        /// </summary>
        [XmlIgnore]
        public List<string> taskShadow { get; set; }

        /// <summary>
        ///
        /// </summary>
        [XmlIgnore]
        public List<string> processedTasks { get; set; }

        /// <summary>
        /// List of tasks left to be performed
        /// </summary>
        [XmlIgnore]
        public fileTextOperater scheduledTasks { get; set; }

        /// <summary> </summary>
        [XmlIgnore]
        public fileTextOperater failedTasks { get; set; }
    }
}