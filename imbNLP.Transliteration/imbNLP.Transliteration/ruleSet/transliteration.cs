// --------------------------------------------------------------------------------------------------------------------
// <copyright file="transliteration.cs" company="imbVeles" >
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
// Project: imbNLP.Transliteration
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
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace imbNLP.Transliteration.ruleSet
{
    /// <summary>
    /// Helper class
    /// </summary>
    public static class transliteration
    {
        /// <summary>
        /// String that separates values in the replacement pair
        /// </summary>
        public const String DEF_MEMBER_SEPARATOR = ":";

        /// <summary>
        /// String that separates pairs from each other
        /// </summary>
        public const String DEF_PAIR_SEPARATOR = "|";

        /// <summary>
        /// Prefix that defines a comment line
        /// </summary>
        public const String DEF_COMMENT = "\\";

        /// <summary>
        /// Parameter value assigment separator
        /// </summary>
        public const String DEF_PARAM_SEPARATOR = "=";

        public static Regex regex_pairSelector = new Regex(@"([\w]+\:[\w]+)");

        /// <summary>
        /// Regex that selects parametar entries in the transliteration file
        /// </summary>
        public static Regex regex_paramSelector = new Regex("([\\w]+\\=[\\w\\s\\\"\"\\,;:]+;)");

        /// <summary>
        /// Format for pair serialization (when writing a pairSet to the harddrive)
        /// </summary>
        public const String FORMAT_PAIR = "{0}:{1}";

        /// <summary>
        /// Format for serializing a parameter and its value
        /// </summary>
        public const String FORMAT_PARAMETER = "{0}={1};";

        private static Dictionary<String, transliterationPairSet> pairSetsByFilename = new Dictionary<string, transliterationPairSet>();

        private static List<transliterationPairSet> pairSets = new List<transliterationPairSet>();

        // private static Dictionary<String, transliterationPairSet> pairSetsByLangA = new Dictionary<string, transliterationPairSet>();
        // private static Dictionary<String, transliterationPairSet> pairSetsByLangB = new Dictionary<string, transliterationPairSet>();

        private static Object prepareLock = new Object();

        /// <summary>
        /// Gets a value indicating whether this instance is prepared.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is prepared; otherwise, <c>false</c>.
        /// </value>
        internal static Boolean isPrepared
        {
            get
            {
                return pairSetsByFilename.Any();
            }
        }

        /// <summary>
        /// Gets the transliteration pair set by filename or <see cref="transliterationPairSet.lang_A_id"/> identifier
        /// </summary>
        /// <param name="id">The identifier [filename or lang_A_id]</param>
        /// <returns>null if no transliteration pair set found</returns>
        public static transliterationPairSet GetTransliterationPairSet(String id)
        {
            if (!isPrepared) getPrepared();

            transliterationPairSet output = null;

            if (pairSetsByFilename.ContainsKey(id))
            {
                return pairSetsByFilename[id];
            }
            else
            {
                foreach (var set in pairSets)
                {
                    if (set.lang_A_id == id) return set;
                }
            }

            return output;
        }

        /// <summary>
        /// Gets the transliteration engine prepared
        /// </summary>
        internal static void getPrepared()
        {
            lock (prepareLock)
            {
                if (!isPrepared)
                {
                    List<FileInfo> files = getDefinitionFiles();

                    foreach (FileInfo fi in files)
                    {
                        transliterationPairSet newSet = new transliterationPairSet();
                        String def = File.ReadAllText(fi.FullName);
                        newSet.LoadFromString(def);

                        String psName = Path.GetFileNameWithoutExtension(fi.Name);
                        pairSetsByFilename.Add(psName, newSet);
                        pairSets.Add(newSet);
                    }
                }
            }
        }

        /// <summary>
        /// Gets the definitions directory path.
        /// </summary>
        /// <returns></returns>
        internal static String getDefinitionDirectoryPath()
        {
            return "resources" + Path.DirectorySeparatorChar + "transliteration";
        }

        /// <summary>
        /// Gets all transliteration ruleset definition files.
        /// </summary>
        /// <returns></returns>
        internal static List<FileInfo> getDefinitionFiles()
        {
            DirectoryInfo di = new DirectoryInfo(getDefinitionDirectoryPath());
            List<FileInfo> allFiles = di.GetFiles("*.txt").ToList();

            List<FileInfo> output = new List<FileInfo>();
            foreach (FileInfo f in allFiles)
            {
                if (f.Name != "readme.txt")
                {
                    output.Add(f);
                }
            }
            return output;
        }
    }
}