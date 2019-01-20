using imbNLP.Toolkit.Core;
using imbSCI.Core.reporting;
using System;

namespace imbNLP.Toolkit.Planes.Core
{

    public abstract class PlaneMethodFunctionBase : IDescribe, IPlaneMethodFunction
    {



        public Boolean IsEnabled { get; set; } = true;

        public abstract void Describe(ILogBuilder logger);
    }
}
