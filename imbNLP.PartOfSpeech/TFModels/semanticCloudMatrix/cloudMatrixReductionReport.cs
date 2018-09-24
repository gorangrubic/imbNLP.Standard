using imbSCI.Core.attributes;
using imbSCI.Core.extensions.table;
using imbSCI.Core.extensions.text;
using imbSCI.Core.math.range.finder;
using imbSCI.DataComplex.tables;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Text;

namespace imbNLP.PartOfSpeech.TFModels.semanticCloudMatrix
{
    public class cloudMatrixReductionReport
    {
        public cloudMatrixReductionReport()
        {
        }

        /// <summary> Name for the report </summary>
        [Category("Label")]
        [DisplayName("name")] //[imb(imbAttributeName.measure_letter, "")]
        [Description("Name for the report")] // [imb(imbAttributeName.reporting_escapeoff)]
        public String name { get; set; } = default(String);

        [Category("Label")]
        [DisplayName("Terms")]
        [imb(imbAttributeName.measure_letter, "")]
        [imb(imbAttributeName.measure_setUnit, "n")]
        [imb(imbAttributeName.reporting_columnWidth, 7)]
        [Description("Number of terms after matrix applied")] // [imb(imbAttributeName.measure_important)][imb(imbAttributeName.reporting_valueformat, "")]
        public Int32 Nodes { get; set; } = default(Int32);

        /// <summary> Total actions - how many actions were taken by the Cloud Matrix </summary>
        [Category("Actions")]
        [DisplayName("Total")]
        [imb(imbAttributeName.measure_letter, "")]
        [imb(imbAttributeName.measure_setUnit, "n")]
        [imb(imbAttributeName.reporting_columnWidth, 7)]
        [Description("Total actions - how many actions were taken by the Cloud Matrix")] // [imb(imbAttributeName.measure_important)][imb(imbAttributeName.reporting_valueformat, "")]
        public Int32 Actions { get; set; } = default(Int32);

        /// <summary> Number of terms got reduction by CF function </summary>
        [Category("Actions")]
        [DisplayName("CF")]
        [imb(imbAttributeName.measure_letter, "CF")]
        [imb(imbAttributeName.measure_setUnit, "n")]
        [imb(imbAttributeName.reporting_columnWidth, 7)]
        [Description("Number of terms got reduction by CF function")] // [imb(imbAttributeName.measure_important)][imb(imbAttributeName.reporting_valueformat, "")]
        public Int32 CFFunctionReduction { get; set; } = default(Int32);

        /// <summary> Number of terms got reduction by LP filter </summary>
        [Category("Actions")]
        [DisplayName("LPF")]
        [imb(imbAttributeName.measure_letter, "LPF")]
        [imb(imbAttributeName.measure_setUnit, "n")]
        [imb(imbAttributeName.reporting_columnWidth, 7)]
        [Description("Number of terms got reduction by LP filter")] // [imb(imbAttributeName.measure_important)][imb(imbAttributeName.reporting_valueformat, "")]
        public Int32 LPFilterReduction { get; set; } = default(Int32);

        /// <summary> Number of terms removed </summary>
        [Category("Removals")]
        [DisplayName("LPF")]
        [imb(imbAttributeName.measure_letter, "")]
        [imb(imbAttributeName.measure_setUnit, "n")]
        [imb(imbAttributeName.reporting_columnWidth, 7)]
        [Description("Number of terms removed by the Low Pass Filter")] // [imb(imbAttributeName.measure_important)][imb(imbAttributeName.reporting_valueformat, "")]
        public Int32 LPFRemovals { get; set; } = default(Int32);

        /// <summary> Number of terms removed by microweight treshhold </summary>
        [Category("Removals")]
        [DisplayName("MW")]
        [imb(imbAttributeName.measure_letter, "")]
        [imb(imbAttributeName.measure_setUnit, "n")]
        [imb(imbAttributeName.reporting_columnWidth, 7)]
        [Description("Number of terms removed by microweight treshhold")] // [imb(imbAttributeName.measure_important)][imb(imbAttributeName.reporting_valueformat, "")]
        public Int32 MWRemovals { get; set; } = default(Int32);

        /// <summary> Ratio </summary>
        [Category("Weight")]
        [DisplayName("Initial")]
        [imb(imbAttributeName.measure_letter, "W")]
        [imb(imbAttributeName.measure_setUnit, "w")]
        [imb(imbAttributeName.reporting_valueformat, "F3")]
        [imb(imbAttributeName.reporting_columnWidth, 7)]
        [Description("Total initial weight of all clouds")] // [imb(imbAttributeName.measure_important)][imb(imbAttributeName.reporting_valueformat, "")][imb(imbAttributeName.reporting_escapeoff)]
        public Double InitialWeight { get; set; } = default(Double);

        [Category("Weight")]
        [DisplayName("Reduced")]
        [imb(imbAttributeName.measure_letter, "W")]
        [imb(imbAttributeName.measure_setUnit, "w")]
        [imb(imbAttributeName.reporting_valueformat, "F3")]
        [imb(imbAttributeName.reporting_columnWidth, 7)]
        [Description("Total reduced weight of all clouds, of affected terms")] // [imb(imbAttributeName.measure_important)][imb(imbAttributeName.reporting_valueformat, "")][imb(imbAttributeName.reporting_escapeoff)]
        public Double ReducedWeight { get; set; } = default(Double);

        [Category("Weight")]
        [DisplayName("Affected")]
        [imb(imbAttributeName.measure_letter, "W")]
        [imb(imbAttributeName.measure_setUnit, "w")]
        [imb(imbAttributeName.reporting_valueformat, "F3")]
        [imb(imbAttributeName.reporting_columnWidth, 7)]
        [Description("Sum of initial weights, of affected terms")] // [imb(imbAttributeName.measure_important)][imb(imbAttributeName.reporting_valueformat, "")][imb(imbAttributeName.reporting_escapeoff)]
        public Double TotalWeight { get; set; } = default(Double);

        /// <summary> Ratio </summary>
        [Category("Reduction Weight")]
        [DisplayName("Total")]
        [imb(imbAttributeName.measure_letter, "W")]
        [imb(imbAttributeName.measure_setUnit, "w")]
        [imb(imbAttributeName.reporting_columnWidth, 7)]
        [imb(imbAttributeName.reporting_valueformat, "F3")]
        [Description("Sum of all reductions made")] // [imb(imbAttributeName.measure_important)][imb(imbAttributeName.reporting_escapeoff)]
        public Double TotalReduction { get; set; } = default(Double);

        /// <summary> Ratio </summary>
        [Category("Reduction Weight")]
        [DisplayName("LPF")]
        [imb(imbAttributeName.measure_letter, "W")]
        [imb(imbAttributeName.measure_setUnit, "w")]
        [imb(imbAttributeName.reporting_valueformat, "F3")]
        [imb(imbAttributeName.reporting_columnWidth, 7)]
        [Description("Sum of reductions made by LPF")] // [imb(imbAttributeName.measure_important)][imb(imbAttributeName.reporting_valueformat, "")][imb(imbAttributeName.reporting_escapeoff)]
        public Double LPFReductionWeight { get; set; } = default(Double);

        /// <summary> Ratio </summary>
        [Category("Reduction Weight")]
        [DisplayName("CF")]
        [imb(imbAttributeName.measure_letter, "W")]
        [imb(imbAttributeName.measure_setUnit, "w")]
        [imb(imbAttributeName.reporting_valueformat, "F3")]
        [imb(imbAttributeName.reporting_columnWidth, 7)]
        [Description("Sum of reductions made by CF Function")] // [imb(imbAttributeName.measure_important)][imb(imbAttributeName.reporting_valueformat, "")][imb(imbAttributeName.reporting_escapeoff)]
        public Double CFReductionWeight { get; set; } = default(Double);

        protected List<cloudMatrixReductionReportEntry> entries = new List<cloudMatrixReductionReportEntry>();

        /// <summary>
        /// Adds action in the report
        /// </summary>
        /// <param name="cloud">The cloud name</param>
        /// <param name="term">The term being affected.</param>
        /// <param name="initial">The initial weight.</param>
        /// <param name="reduced">The reduced weight.</param>
        /// <param name="action">The action that was taken.</param>
        public void Add(String cloud, String term, Double initial, Double reduced, cloudMatrixReductionAction action)
        {
            cloudMatrixReductionReportEntry entry = new cloudMatrixReductionReportEntry();
            entry.Action = action;
            entry.Initial = initial;
            entry.Reduced = reduced;
            entry.Cloud = cloud;
            entry.Term = term;
            entries.Add(entry);

            switch (action)
            {
                case cloudMatrixReductionAction.LPFRemoval:
                    reduced = 0;
                    break;

                case cloudMatrixReductionAction.Microweight:
                    reduced = 0;
                    break;

                case cloudMatrixReductionAction.unknown:
                    break;

                default:
                    break;
            }

            Double reduction = initial - reduced;
            switch (action)
            {
                case cloudMatrixReductionAction.CF_function:
                    CFFunctionReduction++;
                    CFReductionWeight += reduction;
                    break;

                case cloudMatrixReductionAction.LowPassFilter:
                    LPFilterReduction++;
                    LPFReductionWeight += reduction;
                    break;

                case cloudMatrixReductionAction.LPFRemoval:
                    LPFRemovals++;
                    break;

                case cloudMatrixReductionAction.Microweight:
                    MWRemovals++;
                    break;

                case cloudMatrixReductionAction.unknown:
                    break;

                default:
                    break;
            }
            TotalReduction += reduction;
            TotalWeight += initial;
            Actions++;
        }

        /// <summary>
        /// Gets the data table, with all actions
        /// </summary>
        /// <returns></returns>
        public DataTable GetDataTable()
        {
            DataTableTypeExtended<cloudMatrixReductionReportEntry> output = new DataTableTypeExtended<cloudMatrixReductionReportEntry>(name, "Actions done by cloud matrix [" + name + "]");

            rangeFinderForDataTable ranger = new rangeFinderForDataTable(output, nameof(cloudMatrixReductionReportEntry.Hash));

            foreach (var entry in entries)
            {
                output.AddRow(entry);
            }

            output.SetAdditionalInfoEntry(nameof(TotalReduction).imbTitleCamelOperation(true), TotalReduction);
            output.SetAdditionalInfoEntry(nameof(CFFunctionReduction).imbTitleCamelOperation(true), CFFunctionReduction);
            output.SetAdditionalInfoEntry(nameof(LPFilterReduction).imbTitleCamelOperation(true), LPFilterReduction);
            output.SetAdditionalInfoEntry(nameof(LPFRemovals).imbTitleCamelOperation(true), LPFRemovals);
            output.SetAdditionalInfoEntry(nameof(MWRemovals).imbTitleCamelOperation(true), MWRemovals);

            return output;
        }

        /// <summary>
        /// Returns complete string report on reductions that were done by the cloud matrix filtration functions
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("## Cloud Matrix reduction report");

            sb.AppendLine("- Total actions: " + Actions);
            sb.AppendLine("- Total reduction: " + TotalReduction);
            sb.AppendLine("- Total removals: " + LPFRemovals + MWRemovals);

            sb.AppendLine("----------------------------------------------------------------------");

            foreach (var entry in entries)
            {
                sb.AppendLine(entry.ToString());
            }

            return sb.ToString();
        }
    }
}