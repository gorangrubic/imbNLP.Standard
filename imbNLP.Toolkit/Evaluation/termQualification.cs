using imbSCI.Data;
using System;
using System.Collections.Generic;

namespace imbNLP.Toolkit.Evaluation
{
    /// <summary>
    /// Entry for term qualification
    /// </summary>
    public class termQualification
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="termQualification"/> class.
        /// </summary>
        public termQualification()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="termQualification"/> class.
        /// </summary>
        /// <param name="_lemmaForm">The lemma form.</param>
        /// <param name="_score">The score.</param>
        public termQualification(String _lemmaForm, Int32 _score)
        {
            lemmaForm = _lemmaForm;
            score = _score;
        }

        /// <summary>
        /// Gets or sets the lemma form.
        /// </summary>
        /// <value>
        /// The lemma form.
        /// </value>
        public String lemmaForm { get; set; } = "";

        /// <summary>
        /// Gets or sets the score.
        /// </summary>
        /// <value>
        /// The score.
        /// </value>
        public Int32 score { get; set; } = 0;

        /// <summary>
        /// Gets or sets the lemma form.
        /// </summary>
        /// <value>
        /// The lemma form.
        /// </value>
        public String translatedForm { get; set; } = "";

        /// <summary>
        /// The format
        /// </summary>
        public const String FORMAT = "{0,-80} : {1,10} : {2,60}";

        /// <summary>
        /// Performs an implicit conversion from <see cref="String"/> to <see cref="termQualification"/>.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static implicit operator termQualification(String input)
        {
            var lemma = new termQualification();
            lemma.FromString(input);
            return lemma;
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="termQualification"/> to <see cref="String"/>.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static implicit operator String(termQualification input)
        {
            return input.ToString();
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override String ToString()
        {
            return String.Format(FORMAT, lemmaForm, score, translatedForm);
        }

        /// <summary>
        /// From the string.
        /// </summary>
        /// <param name="input">The input.</param>
        public void FromString(String input)
        {
            if (input.Contains(":"))
            {
                List<String> parts = input.SplitSmart(":", "", true, true);
                lemmaForm = parts[0].Trim();
                score = Convert.ToInt32(parts[1].Trim());
                if (parts.Count > 2) translatedForm = parts[2].Trim();
            }
        }
    }
}