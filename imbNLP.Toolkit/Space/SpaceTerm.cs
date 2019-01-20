using System;
using System.Xml.Serialization;

namespace imbNLP.Toolkit.Space
{

    /// <summary>
    /// Represents one distinct word, before stemming and lematizzation
    /// </summary>
    [Serializable]
    public class SpaceTerm : IObjectWithNameWeightAndType
    {
        public SpaceTerm()
        {
        }

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns></returns>
        public virtual T CloneTerm<T>(Boolean cloneChildrenToo = false) where T : SpaceTerm, new()
        {
            T output = new T();
            output.name = name;
            output.type = type;
            output.weight = weight;

            return output;
        }

        /// <summary>
        /// Optionally associated object reference
        /// </summary>
        /// <value>
        /// The data.
        /// </value>
        [XmlIgnore]
        public Object data { get; set; }

        /// <summary>
        /// Gets or sets the content.
        /// </summary>
        /// <value>
        /// The content.
        /// </value>
        public String name { get; set; } = "";

        /// <summary>
        /// type of the term
        /// </summary>
        /// <value>
        /// </value>
        public Int32 type { get; set; } = 0;

        /// <summary>
        /// Gets or sets the weight.
        /// </summary>
        /// <value>
        /// The weight.
        /// </value>
        public Double weight { get; set; } = 1;

        ///// <summary>
        ///// absolute frequency of the term
        ///// </summary>
        ///// <value>
        ///// The frequency.
        ///// </value>
        //public Int32 frequency { get; set; } = 0;
    }
}