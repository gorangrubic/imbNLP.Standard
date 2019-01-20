using System;

namespace imbNLP.Toolkit.Core
{
    public class nlpException : Exception
    {

        public String explanation { get; set; } = "";

        public nlpException(String title, String _explanation) : base(title)
        {
            explanation = _explanation;
        }

    }
}