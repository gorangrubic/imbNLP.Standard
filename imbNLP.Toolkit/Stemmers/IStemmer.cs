using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace imbNLP.Toolkit.Stemmers
{

    public interface IStemmer
    {
        Boolean Stem();
        void SetCurrent(String input);
        String GetCurrent();

    }

}