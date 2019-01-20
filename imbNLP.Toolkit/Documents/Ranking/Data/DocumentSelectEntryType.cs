using imbNLP.Toolkit.Documents.Ranking.Core;
using imbNLP.Toolkit.Space;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace imbNLP.Toolkit.Documents.Ranking.Data
{
[Flags]
    public enum DocumentSelectEntryType
    {
        none = 0,
        unknown = 1,
        textDocument = 2,
        spaceDocument = 4,
        webDocument = 8,
    }
}