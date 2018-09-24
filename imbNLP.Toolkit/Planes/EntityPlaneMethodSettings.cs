using imbNLP.Toolkit.Entity;
using imbNLP.Toolkit.Planes.Core;
using imbSCI.Core.reporting;
using System;
using System.Collections.Generic;

namespace imbNLP.Toolkit.Planes
{


    public class EntityPlaneMethodSettings : IPlaneSettings
    {

        public List<DocumentRenderInstruction> instructions { get; set; } = new List<DocumentRenderInstruction>();



        public DocumentBlenderFunctionOptions blenderOptions { get; set; } = DocumentBlenderFunctionOptions.binaryAggregation | DocumentBlenderFunctionOptions.pageLevel | DocumentBlenderFunctionOptions.uniqueContentUnitsOnly;

        public string filterFunctionName { get; set; } = "DocumentEntropyFunction";

        public Int32 filterLimit { get; set; } = 15;


        public EntityPlaneMethodSettings()
        {

        }

        public void Describe(ILogBuilder logger)
        {

        }
    }

}