using imbNLP.Toolkit.Entity;
using imbNLP.Toolkit.Processing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace imbNLP.Toolkit.Space
{

    /// <summary>
    /// Model of an document, sent for analysis by a algorithm in the toolkit
    /// </summary>
    /// <remarks>
    /// <para>The frequency property of DocumentModel, represents total number of tokens that are represented by the model (i.e. sum of all absolute frequencies of the LemmaTerm)</para>
    /// </remarks>
    [Serializable]
    public class SpaceDocumentModel : SpaceTerm
    {


        /// <summary>
        /// Gets all children.
        /// </summary>
        /// <returns></returns>
        public List<SpaceDocumentModel> GetAllChildren()
        {
            List<SpaceDocumentModel> iteration = new List<SpaceDocumentModel>();
            List<SpaceDocumentModel> output = new List<SpaceDocumentModel>();

            iteration.Add(this);

            while (iteration.Any())
            {
                List<SpaceDocumentModel> nextIteration = new List<SpaceDocumentModel>();
                for (int i = 0; i < iteration.Count; i++)
                {
                    output.Add(iteration[i]);

                    if (iteration[i].Children.Any())
                    {
                        nextIteration.AddRange(iteration[i].Children);
                    }

                }
                iteration = nextIteration;
            }

            return output;

        }

        /// <summary>
        /// Returns all leaf nodes, excluding branch nodes leading to the leafs. If this is leaf node it will be included in the result.
        /// </summary>
        /// <returns>Leaf nodes, including this in case it is leaf</returns>
        public List<SpaceDocumentModel> GetLeafs(Boolean clearBranchTerms = false)
        {
            List<SpaceDocumentModel> iteration = new List<SpaceDocumentModel>();

            iteration.Add(this);

            List<SpaceDocumentModel> output = new List<SpaceDocumentModel>();
            while (iteration.Any())
            {
                List<SpaceDocumentModel> nextIteration = new List<SpaceDocumentModel>();
                for (int i = 0; i < iteration.Count; i++)
                {
                    if (iteration[i].Children.Any())
                    {
                        if (clearBranchTerms) iteration[i].terms.Clear();

                        nextIteration.AddRange(iteration[i].Children);
                    }
                    else
                    {
                        output.Add(iteration[i]);
                    }
                }
                iteration = nextIteration;
            }
            return output;
        }


        public Boolean Contains(String term)
        {
            if (terms.Count > 0)
            {
                return terms.Contains(term);
            }

            List<SpaceDocumentModel> iteration = new List<SpaceDocumentModel>();

            iteration.Add(this);


            while (iteration.Any())
            {
                List<SpaceDocumentModel> nextIteration = new List<SpaceDocumentModel>();
                for (int i = 0; i < iteration.Count; i++)
                {
                    if (iteration[i].terms.Contains(term))
                    {
                        return true;
                    }

                    if (iteration[i].Children.Any())
                    {
                        nextIteration.AddRange(iteration[i].Children);
                    }

                }
                iteration = nextIteration;
            }

            return false;
        }

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns></returns>
        public virtual T Clone<T>(Boolean cloneChildrenToo = false) where T : SpaceDocumentModel, new()
        {
            T output = new T();
            output.name = name;
            output.labels = labels.ToList();

            if (SpaceModelConstructor.spaceSettings.DoMaintainWordIndex)
            {
                output.Words = Words;
            }

            output.terms = terms.Clone();
            output.Length = length;


            output.type = type;
            output.weight = weight;

            output.documentScope = documentScope;

            foreach (SpaceDocumentModel ch in Children)
            {
                output.Children.Add(ch.Clone<SpaceDocumentModel>(cloneChildrenToo));
            }

            return output;
        }


        /// <summary>
        /// Sub documents
        /// </summary>
        /// <value>
        /// The children.
        /// </value>
        public List<SpaceDocumentModel> Children { get; protected set; } = new List<SpaceDocumentModel>();


        /// <summary>
        /// Scope of the document
        /// </summary>
        /// <value>
        /// The document scope.
        /// </value>
        public DocumentBlenderFunctionOptions documentScope { get; set; } = DocumentBlenderFunctionOptions.siteLevel;


        public List<String> GetWordIndexed()
        {
            return terms.GetTokenByIDs(Words);
        }


        /// <summary>
        /// Removes all features other than specirfi
        /// </summary>
        /// <param name="selectedFeatures">Tokens to filter</param>
        /// <param name="inverseFilter">if set to <c>true</c> it will remove all other than specified.</param>
        /// <returns>Number of distinct tokens removed from this model and children nodes</returns>
        public Int32 FilterSelectedFeatures(List<String> keys, Boolean inverseFilter = true, Boolean filterFirstLine = true)
        {
            SpaceDocumentModel output = null;

            Int32 ca = 0;
            Int32 cb = 0;

            List<SpaceDocumentModel> iteration = new List<SpaceDocumentModel>();

            if (filterFirstLine)
            {
                iteration.Add(this);
                iteration.AddRange(Children);
                //terms.FilterTokens(keys);
            }
            else
            {
                iteration = GetLeafs(true);
            }



            for (int i = 0; i < iteration.Count; i++)
            {
                ca += iteration[i].terms.FilterTokens(keys, inverseFilter);
            }


            //Parallel.ForEach<SpaceDocumentModel,Int32>(toProcess,
            //    model => // method invoked by the loop on each iteration
            //                         {
            //                    Int32 subtotal = model.terms.FilterTokens(keys, inverseFilter);  //modify local variable
            //                             return subtotal; // value to be passed to next iteration
            //                         },
            //     () => 0,
            //                // Method to be executed when each partition has completed.
            //                // finalResult is the final value of subtotal for a particular partition.
            //                (finalResult) => Interlocked.Add(ref ca, finalResult)
            //                );

            //Parallel.ForEach(toProcess, model =>
            //{
            //    model.terms.FilterTokens(keys, inverseFilter);
            //});
            //Parallel.ForEach<SpaceDocumentModel, Int32>(toProcess,
            //    () => 0,
            //    (model, loop, subtotal) =>
            //    {
            //        subtotal += model.terms.FilterTokens(keys, inverseFilter);
            //        return subtotal;
            //    },
            //    (finalResult) => Interlocked.Add(ref ca, finalResult)
            //);


            return ca;
        }


        /// <summary>
        /// Projects all term counts and word indexes from children to this instance or new instance
        /// </summary>
        /// <param name="newInstance">if set to <c>true</c> [new instance].</param>
        /// <returns></returns>
        public SpaceDocumentModel Flatten(Boolean newInstance = true)
        {
            SpaceDocumentModel output = null;

            List<SpaceDocumentModel> iteration = new List<SpaceDocumentModel>();


            if (!newInstance)
            {
                output = this;

            }
            else
            {
                output = new SpaceDocumentModel();
                iteration.Add(this);

                output.type = type;
                output.name = name;
                output.weight = weight;
            }


            List<List<String>> wordIndexes = new List<List<string>>();
            TokenDictionary new_terms = new TokenDictionary();

            iteration.AddRange(Children);

            while (iteration.Any())
            {

                List<SpaceDocumentModel> nextIteration = new List<SpaceDocumentModel>();

                for (int i = 0; i < iteration.Count; i++)
                {
                    if (SpaceModelConstructor.spaceSettings.DoMaintainWordIndex) wordIndexes.Add(iteration[i].GetWordIndexed());
                    new_terms.MergeDictionary(iteration[i].terms);

                    if (iteration[i].terms.Count == 0)
                    {
                        nextIteration.AddRange(iteration[i].Children);
                    }

                }


                iteration = nextIteration;

            }

            List<Int32> wsum = new List<int>();
            for (int i2 = 0; i2 < wordIndexes.Count; i2++)
            {
                wsum.AddRange(new_terms.GetIDsByTokens(wordIndexes[i2]));
            }

            output.terms = new_terms;
            if (SpaceModelConstructor.spaceSettings.DoMaintainWordIndex)
            {
                output.Words = wsum.ToArray();
                length = wsum.Count;
            }

            return output;
            //output.terms.MergeDictionary(GetTerms(false, true));


        }

        /// <summary>
        /// Returns matched tokens
        /// </summary>
        /// <param name="tokensToMatch">The tokens to match.</param>
        /// <returns></returns>
        public List<String> GetTokens(List<String> tokensToMatch)
        {

            if (terms.Count > 0)
            {
                return terms.GetTokens(tokensToMatch, false);
            }

            List<String> result = new List<String>();
            tokensToMatch = tokensToMatch.ToList();

            List<SpaceDocumentModel> iteration = new List<SpaceDocumentModel>();

            iteration.AddRange(this.Children);

            List<SpaceDocumentModel> output = new List<SpaceDocumentModel>();
            while (iteration.Any())
            {
                List<SpaceDocumentModel> nextIteration = new List<SpaceDocumentModel>();
                for (int i = 0; i < iteration.Count; i++)
                {

                    result.AddRange(iteration[i].terms.GetTokens(tokensToMatch, true));
                    if (tokensToMatch.Any())
                    {
                        nextIteration.AddRange(iteration[i].Children);
                    }
                    else
                    {
                        return result;
                    }
                }
                iteration = nextIteration;
            }

            return result;
        }

        /// <summary>
        /// Gets the terms.
        /// </summary>
        /// <param name="includingSelf">if set to <c>true</c> [including self].</param>
        /// <param name="includingChildren">if set to <c>true</c> [including children].</param>
        /// <returns></returns>
        public TokenDictionary GetTerms(Boolean includingSelf, Boolean includingChildren, Boolean PassSelfIfNotEmpty = true, Boolean SetToSelfIfEmpty = true)
        {
            TokenDictionary output = new TokenDictionary();

            if (PassSelfIfNotEmpty)
            {
                if (terms.Count > 0)
                {
                    return terms;
                }
            }
            if (includingSelf) output.MergeDictionary(terms);
            if (includingChildren)
            {
                List<SpaceDocumentModel> iteration = GetLeafs();

                for (int i = 0; i < iteration.Count; i++)
                {
                    output.MergeDictionary(iteration[i].terms);
                }

            }

            if (SetToSelfIfEmpty)
            {
                if (terms.HasChanges || terms.Count == 0)
                {
                    terms = output;
                    //terms.MergeDictionary(output);
                }
            }

            return output;
        }

        public void PropagateLabels()
        {
            foreach (var ch in Children)
            {
                ch.labels = labels;
                ch.PropagateLabels();
            }

        }

        /// <summary>
        /// Index of words
        /// </summary>
        /// <value>
        /// The words.
        /// </value>
        public int[] Words
        {
            get { return _words; }
            set { _words = value; }
        }

        /// <summary>
        /// The length - number of tokens
        /// </summary>
        private int length;
        private int[] _words = new int[0];
        private TokenDictionary _terms = new TokenDictionary();

        /// <summary>
        /// Terms of the document
        /// </summary>
        /// <value>
        /// The terms.
        /// </value>
        [XmlIgnore]
        public TokenDictionary terms
        {
            get { return _terms; }
            set { _terms = value; }
        }


        public List<String> labels { get; set; } = new List<String>();

        public int Length { get => length; set => length = value; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SpaceDocumentModel"/> class.
        /// </summary>
        public SpaceDocumentModel()
        {
        }

        public SpaceDocumentModel(String __name)
        {
            name = __name;
        }
    }
}