using System;
using System.Collections.Generic;

namespace imbNLP.Toolkit.Documents
{
    /// <summary>
    /// Layered text rendering of web content or other input document
    /// </summary>
    /// <seealso cref="System.Collections.Generic.List{imbNLP.Toolkit.TextDocumentLayerCollection}" />
    [Serializable]
    public class TextDocumentSet : List<TextDocumentLayerCollection>
    {
        //[XmlIgnore]

        //public TextDocument parent { get; set; }

        /// <summary>
        /// Name of document set (website, folder...). In case of web site, the name is web domain name
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