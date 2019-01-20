using imbNLP.Project.Operations.Data;
using imbNLP.Toolkit;
using imbNLP.Toolkit.Documents;
using imbNLP.Toolkit.Documents.Ranking;
using imbNLP.Toolkit.Documents.Ranking.Data;
using imbNLP.Toolkit.Processing.Relations;
using imbNLP.Toolkit.Space;
using imbSCI.Core.files.folders;
using imbSCI.Core.reporting;
using imbSCI.Core.reporting.render.builders;
using System;
using System.Collections.Generic;
using System.Linq;

namespace imbNLP.Project.Operations.Tools
{
    /// <summary>
    /// 
    /// </summary>
    public static class DocumentRankingTools
    {

        /// <summary>
        /// To the index.
        /// </summary>
        /// <param name="primary">The primary.</param>
        /// <returns></returns>
        public static Dictionary<string, SpaceDocumentModel> ToIndex(this IEnumerable<SpaceDocumentModel> primary)
        {
            Dictionary<string, SpaceDocumentModel> output = new Dictionary<string, SpaceDocumentModel>();

            foreach (var doc in primary)
            {
                output.Add(doc.name, doc);
            }

            return output;

        }

        /// <summary>
        /// Constructs the pair dictionary.
        /// </summary>
        /// <param name="primary">The primary.</param>
        /// <param name="secondary">The secondary.</param>
        /// <returns></returns>
        public static ProjectionDictionary ConstructPairDictionary(IEnumerable<SpaceDocumentModel> primary, IEnumerable<SpaceDocumentModel> secondary)
        {

            ProjectionDictionary output = new ProjectionDictionary();


            var primaryIndex = primary.ToIndex();
            var secondaryIndex = secondary.ToIndex();

            foreach (string k in primaryIndex.Keys)
            {
                if (secondaryIndex.ContainsKey(k))
                {

                    ProjectionPair pair = new ProjectionPair();
                    pair.primary = primaryIndex[k];
                    pair.secondary = secondaryIndex[k];
                    pair.AssignedID = k;
                    output.Add(k, pair);


                }
            }


            return output;

        }

        /// <summary>
        /// Prepares the context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="log">The log.</param>
        /// <returns></returns>
        public static DocumentSelectResult PrepareContext(this OperationContext context, DocumentRankingMethod ranking, folderNode folder, ILogBuilder log)
        {
            DocumentSelectResult selectContext = new DocumentSelectResult();
            selectContext.stemmingContext = context.stemmContext;
            selectContext.spaceModel = context.spaceModel;
            selectContext.folder = folder;
            if (ranking != null)
            {

                selectContext.name = ranking.model.GetSignature();
                selectContext.query = ranking.query;

                builderForText builder = new builderForText();
                ranking.Describe(builder);

                builder.AppendLine("Selected features [" + selectContext.selectedFeatures.description + "].");

                selectContext.description = builder.GetContent().Replace(Environment.NewLine, "");
            }

            selectContext.selectedFeatures = context.SelectedFeatures;



            foreach (KeyValuePair<string, WebSiteDocuments> pair in context.webSiteByDomain)
            {
                selectContext.domainNameToGraph.Add(pair.Key, pair.Value?.extensions?.graph);

                foreach (WebSiteDocument doc in pair.Value.documents)
                {
                    DocumentSelectResultEntry entry = new DocumentSelectResultEntry();
                    TextDocument text = null;

                    string err = "";

                    
                    //if (context.textDocuments.ContainsKey(doc.AssignedID))
                    //{
                    //    text = context.textDocuments[doc.AssignedID];
                    //}
                    //else
                    //{
                    //    err += "Failed to find text document for [" + doc.AssignedID + "]";
                    //}

                    SpaceDocumentModel spaceDocument = context.spaceModel.documents.FirstOrDefault(x => x.name == doc.AssignedID);


                    if (spaceDocument == null)
                    {
                        err += "Failed to find space model document for [" + doc.AssignedID + "]";
                    }


                    string dn = pair.Value.domain;
                    entry.SetEntry(dn, doc, spaceDocument, text);

                    if (!entry.HasTextOrSpaceModel)
                    {
                        log.log(err);
                    }

                    selectContext.items.Add(entry);
                    //entry.SetEntry( context.context.webDocumentByAssignedID[pair.Key], webDocIDToDomain[aID], webDocumentRegistry[aID], spaceDocumentRegistry[aID], textDocumentRegistry[aID]);
                }

            }

            // PREPARATION OF MODEL
            if (ranking != null)
            {

                ranking.model.Prepare(selectContext, log);
            }
            return selectContext;

        }
    }
}
