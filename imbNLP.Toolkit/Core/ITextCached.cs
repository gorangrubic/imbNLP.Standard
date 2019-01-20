using imbNLP.Toolkit.Documents;
using imbSCI.Core.extensions.io;
using imbSCI.Core.math;
using imbSCI.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace imbNLP.Toolkit.Core
{
public interface ITextCached
    {
        String ToString();
        void FromString(String text);
    }
}