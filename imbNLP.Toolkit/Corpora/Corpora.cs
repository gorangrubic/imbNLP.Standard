
using System;
using System.IO;

namespace imbNLP.Toolkit.Corpora
{
    /// <summary>
    ///
    /// </summary>
    public class Corpora
    {
        public int totalWords;
        public int totalDocuments;


        public Document[] Docs;
        public WordDictionary WD;

        public Corpora()
        {
            WD = new WordDictionary();
            totalDocuments = 0;
            totalWords = 0;
        }

        public int MaxWordID()
        {
            return WD.Count;
        }

        public void LoadDataFile(string file)
        {
            try
            {
                string[] f = File.ReadAllLines(file);
                totalDocuments = f.Length;
                Docs = new Document[totalDocuments];
                for (int i = 0; i < totalDocuments; i++)
                {
                    Docs[i] = new Document();
                    Docs[i].Init(f[i], WD);
                    totalWords += Docs[i].Length;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string GetStringByID(int id)
        {
            return WD.GetString(id);
        }
    }
}