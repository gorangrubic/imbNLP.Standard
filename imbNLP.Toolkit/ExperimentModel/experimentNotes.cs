using imbSCI.Core.data;
using imbSCI.Core.extensions.io;
using imbSCI.Core.extensions.table;
using imbSCI.Core.files.folders;
using imbSCI.Core.reporting.render.builders;
using imbSCI.Data;
using imbSCI.Data.collection.nested;
using imbSCI.DataComplex.tables;
using System;
using System.Data;

namespace imbNLP.Toolkit.ExperimentModel
{

    /// <summary>
    ///
    /// </summary>
    /// <seealso cref="imbACE.Core.core.builderForLog" />
    public class ToolkitExperimentNotes : builderForLogBase
    {
        public override bool VAR_AllowInstanceToOutputToConsole { get { return true; } }
        public override bool VAR_AllowAutoOutputToConsole { get { return true; } }

        public aceAuthorNotation signature { get; set; } = new aceAuthorNotation();



        public folderNode folder_corpus { get; set; }
        public folderNode folder_entity { get; set; }
        public folderNode folder_vector { get; set; }
        public folderNode folder_feature { get; set; }
        public folderNode folder_classification { get; set; }

        public Boolean RenderTextTables { get; set; } = false;


        public void SaveDataTable(DataTable table, folderNode subfolder = null)
        {

            folderNode targetFolder = subfolder;
            if (targetFolder == null) targetFolder = folder;

            if (RenderTextTables)
            {
                String p = targetFolder.pathFor("tb_" + table.TableName + ".txt", imbSCI.Data.enums.getWritableFileMode.overwrite, "DataTable : " + table.GetDescription(), true);
                String c = table.GetTextTable();
                c.saveStringToFile(p);
            }

            table.GetReportAndSave(targetFolder, signature);

        }


        public static Boolean isTheFirst { get; set; } = true;

        public folderNode folder { get; set; }

        public String description { get; set; }

        protected String PhaseTag { get { return "---"; } }

        public new void logStartPhase(String title, String message)
        {
            open(PhaseTag, title, message);
            AppendHeading(title, tabLevel);
            AppendComment(message);
        }

        public new void logEndPhase()
        {
            close(PhaseTag);
        }

        /// <summary>
        /// Creates instance of subnotes 
        /// </summary>
        /// <param name="subfolder">The subfolder.</param>
        /// <param name="description">The description.</param>
        /// <returns></returns>
        public ToolkitExperimentNotes StartSubnotes(String subfolder, String description)
        {
            ToolkitExperimentNotes output = new ToolkitExperimentNotes(folder.Add(subfolder, subfolder, description), description);
            return output;
        }


        public ToolkitExperimentNotes(folderNode __folder, String _description)
        {
            folder = __folder;
            description = _description;

            // Deploy();

            //autoFlushLength = 800000;
            //autoFlushDisabled = true;
            if (isTheFirst)
            {
                isTheFirst = false;
                AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            }
        }

        public void Deploy()
        {
            folder_corpus = folder.Add("Corpus", "Corpus data", "Reports on corpus and feature selection");
            folder_entity = folder.Add("Entity", "Entity data", "Reports on dataset");
            folder_vector = folder.Add("Vector", "Vector data", "Reports on Vector Space Model projections");
            folder_feature = folder.Add("Feature", "Feature data", "Reports on Feature Vector Space");
            folder_classification = folder.Add("Class", "Classification data", "Reports on classification test");
        }


        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex = (Exception)e.ExceptionObject;
            ex.LogException("Unhandled exception", "EX");

            log("Runtime terminating:" + e.IsTerminating);
        }

        public aceDictionarySet<String, String> notes { get; set; } = new aceDictionarySet<string, string>();

        //public List<String> GetNotes(kFoldValidationCase validationCase)
        //{
        //    return notes[validationCase.name].ToList();
        //}

        //public void AddNote(String note, kFoldValidationCase validationCase)
        //{
        //    notes[validationCase.name].Add(note);
        //    log(validationCase.name + "> " + note);
        //}

        public const String EXTAGS_ACESCIENCEEXCEPTION = "#ACE_SCIENCE_EXCEPTION#";
        public const String EXTAGS_ACEGENERALEXCEPTION = "#ACE_GENERAL_EXCEPTION#";

        /// <summary>
        /// Logs the exception.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="ex">The ex.</param>
        /// <param name="prefix">The prefix.</param>
        /// <param name="skipSave">if set to <c>true</c> [skip save].</param>
        public void LogException(String context, Exception ex, String prefix = "", Boolean skipSave = false)
        {
            //if (ex is aceScienceException)
            //{
            //    AppendLine(prefix + EXTAGS_ACESCIENCEEXCEPTION);
            //}
            //else if (ex is aceGeneralException)
            //{
            //    AppendLine(prefix + EXTAGS_ACEGENERALEXCEPTION);
            //}

            if (!skipSave) AppendHorizontalLine();
            log(prefix + context);
            AppendLine(prefix + " > " + ex.Message + "");
            AppendLine(prefix + " > " + ex.StackTrace + "");

            if (ex.InnerException != null)
            {
                LogException("Inner exception", ex.InnerException, prefix + " > ", true);
            }


            if (!skipSave)
            {
                AppendHorizontalLine();
                SaveNote();
            }
        }

        /// <summary>
        /// Saves the note into assigned folder. Default name: note.txt
        /// </summary>
        /// <param name="name">The name.</param>
        public void SaveNote(string name = "")
        {
            if (name.isNullOrEmpty())
            {
                name = "note";
            }
            name = name.ensureEndsWith(".txt");

            string path = folder.pathFor(name, imbSCI.Data.enums.getWritableFileMode.overwrite, description).getWritableFile().FullName;
            // aceCommonTypes.reporting.reportOutputQuickTools.saveMarkdownToPDF

            ContentToString().saveStringToFile(path);
        }

        public object StartSubnotes(object p)
        {
            throw new NotImplementedException();
        }
    }

}