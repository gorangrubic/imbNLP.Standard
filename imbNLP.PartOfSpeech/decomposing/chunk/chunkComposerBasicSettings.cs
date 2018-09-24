// --------------------------------------------------------------------------------------------------------------------
// <copyright file="chunkComposerBasicSettings.cs" company="imbVeles" >
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
using imbNLP.PartOfSpeech.flags.basic;
using imbNLP.PartOfSpeech.pipeline.postprocessor;
using imbSCI.Core.extensions.typeworks;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Xml.Serialization;

namespace imbNLP.PartOfSpeech.decomposing.chunk
{
    /// <summary>
    ///
    /// </summary>
    /// <seealso cref="imbNLP.PartOfSpeech.pipeline.postprocessor.pipelinePostprocessSettings" />
    public class chunkComposerBasicSettings : pipelinePostprocessSettings
    {
        /// <summary> If true it will use gram tag criteria for chunk creation </summary>
        [Category("Switch")]
        [DisplayName("doCheckGramTagCriteria")]
        [Description("If true it will use gram tag criteria for chunk creation")]
        public Boolean doCheckGramTagCriteria { get; set; } = true;

        /// <summary>
        /// Creates multi-line description of current configuration
        /// </summary>
        /// <returns>List of description lines</returns>
        public override List<string> DescribeSelf()
        {
            List<String> output = new List<string>();

            output.Add(" > Performs POS chunk detection and extraction in [" + iterations + "] iterations.");

            if (keepAllInOutput)
            {
                output.Add(" > Newly composed POS chunks are fed into iteration output collection together with the input POS tokens.");
                output.Add("----- (i.e.: the tokens that are already processed in the iteration)");
            }
            else
            {
                output.Add(" > Only newly composed POS chunks are fed into iteration output collection.");
                output.Add("----- (i.e.: chunks composed in prior iteration, then consumed in this iteration - is excluded from the output)");
            }

            output.AddRange(rules.DescribeSelf());

            if (doCheckGramTagCriteria)
            {
                output.Add(" > Gram Tag criteria evaluation is enabled.");
            }
            else
            {
                output.Add(" > Gram Tag criteria evaluation is disabled.");
            }

            return output;
        }

        private Dictionary<string, Type> _pos_enum_types;

        /// <summary>
        /// Locally used dictionary of pos enums
        /// </summary>
        /// <value>
        /// The position enum types.
        /// </value>
        [XmlIgnore]
        public Dictionary<string, Type> pos_enum_types
        {
            get
            {
                if (_pos_enum_types == null)
                {
                    prepareEnumTypes();
                }
                return _pos_enum_types;
            }
        }

        private void prepareEnumTypes()
        {
            var t = typeof(pos_type);
            _pos_enum_types = t.CollectTypes(CollectTypeFlags.includeEnumTypes | CollectTypeFlags.ofParentNamespace | CollectTypeFlags.ofThisAssembly);
        }

        public chunkMatchRuleSet rules { get; set; } = new chunkMatchRuleSet();

        public chunkComposerBasicSettings()
        {
        }

        public void setDefaults()
        {
            if (!rules.Any())
            {
                rules.AddTypeAndFlagRule(new pos_type[] { pos_type.A, pos_type.N }, new Type[] { typeof(pos_gender), typeof(pos_gramaticalCase), typeof(pos_number) }, pos_type.N).priority = 50;

                // rules.AddTypeAndFlagRule(new pos_type[] { pos_type.ADV, pos_type.V }, new Type[] { typeof(pos_gender), typeof(pos_verbform), typeof(pos_number) }, pos_type.V).priority = 50;

                rules.AddTypeAndFlagRule(new pos_type[] { pos_type.N, pos_type.PREP, pos_type.N }, new Type[] { }, pos_type.N).priority = 75;

                rules.AddTypeAndFlagRule(new pos_type[] { pos_type.N, pos_type.CONJ, pos_type.N }, new Type[] { typeof(pos_gender), typeof(pos_gramaticalCase), typeof(pos_number) }, pos_type.N).priority = 200;

                rules.SaveTypeNames();
            }
        }

        [XmlIgnore]
        private Boolean isPrepared { get; set; } = false;

        public void checkReady()
        {
            if (!isPrepared)
            {
                rules.prepare(pos_enum_types);

                isPrepared = true;
            }
        }
    }
}