using System.Collections.Generic;

namespace imbNLP.Toolkit.Corpora
{
    public class WordDictionary
    {
        public Dictionary<string, int> Word2Id;

        public List<string> Words;
        public int Count;

        public WordDictionary()
        {
            Word2Id = new Dictionary<string, int>();
            Words = new List<string>();
            Count = 0;
        }

        public string GetString(int id)
        {
            if (id > Count) return null;
            return Words[id];
        }

        public int GetWords(string str)
        {
            if (Word2Id.ContainsKey(str))
            {
                return Word2Id[str];
            }
            else
            {
                return AddWord(str);
            }
        }

        public int AddWord(string str)
        {
            if (!Word2Id.ContainsKey(str))
            {
                Words.Add(str);
                Word2Id[str] = Count;
                Count++;
                return Count - 1;
            }
            return -1;
        }
    }
}