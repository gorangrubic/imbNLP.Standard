// --------------------------------------------------------------------------------------------------------------------
// <copyright file="cloudMatrix.cs" company="imbVeles" >
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
using imbACE.Core.core.exceptions;
using imbNLP.PartOfSpeech.TFModels.semanticCloud;

// using imbNLP.PartOfSpeech.TFModels.semanticCloud.core;
using imbSCI.Core.attributes;
using imbSCI.Core.extensions.data;
using imbSCI.Core.extensions.enumworks;
using imbSCI.Core.extensions.io;
using imbSCI.Core.extensions.table;
using imbSCI.Core.extensions.text;
using imbSCI.Core.files.folders;
using imbSCI.Core.math;
using imbSCI.Core.reporting;
using imbSCI.Data;
using imbSCI.Data.collection.nested;
using imbSCI.DataComplex.extensions.data.schema;
using imbSCI.DataComplex.special;
using imbSCI.Graph.FreeGraph;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;

namespace imbNLP.PartOfSpeech.TFModels.semanticCloudMatrix
{
    [Flags]
    public enum cloudMatrixDataTableType
    {
        none = 1,
        initialState = 2,
        stateAfterReduction = 4,
        absoluteValues = 8,
        normalizedValues = 16,
        maxCloudFrequency = 32,
        overlapSize = 64,
        minCloudFrequency = 128,
        overlapValue = 256,
    }

    public enum cloudMatrixReductionAction
    {
        unknown,
        LPFRemoval,
        CF_function,
        LowPassFilter,
        Microweight,
        Demotion,
    }

    /// <summary>
    /// Matrix of overlaping temrs in <see cref="lemmaSemanticCloud"/>
    /// </summary>
    /// <seealso cref="imbSCI.Data.collection.nested.aceDictionary2D{imbNLP.PartOfSpeech.TFModels.semanticCloud.lemmaSemanticCloud, imbNLP.PartOfSpeech.TFModels.semanticCloud.lemmaSemanticCloud, System.Collections.Generic.List{imbNLP.PartOfSpeech.TFModels.semanticCloud.core.freeGraphNodeBase}}" />
    public class cloudMatrix : aceDictionary2D<lemmaSemanticCloud, lemmaSemanticCloud, List<freeGraphNodeBase>>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="cloudMatrix"/> class.
        /// </summary>
        public cloudMatrix(String _name, String _description)
        {
            name = _name;
            description = _description;
        }

        public String name { get; set; } = "";
        public String description { get; set; } = "";

        /// <summary>
        /// Gets the value for cell targeted
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="type">The type.</param>
        /// <param name="counter">The counter.</param>
        /// <returns></returns>
        public Double GetCellNumber(lemmaSemanticCloud x, lemmaSemanticCloud y, cloudMatrixDataTableType type, instanceCountCollection<String> counter)
        {
            Double output = 0;

            List<freeGraphNodeBase> selected = this[x, y];

            Double min = MaxCloudFrequency;
            Double max = MinCloudFrequency;

            if (type.HasFlag(cloudMatrixDataTableType.overlapValue))
            {
                if (type.HasFlag(cloudMatrixDataTableType.initialState))
                {
                    output = selected.Sum(s => s.weight);
                }
                else
                {
                    output = x.GetOverlap(y).Sum(s => s.weight);
                }
            }

            if (output == 0)
            {
                if (type.HasFlag(cloudMatrixDataTableType.normalizedValues))
                {
                    if (type.HasFlag(cloudMatrixDataTableType.overlapSize))
                    {
                        if (type.HasFlag(cloudMatrixDataTableType.initialState))
                        {
                            output = selected.Count.GetRatio(MaxOverlap);
                        }
                        else
                        {
                            if (x == y)
                            {
                                output = 0;
                            }
                            else
                            {
                                output = x.GetOverlap(y).Count.GetRatio(selected.Count);
                            }
                        }
                    }
                    else if (type.HasFlag(cloudMatrixDataTableType.maxCloudFrequency) || type.HasFlag(cloudMatrixDataTableType.minCloudFrequency))
                    {
                        for (int i = 0; i < selected.Count; i++)
                        {
                            freeGraphNodeBase ne = selected[i];
                            min = Math.Min(min, (Double)counter[ne.name]);
                            max = Math.Max(max, (Double)counter[ne.name]);
                        }

                        if (type.HasFlag(cloudMatrixDataTableType.maxCloudFrequency))
                        {
                            output = max.GetRatio(MaxCloudFrequency);
                        }
                        else
                        {
                            output = min.GetRatio(MinCloudFrequency);
                        }
                    }
                }
                else
                {
                    if (type.HasFlag(cloudMatrixDataTableType.overlapSize))
                    {
                        if (type.HasFlag(cloudMatrixDataTableType.initialState))
                        {
                            output = selected.Count;
                        }
                        else
                        {
                            if (x == y)
                            {
                                output = 0;
                            }
                            else
                            {
                                output = x.GetOverlap(y).Count;
                            }
                        }
                    }
                    else if (type.HasFlag(cloudMatrixDataTableType.maxCloudFrequency) || type.HasFlag(cloudMatrixDataTableType.minCloudFrequency))
                    {
                        for (int i = 0; i < selected.Count; i++)
                        {
                            freeGraphNodeBase ne = selected[i];
                            min = Math.Min(min, (Double)counter[ne.name]);
                            max = Math.Max(max, (Double)counter[ne.name]);
                        }

                        if (type.HasFlag(cloudMatrixDataTableType.maxCloudFrequency))
                        {
                            output = max;
                        }
                        else
                        {
                            output = min;
                        }
                    }
                }
            }

            return output;
        }

        /// <summary>
        /// Builds the table.
        /// </summary>
        /// <param name="settings">The settings.</param>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public DataTable BuildTable(cloudMatrixSettings settings, cloudMatrixDataTableType type)
        {
            DataTable table = new DataTable();
            table.SetTitle("CloudMatrix_" + name);
            table.SetDescription(description.or("Semantic cloud matrix report"));

            List<lemmaSemanticCloud> clouds = this.Get1stKeys().ToList();

            Int32 ci = 0;
            foreach (lemmaSemanticCloud cl in clouds)
            {
                table.SetAdditionalInfoEntry("Cloud " + ci, cl.className);
                if (cl.className.isNullOrEmpty()) cl.className = "C" + ci.ToString("D2");
                if (cl.name.isNullOrEmpty()) cl.name = cl.className;
                ci++;
            }

            instanceCountCollection<String> counter = GetCounter(type.HasFlag(cloudMatrixDataTableType.initialState));

            String format = "F5";

            if (type.HasFlag(cloudMatrixDataTableType.normalizedValues))
            {
                format = "F5";
            }
            else
            {
                format = "";
            }

            table.Add("Class", "Name of DocumentSetClass attached to the semantic clouds", "", typeof(String), imbSCI.Core.enums.dataPointImportance.normal);

            for (int i = 0; i < clouds.Count; i++)
            {
                table.Add(clouds[i].className, clouds[i].description, "C_" + i.ToString(), typeof(Double), imbSCI.Core.enums.dataPointImportance.normal, format, clouds[i].className);
            }

            table.Add("LemmasInitial", "Number of lemmas in the cloud, before reduction", "", typeof(Int32), imbSCI.Core.enums.dataPointImportance.important, "", "Lemmas - initial");

            table.Add("LinkRateInitial", "Link per node ratio, initial state", "", typeof(Double), imbSCI.Core.enums.dataPointImportance.normal, "F3", "Link rate initial");
            table.Add("LemmasAfter", "Number of lemmas in the cloud, after reduction", "", typeof(Int32), imbSCI.Core.enums.dataPointImportance.important, "", "Lemmas - after");

            table.Add("LinkRateAfter", "Link per node ratio, after reduction", "", typeof(Int32), imbSCI.Core.enums.dataPointImportance.normal, "F3", "Link rate after");

            for (int y = 0; y < clouds.Count; y++)
            {
                DataRow dr = table.NewRow();

                dr["Class"] = clouds[y].className;

                for (int x = 0; x < clouds.Count; x++)
                {
                    if (y == x)
                    {
                        dr[clouds[x].className] = 0;
                    }
                    else
                    {
                        dr[clouds[x].className] = GetCellNumber(clouds[x], clouds[y], type, counter);
                    }
                }

                dr["LemmasInitial"] = numberOfLemmas[clouds[y]];
                dr["LemmasAfter"] = clouds[y].CountNodes();

                dr["LinkRateInitial"] = numberOfLinks[clouds[y]].GetRatio(numberOfLemmas[clouds[y]]);
                dr["LinkRateAfter"] = clouds[y].CountLinks().GetRatio(clouds[y].CountNodes());

                table.Rows.Add(dr);
            }

            if (type.HasFlag(cloudMatrixDataTableType.overlapValue))
            {
                DataRow dr = table.NewRow();

                dr["Class"] = "Weight sums";

                for (int y = 0; y < clouds.Count; y++)
                {
                    Double sum = 0;
                    for (int x = 0; x < clouds.Count; x++)
                    {
                        sum += this[clouds[x], clouds[y]].Sum(c => c.weight); // GetCellNumber(clouds[x], clouds[y], type, counter);
                    }
                    dr[clouds[y].className] = sum;
                    //dr[clouds[x].name] = clouds[x].nodes.Sum(s => s.weight);
                }

                dr["LemmasInitial"] = 0;
                dr["LemmasAfter"] = 0;

                dr["LinkRateInitial"] = 0;
                dr["LinkRateAfter"] = 0;

                table.Rows.Add(dr);
            }

            var ty = type.getEnumListFromFlags<cloudMatrixDataTableType>();

            foreach (cloudMatrixDataTableType t in ty)
            {
                table.SetAdditionalInfoEntry(t.toStringSafe(), t.toStringSafe().imbTitleCamelOperation(true));
            }

            if (type.HasFlag(cloudMatrixDataTableType.initialState))
            {
                table.AddExtra("The table shows the state of the matrix before transformation (filtration).");
            }
            else
            {
                table.AddExtra("The table shows the state of the matrix after transformation (filtration).");
            }

            if (type.HasFlag(cloudMatrixDataTableType.overlapSize))
            {
                table.AddExtra("Values in the table are showing number of lemmas that are common to the clouds (of x and y axis).");
            }
            else if (type.HasFlag(cloudMatrixDataTableType.maxCloudFrequency))
            {
                table.AddExtra("Values in the table are showing highest Cloud Frequency for a term (at x and y axis).");
            }
            else if (type.HasFlag(cloudMatrixDataTableType.minCloudFrequency))
            {
                table.AddExtra("Values in the table are showing lowest Cloud Frequency for a term (at x and y axis).");
            }
            else if (type.HasFlag(cloudMatrixDataTableType.overlapValue))
            {
                table.AddExtra("Values in the table are showing sum of local weights for overlapping terms. The last row contains sum of weights for the class cloud.");
            }

            if (type.HasFlag(cloudMatrixDataTableType.normalizedValues))
            {
                if (type.HasFlag(cloudMatrixDataTableType.overlapSize))
                {
                    table.AddExtra("The values are normalized to 0-1, where 1 is overlap size in initial state for each x,y cell.");
                }
                else
                {
                    table.AddExtra("The values are normalized to 0-1.");
                }
            }
            else
            {
                table.AddExtra("The values are absolute.");
            }

            table.SetAdditionalInfoEntry("Max. CF", MaxCloudFrequency);
            table.SetAdditionalInfoEntry("Min. CF", MinCloudFrequency);
            table.SetAdditionalInfoEntry("Max. Overlap", MaxOverlap);
            table.SetAdditionalInfoEntry("Min. Overlap", MinOverlap);
            return table;
        }

        public lemmaSemanticCloud GetUnifiedCloud(String nameForCloud = "")
        {
            lemmaSemanticCloud output = new lemmaSemanticCloud();
            if (nameForCloud.isNullOrEmpty())
            {
                output.name = "UnifiedCloud";
            }
            else
            {
                output.name = nameForCloud;
            }

            output.description = "Created as union of clouds: ";
            var builder = new StringBuilder();
            builder.Append(output.description);

            foreach (lemmaSemanticCloud x in this.Get1stKeys())
            {
                output.RebuildIndex();
                output.AddCloud(x);
                builder.Append(x.name + ";");
            }
            output.description = builder.ToString();
            output.RebuildIndex();

            return output;
        }

        /// <summary>
        /// Exports the text report
        /// </summary>
        /// <param name="folder">The folder.</param>
        /// <param name="reduced">if set to <c>true</c> [reduced].</param>
        /// <param name="prefix">The prefix.</param>
        public void ExportTextReports(folderNode folder, Boolean reduced, String prefix = "")
        {
            foreach (lemmaSemanticCloud x in this.Get1stKeys())
            {
                instanceCountCollection<string> c = GetCounter(reduced);
                var srt = c.getSorted();

                String fn = prefix + x.className;
                if (reduced) { fn = fn + "_reduced_"; } else { fn = fn + "_initial_"; }

                fn = fn + "_overlap.txt";
                fn = folder.pathFor(fn, imbSCI.Data.enums.getWritableFileMode.overwrite, "Cloud Frequency report for all terms in the Cloud Matrix");
                List<String> lines = new List<string>();
                foreach (string ci in srt)
                {
                    if (reduced)
                    {
                        if (c[ci] > 1) lines.Add(String.Format("{1}  :   {0}", ci, c[ci] - 1));
                    }
                    else
                    {
                        lines.Add(String.Format("{1}  :   {0}", ci, c[ci]));
                    }
                }
                File.WriteAllLines(fn, lines);
            }
        }

        /// <summary>
        /// Gets the counter.
        /// </summary>
        /// <param name="ofCurrentState">if set to <c>true</c> [of current state].</param>
        /// <returns></returns>
        public instanceCountCollection<String> GetCounter(Boolean ofCurrentState = true)
        {
            instanceCountCollection<String> counter = new instanceCountCollection<string>();
            //lemmaSemanticCloud cloud = null;

            List<String> doneAnalysis = new List<string>();

            foreach (lemmaSemanticCloud x in this.Get1stKeys())
            {
                //if (cloud == null) cloud = x;

                foreach (lemmaSemanticCloud y in this.Get2ndKeys(x))
                {
                    //if (!doneAnalysis.Any(d => d.Contains(x.className) && d.Contains(y.className)))
                    //{
                    //    if (x != y)
                    //    {
                    var nd = this[x, y];
                    foreach (var n in nd)
                    {
                        counter.AddInstance(n.name);
                    }

                    //        doneAnalysis.Add(x.className + " " + y.className);
                    //    }
                    //}
                    /*
                    if (ofCurrentState)
                    {
                        var nd = x.nodes;
                        foreach (var n in nd)
                        {
                            counter.AddInstance(n.name);
                        }
                    }
                    else
                    {
                    }*/
                }
            }

            instanceCountCollection<String> output = new instanceCountCollection<string>();
            foreach (String n in counter.Keys)
            {
                output.AddInstance(n, Convert.ToInt32(Math.Sqrt(counter[n])));
            }

            output.reCalculate();
            return output;
        }

        /// <summary>
        /// Transforms the clouds, related
        /// </summary>
        /// <param name="settings">The settings.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="reductionReportName">Name of the reduction report.</param>
        /// <returns>
        /// Notes about reduced weights
        /// </returns>
        public cloudMatrixReductionReport TransformClouds(cloudMatrixSettings settings, ILogBuilder logger, String reductionReportName = "")
        {
            cloudMatrixReductionReport reductions = new cloudMatrixReductionReport();
            reductions.name = reductionReportName;

            instanceCountCollection<String> counter = GetCounter(false);
            List<String> passNames = new List<string>();
            List<String> removeNames = new List<string>();
            List<String> removeByLPFNames = new List<string>();
            List<String> setMiniNames = new List<string>();

            //  lemmaSemanticCloud cloud = this.First().Key;

            MinCloudFrequency = counter.minFreq;
            MaxCloudFrequency = counter.maxFreq;

            Double lowPass = settings.lowPassFilter;

            if (!settings.isActive)
            {
                logger.log("Cloud matrix disabled");
                return reductions;
            }
            if (settings.isFilterInAdaptiveMode)
            {
                lowPass = (MinCloudFrequency - 1) + lowPass;
                if (lowPass > MaxCloudFrequency) lowPass = MaxCloudFrequency;
                if (lowPass < 1) lowPass = 1;
                logger.log(": Cloud matrix filter in adaptive mode - cut off frequency set: " + lowPass);
            }

            var sorted = counter.getSorted();
            // <------------------------------------------------------------------------------------------ LOW PASS FILTER LIST
            List<String> doNotReduceWeight = new List<string>();
            foreach (String n in sorted) // <--------- performing cut of filter
            {
                if (settings.doCutOffByCloudFrequency)
                {
                    Int32 freq = counter[n];
                    Boolean passOk = true;

                    if (counter[n] > lowPass)
                    {
                        passOk = false;
                    }
                    if (passOk)
                    {
                        passNames.AddUnique(n);
                    }
                    else
                    {
                        if (settings.doAssignMicroWeightInsteadOfRemoval)
                        {
                            // passNames.AddUnique(n);

                            setMiniNames.AddUnique(n);

                            // reductions.Add("All", n,    "[" + n + "] weight set to the microWeightNoiseGate limit");
                            //                            doNotReduceWeight.Add(n);
                        }
                        else
                        {
                            removeByLPFNames.AddUnique(n);
                            //reductions.Add("[" + n + "] was removed");
                        }
                    }
                }
                else
                {
                    passNames.Add(n);
                }
            }

            // <------------------------------------------------------------------------------------------ LOW PASS FILTER LIST

            foreach (lemmaSemanticCloud y in this.Get1stKeys())
            {
                y.RebuildIndex();
                y.description = y.description + " filtered version of cloud";

                reductions.Nodes += y.CountNodes();
                reductions.InitialWeight += y.nodes.Sum(x => x.weight);
            }

            foreach (lemmaSemanticCloud cloud in this.Get1stKeys())
            {
                // <--- apply LPF

                foreach (String setMini in setMiniNames)
                {
                    var node = cloud.GetNode(setMini, true);
                    if (node != null)
                    {
                        reductions.Add(cloud.name, node.name, node.weight, settings.microWeightNoiseGate, cloudMatrixReductionAction.LowPassFilter);
                        node.weight = settings.microWeightNoiseGate;
                    }
                }

                if (settings.doDivideWeightWithCloudFrequency || settings.doUseSquareFunctionOfCF)
                {
                    Int32 rem = 0;
                    foreach (String n in passNames)
                    {
                        var node = cloud.GetNode(n, true);
                        if (node != null)
                        {
                            Double cf = counter[n];

                            if (settings.doDemoteAnyRepeatingSecondaryTerm)
                            {
                                if (cf > 1)
                                {
                                    if (node.type == 1)
                                    {
                                        node.type = 0;
                                        reductions.Add(cloud.name, node.name, node.weight, node.weight, cloudMatrixReductionAction.Demotion);

                                        //node.weight = node.weight * 0.5;
                                    }
                                }
                            }

                            if (settings.doRemoveAnyRepeatingPrimaryTerm)
                            {
                                if (cf > 1)
                                {
                                    if (node.type == 2)
                                    {
                                        reductions.Add(cloud.name, node.name, node.weight, 0, cloudMatrixReductionAction.Demotion);

                                        node.weight = 0;
                                    }
                                }
                            }
                            else if (settings.doDemoteAnyRepeatingPrimaryTerm)
                            {
                                if (cf > 1)
                                {
                                    if (node.type == 2)
                                    {
                                        reductions.Add(cloud.name, node.name, node.weight, node.weight, cloudMatrixReductionAction.Demotion);

                                        //node.weight = node.weight * 0.5;
                                        node.type = 1;
                                    }
                                }
                            }

                            if (!doNotReduceWeight.Contains(n))
                            {
                                if (node.weight > 0)
                                {
                                    //var cfd = cf + 1;

                                    if (cf > 1)
                                    {
                                        Double nw = node.weight;
                                        if (settings.doUseSquareFunctionOfCF)
                                        {
                                            node.weight = node.weight.GetRatio(cf * cf);
                                        }
                                        else
                                        {
                                            node.weight = node.weight.GetRatio(cf);
                                        }
                                        if (nw > node.weight)
                                        {
                                            reductions.Add(cloud.name, node.name, nw, node.weight, cloudMatrixReductionAction.CF_function);
                                            // reductions.Add("Term [" + node.name + "] weight [" + nw.ToString("F5") + "] reduced to [" + node.weight + "] in " + cloud.className + " CF[" + cf + "]");
                                        }
                                    }
                                }

                                if (node.weight > settings.microWeightNoiseGate)
                                {
                                }
                                else
                                {
                                    if (node.weight < settings.microWeightNoiseGate)
                                    {
                                        removeNames.AddUnique(n);
                                        //y.Remove(n);
                                        rem++;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            foreach (lemmaSemanticCloud y in this.Get1stKeys())
            {
                Int32 rem = 0;
                foreach (String n in removeNames)
                {
                    var node = y.GetNode(n);
                    if (y.Remove(n))
                    {
                        rem++;
                        reductions.Add(y.name, node.name, node.weight, 0, cloudMatrixReductionAction.Microweight);
                        //reductions.Add("Term [" + n + "] removed from [" + y.className + "]");
                    }
                }

                foreach (String n in removeByLPFNames)
                {
                    var node = y.GetNode(n);
                    if (y.Remove(n))
                    {
                        rem++;
                        reductions.Add(y.name, node.name, node.weight, 0, cloudMatrixReductionAction.LPFRemoval);
                    }
                }

                if (rem > 0)
                {
                    logger.log(y.className + ": Terms removed[" + rem.ToString("D6") + "] left[" + y.CountNodes().ToString("D6") + "]");
                }
            }

            foreach (lemmaSemanticCloud y in this.Get1stKeys())
            {
                y.RebuildIndex();
                y.description = y.description + " filtered version of cloud";

                //   reductions.Nodes += y.CountNodes();
                reductions.ReducedWeight += y.nodes.Sum(x => x.weight);
            }

            logger.log("Clouds transformation done.");

            return reductions;
        }

        /// <summary> Lowest cloud frequency among the lemmas </summary>
        [Category("Count")]
        [DisplayName("MinCloudFrequency")]
        [imb(imbAttributeName.measure_letter, "")]
        [imb(imbAttributeName.measure_setUnit, "n")]
        [Description("Lowest cloud frequency among the lemmas")] // [imb(imbAttributeName.measure_important)][imb(imbAttributeName.reporting_valueformat, "")]
        public Double MinCloudFrequency { get; set; } = 0;

        /// <summary> Highest cloud frequency among the lemmas </summary>
        [Category("Count")]
        [DisplayName("MaxCloudFrequency")]
        [imb(imbAttributeName.measure_letter, "")]
        [imb(imbAttributeName.measure_setUnit, "n")]
        [Description("Highest cloud frequency among the lemmas")] // [imb(imbAttributeName.measure_important)][imb(imbAttributeName.reporting_valueformat, "")]
        public Double MaxCloudFrequency { get; set; } = 0;

        /// <summary>
        /// Highest number of overlap nodes between two clouds
        /// </summary>
        /// <value>
        /// The maximum overlap.
        /// </value>
        public Int32 MaxOverlap { get; set; }

        /// <summary>
        /// Lower number of overlap nodes between two clouds
        /// </summary>
        /// <value>
        /// The minimum overlap.
        /// </value>
        public Int32 MinOverlap { get; set; }

        public Dictionary<lemmaSemanticCloud, Int32> numberOfLemmas = new Dictionary<lemmaSemanticCloud, int>();
        public Dictionary<lemmaSemanticCloud, Int32> numberOfLinks = new Dictionary<lemmaSemanticCloud, int>();

        /// <summary>
        /// Builds the specified clouds.
        /// </summary>
        /// <param name="clouds">The clouds.</param>
        public void build(IEnumerable<lemmaSemanticCloud> clouds, ILogBuilder logger)
        {
            var matrix = this;
            MaxOverlap = Int32.MinValue;
            MinOverlap = Int32.MaxValue;
            Int32 ci = 0;
            Int32 oc = 0;
            foreach (lemmaSemanticCloud x in clouds)
            {
                foreach (lemmaSemanticCloud y in clouds)
                {
                    if (matrix[x, y] == null)
                    {
                        if (x == y)
                        {
                            matrix[x, y] = x.GetOverlap(y);
                            matrix[y, x] = x.GetOverlap(y);
                        }
                        else
                        {
                            List<freeGraphNodeBase> overlap = x.GetOverlap(y);
                            matrix[x, y] = overlap;
                            matrix[y, x] = overlap;
                            MinOverlap = Math.Min(MinOverlap, overlap.Count);
                            MaxOverlap = Math.Max(MaxOverlap, overlap.Count);
                            oc++;
                        }
                    }
                }
                ci++;
                numberOfLemmas.Add(x, x.CountNodes());
                numberOfLinks.Add(x, x.CountLinks());
            }

            if (ci < 3)
            {
#if DEBUG
                throw new aceScienceException("MATRIX CLOUD PROBLEM", null, this, "MATRIX");
#endif
            }
            logger.log("Semantic Cloud matrix built from [" + ci + "] clouds - overlaps counted [" + oc + "]");
            logger.log("Max. overlap [" + MaxOverlap + "] - Min. overlap [" + MinOverlap + "]");
        }
    }
}