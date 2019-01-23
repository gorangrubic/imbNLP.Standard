using System;
using System.Collections.Generic;

namespace imbNLP.Toolkit.Space
{

    /// <summary>
    /// Model for a category
    /// </summary>
    /// <seealso cref="imbNLP.Toolkit.Space.SpaceTerm" />
    public class SpaceCategoryModel : SpaceDocumentModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SpaceCategoryModel"/> class.
        /// </summary>
        public SpaceCategoryModel()
        {

        }

        /// <summary>
        /// Clones the specified clone children too.
        /// </summary>
        /// <param name="cloneChildrenToo">if set to <c>true</c> [clone children too].</param>
        /// <returns></returns>
        public SpaceCategoryModel Clone(bool cloneChildrenToo = false)
        {
            return base.Clone<SpaceCategoryModel>(cloneChildrenToo);
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="SpaceCategoryModel"/> class.
        /// </summary>
        /// <param name="label">The label.</param>
        /// <param name="documents">The documents.</param>
        public SpaceCategoryModel(SpaceLabel label, IEnumerable<SpaceDocumentModel> documents)
        {
            name = label.name;
            Length = 0;

            foreach (SpaceDocumentModel doc in documents)
            {
                terms.MergeDictionary(doc.GetTerms(true, true));
                Length += doc.Length;

                Children.Add(doc);
            }
            if (SpaceModelConstructor.spaceSettings.DoMaintainWordIndex) Words = new Int32[Length];

            if (SpaceModelConstructor.spaceSettings.DoMaintainWordIndex)
            {
                Int32 c = 0;
                foreach (SpaceDocumentModel doc in documents)
                {
                    if (doc.Words != null)
                    {
                        foreach (Int32 w in doc.Words)
                        {
                            Words[c] = w;
                            c++;
                        }
                    }

                }
            }
        }



        /// <summary>
        /// Terms of the document
        /// </summary>
        /// <value>
        /// The terms.
        /// </value>
        //[XmlIgnore]
        //public TokenDictionary terms { get; set; } = new TokenDictionary();

    }

}