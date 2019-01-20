using imbNLP.Toolkit.Feature.Settings;
using imbSCI.Core.files.folders;
using imbSCI.Core.reporting;
using System;
using System.IO;

namespace imbNLP.Toolkit.Feature
{


    /// <summary>
    /// Feature vector dictionary with specified dimensions
    /// </summary>
    /// <seealso cref="imbNLP.Toolkit.Feature.FeatureVectorDictionary" />
    public class FeatureVectorDictionaryWithDimensions : FeatureVectorDictionary
    {

        public FeatureVectorDictionaryWithDimensions()
        {

        }

        public const String SUFFIX_VECTORS = "_vectors";
        public const String SUFFIX_DIMENSIONS = "_dimensions";

        public void Save(folderNode folder, String filename, ILogBuilder logger)
        {

            String d_p = folder.pathFor(filename + SUFFIX_VECTORS + ".xml", imbSCI.Data.enums.getWritableFileMode.overwrite, "Data dump from FeatureVector Dictionary with dimensions");
            String d_s = folder.pathFor(filename + SUFFIX_DIMENSIONS + ".xml", imbSCI.Data.enums.getWritableFileMode.overwrite, "Dimension specifications of FeatureVector Dictionary with dimensions");

            SaveVectors(d_p, logger);
            dimensions.Save(d_s, logger);

        }

        /// <summary>
        /// Loads the file.
        /// </summary>
        /// <param name="folder">The folder.</param>
        /// <param name="filename">The filename.</param>
        /// <param name="logger">The logger.</param>
        /// <returns></returns>
        public static FeatureVectorDictionaryWithDimensions LoadFile(folderNode folder, String filename, ILogBuilder logger)
        {
            FeatureVectorDictionaryWithDimensions output = new FeatureVectorDictionaryWithDimensions();



            output.Load(folder, filename, logger);

            return output;
        }

        public void Load(folderNode folder, String filename, ILogBuilder logger)
        {

            String n_p = filename + SUFFIX_VECTORS + ".xml";
            String n_s = filename + SUFFIX_DIMENSIONS + ".xml";

            String d_p = folder.pathMake(n_p); //, imbSCI.Data.enums.getWritableFileMode.overwrite, "Data dump from FeatureVector Dictionary with dimensions");
            String d_s = folder.pathMake(n_s); //, imbSCI.Data.enums.getWritableFileMode.overwrite, "Dimension specifications of FeatureVector Dictionary with dimensions");

            if (!File.Exists(d_p)) { d_p = folder.findFile(n_p, SearchOption.AllDirectories); }
            if (!File.Exists(d_s)) { d_s = folder.findFile(n_s, SearchOption.AllDirectories); }

            //context.folder.findFile(dictionaryFile + "*.xml", SearchOption.AllDirectories);

            dimensions = dimensionSpecificationSet.Load(d_s, logger);
            LoadVectors(d_p, logger);
        }




        public dimensionSpecificationSet dimensions { get; set; } = new dimensionSpecificationSet();



        public FeatureVector Create(String assignedID)
        {
            FeatureVector fv = new FeatureVector(assignedID);
            fv.dimensions = new double[dimensions.Count];

            return fv;

        }


        public FeatureVector GetOrAdd(String assignedID)
        {
            if (!ContainsKey(assignedID))
            {
                var fv = Create(assignedID);
                Add(fv);
            }


            return this[assignedID];

        }

    }
}