using System;
using System.Collections.Generic;
using System.Text;

namespace imbNLP.Toolkit.Documents
{
    /// <summary>
    /// Collection of textual layers
    /// </summary>
    /// <seealso cref="System.Collections.Generic.List{imbNLP.Toolkit.TextDocumentLayer}" />
    [Serializable]
    public class TextDocumentLayerCollection : List<TextDocumentLayer>
    {
        /// <summary>
        /// In case of web page (web document), the name is URL path (without domain name)
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string name { get; set; } = "";

        /// <summary>
        /// Returns factorized Length of all layers
        /// </summary>
        /// <returns></returns>
        public Int32 GetLength()
        {
            Int32 output = 0;

            foreach (var l in this)
            {
                output += l.length;

            }
            return output;
        }


        /// <summary>
        /// Labels attached to the document (i.e. categories)
        /// </summary>
        /// <value>
        /// The labels.
        /// </value>
        public List<String> labels { get; set; } = new List<string>();

        public TextDocumentLayer CreateLayer(String _name, String _content, Double w = 1)
        {
            TextDocumentLayer layer = new TextDocumentLayer(_content, _name, w);
            Add(layer);
            return layer;
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (var l in this)
            {
                if (l.length > 0)
                {
                    for (int i = 0; i < l.layerWeight; i++)
                    {
                        sb.AppendLine(l.content);
                    }
                }
            }
            return sb.ToString();
        }
    }

}