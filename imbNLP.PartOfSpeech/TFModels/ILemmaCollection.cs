using imbNLP.PartOfSpeech.TFModels.webLemma.table;
using imbSCI.Core.reporting;
using System;
using System.Collections.Generic;

namespace imbNLP.PartOfSpeech.TFModels
{
    /// <summary>
    /// Interface for webLemma collections like <see cref="semanticCloud.lemmaSemanticCloud"/> and <see cref="webLemmaTermTable"/>
    /// </summary>
    public interface ILemmaCollection
    {
        /// <summary>
        /// Gets the matching terms.
        /// </summary>
        /// <param name="lemmas">The lemmas.</param>
        /// <param name="reverse">if set to <c>true</c> [reverse].</param>
        /// <param name="loger">The loger.</param>
        /// <returns></returns>
        webLemmaTermPairCollection GetMatchingTerms(ILemmaCollection lemmas, Boolean reverse = false, ILogBuilder loger = null);

        /// <summary>
        /// Resolves the lemma for term.
        /// </summary>
        /// <param name="nominalForm">The nominal form.</param>
        /// <param name="logger">The logger.</param>
        /// <returns></returns>
        webLemmaTerm ResolveLemmaForTerm(String nominalForm, ILogBuilder logger = null);

        /// <summary>
        /// Returns weight of the <c>term</c>, use this where possible instead of <see cref="ResolveLemmaForTerm(string, ILogBuilder)"/>
        /// </summary>
        /// <param name="term">The term.</param>
        /// <param name="logger">The logger.</param>
        /// <returns></returns>
        Double ResolveSingleTerm(String term, ILogBuilder logger = null);

        /// <summary>
        /// Adds the specified term.
        /// </summary>
        /// <param name="term">The term.</param>
        void Add(webLemmaTerm term);

        ///// <summary>
        ///// Loads the specified filepath.
        ///// </summary>
        ///// <param name="__filepath">The filepath.</param>
        ///// <param name="logger">The logger.</param>
        //void Load(String __filepath = null, ILogBuilder logger = null);
        /// <summary>
        /// Saves the specified filepath.
        /// </summary>
        /// <param name="__filepath">The filepath.</param>
        /// <param name="logger">The logger.</param>
        void Save(String __filepath = null, ILogBuilder logger = null);

        /// <summary>
        /// Gets the count.
        /// </summary>
        /// <value>
        /// The count.
        /// </value>
        Int32 Count { get; }

        /// <summary>
        /// Determines whether the specified key contains key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>
        ///   <c>true</c> if the specified key contains key; otherwise, <c>false</c>.
        /// </returns>
        Boolean ContainsKey(String key);

        /// <summary>
        /// Gets the list.
        /// </summary>
        /// <returns></returns>
        List<webLemmaTerm> GetList();

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        String name { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>
        /// The description.
        /// </value>
        String description { get; set; }

        /// <summary>
        /// Gets or sets the filepath.
        /// </summary>
        /// <value>
        /// The filepath.
        /// </value>
        String filepath { get; set; }

        /// <summary>
        /// Clears this instance.
        /// </summary>
        void Clear();

        /// <summary>
        /// Gets the <see cref="webLemmaTerm"/> with the specified key.
        /// </summary>
        /// <value>
        /// The <see cref="webLemmaTerm"/>.
        /// </value>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        webLemmaTerm this[String key] { get; }
    }
}