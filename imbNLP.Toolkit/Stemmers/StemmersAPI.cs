using imbNLP.Toolkit.Typology;
using System;

namespace imbNLP.Toolkit.Stemmers
{
    public static class StemmersAPI
    {





        //private static Object _stemmerTypeDictionary_lock = new Object();
        //private static Dictionary<string, Type> _stemmerTypeDictionary;
        ///// <summary>
        ///// class name vs type dictionary 
        ///// </summary>
        //public static Dictionary<string, Type> stemmerTypeDictionary
        //{
        //    get
        //    {
        //        if (_stemmerTypeDictionary == null)
        //        {
        //            lock (_stemmerTypeDictionary_lock)
        //            {

        //                if (_stemmerTypeDictionary == null)
        //                {

        //                    _stemmerTypeDictionary = new Dictionary<string, Type>();
        //                    List<Type> types = typeof(StemmersAPI).Assembly.GetTypes().Where(x => x.Namespace.Contains("imbNLP.Toolkit.Stemmers")).ToList();
        //                    Type iStemmer = typeof(IStemmer);

        //                    foreach (Type t in types)
        //                    {
        //                        var iList = t.GetInterfaces();
        //                        if (iList.Any())
        //                        {
        //                            if (iList.Contains(iStemmer))
        //                            {
        //                                _stemmerTypeDictionary.Add(t.Name, t);
        //                            }
        //                        }
        //                    }

        //                }
        //            }
        //        }
        //        return _stemmerTypeDictionary;
        //    }
        //}



        //public static IStemmer GetStemmer(string stemmerClassName)
        //{
        //    IStemmer output = null;

        //    if (stemmerTypeDictionary.ContainsKey(stemmerClassName))
        //    {
        //        Type t = stemmerTypeDictionary[stemmerClassName];
        //        output = Activator.CreateInstance(t) as IStemmer;

        //    }

        //    return output;
        //}
    }
}