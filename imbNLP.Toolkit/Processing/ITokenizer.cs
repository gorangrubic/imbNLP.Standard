using System;
using System.Collections.Generic;

namespace imbNLP.Toolkit.Processing
{

    /// <summary>
    /// 
    /// </summary>
    public interface ITokenizer
    {
        string[] Tokenize(String text);

        Boolean LowerCase { get; set; }

        List<TokenizeReplaceRule> InputReplacers { get; set; }

    }

}