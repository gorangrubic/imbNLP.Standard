using imbNLP.Toolkit.Core;
using System;

namespace imbNLP.Toolkit.Documents
{



    /// <summary>
    /// Simple document model
    /// </summary>
    /// <seealso cref="imbNLP.Toolkit.TextDocumentBase" />
    [Serializable]
    public class TextDocument : TextDocumentBase, ITextCached
    {
        /// <summary>
        /// Content of the document
        /// </summary>
        /// <value>
        /// The content.
        /// </value>
        public string content { get; set; } = "";

        public Boolean HasContent
        {
            get
            {
                return content != "";
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TextDocument"/> class.
        /// </summary>
        /// <param name="__content">The content.</param>
        public TextDocument(string __content = "")
        {
            content = __content;
        }

        public TextDocument()
        {


        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return content;
        }

        public void FromString(string text)
        {
            content = text;
        }
    }

}