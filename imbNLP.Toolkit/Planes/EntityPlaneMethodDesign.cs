using imbNLP.Toolkit.Core;
using imbNLP.Toolkit.Documents;
using imbNLP.Toolkit.Entity;
using imbNLP.Toolkit.ExperimentModel;
using imbNLP.Toolkit.Planes.Core;
using imbNLP.Toolkit.Space;
using imbNLP.Toolkit.Typology;
using imbSCI.Core.extensions.data;
using imbSCI.Core.reporting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace imbNLP.Toolkit.Planes
{





    /// <summary>
    /// Entity representation and transformation procedure
    /// </summary>
    /// <seealso cref="imbNLP.Toolkit.Planes.Core.IPlaneMethodDesign" />
    public class EntityPlaneMethodDesign : MethodDesignBase, IPlaneMethodDesign
    {

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
        public DocumentBlenderFunction blender { get; set; } = new DocumentBlenderFunction();

        /// <summary>
        /// Prepares everything for operation
        /// </summary>
        /// <param name="settings">The settings.</param>
        /// <param name="logger">The logger.</param>
        public void DeploySettings(IPlaneSettings settings, ToolkitExperimentNotes _notes, ILogBuilder logger)
        {
            DeploySettingsBase(_notes);
            SetSetupSignature(settings);

            EntityPlaneMethodSettings entitySettings = (EntityPlaneMethodSettings)settings;
            render.instructions = new System.Collections.Generic.List<DocumentRenderInstruction>();
            render.instructions.AddRange(entitySettings.instructions);


            if (!entitySettings.filterFunctionName.isNullOrEmpty())
            {
                filter.function = TypeProviders.InputDocumentFunctions.GetInstance(entitySettings.filterFunctionName);
                filter.limit = entitySettings.filterLimit;
                filter.IsEnabled = true;
            }
            else
            {
                filter.IsEnabled = false;
            }


            blender.options = entitySettings.blenderOptions;

            entitySettings.Describe(notes);
            render.Describe(notes);
            filter.Describe(notes);
            blender.Describe(notes);

            CloseDeploySettingsBase();
        }

        /// <summary>
        /// Executes the plane method, invoking contained functions according to the settings
        /// </summary>
        /// <param name="inputContext">The input context - related to this plane.</param>
        /// <param name="generalContext">General execution context, attached to the <see cref="T:imbNLP.Toolkit.Planes.PlanesMethodDesign" /></param>
        /// <param name="logger">The logger.</param>
        /// <returns>
        /// Retur
        /// </returns>
        public IPlaneContext ExecutePlaneMethod(IPlaneContext inputContext, ExperimentModelExecutionContext generalContext, ILogBuilder logger)
        {
            notes.logStartPhase("[1] Entity Plane - execution", "");

            IEntityPlaneContext context = inputContext as IEntityPlaneContext;
            CorpusPlaneContext outputContext = new CorpusPlaneContext();
            outputContext.provider.StoreAndReceive(context);

            outputContext.dataset = context.dataset;

            // ---------------- rendering procedure
            Dictionary<WebSiteDocumentsSet, List<TextDocumentSet>> renderIndex = new Dictionary<WebSiteDocumentsSet, List<TextDocumentSet>>();
            Dictionary<string, SpaceLabel> labels = new Dictionary<string, SpaceLabel>();

            Dictionary<WebSiteDocuments, TextDocumentSet> sitesToRenders = new Dictionary<WebSiteDocuments, TextDocumentSet>();
            Dictionary<String, WebSiteDocuments> inputSites = new Dictionary<string, WebSiteDocuments>();
            Dictionary<String, TextDocumentSet> inputTextRenders = new Dictionary<string, TextDocumentSet>();
            Dictionary<WebSiteDocuments, List<SpaceLabel>> inputSiteVsLabels = new Dictionary<WebSiteDocuments, List<SpaceLabel>>();

            Int32 c = 0;
            // rendering
            foreach (WebSiteDocumentsSet docSet in context.dataset)
            {
                if (docSet.name.isNullOrEmpty() || docSet.name == SpaceLabel.UNKNOWN)
                {
                    outputContext.space.label_unknown = new SpaceLabel(SpaceLabel.UNKNOWN);
                    labels.Add(SpaceLabel.UNKNOWN, outputContext.space.label_unknown);
                }
                else
                {
                    SpaceLabel lab = new SpaceLabel(docSet.name);
                    labels.Add(lab.name, lab);
                    outputContext.space.labels.Add(lab);
                }

                String datasetSignature = context.dataset.GetDataSetSignature();

                // ---- render
                List<TextDocumentSet> textSetForLabel = new List<TextDocumentSet>();

                if (CacheProvider.IsReady)
                {

                    foreach (WebSiteDocuments site in docSet)
                    {
                        TextDocumentSet tds = CacheProvider.GetCached<TextDocumentSet>(setupSignature, datasetSignature, site.domain);

                        if (tds == null)
                        {
                            tds = render.RenderSiteDocuments(site, logger);
                            CacheProvider.SetCached(setupSignature, datasetSignature, tds.name, tds);
                        }
                        else
                        {
                            tds.name = site.domain;
                        }


                        textSetForLabel.Add(tds);
                    }
                }
                else
                {
                    textSetForLabel = render.RenderDocumentSet(docSet, logger);
                    foreach (TextDocumentSet ws in textSetForLabel)
                    {
                        CacheProvider.SetCached(setupSignature, datasetSignature, ws.name, ws);
                    }
                }

                // // <--- performs the rendering

                textSetForLabel.ForEach(x => inputTextRenders.Add(x.name, x));
                // --- rest of indexing

                docSet.ForEach(x => inputSites.Add(x.domain, x));
                renderIndex.Add(docSet, textSetForLabel);


                foreach (WebSiteDocuments site in docSet)
                {
                    inputSiteVsLabels.Add(site, new List<SpaceLabel>());
                    inputSiteVsLabels[site].Add(labels[docSet.name]);
                    c++;
                }

            }

            notes.log("Text document for [" + c + "] entities created");

            // tmp index
            foreach (String key in inputSites.Keys)
            {
                sitesToRenders.Add(inputSites[key], inputTextRenders[key]);
            }

            // page in site filtering
            if (filter.IsEnabled)
            {
                Dictionary<WebSiteDocuments, TextDocumentSet> renderIndexFiltered = new Dictionary<WebSiteDocuments, TextDocumentSet>();

                filter.Learn(inputTextRenders.Values);

                foreach (KeyValuePair<WebSiteDocuments, TextDocumentSet> pair in sitesToRenders)
                {
                    renderIndexFiltered.Add(pair.Key, filter.FilterDocumentSet(pair.Value));
                }
                sitesToRenders = renderIndexFiltered;
            }


            Dictionary<String, TextDocumentSet> TextDocumentsByDomainName = new Dictionary<string, TextDocumentSet>();
            foreach (var pair in sitesToRenders)
            {
                TextDocumentsByDomainName.Add(pair.Key.domain, pair.Value);
            }


            // blending pages into single page per web site
            foreach (var pair in renderIndex)
            {
                foreach (TextDocumentSet entitySet in pair.Value)
                {
                    TextDocumentSet selectedTexts = TextDocumentsByDomainName[entitySet.name];

                    // filter function
                    TextDocument doc = blender.blendToTextDocument(selectedTexts);
                    WebSiteDocuments web = inputSites[entitySet.name];

                    doc.labels.AddRange(inputSiteVsLabels[web].Select(x => x.name));

                    outputContext.corpus_documents.Add(doc);
                }
            }

            notes.logEndPhase();


            return outputContext;

        }
    }

}