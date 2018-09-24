using imbSCI.Core.extensions.io;
using imbSCI.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace imbNLP.PartOfSpeech.evaluation.termTruthTable
{
    /// <summary>
    /// List of <see cref="termQualification"/>
    /// </summary>
    /// <seealso cref="System.Collections.Generic.List{imbNLP.PartOfSpeech.evaluation.termTruthTable.termQualification}" />
    public class termQualificationList : List<termQualification>
    {
        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder();

            foreach (termQualification tq in this)
            {
                sb.AppendLine(tq.ToString());
            }
            return sb.ToString();
        }

        /// <summary>
        /// Populates the list from source string
        /// </summary>
        /// <param name="source">The source.</param>
        public void FromString(String source)
        {
            List<String> output = source.SplitSmart(Environment.NewLine, "", true, true);

            foreach (String line in output)
            {
                termQualification tq = new termQualification();
                tq.FromString(line);
                if (!this.Any(x => x.lemmaForm == tq.lemmaForm))
                {
                    Add(tq);
                }
            }
        }

        /// <summary>
        /// Loads the specified path.
        /// </summary>
        /// <param name="path">The path.</param>
        public void Load(String path)
        {
            if (File.Exists(path))
            {
                String code = File.ReadAllText(path);
                FromString(code);
            }
        }

        /// <summary>
        /// Saves the specified path.
        /// </summary>
        /// <param name="path">The path.</param>
        public void Save(String path)
        {
            FileInfo fi = path.getWritableFile(imbSCI.Data.enums.getWritableFileMode.appendFile, null);

            if (File.Exists(path))
            {
                termQualificationList tmp = new termQualificationList();
                tmp.Load(path);
                foreach (var v in tmp)
                {
                    if (!this.Any(x => x.lemmaForm == v.lemmaForm))
                    {
                        Add(v);
                    }
                }
            }

            String code = ToString();
            File.WriteAllText(path, code);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="termQualificationList"/> class.
        /// </summary>
        public termQualificationList()
        {
        }
    }
}