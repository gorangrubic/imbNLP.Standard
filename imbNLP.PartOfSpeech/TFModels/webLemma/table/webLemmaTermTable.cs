// --------------------------------------------------------------------------------------------------------------------
// <copyright file="webLemmaTermTable.cs" company="imbVeles" >
//
// Copyright (C) 2018 imbVeles
//
// This program is free software: you can redistribute it and/or modify
// it under the +terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see http://www.gnu.org/licenses/.
// </copyright>
// <summary>
// Project: imbNLP.PartOfSpeech
// Author: Goran Grubic
// ------------------------------------------------------------------------------------------------------------------
// Project web site: http://blog.veles.rs
// GitHub: http://github.com/gorangrubic
// Mendeley profile: http://www.mendeley.com/profiles/goran-grubi2/
// ORCID ID: http://orcid.org/0000-0003-2673-9471
// Email: hardy@veles.rs
// </summary>
// ------------------------------------------------------------------------------------------------------------------
using imbSCI.Core.extensions.data;
using imbSCI.Core.extensions.io;
using imbSCI.Core.extensions.typeworks;
using imbSCI.Core.files;
using imbSCI.Core.reporting;
using imbSCI.DataComplex.tables;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace imbNLP.PartOfSpeech.TFModels.webLemma.table
{
    /// <summary>
    /// Precompiled web lemma term table, ready for saving and application
    /// </summary>
    /// <seealso cref="imbSCI.DataComplex.tables.objectTable{imbMiningContext.TFModels.WLF_ISF.webLemmaTerm}" />
    public class webLemmaTermTable : ILemmaCollection, IEnumerable<webLemmaTerm>
    {
        /// <summary>
        /// The number of retries for loading and saving
        /// </summary>
        public const Int32 NumberOfRetry = 5;

        /// <summary>
        /// The retry delay
        /// </summary>
        public const Int32 RetryDelay = 250;

        public String name { get; set; } = "";
        public String description { get; set; } = "";

        /// <summary>
        /// Initializes a new instance of the <see cref="webLemmaTermTable"/> class.
        /// </summary>
        /// <param name="_name">The name.</param>
        public webLemmaTermTable(String _name)
        {
            name = _name;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="webLemmaTermTable"/> class.
        /// </summary>
        /// <param name="__filepath">The filepath.</param>
        /// <param name="_name">The name.</param>
        /// <param name="logger">The logger.</param>
        public webLemmaTermTable(String __filepath, String _name, ILogBuilder logger = null) //: base(__filepath, autoload, nameof(webLemmaTerm.name), _name)
        {
            name = _name;

            Load(__filepath, logger);
        }

        public void Clear()
        {
            index.Clear();
        }

        private Object addLock = new Object();

        /// <summary>
        /// Adds the specified term.
        /// </summary>
        /// <param name="term">The term.</param>
        public void Add(webLemmaTerm term)
        {
            DeployLemma(term);
        }

        /// <summary>
        /// Temporary storage of unresolved tokens
        /// </summary>
        /// <value>
        /// The unresolved.
        /// </value>
        public List<String> unresolved { get; set; } = new List<string>();

        /// <summary>
        /// Loads the specified filepath.
        /// </summary>
        /// <param name="__filepath">The filepath.</param>
        /// <param name="logger">The logger.</param>
        public void Load(String __filepath = null, ILogBuilder logger = null)
        {
            Int32 retry = NumberOfRetry;

            if (!__filepath.isNullOrEmpty()) filepath = __filepath;
            webLemmaTermPackage package = new webLemmaTermPackage(name, description);

            if (File.Exists(filepath))
            {
                while (retry > 0)
                {
                    try
                    {
                        package = objectSerialization.loadObjectFromXML<webLemmaTermPackage>(filepath);
                        retry = 0;
                    }
                    catch (Exception ex)
                    {
                        retry--;
                        Thread.Sleep(RetryDelay);
                        if (logger != null) logger.log("[" + retry.ToString("D2") + "] retring to load web lemma term table [" + filepath + "] (" + ex.Message + ")");
                    }
                }
            }

            DeployPackage(package);
        }

        public webLemmaTermTableSufix meta { get; protected set; } = new webLemmaTermTableSufix();
        protected Dictionary<String, webLemmaTerm> index = new Dictionary<string, webLemmaTerm>();
        protected Dictionary<String, webLemmaTerm> indexInflections = new Dictionary<string, webLemmaTerm>();

        private Object DeployPackageLock = new Object();

        protected void DeployPackage(webLemmaTermPackage package)
        {
            meta = new webLemmaTermTableSufix();
            if (package == null) return;
            lock (DeployPackageLock)
            {
                Clear();
                if (name.isNullOrEmpty()) name = package.name;
                if (description.isNullOrEmpty()) description = package.description;

                foreach (webLemmaTerm term in package.lemmas)
                {
                    meta.checkMinMax(term);
                    DeployLemma(term);
                }
            }
        }

        private Object indexAddLock = new Object();

        private Int32 updateCounts = 0;

        protected void DeployLemma(webLemmaTerm term)
        {
            if (term == null) return;
            Boolean doUpdate = true;

            webLemmaTerm existingTerm = ResolveLemmaForTerm(term.name);
            if (existingTerm == null)
            {
                doUpdate = false;
                lock (indexAddLock)
                {
                    if (!index.ContainsKey(term.nominalForm))
                    {
                        index.Add(term.nominalForm, term);
                        foreach (String termOther in term.otherForms)
                        {
                            if (!indexInflections.ContainsKey(termOther))
                            {
                                indexInflections.Add(termOther, term);
                            }
                        }
                    }
                    else
                    {
                        doUpdate = true;
                        existingTerm = index[term.nominalForm];
                    }
                }
            }

            if (doUpdate)
            {
                if (existingTerm.weight == term.weight)
                {
                }
                else
                {
                    existingTerm.setObjectBySource(term);
                }
            }
        }

        /// <summary>
        /// Gets the list.
        /// </summary>
        /// <returns></returns>
        public List<webLemmaTerm> GetList() => index.Values.ToList();

        public Boolean ContainsKey(String key) => index.ContainsKey(key);

        public Int32 Count => index.Count;

        private String _filepath = null;

        /// <summary>
        /// File path with the package
        /// </summary>
        /// <value>
        /// The filepath.
        /// </value>
        public String filepath
        {
            get
            {
                if (_filepath.isNullOrEmpty())
                {
                    _filepath = name.getCleanFilepath(".xml");
                }
                return _filepath;
            }
            set
            {
                _filepath = value;
            }
        }

        private Object SaveLock = new Object();

        /// <summary>
        /// Saves the specified filepath.
        /// </summary>
        /// <param name="__filepath">The filepath.</param>
        /// <param name="logger">The logger.</param>
        public void Save(String __filepath = null, ILogBuilder logger = null)
        {
            lock (SaveLock)
            {
                Int32 retry = NumberOfRetry;
                if (!__filepath.isNullOrEmpty()) filepath = __filepath;

                webLemmaTermPackage package = new webLemmaTermPackage(name, description);
                package.lemmas.AddRange(index.Values.ToList(), true);
                while (retry > 0)
                {
                    try
                    {
                        objectSerialization.saveObjectToXML(package, filepath);
                        retry = 0;
                    }
                    catch (Exception ex)
                    {
                        retry--;
                        Thread.Sleep(RetryDelay);
                        if (logger != null) logger.log("[" + retry.ToString("D2") + "] retring to save web lemma term table [" + __filepath + "] (" + ex.Message + ")");
                    }
                }
            }
        }

        /// <summary>
        /// Gets lemmas that are common between this and specified <c>tableB</c>
        /// </summary>
        /// <param name="tableB">The table b.</param>
        /// <param name="logger">The logger.</param>
        /// <returns></returns>
        public webLemmaTermPairCollection GetMatchingTerms(ILemmaCollection tableB, Boolean reverse = false, ILogBuilder logger = null)
        {
            webLemmaTermPairCollection output = new webLemmaTermPairCollection();
            List<webLemmaTerm> lemmas = GetList();
            foreach (webLemmaTerm lemma in lemmas)
            {
                webLemmaTerm lemmaB = tableB.ResolveLemmaForTerm(lemma.nominalForm);
                if (lemmaB != null)
                {
                    output.Add(lemma, lemmaB);
                }
            }
            return output;
        }

        /// <summary>
        /// Indexed approach to lemmas
        /// </summary>
        /// <param name="term">The term.</param>
        /// <param name="logger">The logger.</param>
        /// <returns></returns>
        public webLemmaTerm ResolveLemmaForTerm(String term, ILogBuilder logger = null)
        {
            if (index.ContainsKey(term))
            {
                return index[term];
            }
            else if (indexInflections.ContainsKey(term))
            {
                return indexInflections[term];
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Resolves single term - returns weight for lemma of this term
        /// </summary>
        /// <param name="term">The term.</param>
        /// <param name="logger">The logger.</param>
        /// <returns></returns>
        public Double ResolveSingleTerm(String term, ILogBuilder logger = null)
        {
            webLemmaTerm lemma = ResolveLemmaForTerm(term, logger);
            if (lemma != null) return 0;
            return lemma.weight;
        }

        /// <summary>
        /// Gets the <see cref="webLemmaTerm"/> with the specified key.
        /// </summary>
        /// <value>
        /// The <see cref="webLemmaTerm"/>.
        /// </value>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public new webLemmaTerm this[String key]
        {
            get
            {
                return ResolveLemmaForTerm(key);
            }
        }

        /// <summary>
        /// Gets the data table.
        /// </summary>
        /// <returns></returns>
        public DataTableExtended GetDataTable()
        {
            DataTableTypeExtended<webLemmaTerm> dte = new DataTableTypeExtended<webLemmaTerm>(name, description);
            foreach (var pair in index)
            {
                dte.AddRow(pair.Value);
            }
            return dte;
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// An enumerator that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<webLemmaTerm> GetEnumerator()
        {
            return GetList().GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetList().GetEnumerator();
        }
    }
}