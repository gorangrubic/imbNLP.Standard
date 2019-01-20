using imbNLP.Toolkit.Core;
using imbNLP.Toolkit.Entity;
using imbNLP.Toolkit.Planes.Core;
using imbSCI.Core.reporting;
using System;
using System.Collections.Generic;

namespace imbNLP.Toolkit.Planes
{
public abstract class PlaneSettingsBase : ProcedureSetupBase, IPlaneSettings
    {
        public string cachePath { get; set; } = "";

        public abstract void Describe(ILogBuilder logger);

    }
}