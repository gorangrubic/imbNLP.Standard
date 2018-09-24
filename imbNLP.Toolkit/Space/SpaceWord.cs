using System;
using System.Linq;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace imbNLP.Toolkit.Space
{

    public class SpaceWord
    {
        public SpaceWord()
        {

        }

        public SpaceWord(String _name, Int32 _id)
        {
            name = _name;
            id = _id;
        }

        /// <summary>
        /// Content
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public String name { get; set; } = "";

        /// <summary>
        /// Corpus global ID
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public Int32 id { get; set; } = 0;
    }

}