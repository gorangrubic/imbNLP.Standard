using imbNLP.Toolkit.ExperimentModel;

using imbSCI.Core.files;
using imbSCI.Core.reporting;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace imbNLP.Project.Operations.Core
{
    public class ProceduralFolder<T> : List<T> where T : ProcedureBase, new()
    {

        public ProceduralFolder(IList<ExperimentDataSetFold> folds)
        {
            foreach (ExperimentDataSetFold fold in folds)
            {
                T procedure = new T();
                procedure.fold = fold;
                Add(procedure);
            }

        }




    }
}