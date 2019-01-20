using imbNLP.Toolkit.Documents;
using imbSCI.Core.extensions.io;
using imbSCI.Core.math;
using imbSCI.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace imbNLP.Toolkit.Core
{
public static class CacheServiceHelper
    {

        public static String GetDataSetSignature(this IEnumerable<WebSiteDocumentsSet> dataset)
        {
            StringBuilder sb = new StringBuilder();
            foreach (WebSiteDocumentsSet ws in dataset)
            {
                sb.Append(ws.name + ":" + ws.Count);
                foreach (WebSiteDocuments wd in ws)
                {
                    sb.Append(wd.documents.Count + ":");
                }
            }
            return md5.GetMd5Hash(sb.ToString());
        }
    }
}