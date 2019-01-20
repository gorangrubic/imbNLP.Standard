using imbNLP.Toolkit.Core;
using imbNLP.Toolkit.ExperimentModel;
using imbSCI.Core.data.cache;
using imbSCI.Core.files;
using imbSCI.Core.reporting;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace imbNLP.Project.Operations.Core
{
/// <summary>
    /// Stack of procedures
    /// </summary>
    /// <seealso cref="System.Collections.Generic.Stack{imbNLP.Project.Operations.Core.IProceduralFolderFor}" />
    public class ProceduralStack : Stack<IProceduralFolderFor>
    {

    }
}