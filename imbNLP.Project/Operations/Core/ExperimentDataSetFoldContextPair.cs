using imbNLP.Toolkit.Core;
using imbNLP.Toolkit.ExperimentModel;
using System;

namespace imbNLP.Project.Operations.Core
{


    public class ExperimentDataSetFoldContextPair<TC> where TC : IOperationExecutionContext, new()
    {
        public ExperimentDataSetFoldContextPair(ExperimentDataSetFold _fold, TC _context)
        {
            fold = _fold;

            if (_context != null)
            {
                context = _context;
            }
            else
            {
                context = new TC();
            }
        }

        public ExperimentDataSetFold fold { get; set; }

        public TC context { get; set; }

    }
}