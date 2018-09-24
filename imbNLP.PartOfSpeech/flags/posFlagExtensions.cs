using imbNLP.PartOfSpeech.flags.basic;
using imbSCI.Core.extensions.data;
using System;
using System.Collections.Generic;

namespace imbNLP.PartOfSpeech.flags
{
    /// <summary>
    /// POS Flags filtration extensions
    /// </summary>
    public static class posFlagExtensions
    {
        /// <summary>
        /// Gets the grammar context flags: degree, gender, number, gramaticalCase, definitness, animatness, negation... everything that is not direct subtype flag.
        /// </summary>
        /// <param name="mainType">Main type flag.</param>
        /// <param name="graphTags">The graph tags to filter out</param>
        /// <returns>List of tags</returns>
        /// <seealso cref="getSubTypes(pos_type, List{object})"/>
        public static List<Object> getGrammarContext(this pos_type mainType, List<Object> graphTags)
        {
            var output = new List<Object>();

            switch (mainType)
            {
                case pos_type.A:
                    graphTags.getAllOfType<pos_degree>(false).ForEach(x => output.AddUnique(x));
                    graphTags.getAllOfType<pos_gender>(false).ForEach(x => output.AddUnique(x));
                    graphTags.getAllOfType<pos_number>(false).ForEach(x => output.AddUnique(x));
                    graphTags.getAllOfType<pos_gramaticalCase>(false).ForEach(x => output.AddUnique(x));
                    graphTags.getAllOfType<pos_definitness>(false).ForEach(x => output.AddUnique(x));
                    graphTags.getAllOfType<pos_animatness>(false).ForEach(x => output.AddUnique(x));
                    break;

                case pos_type.N:
                    graphTags.getAllOfType<pos_gender>(false).ForEach(x => output.AddUnique(x));
                    graphTags.getAllOfType<pos_number>(false).ForEach(x => output.AddUnique(x));
                    graphTags.getAllOfType<pos_gramaticalCase>(false).ForEach(x => output.AddUnique(x));
                    graphTags.getAllOfType<pos_animatness>(false).ForEach(x => output.AddUnique(x));
                    break;

                case pos_type.NUM:
                    graphTags.getAllOfType<pos_gender>(false).ForEach(x => output.AddUnique(x));
                    graphTags.getAllOfType<pos_number>(false).ForEach(x => output.AddUnique(x));
                    graphTags.getAllOfType<pos_gramaticalCase>(false).ForEach(x => output.AddUnique(x));
                    graphTags.getAllOfType<pos_degree>(false).ForEach(x => output.AddUnique(x));
                    graphTags.getAllOfType<pos_animatness>(false).ForEach(x => output.AddUnique(x));
                    break;

                case pos_type.PRO:
                    graphTags.getAllOfType<pos_gender>(false).ForEach(x => output.AddUnique(x));
                    graphTags.getAllOfType<pos_number>(false).ForEach(x => output.AddUnique(x));
                    graphTags.getAllOfType<pos_gramaticalCase>(false).ForEach(x => output.AddUnique(x));
                    graphTags.getAllOfType<pos_person>(false).ForEach(x => output.AddUnique(x));
                    graphTags.getAllOfType<pos_animatness>(false).ForEach(x => output.AddUnique(x));
                    break;

                case pos_type.V:
                    graphTags.getAllOfType<pos_person>(false).ForEach(x => output.AddUnique(x));
                    graphTags.getAllOfType<pos_gender>(false).ForEach(x => output.AddUnique(x));
                    graphTags.getAllOfType<pos_number>(false).ForEach(x => output.AddUnique(x));
                    graphTags.getAllOfType<pos_negation>(false).ForEach(x => output.AddUnique(x));
                    break;

                case pos_type.ADV:

                    graphTags.getAllOfType<pos_person>(false).ForEach(x => output.AddUnique(x));
                    graphTags.getAllOfType<pos_gender>(false).ForEach(x => output.AddUnique(x));
                    graphTags.getAllOfType<pos_number>(false).ForEach(x => output.AddUnique(x));
                    graphTags.getAllOfType<pos_negation>(false).ForEach(x => output.AddUnique(x));
                    graphTags.getAllOfType<pos_degree>(false).ForEach(x => output.AddUnique(x));
                    break;

                case pos_type.PREP:
                    graphTags.getAllOfType<pos_gramaticalCase>(false).ForEach(x => output.AddUnique(x));
                    break;
            }
            return output;
        }

        /// <summary>
        /// Gets the sub type flags from <c>graphTags</c>, for the <c>mainType</c> specified
        /// </summary>
        /// <param name="mainType">Type of the main.</param>
        /// <param name="graphTags">The graph tags.</param>
        /// <returns></returns>
        public static List<Object> getSubTypes(this pos_type mainType, List<Object> graphTags)
        {
            var output = new List<Object>();

            switch (mainType)
            {
                case pos_type.A:
                    graphTags.getAllOfType<pos_adjectiveType>(false).ForEach(x => output.AddUnique(x));
                    graphTags.getAllOfType<pos_degree>(false).ForEach(x => output.AddUnique(x));
                    break;

                case pos_type.ABB:
                    break;

                case pos_type.ADV:
                    graphTags.getAllOfType<pos_adverbType>(false).ForEach(x => output.AddUnique(x));
                    break;

                case pos_type.CONJ:
                    graphTags.getAllOfType<pos_conjunctionType>(false).ForEach(x => output.AddUnique(x));
                    graphTags.getAllOfType<pos_conjunctionFormation>(false).ForEach(x => output.AddUnique(x));
                    break;

                case pos_type.INT:
                    break;

                case pos_type.N:
                    graphTags.getAllOfType<pos_nounGroup>(false).ForEach(x => output.AddUnique(x));
                    graphTags.getAllOfType<pos_nounType>(false).ForEach(x => output.AddUnique(x));
                    break;

                case pos_type.none:
                    break;

                case pos_type.NUMnumerical:
                case pos_type.NUM:
                    graphTags.getAllOfType<pos_numeralType>(false).ForEach(x => output.AddUnique(x));
                    graphTags.getAllOfType<pos_numeralForm>(false).ForEach(x => output.AddUnique(x));
                    graphTags.getAllOfType<pos_number>(false).ForEach(x => output.AddUnique(x));
                    break;

                case pos_type.PAR:
                    graphTags.getAllOfType<pos_particleType>(false).ForEach(x => output.AddUnique(x));
                    break;

                case pos_type.PREF:
                    break;

                case pos_type.PREP:
                    break;

                case pos_type.PRO:
                    graphTags.getAllOfType<pos_pronounType>(false).ForEach(x => output.AddUnique(x));
                    break;

                case pos_type.PUNCT:
                    break;

                case pos_type.RES:
                    graphTags.getAllOfType<pos_residualType>(false).ForEach(x => output.AddUnique(x));
                    break;

                case pos_type.TEMP:
                    break;

                case pos_type.V:
                    graphTags.getAllOfType<pos_verbform>(false).ForEach(x => output.AddUnique(x));
                    graphTags.getAllOfType<pos_verbType>(false).ForEach(x => output.AddUnique(x));
                    break;
            }

            return output;
        }
    }
}