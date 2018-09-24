//namespace imbNLP.Core.contentStructureHtml.tokenizator
//{
//    #region imbVELES USING

//    using System;
//    using System.Collections.Generic;
//    using System.ComponentModel;
//    using System.Linq;
//    using System.Xml.Serialization;
//    using System.Xml.XPath;
//    using HtmlAgilityPack;
//    using imbACE.Core.commands.menu;
//    using imbACE.Core.core;
//    using imbACE.Core.operations;
//    using imbACE.Services.console;
//    using imbACE.Services.terminal;
//    using imbNLP.Core.contentStructure.collections;
//    using imbNLP.Core.contentStructure.interafaces;
//    using imbNLP.Core.contentStructure.tokenizator;
//    using imbNLP.Core.contentStructureHtml.elements;
//    using imbNLP.Core.contentTree;
//    using imbNLP.Data.extended.domain;
//    using imbNLP.Data.extended.unitex;
//    using imbNLP.Data.semanticLexicon.core;
//    using imbNLP.Data.semanticLexicon.explore;
//    using imbNLP.Data.semanticLexicon.morphology;
//    using imbNLP.Data.semanticLexicon.procedures;
//    using imbNLP.Data.semanticLexicon.source;
//    using imbNLP.Data.semanticLexicon.term;
//    using imbSCI.Core.extensions.io;
//    using imbSCI.Core.extensions.text;
//    using imbSCI.Core.files.folders;
//    using imbSCI.Core.files.unit;
//    using imbSCI.Core.reporting;
//    using imbSCI.Data;
//    using imbSCI.Data.collection.nested;
//    using imbSCI.Data.data;
//    using imbSCI.Data.enums.reporting;
//    using imbSCI.DataComplex.extensions.data.formats;
//    using imbSCI.DataComplex.extensions.text;
//    using imbSCI.DataComplex.special;
//    using imbACE.Core.xml.query;
//    using imbNLP.Data.basic;
//    using imbCommonModels.structure;
//    using imbSCI.Core.extensions.data;
//    using imbNLP.Core.contentPreprocess;
//    using imbACE.Network.web.result;
//    using imbSCI.DataComplex.tree;

//    #endregion

//    //public static class htmlSmartTokenizatorTools  {
//    //    public static
//    //}

//    /// <summary>
//    ///
//    /// </summary>
//    /// <seealso cref="tokenizatorBase" />
//    public class htmlSmartTokenizator : tokenizatorBase
//    {
//        public xPathQueryCache _xpath_allNodesWithText; // = new xPathQueryCache()

//        /// <summary>
//        /// Initializes a new instance of the <see cref="htmlSmartTokenizator"/> class.
//        /// </summary>
//        /// <param name="__settings">The settings.</param>
//        public htmlSmartTokenizator(nlpTokenizatorSettings __settings) : base(__settings)
//        {
//        }

//        public htmlContentPage tokenizeContent(ILogBuilder pRecordLog, HtmlDocument htmlDoc, basicLanguage language, node page)
//        {
//            var starttime = DateTime.Now;

//            htmlContentPage contentPage = new htmlContentPage();
//            contentPage.acceptSourcePage(page);

//            string domain = page.domain;

//            object[] resources = new object[] { language, page, flags, sentenceFlags,tokenFlags, preprocessFlags };

//            var ctb = contentTreeBuilder.getInstance(htmlDoc.CreateNavigator(), domain, page);
//            contentPage.treeBuilder = ctb;
//            var blocks = ctb.tree.breakToBlocks();

//            int b = 0;
//            for (int bi = 0; bi < blocks.Count; bi++)
//            {
//                imbTreeNodeBlock bl = blocks[bi];
//                b++;
//                makeBlock(bl, contentPage, language, resources);
//                // pRecordLog.close();
//            }

//            contentPage.recountItems();

//            contentPage.primaryFlaging(resources);
//            contentPage.secondaryFlaging(resources);

//            // <---------------

//          //  pRecordLog.log("SKIP: complete exploration of all tokens is turned off.");

//           // contentPage.saveCache();

//            pRecordLog.log("Basic semantic analysis done. Closing the process.");

//            var time = DateTime.Now.Subtract(starttime);
//            pRecordLog.log("Tree-building and tokenization [" + page.url + "] done in: " + time.TotalMilliseconds.getSeconds(4) + "s");

//            return contentPage;
//        }

//        /// <summary>
//        /// paragraphDetectionFlags flags, sentenceDetectionFlags sentenceFlags, contentPreprocessFlags preprocessFlags,  tokenDetectionFlags tokenFlags,  String content,  node page, basicLanguage language
//        /// </summary>
//        /// <param name="resources"></param>
//        /// <returns></returns>
//        public htmlContentPage tokenizeContent(ILogBuilder pRecordLog, contentTreeGlobalCollection treeGlobalRegistry, webDocument doc,  params object[] resources)
//        {
//            var starttime = DateTime.Now;
//            //paragraphDetectionFlags flags = new paragraphDetectionFlags(resources);
//            //sentenceDetectionFlags sentenceFlags = new sentenceDetectionFlags(resources);
//            //contentPreprocessFlags preprocessFlags = new contentPreprocessFlags(resources);
//            //tokenDetectionFlags tokenFlags = new tokenDetectionFlags(resources);

//            string content = resources.getFirstOfType<string>();
//            basicLanguage language = resources.getFirstOfType<basicLanguage>();
//            node page = resources.getFirstOfType<node>();

//            // <------------------------ prepare

//            htmlContentPage contentPage = new htmlContentPage();

//            //if (!imbSemanticEngineManager.settings.doEnablePageContentTokenization)
//            //{
//            //    return contentPage;
//            //}

//            contentPage.acceptSourcePage(page);

//            string domain = page.domain; // page.url.getDomainNameFromUrl(true);

//            // <---------- prethodna implementacija
//            XPathNavigator navigator = doc.getDocumentNavigator();  // resources.getOfType<XPathNavigator>();

//            HtmlDocument hapDocument = doc.document as HtmlDocument;

//            //List<IEnumerable<HtmlNode>> nodes = hapDocument.DocumentNode.Descendants("input").Select(y => y.Descendants().Where(x => x.InnerText != "")).ToList();

//            // <--------------- tree building
//            // contentTreeGlobalCollection treeGlobalRegistry = resources.getFirstOfType< contentTreeGlobalCollection>(false, false);

//            contentTreeBuilder ctb_old = treeGlobalRegistry.GetTreeBuilder(page.url);
//            contentTreeBuilder ctb = null;
//            bool buildTree = false;

//            if (ctb_old != null) {
//            } else {
//                buildTree = true;
//            }

//            ctb = ctb_old;

//            ctb = contentTreeBuilder.getInstance(navigator, domain, page);

//            //ctb.saveCache();

//            //if (buildTree) {
//            //   // pRecordLog.log("Tree structure not found at global registry (activityJobRecord) - building new. ");

//            //}
//            contentPage.treeBuilder = ctb;
//          //  pRecordLog.log("Tree structure done. ");

//            // <-------------------- tree building end

//            imbTreeNodeBlockCollection blocks = ctb.tree.breakToBlocks();
//            //pRecordLog.log("Blocks extracted from tree structure: " + blocks.Count());

//            //flags = paragraphDetectionFlags.getDefaultFlags();
//            //sentenceFlags.Add(sentenceDetectionFlag.setSentenceToParagraph,
//            //                  sentenceDetectionFlag.preprocessParagraphContent);
//            //tokenFlags = tokenDetectionFlags.getDefaultFlags();
//            //preprocessFlags = contentPreprocessFlags.getDefaultFlags();

//            //pRecordLog.log(nameof(flags) + " => " + flags.toCsvInLine(";"));
//            //pRecordLog.log(nameof(sentenceFlags) + " => " + sentenceFlags.toCsvInLine(";"));
//            //pRecordLog.log(nameof(tokenFlags) + " => " + tokenFlags.toCsvInLine(";"));
//            //pRecordLog.log(nameof(preprocessFlags) + " => " + preprocessFlags.toCsvInLine(";"));

//           // pRecordLog.open(bootstrap_containers.well.ToString(), "Block structure analysis", "NLP tokenization using hybrid [" + this.GetType().Name + "] tokenization engine");

//            int b = 0;
//            for (int bi = 0; bi < blocks.Count; bi++)
//            {
//                imbTreeNodeBlock bl = blocks[bi];
//                b++;
//                makeBlock(bl, contentPage, language, resources);
//               // pRecordLog.close();
//            }

//            //pRecordLog.close();

//           // pRecordLog.log("Tokenized content structure done. ");

//            contentPage.recountItems();

//            //pRecordLog.log("Total token counts:");
//            //var data = contentPage.AppendDataFields(null);
//            //var dt = data.buildDataTable("Token statistics");
//            //pRecordLog.AppendTable(dt);

//            contentPage.primaryFlaging(resources);
//            contentPage.secondaryFlaging(resources);

//            // <---------------

//            pRecordLog.log("SKIP: complete exploration of all tokens is turned off.");

//           // contentPage.saveCache();

//            pRecordLog.log("Basic semantic analysis done. Closing the process.");

//            var time = DateTime.Now.Subtract(starttime);
//           // imbSemanticEngineManager.log.log("Tree-building and tokenization [" + page.url + "] done in: " + time.TotalMilliseconds.getSeconds(4)+"s");
//            return contentPage;
//        }

//        /// <summary>
//        ///
//        /// </summary>
//        public paragraphDetectionFlag flags { get; set; } ;

//        /// <summary>
//        ///
//        /// </summary>
//        public sentenceDetectionFlag sentenceFlags { get; set; } =  sentenceDetectionFlag.setSentenceToParagraph | sentenceDetectionFlag.preprocessParagraphContent;

//        /// <summary>
//        ///
//        /// </summary>
//        public tokenDetectionFlag tokenFlags { get; set; } ;

//        /// <summary>
//        ///
//        /// </summary>
//        public contentPreprocessFlag preprocessFlags { get; set; } ;

//        public htmlContentBlock makeBlock(imbTreeNodeBlock bl, htmlContentPage contentPage, basicLanguage language, params object[] resources)
//        {
//            htmlContentBlock block = new htmlContentBlock();
//            block.title = "";

//            contentPage.items.Add(block);

//            int iLimit = 100;
//            //pRecordLog.open("info", "Block node [" + bl.name + "]", "processing... id:[" + b + "]");
//            for (int li = 0; li < bl.Count; li++)
//            {
//                var leaf = bl[li];

//                HtmlNode htmlNode = leaf.value as HtmlNode;
//                //IHtmlContentElement
//                htmlContentParagraph cp = new htmlContentParagraph(leaf);
//                contentTokenCollection snt = htmlNode.createSentencesFromNode(cp, null, preprocessFlags, sentenceFlags);
//                contentTokenCollection toDo = snt;
//                int i = 0;

//                do
//                {
//                    contentTokenCollection newToDo = new contentTokenCollection();
//                    //  pRecordLog.log("[" + i.ToString("D3") + "] highlevel tokens to process: " + toDo.Count() );
//                    for (int si = 0; si < toDo.Count; si++)
//                    {
//                        IContentSentence sn = toDo[si] as IContentSentence;

//                        contentTokenCollection tkns = sn.setTokensFromContent<htmlContentToken, htmlContentSubSentence>(flags, sentenceFlags,
//                                                                                              preprocessFlags,
//                                                                                              tokenFlags, resources,
//                                                                                              language);
//                        for (int ti = 0; ti < tkns.Count; ti++)
//                        {
//                            IContentToken nt = tkns[ti];

//                            if (nt is IContentSubSentence)
//                            {
//                            }
//                            else if (nt is IContentSentence)
//                            {
//                                newToDo.Add(nt);
//                            }
//                            else
//                            {
//                            }
//                        }

//                        if (flags.HasFlag(paragraphDetectionFlag.dropSentenceWithNoToken))
//                        {
//                            if (sn.content == "")
//                            {
//                                if (sn.items.Count == 0)
//                                {
//                                    cp.items.Remove(sn);
//                                }
//                            }
//                        }

//                    }
//                    toDo = newToDo;
//                    i++;
//                    if (i > iLimit)
//                    {
//                        break;
//                    }
//                } while (toDo.Any());

//                block.setItem(cp);
//            }

//            return block;
//        }
//    }
//}