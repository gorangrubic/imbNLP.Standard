using System;
using System.Xml.Serialization;

namespace imbNLP.Toolkit.Documents
{

    /// <summary>
    /// Textual component of the document
    /// </summary>
    [Serializable]
    public class TextDocumentLayer
    {
        public const String MAINTEXT_LAYER = "text";

        public TextDocumentLayer() { }

        public TextDocumentLayer(String _content, String _name, Double w = 1)
        {
            name = _name;
            content = _content;
            layerWeight = w;
        }

        public Double layerWeight { get; set; } = 1;

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


        /// <summary>
        /// Gets the length.
        /// </summary>
        /// <value>
        /// The length.
        /// </value>
        [XmlIgnore]
        public Int32 length
        {
            get
            {
                return content.Length;
            }
        }
    }

}