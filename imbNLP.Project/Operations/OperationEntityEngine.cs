using imbNLP.Project.Operations.Data;
using imbNLP.Toolkit;
using imbNLP.Toolkit.Documents;
using imbNLP.Toolkit.Documents.Ranking.Core;
using imbNLP.Toolkit.Entity;
using imbNLP.Toolkit.ExperimentModel;

using imbNLP.Toolkit.Planes;
using imbNLP.Toolkit.Typology;
using imbSCI.Core.reporting;
using System;
using System.Collections.Generic;
using System.IO;

namespace imbNLP.Project.Operations
{
    /// <summary>
    ///
    /// </summary>
    /// <seealso cref="imbNLP.Toolkit.Planes.MethodDesignBase" />
    public class OperationEntityEngine : MethodDesignBase
    {
        //  public static String RenderCachePath { get; set; } = CacheProvider.Deploy(new System.IO.DirectoryInfo(mainSettings.cachePath));
        public OperationEntityEngine()
        {
        }

        public OperationEntityEngine(EntityPlaneMethodSettings settings, ToolkitExperimentNotes _notes, ILogBuilder logger)
        {
            DeploySettings(settings, _notes, logger);
        }

        public void Describe(ILogBuilder logger)
        {
            if (logger != null)
            {
                render.Describe(logger);
                filter.Describe(logger);
            }
        }

        /// <summary>
        /// Gets or sets the render.
        /// </summary>
        /// <value>
        /// The render.
        /// </value>
        public DocumentRenderFunction render { get; set; } = new DocumentRenderFunction();

        /// <summary>
        /// Gets or sets the filter.
        /// </summary>
        /// <value>
        /// The filter.
        /// </value>
        public DocumentFilterFunction filter { get; set; } = new DocumentFilterFunction();

        /// <summary>
        /// Gets or sets the blender.
        /// </summary>
        /// <value>
        /// The blender.
        /// </value>
        //public DocumentBlenderFunction blender { get; set; } = new DocumentBlenderFunction();

        /// <summary>
        /// Prepares everything for operation
        /// </summary>
        /// <param name="settings">The settings.</param>
        /// <param name="logger">The logger.</param>
        public void DeploySettings(EntityPlaneMethodSettings settings, ToolkitExperimentNotes _notes, ILogBuilder logger)
        {
            DeploySettingsBase(_notes);
            settings.cachePath = imbACE.Core.appManager.Application.folder_cache.path + Path.DirectorySeparatorChar + "BEC";  //.Add("BEC", "BEC", "Cached content for BEC experiments").path;
            SetSetupSignature(settings);

            render.instructions = new System.Collections.Generic.List<DocumentRenderInstruction>();
            render.instructions.AddRange(settings.instructions);

            if (settings.filterLimit > 0)
            {
                filter.function = TypeProviders.InputDocumentFunctions.GetInstance(settings.filterFunctionName);
                filter.limit = settings.filterLimit;
                filter.IsEnabled = true;
            }
            else
            {
                filter.IsEnabled = false;
            }

            // blender.options = settings.blenderOptions;

            //   Describe(_notes);

            CloseDeploySettingsBase();
        }

        public void DataSetInitialization(IEnumerable<WebSiteDocumentsSet> _dataset, ILogBuilder log)
        {
            OperationContext context = new OperationContext();
            context.DeployDataSet(_dataset, log);
        }

        public const Boolean DoUseCache = true;

        /// <summary>
        /// Texts the rendering.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="log">The log.</param>
        public void TextRendering(OperationContext context, ILogBuilder log, Boolean EnableRendering = true)
        {
            log.log("Text rendering");
            foreach (KeyValuePair<string, WebSiteDocumentsSet> pair in context.dataset)
            {
                foreach (WebSiteDocuments site in pair.Value)
                {
                    TextDocumentSet tds = null;

                    if (DoUseCache)
                    {
                        if (CacheProvider.IsReady)
                        {
                            tds = CacheProvider.GetCached<TextDocumentSet>(setupSignature, context.dataSetSignature, site.domain);
                        }
                    }

                    if (tds == null)
                    {
                        tds = render.RenderSiteDocuments(site, log, EnableRendering);
                    }

                    tds.name = site.domain;

                    context.renderSiteByDomain.Add(tds.name, tds);
                    foreach (var td in tds)
                    {
                        context.renderLayersByAssignedID.Add(td.name, td);
                    }

                    if (DoUseCache)
                    {
                        if (CacheProvider.IsReady)
                        {
                            CacheProvider.SetCached(setupSignature, context.dataSetSignature, tds.name, tds);
                        }
                    }
                }
            }
        }

        /*

        /// <summary>
        /// Texts the preblend filter.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="log">The log.</param>
        public void TextPreblendFilter(OperationContext context, ILogBuilder log)
        {
            // page in site filtering
            if (filter.IsEnabled)
            {
                log.log("Text preblend filter");
                Dictionary<String, TextDocumentSet> renderIndexFiltered = new Dictionary<String, TextDocumentSet>();

                filter.Learn(context.renderSiteByDomain.Values);

                foreach (KeyValuePair<string, TextDocumentSet> tds_pair in context.renderSiteByDomain)
                {
                    renderIndexFiltered.Add(tds_pair.Key, filter.FilterDocumentSet(tds_pair.Value));
                }

                foreach (KeyValuePair<string, TextDocumentSet> tds_pair in renderIndexFiltered)
                {
                    context.renderSiteByDomain[tds_pair.Key] = tds_pair.Value;
                    foreach (var td in tds_pair.Value)
                    {
                        context.renderLayersByAssignedID.Add(td.name, td);
                    }
                }
            }
        }
        */
        /*
        /// <summary>
        /// Texts the blending.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="log">The log.</param>
        public void TextBlending(OperationContext context, ILogBuilder log)
        {
            log.log("Text blending");
            Boolean DoBlendPagesIntoSingleEntity = !blender.options.HasFlag(DocumentBlenderFunctionOptions.separatePages);

            Int32 EmptyVectors = 0;
            Int32 Vectors = 0;

            foreach (TextDocumentSet entitySet in context.renderSiteByDomain.Values)
            {
                WebSiteDocuments web = context.webSiteByDomain[entitySet.name];
                SpaceLabel spaceLabel = context.spaceLabelsDomains[entitySet.name];

                if (DoBlendPagesIntoSingleEntity)
                {
                    // filter function
                    TextDocument doc = blender.blendToTextDocument(entitySet);
                    doc.labels.Add(spaceLabel.name);
                    context.textDocuments.Add(doc.name, doc);
                    if (!doc.HasContent) EmptyVectors++;
                    Vectors++;
                }
                else
                {
                    var docs = blender.blendToSeparateTextDocuments(entitySet); //blender.blendToTextDocument(selectedTexts);
                    foreach (TextDocument doc in docs)
                    {
                        doc.labels.Add(spaceLabel.name);
                        context.textDocuments.Add(doc.name, doc);

                        if (!doc.HasContent) EmptyVectors++;
                        Vectors++;
                    }
                }

                //if (!blender.options.HasFlag(DocumentBlenderFunctionOptions.keepLayersInMemory))
                //{
                //    entitySet.Clear();
                //}
            }

            log.log("Rate of empty renders [" + context.rateOfEmptyRenders.ToString("P3") + "]");

            context.rateOfEmptyRenders = EmptyVectors.GetRatio(Vectors);

            if (context.rateOfEmptyRenders > 0.5)
            {
                log.log("EMPTY RENDERS RATE [" + context.rateOfEmptyRenders.ToString("P3") + "]");
            }

            if (context.rateOfEmptyRenders > 0.9)
            {
                throw new Exception("Empty rates two high [" + context.rateOfEmptyRenders + "]");
            }

            //if (!blender.options.HasFlag(DocumentBlenderFunctionOptions.keepLayersInMemory))
            //{
            //    context.renderSiteByDomain.Clear();
            //}
        }
        */

        /// <summary>
        /// Queries factors for preprocessing requirements
        /// </summary>
        /// <param name="requirements">The requirements.</param>
        /// <returns></returns>
        public override ScoreModelRequirements CheckRequirements(ScoreModelRequirements requirements = null)
        {
            if (requirements == null) requirements = new ScoreModelRequirements();
            render.CheckRequirements(requirements);
            if (filter.IsEnabled)
            {
                filter.CheckRequirements(requirements);
            }

            //model.Deploy();
            //model.CheckRequirements(requirements);

            return requirements;
        }
    }
}