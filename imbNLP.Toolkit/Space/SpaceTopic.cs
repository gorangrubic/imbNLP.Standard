using System;
using System.Collections.Generic;

namespace imbNLP.Toolkit.Space
{
    /// <summary>
    /// Topic - represents extracted topics, i.e. ones produced by an algorithm for latent semantic analysis and/or topic modeling
    /// </summary>
    /// <seealso cref="imbNLP.Toolkit.Space.SpaceTerm" />
    public class SpaceTopic : SpaceTerm
    {
        public SpaceTopic()
        {
        }

        public SpaceTopic(string _name)
        {
            name = _name;
        }

        ///// <summary>
        ///// Lemma terms that are related with this topic
        ///// </summary>
        ///// <value>
        ///// The items.
        ///// </value>
        //public List<SpaceLemmaTerm> items { get; set; } = new List<SpaceLemmaTerm>();
    }
}