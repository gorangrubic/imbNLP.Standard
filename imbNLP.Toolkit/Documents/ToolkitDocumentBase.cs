using System;
using System.Collections.Generic;

namespace imbNLP.Toolkit.Documents
{
    public abstract class TextDocumentBase
    {
        protected TextDocumentBase()
        {
        }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public String name { get; set; } = "";

        /// <summary>
        /// Labels attached to the document (i.e. categories)
        /// </summary>
        /// <value>
        /// The labels.
        /// </value>
        public List<String> labels { get; set; } = new List<string>();
    }
}