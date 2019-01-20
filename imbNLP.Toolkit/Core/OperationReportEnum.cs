using System;

namespace imbNLP.Toolkit.Core
{

    [Flags]
    public enum OperationReportEnum
    {
        none = 0,
        reportRenderingLayers = 1,
        reportBlendedRenders = 2,

        reportPreblendFilter = 4,

        reportSpaceModel = 8,
        reportFeatures = 16,

        tableExportText = 32,
        reportClassification = 64,
        tableExportExcel = 128,
        reportDataset = 256,
        saveSetupXML = 512,

        standard = saveSetupXML | tableExportText,
        all = reportRenderingLayers | reportBlendedRenders | reportPreblendFilter | reportSpaceModel | reportFeatures | tableExportExcel | tableExportText | reportClassification | reportDataset | saveSetupXML | randomSampledDemo,
        randomSampledDemo = 1024
    }
}
