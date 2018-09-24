using System;
using System.Collections.Generic;
using System.Text;

namespace imbNLP.Toolkit
{

    /// <summary>
    /// Textual component of the document
    /// </summary>
    [Serializable]
    public class TextDocumentLayer
    {
        public const String MAINTEXT_LAYER = "text";

        public TextDocumentLayer() { }

        public TextDocumentLayer(String _content, String _name, Int32 w = 1)
        {
            name = _name;
            content = _content;
            layerWeight = w;
        }

        public Int32 layerWeight { get; set; } = 1;

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string name { get; set; } = "";

        /// <summary>
        /// Content of the document
        /// </summary>
        /// <value>
        /// The content.
        /// </value>
        public string content { get; set; } = "";
    }

}