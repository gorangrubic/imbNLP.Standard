using imbNLP.Toolkit.Documents;
using imbSCI.Data;
using System;
using System.Collections.Generic;

namespace imbNLP.Toolkit.ExperimentModel
{

    /// <summary>
    /// One experimental "fold" with training and testing documents.
    /// </summary>
    /// <seealso cref="System.Collections.Generic.List{imbNLP.Toolkit.Documents.WebSiteDocumentsSet}" />
    public class ExperimentDataSetFold : List<WebSiteDocumentsSet>
    {
        public ExperimentDataSetFold() { }

        public ExperimentDataSetFold(IEnumerable<WebSiteDocumentsSet> dataset, String __name)
        {
            name = __name;
            foreach (var cat in dataset)
            {
                Add(cat);
            }
        }

        ///// <summary>
        ///// Notes attached to this fold
        ///// </summary>
        ///// <value>
        ///// The notes.
        ///// </value>
        //public { get; protected set; }




        public ToolkitExperimentNotes StartNote(ToolkitExperimentNotes _notes, String callerName = "")
        {

            ToolkitExperimentNotes notes = null;

            String desc = "Executing fold [" + name + "]";
            if (!callerName.isNullOrEmpty())
            {
                desc = desc + "for [" + callerName + "]";
            }



            notes = _notes.StartSubnotes(name, desc);

            imbSCI.Core.screenOutputControl.logToConsoleControl.removeFromOutput(notes);

            // notes.logStartPhase("Fold [" + name + "]", desc);

            return notes;
            //return subnotes;

        }

        public void EndNote()
        {
            //notes.logEndPhase();

        }


        /// <summary>
        /// Copies the label names.
        /// </summary>
        /// <param name="input">The input.</param>
        public void CopyLabelNames(IEnumerable<WebSiteDocumentsSet> input)
        {
            foreach (WebSiteDocumentsSet s in input)
            {
                WebSiteDocumentsSet tmp = new WebSiteDocumentsSet();
                tmp.name = s.name;
                tmp.description = s.description;
                Add(tmp);
            }
        }

        /// <summary>
        /// Identified of the fold
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string name { get; set; } = "";
    }

}