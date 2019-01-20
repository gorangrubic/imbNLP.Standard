using imbNLP.Toolkit.Documents;
using imbNLP.Toolkit.Planes.Core;
using imbSCI.Core.reporting;
using System;
using System.Collections.Generic;
using System.Text;

namespace imbNLP.Toolkit.Entity
{
    /// <summary>
    /// Function that blends multiple documents into single document representation
    /// </summary>
    public class DocumentBlenderFunction : PlaneMethodFunctionBase, IEntityPlaneFunction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentBlenderFunction"/> class.
        /// </summary>
        public DocumentBlenderFunction()
        {
        }

        public Boolean DoKeepPagesSeparated
        {
            get
            {
                return options.HasFlag(DocumentBlenderFunctionOptions.separatePages);
            }
        }

        /// <summary>
        /// Gets or sets the options.
        /// </summary>
        /// <value>
        /// The options.
        /// </value>
        public DocumentBlenderFunctionOptions options { get; set; } = DocumentBlenderFunctionOptions.binaryAggregation | DocumentBlenderFunctionOptions.pageLevel | DocumentBlenderFunctionOptions.keepLayersInMemory;

        /// <summary>
        /// Breaks to content units.
        /// </summary>
        /// <param name="layers">The layers.</param>
        /// <param name="options">The options.</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException">
        /// </exception>
        public List<String> breakToContentUnits(TextDocumentLayerCollection layers, DocumentBlenderFunctionOptions options)
        {
            List<String> units = new List<string>();
            if (options.HasFlag(DocumentBlenderFunctionOptions.pageLevel))
            {
                units.Add(layers.ToString());
                return units;
            }
            if (options.HasFlag(DocumentBlenderFunctionOptions.blockLevel))
            {
                throw new NotImplementedException();//units.Add(layers.ToString());
                return units;
            }
            if (options.HasFlag(DocumentBlenderFunctionOptions.sentenceLevel))
            {
                throw new NotImplementedException();
                //units.Add(layers.ToString());
                return units;
            }
            return units;
        }

        private List<String> FilterUnits(List<String> units)
        {
            List<string> contentUnitHashList = new List<string>();
            List<String> filtered = new List<string>();
            foreach (String unit in units)
            {
                if (unit.Length > 0)
                {
                    if (!contentUnitHashList.Contains(unit))
                    {
                        filtered.Add(unit);
                    }
                }
            }
            units = filtered;
            return filtered;
        }

        private String JoinUnits(List<String> units)
        {
            StringBuilder sb = new StringBuilder();
            foreach (String unit in units)
            {
                if (unit.Length > 0)
                {
                    sb.AppendLine(unit);
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// Blends all pages into one text document.
        /// </summary>
        /// <param name="entityTexts">The entity texts.</param>
        /// <returns></returns>
        public TextDocument blendToTextDocument(TextDocumentSet entityTexts)
        {
            List<String> units = new List<string>();

            // break down
            foreach (TextDocumentLayerCollection entityText in entityTexts)
            {
                units.AddRange(breakToContentUnits(entityText, options));
            }

            // filter for unique
            if (options.HasFlag(DocumentBlenderFunctionOptions.uniqueContentUnitsOnly))
            {
                units = FilterUnits(units);
            }

            TextDocument output = new TextDocument(JoinUnits(units));
            output.name = entityTexts.name;

            return output;
        }

        /// <summary>
        /// Blends pages into separate text documents.
        /// </summary>
        /// <param name="entityTexts">The entity texts.</param>
        /// <returns></returns>
        public List<TextDocument> blendToSeparateTextDocuments(TextDocumentSet entityTexts)
        {
            List<TextDocument> output = new List<TextDocument>();

            foreach (TextDocumentLayerCollection entityText in entityTexts)
            {
                List<String> units = breakToContentUnits(entityText, options);
                if (options.HasFlag(DocumentBlenderFunctionOptions.uniqueContentUnitsOnly))
                {
                    units = FilterUnits(units);
                }
                TextDocument txDoc = new TextDocument(JoinUnits(units));
                txDoc.name = entityText.name;
                output.Add(txDoc);
            }

            return output;
        }

        /// <summary>
        /// Describes the specified logger.
        /// </summary>
        /// <param name="logger">The logger.</param>
        public override void Describe(ILogBuilder logger)
        {
            logger.AppendPair("Blending options", options.ToString(), true, "\t\t\t");
        }
    }
}