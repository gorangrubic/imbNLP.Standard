using System;

namespace imbNLP.Toolkit.Space
{
    /// <summary>
    /// Label - represents Category or Class in context of text classification problem
    /// </summary>
    /// <seealso cref="imbNLP.Toolkit.Space.SpaceTerm" />
    public class SpaceLabel : SpaceTerm
    {

        /// <summary>
        /// The unknown - label name for non label :)
        /// </summary>
        public const String UNKNOWN = "#UNKNOWN#";

        public SpaceLabel()
        {

        }

        public SpaceLabel(String _name)
        {
            name = _name;
        }
    }


}