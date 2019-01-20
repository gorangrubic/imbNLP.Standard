using System;
using System.IO;
using System.Text;

namespace imbNLP.Toolkit.ExperimentModel.Settings
{







    /// <summary>
    /// Settings defining input dataset and basic transformation
    /// </summary>
    public class becDataSetSettings
    {


        public becDataSetSettings()
        {

        }



        public String GetShortSignature()
        {
            StringBuilder sb = new StringBuilder();



            String p = "";
            DirectoryInfo di = new DirectoryInfo(path);
            p = di.Name;

            sb.Append("P" + p);
            if (minPageLimit > 0)
            {
                sb.Append("" + minPageLimit);
            }
            if (maxPageLimit > 0)
            {
                sb.Append("" + maxPageLimit);
            }
            if (filterEmptyDocuments)
            {
                sb.Append("E");
            }
            if (flattenCategoryHierarchy)
            {
                sb.Append("F");
            }
            return sb.ToString();

        }

        public String GetDataSetName()
        {

            String dirPath = Path.GetDirectoryName(path);
            String dir = path.Substring(dirPath.Length).Trim(Path.DirectorySeparatorChar);
            return dir;
        }

        /// <summary>
        /// Harddrive path with the input dataset
        /// </summary>
        /// <value>
        /// The path.
        /// </value>
        public String path { get; set; } = "G:\\imbWBI\\datasets\\7sectors";

        /// <summary>
        /// Lowest number of pages that a website must have in order to be accepted for the experiment
        /// </summary>
        /// <value>
        /// The minimum page limit.
        /// </value>
        public Int32 minPageLimit { get; set; } = 1;


        /// <summary>
        /// Highest number of pages to be loaded for a website
        /// </summary>
        /// <value>
        /// The maximum page limit.
        /// </value>
        public Int32 maxPageLimit { get; set; } = -1;


        /// <summary>
        /// Should the empty documents be filtered out
        /// </summary>
        /// <value>
        ///   <c>true</c> if [filter empty documents]; otherwise, <c>false</c>.
        /// </value>
        public Boolean filterEmptyDocuments { get; set; } = true;

        /// <summary>
        /// Aggregates all Cases fron underneath categories into first-level (from root) categories
        /// </summary>
        /// <value>
        ///   <c>true</c> if [flatten category hierarchy]; otherwise, <c>false</c>.
        /// </value>
        public Boolean flattenCategoryHierarchy { get; set; } = true;
    }
}