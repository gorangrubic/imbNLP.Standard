using System;
using System.Xml.Serialization;

namespace imbNLP.Toolkit.Processing
{

    /// <summary>
    /// Entry describing weight for a textual token
    /// </summary>
    [XmlRoot(ElementName = "wde")]
    public class WeightDictionaryEntry
    {
        public static WeightDictionaryEntry operator +(WeightDictionaryEntry entryA, WeightDictionaryEntry entryB)
        {
            Int32 d = Math.Min(entryA.dimensions.Length, entryB.dimensions.Length);
            Double[] vec = new double[d];
            for (int i = 0; i < d; i++)
            {
                vec[i] = entryA.dimensions[i] + entryB.dimensions[i];
            }
            WeightDictionaryEntry output = new WeightDictionaryEntry(entryA.name, vec);
            return output;
        }

        public static WeightDictionaryEntry operator /(WeightDictionaryEntry entryA, WeightDictionaryEntry entryB)
        {
            Int32 d = Math.Min(entryA.dimensions.Length, entryB.dimensions.Length);
            Double[] vec = new double[d];
            for (int i = 0; i < d; i++)
            {
                vec[i] = entryA.dimensions[i] / entryB.dimensions[i];
            }
            WeightDictionaryEntry output = new WeightDictionaryEntry(entryA.name, vec);
            return output;
        }

        public static WeightDictionaryEntry operator *(WeightDictionaryEntry entryA, WeightDictionaryEntry entryB)
        {
            Int32 d = Math.Min(entryA.dimensions.Length, entryB.dimensions.Length);
            Double[] vec = new double[d];
            for (int i = 0; i < d; i++)
            {
                vec[i] = entryA.dimensions[i] * entryB.dimensions[i];
            }
            WeightDictionaryEntry output = new WeightDictionaryEntry(entryA.name, vec);
            return output;
        }

        public static WeightDictionaryEntry operator /(WeightDictionaryEntry entryA, Double k)
        {
            Int32 d = entryA.dimensions.Length;
            Double[] vec = new double[d];
            for (int i = 0; i < d; i++)
            {
                vec[i] = entryA.dimensions[i] / k;
            }
            WeightDictionaryEntry output = new WeightDictionaryEntry(entryA.name, vec);
            return output;
        }


        public static WeightDictionaryEntry operator *(WeightDictionaryEntry entryA, Double k)
        {
            Int32 d = entryA.dimensions.Length;
            Double[] vec = new double[d];
            for (int i = 0; i < d; i++)
            {
                vec[i] = entryA.dimensions[i] * k;
            }
            WeightDictionaryEntry output = new WeightDictionaryEntry(entryA.name, vec);
            return output;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WeightDictionaryEntry"/> class.
        /// </summary>
        public WeightDictionaryEntry()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WeightDictionaryEntry"/> class.
        /// </summary>
        /// <param name="_name">The name.</param>
        /// <param name="_weight">The weight.</param>
        public WeightDictionaryEntry(String _name, Double _weight)
        {
            name = _name;
            dimensions = new double[1];
            dimensions[0] = _weight;

            //weight = _weight;
        }


        /// <summary>
        /// Creates multi dimensional weight entry
        /// </summary>
        /// <param name="_name">The name.</param>
        /// <param name="_weight">The weight.</param>
        public WeightDictionaryEntry(String _name, Double[] _dimensions)
        {
            name = _name;
            dimensions = _dimensions;
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
        [XmlIgnore]
        public Double weight
        {
            get
            {
                return dimensions[0];
            }
            set
            {
                dimensions[0] = value;
            }
        }

        [XmlElement(ElementName = "w")]
        public Double[] dimensions { get; set; }
    }
}