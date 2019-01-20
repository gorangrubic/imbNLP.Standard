using imbSCI.Core.extensions.data;
using imbSCI.Core.files.folders;
using imbSCI.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace imbNLP.Toolkit.ExperimentModel
{
    /// <summary>
    /// Provides access to external resources
    /// </summary>
    public class ExperimentResourceProvider
    {

        public ExperimentResourceProvider()
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExperimentResourceProvider"/> class.
        /// </summary>
        /// <param name="reportFolder">Root report folder</param>
        /// <param name="runName">Source Experiment runName </param>
        /// <param name="groupName">Name of the experiment group.</param>
        public ExperimentResourceProvider(folderNode reportFolder, String runName, String groupName = "")
        {

            folderNode directory = reportFolder;
            if (!groupName.isNullOrEmpty())
            {
                directory = directory.Attach(groupName); //.Add(groupName, groupName, "");
            }
            directory = directory.Attach(runName);
            folder = directory;

        }

        public folderNode folder { get; set; }

        public String SetResourceFilePath(String filename, ExperimentDataSetFold fold)
        {
            if (folder == null) return "";

            var fold_folder = folder.Attach(fold.name);

            filename = filename.ensureEndsWith(".xml");

            String p = fold_folder.pathFor(filename, imbSCI.Data.enums.getWritableFileMode.overwrite);
            return p;

        }


        public List<String> GetResourceFiles(String filenames, ExperimentDataSetFold fold)
        {
            var fs = filenames.Split(new char[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries).ToList();

            return GetResourceFiles(fs, fold);


        }


        /// <summary>
        /// Gets the resource files.
        /// </summary>
        /// <param name="filenames">The filenames.</param>
        /// <param name="fold">The fold.</param>
        /// <returns></returns>
        public List<String> GetResourceFiles(List<String> filenames, ExperimentDataSetFold fold)
        {
            List<String> output = new List<string>();

            if (folder == null) return output;

            var fold_folder = folder.Attach(fold.name);
            foreach (String fn in filenames)
            {
                var filename = fn.ensureEndsWith(".xml");

                String p = fold_folder.findFile(filename, SearchOption.TopDirectoryOnly);
                if (!p.isNullOrEmpty())
                {
                    output.Add(p);
                }

            }
            return output;
        }


        public String GetResourceFile(String filename, ExperimentDataSetFold fold)
        {
            if (folder == null) return "";

            var fold_folder = folder.Attach(fold.name);
            filename = filename.ensureEndsWith(".xml");

            String p = fold_folder.findFile(filename, SearchOption.TopDirectoryOnly);

            return p;
        }

    }
}