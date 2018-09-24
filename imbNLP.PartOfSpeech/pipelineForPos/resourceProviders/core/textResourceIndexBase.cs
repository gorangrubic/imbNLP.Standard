// --------------------------------------------------------------------------------------------------------------------
// <copyright file="textResourceIndexBase.cs" company="imbVeles" >
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
using System;
using System.Collections.Generic;
using System.Linq;

namespace imbNLP.PartOfSpeech.resourceProviders.core
{
    using imbNLP.PartOfSpeech.lexicUnit;
    using imbNLP.Transliteration.ruleSet;
    using imbSCI.Core.extensions.data;
    using imbSCI.Core.extensions.io;
    using imbSCI.Core.files.folders;
    using imbSCI.Core.math;
    using imbSCI.Core.reporting;
    using imbSCI.Data;
    using imbSCI.Data.collection.nested;
    using System.Collections.Concurrent;
    using System.IO;
    using System.Runtime.Serialization;
    using System.Runtime.Serialization.Formatters.Binary;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    //    using Accord.IO;

    /// <summary>
    /// Common base for lexical resources based on indexed nested dictionaries. Higher performance then direct file search based.
    /// </summary>
    /// <seealso cref="imbSCI.Data.collection.nested.aceDictionaryLetterIndexSet{imbNLP.PartOfSpeech.lexicUnit.lexicInflection}" />
    [Serializable]
    public abstract class textResourceIndexBase : aceDictionaryLetterIndexSet<lexicInflection>, ITextResourceResolver
    {
        public lexicResolverSettings settings { get; set; } = new lexicResolverSettings();

        protected abstract void SelectFromLine(string line, out string inflectForm, out string lemmaForm, out string gramTag);

        protected abstract String RenderToLine(string inflectForm, string lemmaForm, string gramTag);

        public string localCache { get; set; } = "";

        public resourceConverterForGramaticTags grammTagConverter { get; protected set; }

        protected textResourceIndexResolveMode mode { get; set; } = textResourceIndexResolveMode.resolveOnLoad;

        /// <summary>
        /// Inflection to lemma index
        /// </summary>
        /// <value>
        /// The index of the registrated lemma.
        /// </value>
        protected ConcurrentDictionary<String, lexicGraphSetWithLemma> registratedLemmaIndex { get; set; } = new ConcurrentDictionary<string, lexicGraphSetWithLemma>();

        /// <summary>
        /// Setups the specified resource file path.
        /// </summary>
        /// <param name="resourceFilePath">The resource file path.</param>
        /// <param name="grammSpecFilename">The gramm spec filename.</param>
        /// <param name="output">The output.</param>
        /// <exception cref="ArgumentNullException">
        /// resourceFilePath
        /// or
        /// grammSpecFilename
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">File format not recognized for " + nameof(textResourceResolverBase) + " Setup call. - grammSpecFilename</exception>
        public void Setup(string resourceFilePath, string grammSpecFilename, String spellAlternatorFilename, ILogBuilder output = null, textResourceIndexResolveMode __mode = textResourceIndexResolveMode.resolveOnQuery)
        {
            if (resourceFilePath.isNullOrEmpty())
            {
                imbACE.Services.terminal.aceTerminalInput.askYesNo("Resource file path is empty (textResourceIndexBase.Setup)!");
                throw new ArgumentNullException(nameof(resourceFilePath));
                return;
            }

            if (grammSpecFilename.isNullOrEmpty())
            {
                imbACE.Services.terminal.aceTerminalInput.askYesNo("Grammar conversion specification file path is empty (textResourceIndexBase.Setup)!");
                throw new ArgumentNullException(nameof(grammSpecFilename));
                return;
            }

            mode = __mode;

            // <---------------------------------------------- [

            grammTagConverter = new resourceConverterForGramaticTags();

            if (grammSpecFilename.EndsWith(".xlsx"))
            {
                if (output != null) output.log("Grammar conversion specification loading from Excel file");

                grammTagConverter.LoadSpecificationExcelFile(grammSpecFilename, output);
            }
            else if (grammSpecFilename.EndsWith(".csv"))
            {
                string filebase = Path.GetFileNameWithoutExtension(grammSpecFilename);
                string filepath = Path.GetDirectoryName(grammSpecFilename);

                if (output != null) output.log("Grammar conversion specification loading from CSV files");

                string gramSpecFileFormat = filepath + Path.DirectorySeparatorChar + filebase + "_format.csv";
                string gramSpecFileTranslation = filepath + Path.DirectorySeparatorChar + filebase + "_translation.csv";

                grammTagConverter.LoadSpecificationCSV(gramSpecFileFormat, gramSpecFileTranslation, output);
            }
            else
            {
                if (output != null)
                {
                    output.log("Grammar conversion file format not recognized from the filepath! [" + grammSpecFilename + "]");
                }
                else
                {
                    throw new ArgumentOutOfRangeException("File format not recognized for " + nameof(textResourceResolverBase) + " Setup call.", nameof(grammSpecFilename));
                }
            }

            resourcePath = resourceFilePath;

            if (!spellAlternatorFilename.isNullOrEmpty())
            {
                var alternator = new transliterationPairSet();
                var altDef = File.ReadAllText(spellAlternatorFilename);
                alternator.LoadFromString(altDef);
                spellAlternator = alternator;
            }

            if (mode != textResourceIndexResolveMode.loadAndResolveOnQuery)
            {
                LoadLexicResource(output, resourceFilePath);
            }
            else
            {
                mode = textResourceIndexResolveMode.resolveOnQuery;
            }
        }

        protected String resourcePath = "";
        protected Boolean isLoaded { get; set; } = false;

        protected Object loadStatusLock = new Object();

        public override lexicInflection this[string key]
        {
            get
            {
                return GetLexicUnit(key);
            }
        }

        private Object GetLexicUnitLock = new Object();

        /// <summary>
        /// Gets the lexic unit.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="logger">The logger.</param>
        /// <returns></returns>
        public lexicInflection GetLexicUnit(String key, ILogBuilder logger = null)
        {
            if (settings.doUseLockOnResolveCall)
            {
                lock (GetLexicUnitLock)
                {
                    switch (mode)
                    {
                        case textResourceIndexResolveMode.resolveOnQuery:
                            return resolve(key, logger);
                            break;

                        case textResourceIndexResolveMode.resolveOnLoad:
                            return GetLexicUnitBase(key, logger);
                            break;

                        default:
                        case textResourceIndexResolveMode.none:
                        case textResourceIndexResolveMode.loadAndResolveOnQuery:
                            return resolve(key, logger);
                            break;
                    }
                }
            }
            else
            {
                switch (mode)
                {
                    case textResourceIndexResolveMode.resolveOnQuery:
                        return resolve(key, logger);
                        break;

                    case textResourceIndexResolveMode.resolveOnLoad:
                        return GetLexicUnitBase(key, logger);
                        break;

                    default:
                    case textResourceIndexResolveMode.none:
                    case textResourceIndexResolveMode.loadAndResolveOnQuery:
                        return resolve(key, logger);
                        break;
                }
            }
            return null;
        }

        protected lexicInflection GetLexicUnitBase(String key, ILogBuilder logger = null)
        {
            lexicInflection item = base[key];
            if (item == null)
            {
                if (spellAlternator.IsInitiated)
                {
                    if (spellAlternatives.ContainsKey(key))
                    {
                        key = spellAlternatives[key];
                        item = base[key];
                    }
                }
                if (item != null)
                {
                }
            }
            return item;
        }

        /// <summary>
        /// Saves the used cache.
        /// </summary>
        /// <param name="localLemmaResourcePath">The local lemma resource path.</param>
        public void SaveUsedCache(string localLemmaResourcePath, Boolean clearCache = true)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var entries in base.items.Values)
            {
                foreach (var pair in entries)
                {
                    lexicInflection inflect = pair.Value;

                    foreach (lexicGrammarCase gCase in inflect)
                    {
                        if (gCase.tags != null)
                        {
                            sb.AppendLine(RenderToLine(pair.Value.inflectedForm, pair.Value.lemmaForm, grammTagConverter.ConvertToString(gCase.tags)));

                            if (clearCache)
                            {
                                gCase.tags = null;
                            }
                        }
                    }
                }
            }

            String txt = sb.ToString();
            var fi = localLemmaResourcePath.getWritableFile(imbSCI.Data.enums.getWritableFileMode.overwrite);
            File.AppendAllText(fi.FullName, txt);
        }

        /// <summary>
        /// It will get all inflections of the same lemma, if <c>allInflections</c> is supplied, it will remove all matched inflectional form from the list.
        /// </summary>
        /// <param name="inflection">The inflection.</param>
        /// <param name="allInflections">All inflections.</param>
        /// <param name="logger">The logger.</param>
        /// <returns></returns>
        public lexicGraphSetWithLemma GetLemmaSetForInflection(String inflection, List<String> allInflections = null, ILogBuilder logger = null)
        {
            if (!isLoaded) LoadLexicResource(logger, resourcePath);

            lexicInflection inflect = resolve(inflection, logger);

            if (inflect == null) return new lexicGraphSetWithLemma();

            lexicGraphSetWithLemma inflectSet = registratedLemmaIndex[inflect.lemmaForm];
            List<String> keys = inflectSet.Keys.ToList();
            if (allInflections != null)
            {
                foreach (String k in keys)
                {
                    allInflections.RemoveAll(x => x.Equals(k, StringComparison.InvariantCultureIgnoreCase));
                }
            }

            resolveIfRequired(inflectSet.Values);

            return inflectSet;
        }

        /// <summary>
        /// Resolves if required.
        /// </summary>
        /// <param name="ls">The ls.</param>
        protected void resolveIfRequired(IEnumerable<lexicInflection> ls)
        {
            foreach (lexicInflection output in ls)
            {
                foreach (lexicGrammarCase ch in output)
                {
                    if (ch.tags == null)
                    {
                        ch.tags = grammTagConverter.ConvertFromString(ch.lexicalDefinitionLine);
                    }
                }
            }
        }

        /// <summary>
        /// Retrieves the lexicInflection information, if required it will process the gram tags
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="logger">The logger.</param>
        /// <returns></returns>
        protected lexicInflection resolve(String key, ILogBuilder logger = null)
        {
            var output = GetLexicUnitBase(key, logger);

            if (output == null)
            {
                if (settings.doLogUnresolvedTokens)
                {
                    if (logger != null) logger.Append("![" + key + "] ");
                }
            }
            else
            {
                foreach (lexicGrammarCase ch in output)
                {
                    if (ch.tags == null)
                    {
                        ch.tags = grammTagConverter.ConvertFromString(ch.lexicalDefinitionLine);
                    }
                }
            }
            return output;
        }

        protected ConcurrentDictionary<String, String> spellAlternatives { get; set; } = new ConcurrentDictionary<string, string>();

        /// <summary>
        /// Used to create alternative misspelled form of inflection word in case initial query failed
        /// </summary>
        /// <value>
        /// The spell alternator.
        /// </value>
        public transliterationPairSet spellAlternator { get; set; } = new transliterationPairSet();

        public void ResetIsLoaded()
        {
            isLoaded = false;
        }

        /// <summary>
        /// Loads the lexic resource.
        /// </summary>
        /// <param name="output">The output.</param>
        /// <param name="resourceFilePath">The resource file path.</param>
        public void LoadLexicResource(ILogBuilder output, String resourceFilePath)
        {
            List<String> lines = new List<String>();

            // <---------------------------------------------- [
            if (isLoaded) return;
            String pt = "";
            if (!localCache.isNullOrEmpty())
            {
                pt = localCache;
                lines.AddRange(File.ReadLines(localCache));
            }

            if (lines.Count < 100)
            {
                pt = resourceFilePath;
                lines = new List<string>();
                lines.AddRange(File.ReadAllLines(resourceFilePath));
            }

            Int32 i = 0;
            Int32 iCycle = lines.Count() / 20;
            Int32 l = lines.Count();
            Int32 c = 0;
            Double p = 0;

            output.logStartPhase("Loading", "Loading the lexic resource - with mode: " + mode.ToString());
            output.log("Start of loading lexic resource [" + pt + "]");
            //   Parallel.ForEach(lines, new ParallelOptions { MaxDegreeOfParallelism=1 }, (line) =>

            Parallel.ForEach(lines, new ParallelOptions { MaxDegreeOfParallelism = 1 }, (line) =>
            //  Parallel.ForEach(lines, (line) =>
            {
                string inflectForm = "";
                string lemma = "";
                string gramTag = "";

                SelectFromLine(line, out inflectForm, out lemma, out gramTag);

                lexicInflection inflect = null;

                if (!inflectForm.isNullOrEmpty())
                {
                    if (!ContainsKey(inflectForm))
                    {
                        inflect = new lexicInflection(line);
                        inflect.lemmaForm = lemma;
                        inflect.name = inflectForm;
                        inflect.inflectedForm = inflectForm;
                        inflect.lexicalDefinitionLine = line;

                        if (spellAlternator.IsInitiated)
                        {
                            String altInflectedForm = spellAlternator.ConvertFromAtoB(inflectForm);
                            spellAlternatives.GetOrAdd(altInflectedForm, inflectForm);
                        }

                        Add(inflectForm, inflect);
                    }
                    else
                    {
                        inflect = base[inflectForm];
                    }

                    lexicGrammarCase gramCase = null;

                    if (mode == textResourceIndexResolveMode.resolveOnLoad)
                    {
                        var gramTagColl = grammTagConverter.ConvertFromString(gramTag);

                        gramCase = inflect.AddGrammarCase(gramTagColl);
                        gramCase.lexicalDefinitionLine = gramTag;
                    }
                    else
                    {
                        gramCase = new lexicGrammarCase();
                        gramCase.lexicalDefinitionLine = gramTag;
                        gramCase.name = "gc" + i.ToString();
                        inflect.Add(gramCase);
                    }

                    // <----------------- construction of Lemma centered dictionary

                    lexicGraphSetWithLemma lxSet = null;

                    if (!registratedLemmaIndex.ContainsKey(lemma))
                    {
                        lock (LemmaIndexLock)
                        {
                            if (!registratedLemmaIndex.ContainsKey(lemma))
                            {
                                lxSet = new lexicGraphSetWithLemma();
                                lxSet.lemmaForm = lemma;
                                registratedLemmaIndex.TryAdd(lemma, lxSet);
                            }
                        }
                    }

                    lxSet = registratedLemmaIndex[lemma];

                    if (!lxSet.ContainsKey(inflectForm))
                    {
                        lock (SetLock)
                        {
                            if (!lxSet.ContainsKey(inflectForm))
                            {
                                lxSet.TryAdd(inflect.name, inflect);
                            }
                        }
                    }

                    Interlocked.Increment(ref c);
                    Interlocked.Increment(ref i);
                    if (c > iCycle)
                    {
                        lock (loadStatusLock)
                        {
                            if (c > iCycle)
                            {
                                c = 0;
                                p = i.GetRatio(l);
                                output.AppendLine("Done: _" + p.ToString("P2") + "_");
                            }
                        }
                    }
                }
            });

            output.logEndPhase();
            output.log("End of loading process");
            isLoaded = true;
        }

        /// <summary>
        /// Gets the signature.
        /// </summary>
        /// <returns></returns>
        public new String GetSignature()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("Source file:                     " + resourcePath);

            sb.AppendLine("Registered lemma index count:    " + registratedLemmaIndex.Count);
            sb.AppendLine("Items:                           " + items.Count);

            sb.AppendLine(base.GetSignature());

            return sb.ToString();
        }

        private Object SetLock = new Object();

        private Object LemmaIndexLock = new Object();

        /// <summary>
        /// Saves the bin try.
        /// </summary>
        /// <param name="pathToCachedFile">The path to cached file.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="overwrite">if set to <c>true</c> [overwrite].</param>
        /// <returns></returns>
        public Boolean SaveBinTry(String pathToCachedFile, ILogBuilder logger, Boolean overwrite = false)
        {
            String directoryName = Path.GetDirectoryName(pathToCachedFile);
            String filePrefix = Path.GetFileNameWithoutExtension(pathToCachedFile);

            folderNode folder = new DirectoryInfo(directoryName);

            Boolean binLoaded = SaveBin(folder, logger, overwrite, filePrefix);

            return binLoaded;
        }

        /// <summary>
        /// Loads the bin try.
        /// </summary>
        /// <param name="pathToCachedFile">The path to cached file.</param>
        /// <param name="logger">The logger.</param>
        /// <returns></returns>
        public Boolean LoadBinTry(String pathToCachedFile, ILogBuilder logger)
        {
            String directoryName = Path.GetDirectoryName(pathToCachedFile);
            String filePrefix = Path.GetFileNameWithoutExtension(pathToCachedFile);

            folderNode folder = new DirectoryInfo(directoryName);

            Boolean binLoaded = LoadBin(folder, logger, filePrefix);
            if (binLoaded)
            {
                isLoaded = true;
            }
            return binLoaded;
        }

        /// <summary>
        /// Loads the bin.
        /// </summary>
        /// <param name="folder">The folder.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="filenamePrefix">The filename prefix.</param>
        public Boolean LoadBin(folderNode folder, ILogBuilder logger, String filenamePrefix = "lexicResource")
        {
            var files = folder.findFiles(filenamePrefix + "_*.bin", SearchOption.TopDirectoryOnly);
            Int32 c = 0;
            foreach (var pair in files)
            {
                String filename = Path.GetFileNameWithoutExtension(pair);
                String letter = filename.Replace(filenamePrefix + "_", "");

                IFormatter formatter = new BinaryFormatter();
                Stream stream = new FileStream(pair, FileMode.Open, FileAccess.Read, FileShare.Read);
                ConcurrentDictionary<String, lexicInflection> dict = (ConcurrentDictionary<String, lexicInflection>)formatter.Deserialize(stream);
                stream.Close();

                //ConcurrentDictionary<String, lexicInflection> dict = Accord.IO.Serializer.Load<ConcurrentDictionary<String, lexicInflection>>(pair);
                if (dict.Any())
                {
                    c++;
                    logger.log("File [" + filename + "] loaded --> index [" + letter + "]");
                    if (items.ContainsKey(letter))
                    {
                        items[letter].Clear();
                        items[letter].AddRange(dict);
                    }
                    else
                    {
                        items.TryAdd(letter, dict);
                    }
                }
            }

            if (c > 0)
            {
                logger.log("[" + c + "] lexic files loaded from [" + folder.path + "]");
                return true;
            }
            return false;
        }

        /// <summary>
        /// Saves the bin.
        /// </summary>
        /// <param name="folder">The folder.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="skipExisting">if set to <c>true</c> [skip existing].</param>
        /// <param name="filenamePrefix">The filename prefix.</param>
        public Boolean SaveBin(folderNode folder, ILogBuilder logger, Boolean skipExisting = true, String filenamePrefix = "lexicResource")
        {
            Int32 c = 0;
            foreach (var pair in items)
            {
                String pbin = folder.pathFor(filenamePrefix + "_" + pair.Key + ".bin", imbSCI.Data.enums.getWritableFileMode.none, "Binary serialized lexic entries starting with [" + pair.Key + "]");
                if (skipExisting && File.Exists(pbin))
                {
                    logger.log("File [" + pbin + "] exists. Skipping binary serialization");
                }
                else
                {
                    IFormatter formatter = new BinaryFormatter();
                    Stream stream = new FileStream(pair.Key, FileMode.Create, FileAccess.Write, FileShare.None);
                    formatter.Serialize(stream, pair.Value);
                    stream.Close();

                    //ConcurrentDictionary<String, lexicInflection> dict = pair.Value;
                    //dict.Save(pbin);
                    c++;
                }
            }

            if (c > 0)
            {
                logger.log("[" + c + "] lexic files serialized to [" + folder.path + "]");
                return true;
            }
            return false;
        }
    }
}