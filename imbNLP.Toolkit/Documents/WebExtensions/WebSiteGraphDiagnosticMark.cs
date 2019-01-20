
using System;

namespace imbNLP.Toolkit.Documents.WebExtensions
{
    
    [Flags]
    public enum WebSiteGraphDiagnosticMark
    {
        none = 0,

        evaluated = 1,
        error = 2,
        graphNotInExtensions = 4,
        conversionErrors = 8,
        graphFractured = 16,
        linkNoLabel = 32,
        NoPagesLoaded = 64,
        OnlyHomePageLoaded = 128,
        graphEmpty = 256,
    }
}