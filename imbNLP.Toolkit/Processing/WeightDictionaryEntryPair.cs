using System;
using System.Xml.Serialization;

namespace imbNLP.Toolkit.Processing
{
    /// <summary>
    /// Pair of matched features
    /// </summary>
    public class WeightDictionaryEntryPair
    {
        public WeightDictionaryEntryPair()
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WeightDictionaryEntryPair"/> class.
        /// </summary>
        /// <param name="_name">The name.</param>
        /// <param name="_wa">The wa.</param>
        /// <param name="_wb">The wb.</param>
        public WeightDictionaryEntryPair(String _name, Double _wa, Double _wb)
        {
            name = _name;
            weight_A = _wa;
            weight_B = _wb;

        }
        /// <summary>
        /// Text token
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        [XmlElement(ElementName = "n")]
        public String name { get; set; } = "";

        /// <summary>
        /// Wight
        /// </summary>
        /// <value>
        /// The weight.
        /// </value>
        [XmlElement(ElementName = "wA")]
        public Double weight_A { get; set; } = 0;

        /// <summary>
        /// Wight
        /// </summary>
        /// <value>
        /// The weight.
        /// </value>
        [XmlElement(ElementName = "wB")]
        public Double weight_B { get; set; } = 0;
    }

}