using System;
using System.Linq;
using System.Collections.Generic;
using imbSCI.Core.reporting;
using imbSCI.Data.interfaces;
using System.ComponentModel;

namespace imbNLP.Toolkit.Core
{

    /// <summary>
    /// Object having Describe method 
    /// </summary>
    public interface IDescribe
    {

        void Describe(ILogBuilder logger);

    }
}