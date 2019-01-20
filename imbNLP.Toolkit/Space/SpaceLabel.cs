using imbSCI.Core.extensions.data;
using System;
using System.Collections.Generic;

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

        public const String CORRECT = "#CORRECT#";

        public const String INCORRECT = "#INCORRECT#";

        public static List<String> SetDefaultLabelList(Boolean includeUnknownAsZero = true, List<String> labels = null)
        {
            var label_index = new List<string>();

            if (labels == null) labels = new List<string>();



            if (includeUnknownAsZero) label_index.Add(SpaceLabel.UNKNOWN);

            label_index.AddRange(labels, true);

            if (label_index.Count < 2)
            {
                label_index.Add(SpaceLabel.INCORRECT);
                label_index.Add(SpaceLabel.CORRECT);

            }


            return label_index;
        }


        public SpaceLabel()
        {

        }

        public SpaceLabel(String _name)
        {
            name = _name;
        }
    }


}