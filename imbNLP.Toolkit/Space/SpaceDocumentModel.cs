using imbNLP.Toolkit.Processing;
using System.Xml.Serialization;

namespace imbNLP.Toolkit.Space
{

    /// <summary>
    /// Model of an document, sent for analysis by a algorithm in the toolkit
    /// </summary>
    /// <remarks>
    /// <para>The frequency property of DocumentModel, represents total number of tokens that are represented by the model (i.e. sum of all absolute frequencies of the LemmaTerm)</para>
    /// </remarks>
    public class SpaceDocumentModel : SpaceTerm
    {
        /// <summary>
        /// Index of words
        /// </summary>
        /// <value>
        /// The words.
        /// </value>
        public int[] Words;

        /// <summary>
        /// The length - number of tokens
        /// </summary>
        public int Length;

        /// <summary>
        /// Terms of the document
        /// </summary>
        /// <value>
        /// The terms.
        /// </value>
        [XmlIgnore]
        public TokenDictionary terms { get; set; } = new TokenDictionary();

        /// <summary>
        /// Initializes a new instance of the <see cref="SpaceDocumentModel"/> class.
        /// </summary>
        public SpaceDocumentModel()
        {
        }
    }
}