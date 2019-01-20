using System;
using System.Collections.Generic;

namespace imbNLP.Toolkit.Space
{
/// <summary>
    /// A termset - also known as itemset [16] or word combina- tion feature [28] - is assumed to occur in a given document if all members are present, regardless of their order and position. Selecting a discriminative set of n-termsets is a highly crucial, but very challenging, task since all groups of n terms can be candidate n-termsets. In order to sim- plify this process, one can include only frequent terms in generating termsets [29]. Defining termsets as sets of dis- criminative terms has also been studied [15]. Alternatively, using various feature selection methods such as χ2, mutual information, odds ratio and information gain are evaluated for this purpose [16]. The use of both frequency of termsets and their distribution across different classes has also been studied [30].
    /// </summary>
    /// <seealso cref="imbNLP.Toolkit.Space.SpaceTopic" />
    public class SpaceTermset : SpaceTopic
    {
        public SpaceTermset()
        {

        }
        public List<String> terms { get; set; } = new List<String>();

    }
}