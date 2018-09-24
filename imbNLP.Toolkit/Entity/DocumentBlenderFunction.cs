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

        public DocumentBlenderFunction()
        {

        }

        public DocumentBlenderFunctionOptions options { get; set; } = DocumentBlenderFunctionOptions.binaryAggregation | DocumentBlenderFunctionOptions.pageLevel;

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
                List<string> contentUnitHashList = new List<string>();
                List<String> filtered = new List<string>();
                foreach (String unit in units)
                {
                    if (!contentUnitHashList.Contains(unit))
                    {
                        filtered.Add(unit);
                    }
                }
                units = filtered;
            }

            // options.HasFlag(DocumentBlenderFunctionOptions.binaryAggregation)

            // blend
            StringBuilder sb = new StringBuilder();
            foreach (String unit in units)
            {
                sb.AppendLine(unit);
            }

            TextDocument output = new TextDocument(sb.ToString());
            output.name = entityTexts.name;

            return output;

        }

        public override void Describe(ILogBuilder logger)
        {
            logger.AppendPair("Blending options", options.ToString(), true, "\t\t\t");
        }
    }

}