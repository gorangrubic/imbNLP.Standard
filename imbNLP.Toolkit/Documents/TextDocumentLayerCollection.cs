using System;
using System.Collections.Generic;
using System.Text;

namespace imbNLP.Toolkit
{
    [Serializable]
    public class TextDocumentLayerCollection : List<TextDocumentLayer>
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string name { get; set; } = "";

        public TextDocumentLayer CreateLayer(String _name, String _content, Int32 w = 1)
        {
            TextDocumentLayer layer = new TextDocumentLayer(_content, _name, w);
            Add(layer);
            return layer;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (var l in this)
            {
                for (int i = 0; i < l.layerWeight; i++)
                {
                    sb.AppendLine(l.content);
                }

            }
            return sb.ToString();
        }
    }

}