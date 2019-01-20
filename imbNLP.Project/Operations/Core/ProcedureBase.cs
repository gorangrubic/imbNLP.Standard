using imbNLP.Toolkit.ExperimentModel;

using imbSCI.Core.extensions.data;
using imbSCI.Core.extensions.text;
using System;

namespace imbNLP.Project.Operations.Core
{


    public abstract class ProcedureBase
    {

        public ProceduralStackOptions executionOptions { get; set; } = ProceduralStackOptions.none;

        /// <summary>
        /// Position at Procedural folder
        /// </summary>
        /// <value>
        /// The index.
        /// </value>
        public Int32 index { get; set; } = 0;


        public ExperimentDataSetFold fold { get; set; }

        protected ProcedureBase()
        {

        }

        protected ProcedureBase(ExperimentDataSetFold _fold)
        {
            fold = _fold;
        }

        private String _name;

        public String name
        {
            get
            {
                if (_name.isNullOrEmpty())
                {
                    _name = this.GetType().Name.imbTitleCamelOperation(true).imbGetAbbrevation(8, true);
                }
                return _name;
            }
            set { _name = value; }
        }

        public String description { get; set; } = "";


        ///// <summary>
        ///// Gets or sets the report options.
        ///// </summary>
        ///// <value>
        ///// The report options.
        ///// </value>



        //public ToolkitExperimentNotes StartSubNote(ToolkitExperimentNotes notes, ExperimentDataSetFold fold)
        //{

        //    var node_notes = fold.StartNote(notes, this.GetType().Name);

        //    String desc = "Execution record of [" + this.GetType().Name + "] for [" + fold.name + "] --- " + description;

        //    return node_notes.StartSubnotes(name, desc);
        //    //String desc = "Executing fold [" + foldName + "] for [" + this.GetType().Name + "]";
        //    //var subnotes = notes.StartSubnotes(foldName, desc);

        //    //subnotes.logStartPhase("Fold [" + foldName + "]", desc);

        //    //return subnotes;

        //}

        //public ToolkitExperimentNotes EndNote(ToolkitExperimentNotes notes, ExperimentDataSetFold fold)
        //{


        //    return fold.EndNote(notes);
        //}
    }
}