using imbSCI.Core.attributes;
using imbSCI.Core.math;
using System;
using System.ComponentModel;

namespace imbNLP.PartOfSpeech.TFModels.semanticCloudMatrix
{
    /// <summary>
    /// Represents single action done by <see cref="cloudMatrix"/> filtration functions
    /// </summary>
    public class cloudMatrixReductionReportEntry
    {
        /// <summary>
        /// Format used for <see cref="ToString()"/>
        /// </summary>
        public const String ToStringFormat = "C:{0,-20} T:{1,-25} Wi:{2,10:F6} -> Wr:{3,10:F6} A:{4,-10}";

        /// <summary>
        /// Returns a formatted <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return String.Format(ToStringFormat, Cloud, Term, Initial, Reduced, Action);
        }

        /// <summary>
        /// Never use this constructor, it's for serialization. Use <see cref="cloudMatrixReductionReport.Add(string, string, double, double, cloudMatrixReductionAction)"/> method
        /// </summary>
        public cloudMatrixReductionReportEntry() { }

        /// <summary> Name of the cloud that owns the term </summary>
        [Category("Label")]
        [DisplayName("Cloud")] //[imb(imbAttributeName.measure_letter, "")]
        [Description("Name of the cloud that owns the term")] // [imb(imbAttributeName.reporting_escapeoff)]
        public String Cloud { get; set; } = default(String);

        /// <summary> Term that was affected by reduction </summary>
        [Category("Label")]
        [DisplayName("Term")] //[imb(imbAttributeName.measure_letter, "")]
        [Description("Term that was affected by reduction")] // [imb(imbAttributeName.reporting_escapeoff)]
        public String Term { get; set; } = default(String);

        /// <summary> Ratio </summary>
        [Category("Weight")]
        [DisplayName("Initial")]
        [imb(imbAttributeName.measure_letter, "W_i")]
        [imb(imbAttributeName.measure_setUnit, "w")]
        [imb(imbAttributeName.reporting_valueformat, "F3")]
        [imb(imbAttributeName.reporting_columnWidth, 7)]
        [Description("Initial weight the term had in the category/cloud")] // [imb(imbAttributeName.measure_important)][imb(imbAttributeName.reporting_valueformat, "")][imb(imbAttributeName.reporting_escapeoff)]
        public Double Initial { get; set; } = default(Double);

        [Category("Weight")]
        [DisplayName("Reduced")]
        [imb(imbAttributeName.measure_letter, "W_r")]
        [imb(imbAttributeName.measure_setUnit, "w")]
        [imb(imbAttributeName.reporting_valueformat, "F3")]
        [imb(imbAttributeName.reporting_columnWidth, 7)]
        [Description("Reduced weight the term had in the category/cloud")] // [imb(imbAttributeName.measure_important)][imb(imbAttributeName.reporting_valueformat, "")][imb(imbAttributeName.reporting_escapeoff)]
        public Double Reduced { get; set; } = default(Double);

        /// <summary> Type of action that was applied </summary>
        [Category("Label")]
        [DisplayName("Action")] //[imb(imbAttributeName.measure_letter, "")]
        [Description("Type of action that was applied")] // [imb(imbAttributeName.reporting_escapeoff)]
        public cloudMatrixReductionAction Action { get; set; } = default(cloudMatrixReductionAction);

        /// <summary> Hash - used to identify this particular action during report creation </summary>
        [Category("ID")]
        [DisplayName("Hash")] //[imb(imbAttributeName.measure_letter, "")]
        [Description("Hash - used to identify this particular action during report creation")] // [imb(imbAttributeName.reporting_escapeoff)]
        public String Hash
        {
            get
            {
                return md5.GetMd5Hash(Cloud + Term + Initial + Reduced + Action);
            }
            set
            {
            }
        }
    }
}