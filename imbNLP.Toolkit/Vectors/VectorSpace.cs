using System.Collections.Generic;

namespace imbNLP.Toolkit.Vectors
{

    /// <summary>
    /// Vector space
    /// </summary>
    public class VectorSpace
    {
        public VectorSpace()
        {

        }

        // 


        //public Relationships<VectorLabel, VectorDocument> labelToDocumentAssociations { get; set; } = new Relationships<VectorLabel, VectorDocument>();

        /// <summary>
        /// Gets or sets the documents.
        /// </summary>
        /// <value>
        /// The documents.
        /// </value>
        public List<VectorDocument> documents { get; set; } = new List<VectorDocument>();
        /// <summary>
        /// Gets or sets the labels.
        /// </summary>
        /// <value>
        /// The labels.
        /// </value>
        public List<VectorLabel> labels { get; set; } = new List<VectorLabel>();

    }

}