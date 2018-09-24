using System;
using System.Collections.Generic;

namespace imbNLP.Toolkit
{
    [Serializable]
    public class TextDocumentSet : List<TextDocumentLayerCollection>
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string name { get; set; } = "";

        public TextDocumentSet()
        {

        }

        public TextDocumentSet(String _name)
        {
            name = _name;
        }


    }

}