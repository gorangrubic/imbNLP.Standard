using System;
using System.Collections.Generic;

namespace imbNLP.Toolkit.Core
{


    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="imbNLP.Toolkit.Core.NLPBindable" />
    public abstract class ProcedureSetupBase : NLPBindable
    {

        public OperationReportEnum reportOptions { get; set; } = OperationReportEnum.tableExportText | OperationReportEnum.saveSetupXML;

        public List<String> descriptionAppendix { get; set; } = new List<string>();

        public string OutputFilename { get; set; } = "";

        public Boolean skipIfExisting { get; set; } = false;

        public Boolean useCacheProvider { get; set; } = false;


        public void LearnFrom(ProcedureSetupBase setup)
        {
            useCacheProvider = setup.useCacheProvider;
            reportOptions = setup.reportOptions;
            skipIfExisting = setup.skipIfExisting;
            descriptionAppendix.AddRange(setup.descriptionAppendix);
        }




    }
}