using System;
using System.Xml.Serialization;

namespace imbNLP.Toolkit.Processing
{
    /// <summary>
    /// Pair of matched features
    /// </summary>
    public class WeightDictionaryEntryPair
    {
        private Double _weight_B_sq = Double.MinValue;
        private Double _weight_A_sq = Double.MinValue;

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

        /// <summary>
        /// Wight
        /// </summary>
        /// <value>
        /// The weight.
        /// </value>
        [XmlIgnore]
        public Double weight_A_sq
        {
            get
            {
                if (_weight_A_sq == Double.MinValue)
                {
                    _weight_A_sq = weight_A * weight_A;
                }
                return _weight_A_sq;
            }
            set { _weight_A_sq = value; }
        }

        /// <summary>
        /// Wight
        /// </summary>
        /// <value>
        /// The weight.
        /// </value>
        [XmlIgnore]
        public Double weight_B_sq
        {
            get
            {
                if (_weight_B_sq == Double.MinValue)
                {
                    _weight_B_sq = weight_B * weight_B;
                }
                return _weight_B_sq;
            }
            set { _weight_B_sq = value; }
        }
    }

}