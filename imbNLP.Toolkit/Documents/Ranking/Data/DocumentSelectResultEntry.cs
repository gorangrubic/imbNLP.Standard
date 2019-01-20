using imbNLP.Toolkit.Documents.Ranking.Core;
using imbNLP.Toolkit.Space;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace imbNLP.Toolkit.Documents.Ranking.Data
{
    /// <summary>
    /// 
    /// </summary>
//    [XmlElement(ElementName="entry")]
    [XmlRoot(ElementName = "Entry")]
    public class DocumentSelectResultEntry : IAssignedID
    {
        public DocumentSelectResultEntry()
        {

        }

        public DocumentSelectResultEntry(TextDocument document)
        {
            textDocument = document;
            AssignedID = document.name;
            type = DocumentSelectEntryType.textDocument;
        }

        public DocumentSelectResultEntry(SpaceDocumentModel document)
        {
            spaceDocument = document;
            AssignedID = document.name;
            type = DocumentSelectEntryType.spaceDocument;
        }

        public DocumentSelectResultEntry(WebSiteDocument document)
        {
            AssignedID = document.AssignedID;
            type = DocumentSelectEntryType.webDocument;
            webDocument = document;
        }

        public void SetEntry(String _domainID, WebSiteDocument _webDocument, SpaceDocumentModel _spaceDocument, TextDocument _textDocument)
        {

            type = DocumentSelectEntryType.unknown;
            DomainID = _domainID;

            webDocument = _webDocument;
            spaceDocument = _spaceDocument;
            textDocument = _textDocument;

            if (textDocument != null)
            {
                AssignedID = textDocument.name;
                type |= DocumentSelectEntryType.textDocument;
            }
            if (spaceDocument != null)
            {
                AssignedID = spaceDocument.name;
                type |= DocumentSelectEntryType.spaceDocument;
            }
            if (webDocument != null)
            {
                AssignedID = _webDocument.AssignedID;
                type |= DocumentSelectEntryType.webDocument;
            }
        }

        [XmlElement(ElementName = "aID")]
        public String AssignedID { get; set; } = "";

        [XmlElement(ElementName = "dID")]
        public String DomainID { get; set; } = "";


        [XmlIgnore]
        public Boolean HasTextOrSpaceModel
        {
            get
            {

                if (textDocument != null) return true;
                if (spaceDocument != null) return true;
                return false;
            }
        }


        /// <summary>
        /// Gets the type of entry
        /// </summary>
        /// <value>
        /// The type.
        /// </value>
        [XmlIgnore]
        public DocumentSelectEntryType type { get; set; }


        [XmlIgnore]
        private Dictionary<IScoreModelFactor, Double> AbsoluteFactorScores { get; set; } = new Dictionary<IScoreModelFactor, double>();

        /// <summary>
        /// Normalized and weighted factor scores
        /// </summary>
        /// <value>
        /// The factor scores.
        /// </value>
        [XmlIgnore]
        private Dictionary<IScoreModelFactor, Double> FactorScores { get; set; } = new Dictionary<IScoreModelFactor, double>();



        /// <summary>
        /// Absolute score for the factor specified
        /// </summary>
        /// <param name="factor">The factor.</param>
        /// <param name="score">The score.</param>
        /// <param name="SetToAbsolute">if set to <c>true</c> it will set to absolute score registry, otherwise it will set to final factor scores.</param>
        public void SetScore(IScoreModelFactor factor, Double score, Boolean SetToAbsolute = true)
        {
            Dictionary<IScoreModelFactor, Double> Scores = AbsoluteFactorScores;
            if (!SetToAbsolute)
            {
                Scores = FactorScores;
            }
            if (!Scores.ContainsKey(factor)) Scores.Add(factor, 0);
            Scores[factor] = score;
        }

        /// <summary>
        /// Gets the score
        /// </summary>
        /// <param name="factor">The factor.</param>
        /// <param name="GetAbsolute">if set to <c>true</c> [get absolute].</param>
        /// <returns></returns>
        public Double GetScore(IScoreModelFactor factor, Boolean GetAbsolute = true)
        {
            Dictionary<IScoreModelFactor, Double> Scores = AbsoluteFactorScores;
            if (!GetAbsolute)
            {
                Scores = FactorScores;
            }
            if (!Scores.ContainsKey(factor)) Scores.Add(factor, 0);
            return Scores[factor];
        }

        /// <summary>
        /// Sums the factor scores from <see cref="FactorScores"/>, sets the <see cref="score"/> and returns 
        /// </summary>
        /// <returns></returns>
        public Double SumFactorScores()
        {
            Double output = 0;
            foreach (var pair in FactorScores)
            {
                if (pair.Value == Double.NaN)
                {

                }
                else
                {

                    output += pair.Value;
                }
            }
            
            score = output;
            return output;
        }

        [XmlIgnore]
        public WebSiteDocument webDocument { get; set; }


        //public TextDocumentSet textDocumentSetForWebSite { get; set; }

        /// <summary>
        /// Flat textual rendering of the document
        /// </summary>
        /// <value>
        /// The text document.
        /// </value>
        [XmlIgnore]
        public TextDocument textDocument { get; set; }


        //public TextDocument textDocument { get; set; }

        /// <summary>
        /// Document model with token absolute frequencies
        /// </summary>
        /// <value>
        /// The space document.
        /// </value>
        [XmlIgnore]
        public SpaceDocumentModel spaceDocument { get; set; }


        //public TextDocumentLayerCollection textRenderLayers { get; set; }

        /// <summary>
        /// Final score
        /// </summary>
        /// <value>
        /// The score.
        /// </value>
        [XmlElement(ElementName = "score")]
        public Double score { get; set; } = 0;
    }
}