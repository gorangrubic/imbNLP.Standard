// --------------------------------------------------------------------------------------------------------------------
// <copyright file="lexiconConsole.cs" company="imbVeles" >
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
// Project: imbNLP.Data
// Author: Goran Grubic
// ------------------------------------------------------------------------------------------------------------------
// Project web site: http://blog.veles.rs
// GitHub: http://github.com/gorangrubic
// Mendeley profile: http://www.mendeley.com/profiles/goran-grubi2/
// ORCID ID: http://orcid.org/0000-0003-2673-9471
// Email: hardy@veles.rs
// </summary>
// ------------------------------------------------------------------------------------------------------------------
namespace imbNLP.Data.semanticLexicon.console
{
    using imbACE.Core.commands.menu;
    using imbACE.Core.core;
    using imbACE.Core.operations;
    using imbACE.Services.console;
    using imbACE.Services.terminal;
    using imbNLP.Data.extended.domain;
    using imbNLP.Data.extended.unitex;
    using imbNLP.Data.semanticLexicon.core;
    using imbNLP.Data.semanticLexicon.explore;
    using imbNLP.Data.semanticLexicon.morphology;
    using imbNLP.Data.semanticLexicon.procedures;
    using imbNLP.Data.semanticLexicon.source;
    using imbNLP.Data.semanticLexicon.term;
    using imbSCI.Core.extensions.data;
    using imbSCI.Core.extensions.io;
    using imbSCI.Core.extensions.table;
    using imbSCI.Core.extensions.text;
    using imbSCI.Core.extensions.typeworks;
    using imbSCI.Core.files.search;
    using imbSCI.Data;
    using imbSCI.Data.enums;
    using imbSCI.Data.enums.reporting;
    using imbSCI.DataComplex.extensions.data.formats;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading;

    /// <summary>
    /// Lexicon Console - performs operations over Semantic Lexicon
    /// </summary>
    /// <seealso cref="aceCommonTypes.primitives.imbBindable" />
    public class lexiconConsole : aceAdvancedConsole<lexiconConsoleState, lexiconConsoleWorkspace>
    {
        public override lexiconConsoleWorkspace workspace
        {
            get
            {
                if (_workspace == null)
                {
                    _workspace = new lexiconConsoleWorkspace(this);
                }
                return _workspace;
            }
        }

        [aceMenuItem(aceMenuItemAttributeRole.Key, "ini")]
        [aceMenuItem(aceMenuItemAttributeRole.aliasNames, "ini")]
        [aceMenuItem(aceMenuItemAttributeRole.DisplayName, "Initialize")]
        [aceMenuItem(aceMenuItemAttributeRole.Category, "corpus")]
        [aceMenuItem(aceMenuItemAttributeRole.Description, "Initializes corpus creation")]
        [aceMenuItem(aceMenuItemAttributeRole.ExpandedHelp, "It makes local copy of the corpus input and runs initial checks: separates english words from corpus, known personal names, lastnames, towns, language and country names and codes")]
        [aceMenuItem(aceMenuItemAttributeRole.DisabledRemarks, "(disabled)")]
        // [aceMenuItem(aceMenuItemAttributeRole.ConfirmMessage, "Are you sure?")]  // [aceMenuItem(aceMenuItemAttributeRole.EnabledRemarks, "")]
        // [aceMenuItem(aceMenuItemAttributeRole.externalHelpFilename, "aceOperation_corpusInitialize.md")]
        //[aceMenuItem(aceMenuItemAttributeRole.CmdParamList, "param:type;paramb:type;")]
        /// <summary>
        /// Method of menu option Initialize (key:ini). <args> expects param: param:type;paramb:type;
        /// </summary>
        /// <param name="args"><seealso cref="aceOperationArgs"/> requered parameters: param:type;paramb:type;</param>
        /// <remarks>
        /// <para>It makes local copy of the corpus input and runs initial checks: separates english words from corpus, known personal names, lastnames, towns, language and country names and codes</para>
        /// <para>Initializes corpus creation</para>
        /// <para>Message if item disabled: (disabled)</para>
        /// </remarks>
        /// <seealso cref="aceOperationSetExecutorBase"/>
        public void aceOperation_corpusInitialize(aceOperationArgs args)
        {
            semanticLexiconManager.manager.constructor.startConstruction();

            semanticLexiconManager.manager.constructor.runStage("s0", true, 1, false, true, true, response);
        }

        [aceMenuItem(aceMenuItemAttributeRole.Key, "L")]
        [aceMenuItem(aceMenuItemAttributeRole.aliasNames, "list")]
        [aceMenuItem(aceMenuItemAttributeRole.DisplayName, "List")]
        [aceMenuItem(aceMenuItemAttributeRole.Category, "lexicon")]
        [aceMenuItem(aceMenuItemAttributeRole.Description, "List will create a text output in the corpus project folder with all elements queried from the Lexicon ")]
        [aceMenuItem(aceMenuItemAttributeRole.ExpandedHelp, "Command will ask for type of element to be extracted (Concept, Lemma, Instance...)")]
        [aceMenuItem(aceMenuItemAttributeRole.DisabledRemarks, "(disabled)")]
        // [aceMenuItem(aceMenuItemAttributeRole.ConfirmMessage, "Are you sure?")]  // [aceMenuItem(aceMenuItemAttributeRole.EnabledRemarks, "")]
        // [aceMenuItem(aceMenuItemAttributeRole.externalHelpFilename, "aceOperation_lexiconList.md")]
        //[aceMenuItem(aceMenuItemAttributeRole.CmdParamList, "")]
        /// <summary>
        /// Method of menu option List (key:L). <args> expects param:
        /// </summary>
        /// <param name="args"><seealso cref="aceOperationArgs"/> requered parameters: </param>
        /// <remarks>
        /// <para>Command will ask for type of element to be extracted (Concept, Lemma, Instance...)</para>
        /// <para>List will create a text output in the corpus project folder with all elements queried from the Lexicon </para>
        /// <para>Message if item disabled: (disabled)</para>
        /// </remarks>
        /// <seealso cref="aceOperationSetExecutorBase"/>
        public void aceOperation_lexiconList(aceOperationArgs args)
        {
            listElementType el = (listElementType)aceTerminalInput.askForOption("What element type you want to list out into txt file?", listElementType.concepts);
            StringBuilder sb = new StringBuilder();
            switch (el)
            {
                case listElementType.concepts:
                    foreach (var c in semanticLexiconManager.manager.lexiconContext.Concepts)
                    {
                        sb.AppendLine(c.name);
                    }
                    break;

                case listElementType.instances:
                    foreach (var c in semanticLexiconManager.manager.lexiconContext.TermInstances)
                    {
                        sb.AppendLine(c.name);
                    }
                    break;

                case listElementType.lemmas:
                    foreach (var c in semanticLexiconManager.manager.lexiconContext.TermLemmas)
                    {
                        sb.AppendLine(c.name);
                    }
                    break;
            }
            string pt = semanticLexiconManager.manager.constructor.projectFolderStructure.pathFor(el.toString() + "_list.txt");
            sb.ToString().saveStringToFile(pt, getWritableFileMode.overwrite);
        }

        [aceMenuItem(aceMenuItemAttributeRole.Key, "s")]
        [aceMenuItem(aceMenuItemAttributeRole.aliasNames, "stage")]
        [aceMenuItem(aceMenuItemAttributeRole.DisplayName, "Stage run")]
        [aceMenuItem(aceMenuItemAttributeRole.Category, "lexicon")]
        [aceMenuItem(aceMenuItemAttributeRole.Description, "The n-th stage of the Semantic Lexicon construction")]
        [aceMenuItem(aceMenuItemAttributeRole.ExpandedHelp, "Process the specified number of entries form the stage input")]
        [aceMenuItem(aceMenuItemAttributeRole.DisabledRemarks, "(disabled)")]
        // [aceMenuItem(aceMenuItemAttributeRole.ConfirmMessage, "Are you sure?")]  // [aceMenuItem(aceMenuItemAttributeRole.EnabledRemarks, "")]
        // [aceMenuItem(aceMenuItemAttributeRole.externalHelpFilename, "aceOperation_lexiconStage01.md")]
        [aceMenuItem(aceMenuItemAttributeRole.CmdParamList, "sname=\"s1\":String;take=100:Int32;savemodels=true:Boolean;debug=true:Boolean;verbose=true:Boolean;isReset=false:Boolean;")]
        /// <summary>
        /// Method of menu option Stage01 (key:stageOne). <args> expects param: take=100:Int32;savemodels=true:Boolean;debug=true:Boolean;verbose=true:Boolean;
        /// </summary>
        /// <param name="args"><seealso cref="aceOperationArgs"/> requered parameters: take=100:Int32;savemodels=true:Boolean;debug=true:Boolean;verbose=true:Boolean;</param>
        /// <remarks>
        /// <para>Process the specified number of entries form the corpus input, for resolved terms it updates the Semantic Lexicon wtih Lemmas and Instances</para>
        /// <para>The first stage of the Semantic Lexicon construction: Unitex discovery of Lemmas and Instances</para>
        /// <para>Message if item disabled: (disabled)</para>
        /// </remarks>
        /// <seealso cref="aceOperationSetExecutorBase"/>
        public void aceOperation_lexiconStageRun(aceOperationArgs args)
        {
            //lexiconConstructTaskOne taskOne = new lexiconConstructTaskOne(args.Get<Int32>("take"), args.Get<Boolean>("savemodels"), args.Get<Boolean>("debug"), args.Get<Boolean>("verbose"));
            //taskOne.execute(response);
            semanticLexiconManager.manager.constructor.runStage(args.Get<string>("sname"), args.Get<bool>("isReset"), args.Get<int>("take"), args.Get<bool>("savemodels"), args.Get<bool>("debug"), args.Get<bool>("verbose"), response);
        }

        [aceMenuItem(aceMenuItemAttributeRole.Key, "o")]
        [aceMenuItem(aceMenuItemAttributeRole.aliasNames, "operation")]
        [aceMenuItem(aceMenuItemAttributeRole.DisplayName, "SpecialOperation")]
        [aceMenuItem(aceMenuItemAttributeRole.Category, "run")]
        [aceMenuItem(aceMenuItemAttributeRole.Description, "Calls the selected special lexicon construction operation.")]
        [aceMenuItem(aceMenuItemAttributeRole.ExpandedHelp, "If operation type not defined in the parameter list, tt will ask for type of operation to perform.")]
        [aceMenuItem(aceMenuItemAttributeRole.DisabledRemarks, "(disabled)")]
        // [aceMenuItem(aceMenuItemAttributeRole.ConfirmMessage, "Are you sure?")]  // [aceMenuItem(aceMenuItemAttributeRole.EnabledRemarks, "")]
        // [aceMenuItem(aceMenuItemAttributeRole.externalHelpFilename, "aceOperation_runSpecialOperation.md")]
        [aceMenuItem(aceMenuItemAttributeRole.CmdParamList, "operation=none:specialOperationEnum;verbose=true:Boolean;simulation=true:Boolean;")]
        /// <summary>
        /// Method of menu option SpecialOperation (key:o). <args> expects param: operation:type;paramb:type;
        /// </summary>
        /// <param name="args"><seealso cref="aceOperationArgs"/> requered parameters: operation:type;paramb:type;</param>
        /// <remarks>
        /// <para>If operation type not defined in the parameter list, tt will ask for type of operation to perform.</para>
        /// <para>Calls the selected special lexicon construction operation.</para>
        /// <para>Message if item disabled: (disabled)</para>
        /// </remarks>
        /// <seealso cref="aceOperationSetExecutorBase"/>
        public void aceOperation_runSpecialOperation(aceOperationArgs args)
        {
            specialOperationEnum op = (specialOperationEnum)args.Get<specialOperationEnum>("operation");
            bool verbose = args.Get<bool>("verbose");
            bool simulation = args.Get<bool>("simulation");

            if (op == specialOperationEnum.none)
            {
                op = (specialOperationEnum)aceTerminalInput.askForOption("Select special operation to run:", op);
            }

            builderForLog report = response;
            response.getLastLine();
            response.AppendLine("Performing operation: " + op.toString());

            switch (op)
            {
                case specialOperationEnum.deployDomainConcepts:
                    domainConceptGraph gr = languageManagerConcepts.manager.getConceptGraph();

                    report.AppendLine();
                    // report.Append(gr.ToStringTreeview());

                    report.AppendLine();
                    gr.buildConceptualMesh(report, simulation);

                    report.AppendLine();
                    gr.connectToHooks(report, simulation);

                    break;

                case specialOperationEnum.deployNameAndLastnameImb:
                    break;

                case specialOperationEnum.deployTownImb:
                    break;

                case specialOperationEnum.reportOnLemmas:
                    DataTable repLem = semanticLexiconManager.manager.getLemmaStats(report);
                    var lfi = repLem.serializeDataTable(dataTableExportEnum.excel, "lemma_report", Directory.CreateDirectory(semanticLexiconManager.manager.constructor.projectFolderStructure.path));
                    response.AppendLine();
                    repLem.GetExtraDesc().ForEach(x => report.AppendLine(x));
                    response.AppendLine("File created: " + lfi);
                    //report.AppendTable(repLem);
                    break;

                case specialOperationEnum.reportOnConcepts:
                    DataTable repCon = semanticLexiconManager.manager.getConceptStats(report);
                    var cfi = repCon.serializeDataTable(dataTableExportEnum.excel, "concept_report", Directory.CreateDirectory(semanticLexiconManager.manager.constructor.projectFolderStructure.path));
                    response.AppendLine();
                    repCon.GetExtraDesc().ForEach(x => report.AppendLine(x));
                    response.AppendLine("File created: " + cfi);
                    //report.AppendTable(repCon);
                    break;

                case specialOperationEnum.none:
                    break;
            }

            response.AppendLine("Done operation: " + op.toString());

            string repstr = report.getLastLine();
            repstr.ToString().saveStringToFile(semanticLexiconManager.manager.constructor.projectFolderStructure.pathFor(op.ToString() + ".txt"));
        }

        [aceMenuItem(aceMenuItemAttributeRole.Key, "x")]
        [aceMenuItem(aceMenuItemAttributeRole.aliasNames, "explore")]
        [aceMenuItem(aceMenuItemAttributeRole.DisplayName, "Explore")]
        [aceMenuItem(aceMenuItemAttributeRole.Category, "term")]
        [aceMenuItem(aceMenuItemAttributeRole.Description, "Explores all knowledge available on specified word or phrase using all resources available")]
        [aceMenuItem(aceMenuItemAttributeRole.ExpandedHelp, "It will print out all semantic and lexicon information available about the word")]
        [aceMenuItem(aceMenuItemAttributeRole.DisabledRemarks, "(disabled)")]
        // [aceMenuItem(aceMenuItemAttributeRole.ConfirmMessage, "Are you sure?")]  // [aceMenuItem(aceMenuItemAttributeRole.EnabledRemarks, "")]
        // [aceMenuItem(aceMenuItemAttributeRole.externalHelpFilename, "aceOperation_runExplore.md")]
        [aceMenuItem(aceMenuItemAttributeRole.CmdParamList, "word=\"kontakt\":String;mode=\"none\":String;verbose=false:Boolean;")]
        /// <summary>
        /// Method of menu option Explore (key:x). <args> expects param: word:String;
        /// </summary>
        /// <param name="args"><seealso cref="aceOperationArgs"/> requered parameters: word:String;</param>
        /// <remarks>
        /// <para>It will print out all semantic and lexicon information available about the word</para>
        /// <para>Explores all knowledge available on specified word or phrase using all resources available</para>
        /// <para>Message if item disabled: (disabled)</para>
        /// </remarks>
        /// <seealso cref="aceOperationSetExecutorBase"/>
        public void aceOperation_termExplore(aceOperationArgs args)
        {
            string inputWord = args.paramSet["word"].toStringSafe();
            string modeStr = args.Get<string>("mode");
            bool verbose = args.Get<bool>("verbose");
            termExploreMode mode = termExploreMode.none;

            if (modeStr == "none")
            {
                mode = (termExploreMode)aceTerminalInput.askForOption("Select term explore mode", termExploreMode.apertium_wordnet_eng, output);
            }
            else
            {
                mode = modeStr.imbConvertValueSafeTyped<termExploreMode>();
            }

            state.current = termExploreProcedures.explore(inputWord, response, mode, verbose);

            /*
            var item = new termExploreItem(inputWord);

            state.current = item.exploreWithHunspell(response);
            */

            // var model = termExploreProcedures.getSynonymsWithApertium(inputWord, response);

            //var model = termExploreProcedures.getSynonymsWithWordnetViaApertium(inputWord, response);

            //var outset = termExploreProcedures.exploreWithApertiumAndWordnet(inputWord, response);
            //outset.ToString(response, true);
        }

        [aceMenuItem(aceMenuItemAttributeRole.Key, "t2")]
        [aceMenuItem(aceMenuItemAttributeRole.aliasNames, "test-s2")]
        [aceMenuItem(aceMenuItemAttributeRole.DisplayName, "TestStageTwo")]
        [aceMenuItem(aceMenuItemAttributeRole.Category, "term")]
        [aceMenuItem(aceMenuItemAttributeRole.Description, "What is purpose of this?")]
        [aceMenuItem(aceMenuItemAttributeRole.ExpandedHelp, "What it will do?")]
        [aceMenuItem(aceMenuItemAttributeRole.DisabledRemarks, "(disabled)")]
        // [aceMenuItem(aceMenuItemAttributeRole.ConfirmMessage, "Are you sure?")]  // [aceMenuItem(aceMenuItemAttributeRole.EnabledRemarks, "")]
        // [aceMenuItem(aceMenuItemAttributeRole.externalHelpFilename, "aceOperation_termTestStageTwo.md")]
        [aceMenuItem(aceMenuItemAttributeRole.CmdParamList, "word=\"kontakt\":String;savemodels=true:Boolean;debug=true:Boolean;deploy=false:Boolean;verbose=true:Boolean;")]
        /// <summary>
        /// Method of menu option TestStageTwo (key:t). <args> expects param: word=\"kontakt\":String;savemodels=true:Boolean;debug=true:Boolean;deploy=false;verbose:Boolean=true;
        /// </summary>
        /// <param name="args"><seealso cref="aceOperationArgs"/> requered parameters: word=\"kontakt\":String;savemodels=true:Boolean;debug=true:Boolean;deploy=false;verbose:Boolean=true;</param>
        /// <remarks>
        /// <para>What it will do?</para>
        /// <para>What is purpose of this?</para>
        /// <para>Message if item disabled: (disabled)</para>
        /// </remarks>
        /// <seealso cref="aceOperationSetExecutorBase"/>
        public void aceOperation_termTestStageTwo(aceOperationArgs args)
        {
            string inputWord = args.paramSet["word"].toStringSafe();
            bool verbose = args.Get<bool>("verbose");
            bool savemodels = (bool)args.paramSet["savemodels"];
            bool debug = (bool)args.paramSet["debug"];
            bool deploy = (bool)args.paramSet["deploy"];

            var models = termExploreProcedures.exploreStageTwo(inputWord, response, savemodels, debug, verbose);
        }

        [aceMenuItem(aceMenuItemAttributeRole.Key, "e")]
        [aceMenuItem(aceMenuItemAttributeRole.aliasNames, "expand")]
        [aceMenuItem(aceMenuItemAttributeRole.DisplayName, "Expand")]
        [aceMenuItem(aceMenuItemAttributeRole.Category, "term")]
        [aceMenuItem(aceMenuItemAttributeRole.Description, "Expands specified term for specified steps, using Semantic Term Expansion algorithm")]
        [aceMenuItem(aceMenuItemAttributeRole.ExpandedHelp, "It will print out Semantic Term Expansion table with assigned weights")]
        [aceMenuItem(aceMenuItemAttributeRole.DisabledRemarks, "(disabled)")]
        // [aceMenuItem(aceMenuItemAttributeRole.ConfirmMessage, "Are you sure?")]  // [aceMenuItem(aceMenuItemAttributeRole.EnabledRemarks, "")]
        // [aceMenuItem(aceMenuItemAttributeRole.externalHelpFilename, "aceOperation_runExpand.md")]
        [aceMenuItem(aceMenuItemAttributeRole.CmdParamList, "word=\"kontakt\":String;steps=3:Integer;")]
        /// <summary>
        /// Method of menu option Expand (key:e). <args> expects param: word:String;steps=5:Integer;
        /// </summary>
        /// <param name="args"><seealso cref="aceOperationArgs"/> requered parameters: word:String;steps=5:Integer;</param>
        /// <remarks>
        /// <para>It will print out Semantic Term Expansion table with assigned weights</para>
        /// <para>Expands specified term for specified steps, using Semantic Term Expansion algorithm</para>
        /// <para>Message if item disabled: (disabled)</para>
        /// </remarks>
        /// <seealso cref="aceOperationSetExecutorBase"/>
        public void aceOperation_termExpand(aceOperationArgs args)
        {
            string word = args.Get<string>("word");
            int steps = args.Get<int>("steps");

            termGraph tg = new termGraph(word);

            tg.expand(steps);

            if (tg.termNotFoundInLexicon)
            {
                response.AppendLine("Not found in the Lexicon");
                return;
            }

            //String tree = tg.ToStringTreeview();
            //response.Append(tree);
            //String pt = semanticLexiconManager.manager.constructor.projectFolderStructure[lexiconConstructorProjectFolder.logs].pathFor("expand_" + steps.ToString("D2") + word);
            //tree.saveStringToFile(pt+".txt");

            termSpark spark = tg.getSpark();

            DataTable dt = spark.GetDataTable();
            response.AppendTable(dt);

            dt.serializeDataTable(dataTableExportEnum.csv, "expand_" + steps.ToString("D2") + word, semanticLexiconManager.manager.constructor.projectFolderStructure[lexiconConstructorProjectFolder.logs]);
        }

        [aceMenuItem(aceMenuItemAttributeRole.Key, "queryTest")]
        [aceMenuItem(aceMenuItemAttributeRole.aliasNames, "query")]
        [aceMenuItem(aceMenuItemAttributeRole.DisplayName, "Query test")]
        [aceMenuItem(aceMenuItemAttributeRole.Category, "lexicon")]
        [aceMenuItem(aceMenuItemAttributeRole.Description, "Testing query expansion and query versus document vector similarity calculation")]
        [aceMenuItem(aceMenuItemAttributeRole.ExpandedHelp, "It will expand query terms, load all page_##.txt documents as pages of single set and perform query evaluation")]
        [aceMenuItem(aceMenuItemAttributeRole.DisabledRemarks, "(disabled)")]
        // [aceMenuItem(aceMenuItemAttributeRole.ConfirmMessage, "Are you sure?")]  // [aceMenuItem(aceMenuItemAttributeRole.EnabledRemarks, "")]
        // [aceMenuItem(aceMenuItemAttributeRole.externalHelpFilename, "aceOperation_lexiconQuerytest.md")]
        [aceMenuItem(aceMenuItemAttributeRole.CmdParamList, "qexpand=2:Int32;dexpand=1:Int32;")]
        /// <summary>
        /// Method of menu option Querytest (key:queryTest). <args> expects param:
        /// </summary>
        /// <param name="args"><seealso cref="aceOperationArgs"/> requered parameters: </param>
        /// <remarks>
        /// <para>It will expand query terms, load all page_##.txt documents as pages of single set and perform query evaluation</para>
        /// <para>Testing query expansion and query versus document vector similarity calculation</para>
        /// <para>Message if item disabled: (disabled)</para>
        /// </remarks>
        /// <seealso cref="aceOperationSetExecutorBase"/>
        public void aceOperation_lexiconQuerytest(aceOperationArgs args)
        {
            var con = semanticLexiconManager.manager.constructor;
            List<string> queryList = con.projectFolderStructure[lexiconConstructorProjectFolder.documents].pathFor("query_list.txt").openFileToList(true);

            DirectoryInfo docDir = con.projectFolderStructure[lexiconConstructorProjectFolder.documents];

            int qe = args.Get<int>("qexpand");
            int de = args.Get<int>("dexpand");

            termDocumentSet pages = new termDocumentSet();

            foreach (FileInfo fi in docDir.GetFiles("page_*.txt"))
            {
                string pcontent = openBase.openFileToString(fi.FullName, false);

                //var mchs = wordSplit.Matches(pcontent);
                List<string> tokens = pcontent.getTokens();

                response.AppendLine("Loading page: " + fi.FullName);

                termDocument tdc = (termDocument)pages.AddTable(fi.Name);
                tdc.AddTokens(tokens, response);

                tdc.GetDataTable("", pages.dataSet).serializeDataTable(dataTableExportEnum.csv, "term_doc", con.projectFolderStructure[lexiconConstructorProjectFolder.logs]);
            }

            pages.GetAggregateDataTable().serializeDataTable(dataTableExportEnum.csv, "aggregate_doc", con.projectFolderStructure[lexiconConstructorProjectFolder.logs]);
            response.AppendLine("Aggregate table for documents created. ");
            // DataSet ds = pages.GetDataSet(true);
            // <--- serialize data set

            termQueryDocumentSet queries = new termQueryDocumentSet("queries");

            int i = 0;
            foreach (string q in queryList)
            {
                i++;

                termQueryDocument qt = queries.AddTable(i.ToString("D3")) as termQueryDocument;
                qt.SetQuery(q, qe, response);

                response.AppendLine("Query terms[" + qt.Count() + "] => " + q);

                // qt.SetWeightTo_FrequencyRatio();

                //var tkns = q.SplitSmart(",", "", true);
                // termQueryDocument qdoc = new termQueryDocument(queries, "query_" + i.ToString("D2"), tokens);

                //qdoc.GetDataTable("", pages.dataSet).serializeDataTable(aceCommonTypes.enums.dataTableExportEnum.csv, qdoc.name, con.projectFolderStructure[lexiconConstructorProjectFolder.logs]);
            }

            foreach (termQueryDocument qt in queries)
            {
                var qtd = queries.GetDataTable(qt.name);

                qtd.serializeDataTable(dataTableExportEnum.csv, "query_" + qt.name, con.projectFolderStructure[lexiconConstructorProjectFolder.logs]);
                response.consoleAltColorToggle();
                // response.AppendTable(qtd);
            }

            response.consoleAltColorToggle(true, 1);
            var qtda = queries.GetAggregateDataTable();
            qtda.serializeDataTable(dataTableExportEnum.csv, "query_all", con.projectFolderStructure[lexiconConstructorProjectFolder.logs]);
            // response.AppendTable(qtda);
            // <-------------------
        }

        [aceMenuItem(aceMenuItemAttributeRole.Key, "uni")]
        [aceMenuItem(aceMenuItemAttributeRole.aliasNames, "uni")]
        [aceMenuItem(aceMenuItemAttributeRole.DisplayName, "Unitex")]
        [aceMenuItem(aceMenuItemAttributeRole.Category, "term")]
        [aceMenuItem(aceMenuItemAttributeRole.Description, "Evaluates term using Unitex DELAF dictionary. Set _corpus_ parameter above 0 to run n terms from corpus input, set _savemodels_ to TRUE to save XML model of the term.")]
        [aceMenuItem(aceMenuItemAttributeRole.ExpandedHelp, "It will construct knowledge about the term using Unitex DELAF.")]
        [aceMenuItem(aceMenuItemAttributeRole.DisabledRemarks, "(disabled)")]
        // [aceMenuItem(aceMenuItemAttributeRole.ConfirmMessage, "Are you sure?")]  // [aceMenuItem(aceMenuItemAttributeRole.EnabledRemarks, "")]
        // [aceMenuItem(aceMenuItemAttributeRole.externalHelpFilename, "aceOperation_termUnitex.md")]
        [aceMenuItem(aceMenuItemAttributeRole.CmdParamList, "word=\"kontakt\":String;corpus=-1:Int32;savemodels=false:Boolean;debug=false:Boolean;deploy=false:Boolean;")]
        /// <summary>
        /// Method of menu option Unitex (key:). <args> expects param: word=\"kontakt\":String;
        /// </summary>
        /// <param name="args"><seealso cref="aceOperationArgs"/> requered parameters: word=\"kontakt\":String;</param>
        /// <remarks>
        /// <para>It will construct knowledge about the term using Unitex DELAF.</para>
        /// <para>Evaluates term using Unitex DELAF dictionary</para>
        /// <para>Message if item disabled: (disabled)</para>
        /// </remarks>
        /// <seealso cref="aceOperationSetExecutorBase"/>
        public void aceOperation_termUnitex(aceOperationArgs args)
        {
            string term = args.paramSet["word"].toStringSafe();
            int corpTake = (int)args.paramSet["corpus"];
            bool savemodels = (bool)args.paramSet["savemodels"];
            bool debug = (bool)args.paramSet["debug"];
            bool deploy = (bool)args.paramSet["deploy"];

            List<string> terms = new List<string>();
            //if (corpTake > 0)
            //{
            //    var lns = semanticLexiconManager.manager.constructor.corpusOperater.Take(corpTake, new List<string>());
            //    terms = lns.getLineContentList();
            //    terms.ForEach(x => x = x.Trim('"'));
            //} else
            //{
            //
            //}
            terms.Add(term);
            foreach (string word in terms)
            {
                var output = termExploreProcedures.exploreWithUnitex(word, response);
                state.current = output;

                if (savemodels)
                {
                    if (!output.wasExploreFailed)
                    {
                        semanticLexiconManager.manager.constructor.saveTermModel(output);
                    }
                }

                if (output.wasExploreFailed)
                {
                    if (debug)
                    {
                        response.consoleAltColorToggle();
                        response.AppendLine("--- running debug search for [" + word + "]");
                        var exp = languageManagerUnitex.manager.operatorDelaf.Search(word, false, 25);
                        exp.ToString(response, true);
                        response.consoleAltColorToggle();
                    }
                }
                Thread.Sleep(100);
            }
            response.save();
            state.Save();
        }

        [aceMenuItem(aceMenuItemAttributeRole.Key, "save")]
        [aceMenuItem(aceMenuItemAttributeRole.aliasNames, "save")]
        [aceMenuItem(aceMenuItemAttributeRole.DisplayName, "Save")]
        [aceMenuItem(aceMenuItemAttributeRole.Category, "corpus")]
        [aceMenuItem(aceMenuItemAttributeRole.Description, "Saving all work files: log, console state and etc.")]
        [aceMenuItem(aceMenuItemAttributeRole.ExpandedHelp, "Save current state")]
        [aceMenuItem(aceMenuItemAttributeRole.DisabledRemarks, "(disabled)")]
        // [aceMenuItem(aceMenuItemAttributeRole.ConfirmMessage, "Are you sure?")]  // [aceMenuItem(aceMenuItemAttributeRole.EnabledRemarks, "")]
        // [aceMenuItem(aceMenuItemAttributeRole.externalHelpFilename, "aceOperation_corpusSave.md")]
        //[aceMenuItem(aceMenuItemAttributeRole.CmdParamList, "param:type;paramb:type;")]
        /// <summary>
        /// Method of menu option Save (key:save). <args> expects param: param:type;paramb:type;
        /// </summary>
        /// <param name="args"><seealso cref="aceOperationArgs"/> requered parameters: param:type;paramb:type;</param>
        /// <remarks>
        /// <para>Save current state</para>
        /// <para>Saving all work files: log, console state and etc.</para>
        /// <para>Message if item disabled: (disabled)</para>
        /// </remarks>
        /// <seealso cref="aceOperationSetExecutorBase"/>
        public void aceOperation_corpusSave(aceOperationArgs args)
        {
            semanticLexiconManager.manager.constructor.saveAll(response);

            response.save();
            state.Save();
        }

        [aceMenuItem(aceMenuItemAttributeRole.Key, "show")]
        [aceMenuItem(aceMenuItemAttributeRole.aliasNames, "show")]
        [aceMenuItem(aceMenuItemAttributeRole.DisplayName, "Show")]
        [aceMenuItem(aceMenuItemAttributeRole.Category, "term")]
        [aceMenuItem(aceMenuItemAttributeRole.Description, "Describes the current termExploreModel instance loaded in the console")]
        [aceMenuItem(aceMenuItemAttributeRole.ExpandedHelp, "It will show lemma information and all instances with complete gram flags details")]
        [aceMenuItem(aceMenuItemAttributeRole.DisabledRemarks, "(disabled)")]
        // [aceMenuItem(aceMenuItemAttributeRole.ConfirmMessage, "Are you sure?")]  // [aceMenuItem(aceMenuItemAttributeRole.EnabledRemarks, "")]
        // [aceMenuItem(aceMenuItemAttributeRole.externalHelpFilename, "aceOperation_termShow.md")]
        //[aceMenuItem(aceMenuItemAttributeRole.CmdParamList, "")]
        /// <summary>
        /// Method of menu option Show (key:show). <args> expects param:
        /// </summary>
        /// <param name="args"><seealso cref="aceOperationArgs"/> requered parameters: </param>
        /// <remarks>
        /// <para>It will show lemma information and all instances with complete gram flags details</para>
        /// <para>Describes the current termExploreModel instance loaded in the console</para>
        /// <para>Message if item disabled: (disabled)</para>
        /// </remarks>
        /// <seealso cref="aceOperationSetExecutorBase"/>
        public void aceOperation_termShow(aceOperationArgs args)
        {
            if (state.current != null)
            {
                state.current.ToString(response, true);
            }
        }

        [aceMenuItem(aceMenuItemAttributeRole.Key, "r")]
        [aceMenuItem(aceMenuItemAttributeRole.aliasNames, "reset, purge")]
        [aceMenuItem(aceMenuItemAttributeRole.DisplayName, "Reset")]
        [aceMenuItem(aceMenuItemAttributeRole.Category, "corpus")]
        [aceMenuItem(aceMenuItemAttributeRole.Description, "Deletes all files and entries in the Lexicon and restores original corpus input file")]
        [aceMenuItem(aceMenuItemAttributeRole.ExpandedHelp, "Before removing all entries from the Lexicon it will ask for confirmation")]
        [aceMenuItem(aceMenuItemAttributeRole.DisabledRemarks, "(disabled)")]
        [aceMenuItem(aceMenuItemAttributeRole.ConfirmMessage, "Are you sure to delete all model files, splits and other files in the project?")]  // [aceMenuItem(aceMenuItemAttributeRole.EnabledRemarks, "")]
                                                                                                                                                  // [aceMenuItem(aceMenuItemAttributeRole.externalHelpFilename, "aceOperation_corpusReset.md")]

        /// <summary>
        /// Method of menu option Reset (key:reset). <args> expects param: param:type;paramb:type;
        /// </summary>
        /// <param name="args"><seealso cref="aceOperationArgs"/> requered parameters: param:type;paramb:type;</param>
        /// <remarks>
        /// <para>Before removing all entries from the Lexicon it will ask for confirmation</para>
        /// <para>Deletes all files and entries in the Lexicon and restores original corpus input file</para>
        /// <para>Message if item disabled: (disabled)</para>
        /// </remarks>
        /// <seealso cref="aceOperationSetExecutorBase"/>
        public void aceOperation_corpusReset(aceOperationArgs args)
        {
            semanticLexiconManager.manager.constructor.resetLexiconAndCorpus();
        }

        [aceMenuItem(aceMenuItemAttributeRole.Key, "raw")]
        [aceMenuItem(aceMenuItemAttributeRole.aliasNames, "raw")]
        [aceMenuItem(aceMenuItemAttributeRole.DisplayName, "Raw")]
        [aceMenuItem(aceMenuItemAttributeRole.Category, "term")]
        [aceMenuItem(aceMenuItemAttributeRole.Description, "Source files raw search for specified word. Use wilcard * for the last word, or *.* for all words in current memory")]
        [aceMenuItem(aceMenuItemAttributeRole.ExpandedHelp, "Search for raw data entries about the specified word")]
        [aceMenuItem(aceMenuItemAttributeRole.DisabledRemarks, "(disabled)")]
        // [aceMenuItem(aceMenuItemAttributeRole.ConfirmMessage, "Are you sure?")]  // [aceMenuItem(aceMenuItemAttributeRole.EnabledRemarks, "")]
        // [aceMenuItem(aceMenuItemAttributeRole.externalHelpFilename, "aceOperation_runRaw.md")]
        [aceMenuItem(aceMenuItemAttributeRole.CmdParamList, "word:String;")]
        /// <summary>
        /// Method of menu option Raw (key:raw). <args> expects param: param:type;paramb:type;
        /// </summary>
        /// <param name="args"><seealso cref="aceOperationArgs"/> requered parameters: param:type;paramb:type;</param>
        /// <remarks>
        /// <para>Search for raw data entries about the specified word</para>
        /// <para>Source files raw search for specified word. Use wilcard * for the last word, or *.* for all words in current memory</para>
        /// <para>Message if item disabled: (disabled)</para>
        /// </remarks>
        /// <seealso cref="aceOperationSetExecutorBase"/>
        public void aceOperation_termRaw(aceOperationArgs args)
        {
            string inputWord = args.paramSet["word"].toStringSafe();
            if (inputWord == "*") inputWord = state.current.inputForm;

            var result = semanticLexiconManager.manager.sourceSearch(new string[] { inputWord }, lexiconSourceTypeEnum.apertium | lexiconSourceTypeEnum.serbianWordNet | lexiconSourceTypeEnum.unitexDelaf | lexiconSourceTypeEnum.corpus);

            foreach (KeyValuePair<string, fileTextSearchResultSet> resfile in result)
            {
                response.AppendLine("Source file [" + resfile.Key + "] ");

                foreach (fileTextSearchResult resset in resfile.Value)
                {
                    response.AppendLine("Needle [" + resset.needle + "] Regex[" + resset.useRegex.ToString() + "]");
                    response.AppendLine(resset.ToString(true));
                }
                response.AppendLine();
            }
        }

        [aceMenuItem(aceMenuItemAttributeRole.Key, "z")]
        [aceMenuItem(aceMenuItemAttributeRole.aliasNames, "chk")]
        [aceMenuItem(aceMenuItemAttributeRole.DisplayName, "Check")]
        [aceMenuItem(aceMenuItemAttributeRole.Category, "lexicon")]
        [aceMenuItem(aceMenuItemAttributeRole.Description, "Testing single term retrieval")]
        [aceMenuItem(aceMenuItemAttributeRole.ExpandedHelp, "It will try to resolve the specified word")]
        [aceMenuItem(aceMenuItemAttributeRole.DisabledRemarks, "(disabled)")]
        // [aceMenuItem(aceMenuItemAttributeRole.ConfirmMessage, "Are you sure?")]  // [aceMenuItem(aceMenuItemAttributeRole.EnabledRemarks, "")]
        // [aceMenuItem(aceMenuItemAttributeRole.externalHelpFilename, "aceOperation_lexiconCheck.md")]
        [aceMenuItem(aceMenuItemAttributeRole.CmdParamList, "word=\"kontakti\":String;debug=true:Boolean;")]
        /// <summary>
        /// Method of menu option Check (key:check). <args> expects param: word:String;save:Boolean;
        /// </summary>
        /// <param name="args"><seealso cref="aceOperationArgs"/> requered parameters: word:String;save:Boolean;</param>
        /// <remarks>
        /// <para>It will try to resolve the specified word</para>
        /// <para>Testing single term retrieval</para>
        /// <para>Message if item disabled: (disabled)</para>
        /// </remarks>
        /// <seealso cref="aceOperationSetExecutorBase"/>
        public void aceOperation_lexiconCheck(aceOperationArgs args)
        {
            string inputWord = args.paramSet["word"].toStringSafe();
            bool debug = (bool)args.paramSet["debug"];
            List<string> words = new List<string>();
            words.Add(inputWord);
            if (inputWord.EndsWith(".csv"))
            {
                words = openBase.openFile(semanticLexiconManager.manager.constructor.projectFolderStructure[lexiconConstructorProjectFolder.documents].pathFor(inputWord));
            }
            else
            {
            }
            foreach (string wr in words)
            {
                int index = words.IndexOf(wr);

                List<ILexiconItem> lis = semanticLexiconManager.manager.getLexiconItems(wr, response);

                // response.AppendLine("[" + wr + "]");

                foreach (ILexiconItem li in lis)
                {
                    if (li != null)
                    {
                        string dbg = "[" + wr + "]" + "[" + li.name + "]";
                        if (li is ITermLemma)
                        {
                            ITermLemma li_ITermLemma = (ITermLemma)li;
                            dbg = dbg.add("TermLemma", " => ");
                            dbg = dbg.add(li_ITermLemma.type, ":");
                            dbg = dbg.add(li_ITermLemma.gramSet, ":");

                            dbg = dbg.add(li_ITermLemma.Id, " = ");
                        }

                        if (li is ITermInstance)
                        {
                            ITermInstance li_ITermInstance = (ITermInstance)li;

                            dbg = dbg.add("TermInstance", " => ");
                            dbg = dbg.add(li_ITermInstance.type, ":");
                            dbg = dbg.add(li_ITermInstance.gramSet, ":");
                            dbg = dbg.add(li_ITermInstance.Id, " = ");
                        }

                        response.AppendLine(dbg);
                    }
                    else
                    {
                        response.AppendLine("[" + wr + "] => null");
                    }
                }

                /*
                termSpark spark = termTools.getExpandedSpark(inputWord, 1, response, debug);

                if (debug)
                {
                    DataTable dt = spark.GetDataTable();

                    dt.serializeDataTable(aceCommonTypes.enums.dataTableExportEnum.csv, "check_" + inputWord, semanticLexiconManager.manager.constructor.projectFolderStructure[lexiconConstructorProjectFolder.logs]);
                }*/
                response.log("Words processed: " + index.imbGetPercentage(words.Count()));
            }
        }

        [aceMenuItem(aceMenuItemAttributeRole.Key, "preloadLexicon")]
        [aceMenuItem(aceMenuItemAttributeRole.aliasNames, "preloadLexicon")]
        [aceMenuItem(aceMenuItemAttributeRole.DisplayName, "PreloadLexicon")]
        [aceMenuItem(aceMenuItemAttributeRole.Category, "run")]
        [aceMenuItem(aceMenuItemAttributeRole.Description, "What is purpose of this?")]
        [aceMenuItem(aceMenuItemAttributeRole.ExpandedHelp, "What it will do?")]
        [aceMenuItem(aceMenuItemAttributeRole.DisabledRemarks, "(disabled)")]
        // [aceMenuItem(aceMenuItemAttributeRole.ConfirmMessage, "Are you sure?")]  // [aceMenuItem(aceMenuItemAttributeRole.EnabledRemarks, "")]
        // [aceMenuItem(aceMenuItemAttributeRole.externalHelpFilename, "aceOperation_runPreloadLexicon.md")]
        //[aceMenuItem(aceMenuItemAttributeRole.CmdParamList, "word=\"word\":String;steps=5:Int32;debug=true:Boolean;")]

        /// <summary>
        /// Method of menu option PreloadLexicon (key:preloadLexicon). <args> expects param: word:String;steps:Int32;debug:Boolean;
        /// </summary>
        /// <param name="args"><seealso cref="aceOperationArgs"/> requered parameters:  word:String;steps:Int32;debug:Boolean;</param>
        /// <remarks>
        /// <para>What it will do?</para>
        /// <para>What is purpose of this?</para>
        /// <para>Message if item disabled: (disabled)</para>
        /// </remarks>
        /// <seealso cref="aceOperationSetExecutorBase"/>
        public void aceOperation_runPreloadLexicon(aceOperationArgs args)
        {
            //manager.prepareCache(output, workspace.folder);
            semanticLexiconManager.lexiconCache.preloadLexicon(output, manager.lexiconContext);
        }

        [aceMenuItem(aceMenuItemAttributeRole.Key, "reviewNegatives")]
        [aceMenuItem(aceMenuItemAttributeRole.aliasNames, "reviewNegatives")]
        [aceMenuItem(aceMenuItemAttributeRole.DisplayName, "ReviewNegatives")]
        [aceMenuItem(aceMenuItemAttributeRole.Category, "run")]
        [aceMenuItem(aceMenuItemAttributeRole.Description, "Starts negatives review process")]
        [aceMenuItem(aceMenuItemAttributeRole.ExpandedHelp, "Loads active lexiconCache negatives and starts manual review process")]
        [aceMenuItem(aceMenuItemAttributeRole.DisabledRemarks, "(disabled)")]
        // [aceMenuItem(aceMenuItemAttributeRole.ConfirmMessage, "Are you sure?")]  // [aceMenuItem(aceMenuItemAttributeRole.EnabledRemarks, "")]
        // [aceMenuItem(aceMenuItemAttributeRole.externalHelpFilename, "aceOperation_runReviewNegatives.md")]
        //[aceMenuItem(aceMenuItemAttributeRole.CmdParamList, "word=\"word\":String;steps=5:Int32;debug=true:Boolean;")]
        /// <summary>
        /// Method of menu option ReviewNegatives (key:reviewNegatives). <args> expects param: word:String;steps:Int32;debug:Boolean;
        /// </summary>
        /// <param name="args"><seealso cref="aceOperationArgs"/> requered parameters:  word:String;steps:Int32;debug:Boolean;</param>
        /// <remarks>
        /// <para>Loads active lexiconCache negatives and starts manual review process</para>
        /// <para>Starts negatives review process</para>
        /// <para>Message if item disabled: (disabled)</para>
        /// </remarks>
        /// <seealso cref="aceOperationSetExecutorBase"/>
        public void aceOperation_runReviewNegatives(aceOperationArgs args)
        {
            morphMachineSerbian machine = new morphMachineSerbian(workspace.folder);

            log("MorphMachine session start [" + machine.inputFile.getLineCount() + " entries]");

            bool work = true;

            string token = machine.GetNextToken();

            while (work)
            {
                morphRuleMatchSet mSet = machine.Explore(token, output);
                string question = "Exploring token [" + token + "] ";

                if (!mSet.matches.Any())
                {
                    question += "no morphology rulesets were triggered.";
                }
                else
                {
                    question += mSet.matches.Count() + " rulesets triggered.";
                    foreach (var ms in mSet.matches)
                    {
                        var lemmas = semanticLexiconManager.lexiconCache.getLemmasWithRoot(ms.item.rootWord, output);
                        foreach (var lemma in lemmas)
                        {
                            ms.item.synonyms.Add(lemma.name);
                        }

                        ms.item.ToString(response, false, false);
                    }
                }

                morphActionEnum action = aceTerminalInput.askForEnum<morphActionEnum>(question, morphActionEnum.skip);

                switch (action)
                {
                    case morphActionEnum.stop:
                        work = false;
                        break;

                    case morphActionEnum.skip:
                        token = machine.GetNextToken();
                        break;

                    case morphActionEnum.ignore:
                        machine.SetIgnore(token);
                        token = machine.GetNextToken();
                        break;

                    case morphActionEnum.details:
                        if (!mSet.matches.Any())
                        {
                            output.AppendLine("No details to show.");
                        }
                        else
                        {
                            foreach (var ms in mSet.matches)
                            {
                                ms.item.ToString(response, false, true);
                                aceTerminalInput.askPressAnyKeyInTime("Press key to continue", true, 5, true, 1);
                            }
                        }
                        break;

                    case morphActionEnum.manual:

                        break;

                    case morphActionEnum.accept:
                        foreach (var ms in mSet.matches)
                        {
                            manager.constructor.saveTermModel(ms.item, "morph_");
                            manager.constructor.addTermModelToLexicon(ms.item);
                            manager.constructor.addSynonymsAndConceptLinks(ms.item);

                            var sh = ms.item.GetShadow();
                            machine.SetResolved(sh);
                            aceTerminalInput.askPressAnyKeyInTime("Item model [" + ms.item.lemmaForm + "] saved to lexicon", true, 3, true, 1);

                            token = machine.GetNextToken();
                        }
                        break;
                }
            }

            machine.Save();

            response.log("MorphMachine session ended");
        }

        public lexiconConsole(string sessionName, lexiconConsoleSettings __settings) : base() //:base("Lexicon Workshop Console", null)
        {
            settings = __settings;

            //state = new lexiconConsoleState(sessionName);
        }

        private semanticLexiconManager _manager;

        /// <summary>
        ///
        /// </summary>
        public semanticLexiconManager manager
        {
            get
            {
                if (_manager == null) _manager = semanticLexiconManager.manager;
                return _manager;
            }
            set { _manager = value; }
        }

        public override void onStartUp()
        {
            CultureInfo ci = new CultureInfo(6170);
            Thread.CurrentThread.CurrentCulture = ci;
            Thread.CurrentThread.CurrentUICulture = ci;
            //output.log("Culture: " + ci.Name);

            Console.OutputEncoding = Encoding.Unicode;
            Console.InputEncoding = Encoding.Unicode;

            manager.prepare();
            log("Preparing Semantic Lexicon cache", true);
            manager.prepareCache(output, workspace.folder);
            manager.constructor.startConstruction(workspace.folder[lexFolder.constructor].path.removeEndsWith("\\"));

            //semanticLexiconManager.manager.constructor.startConstruction();

            //  response.immediateSaveOn = false;
            // response.outputPath = semanticLexiconManager.manager.constructor.projectFolderStructure[lexiconConstructorProjectFolder.logs].path.add("console_response.txt", "\\");

            if (semanticLexiconManager.manager.consoleSettings.doRunInitiationAutomatically)
            {
                aceOperation_corpusInitialize(null);
            }

            var args = Environment.GetCommandLineArgs();
            if (args.Any())
            {
                string line = String.Join(" ", args);
                executeCommand(line);
            }
            else
            {
                var script = workspace.loadScript("autoexec.ace");
                executeScript(script);
            }

            //if (settings.doRunAutoexecScript)
            //{
            //    var script = semanticLexiconManager.manager.constructor.loadScript(settings.autoexecScriptFilename);
            //    executeScript(script, 200);
            //    response.save();
            //}
        }

        protected override void doCustomSpecialCall(aceCommandActiveInput input)
        {
            //throw new NotImplementedException();
        }

        private lexiconConsoleState _state;

        /// <summary> </summary>
        public lexiconConsoleState state
        {
            get
            {
                return _state;
            }
            protected set
            {
                _state = value;
                OnPropertyChanged("state");
            }
        }

        private lexiconConsoleSettings _settings;

        /// <summary> </summary>
        public lexiconConsoleSettings settings
        {
            get
            {
                return _settings;
            }
            protected set
            {
                _settings = value;
                OnPropertyChanged("settings");
            }
        }

        public override aceCommandConsoleIOEncode encode
        {
            get
            {
                return aceCommandConsoleIOEncode.dosx;
            }
        }
    }
}