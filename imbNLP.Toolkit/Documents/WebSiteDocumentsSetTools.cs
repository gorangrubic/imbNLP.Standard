using imbNLP.Toolkit.Space;
using imbSCI.Core.extensions.data;
using imbSCI.Core.math;
using imbSCI.Core.reporting.render;
using imbSCI.Core.reporting.render.builders;
using imbSCI.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace imbNLP.Toolkit.Documents
{
    /// <summary>
    /// Extension methods and tools for operations over data set
    /// </summary>
    public static class WebSiteDocumentsSetTools
    {

        public static void TransferExtensionsTo(this IEnumerable<WebSiteDocumentsSet> source, IEnumerable<WebSiteDocumentsSet> target)
        {

            Dictionary<string, WebSiteDocuments> source_dictionary = source.GetSiteByDomainName();

            Dictionary<string, WebSiteDocuments> target_dictionary = target.GetSiteByDomainName();

            foreach (var pair in source_dictionary)
            {
                if (target_dictionary.ContainsKey(pair.Key))
                {
                    target_dictionary[pair.Key].extensions = pair.Value.extensions;
                }
            }
        }

        public static Dictionary<String, WebSiteDocuments> GetSiteByDomainName(this IEnumerable<WebSiteDocumentsSet> dataset)
        {

            Dictionary<String, WebSiteDocuments> output = new Dictionary<string, WebSiteDocuments>();

            foreach (WebSiteDocumentsSet ws in dataset)
            {
                foreach (WebSiteDocuments site in ws)
                {
                    output.Add(site.domain, site);
                }
            }

            return output;
        }



        /// <summary>
        /// Returns document with shortest <see cref="WebSiteDocument.AssignedID"/>
        /// </summary>
        /// <param name="documents">Set of documents</param>
        /// <returns></returns>
        public static T GetDocWithShortestID<T>(this IEnumerable<T> documents) where T : IAssignedID
        {

            Int32 len = Int32.MaxValue;
            T output = default(T);

            foreach (T doc in documents)
            {
                if (doc.AssignedID.Length < len)
                {
                    output = doc;
                    len = doc.AssignedID.Length;
                }

            }

            return output;

        }


        /// <summary>
        /// Returns set that contains unlabeled entries
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns></returns>
        public static WebSiteDocumentsSet GetUnlabeledDataSet(this IEnumerable<WebSiteDocumentsSet> input)
        {
            WebSiteDocumentsSet UnlabeledSites = null;

            List<WebSiteDocumentsSet> inputSet = input.ToList();


            UnlabeledSites = inputSet.FirstOrDefault(x => x.name.isNullOrEmpty());
            if (UnlabeledSites == null)
            {
                UnlabeledSites = inputSet.FirstOrDefault(x => x.name == SpaceLabel.UNKNOWN);
            }

            return UnlabeledSites;
        }



        /// <summary>
        /// Makes a subset of input dataset, containing only documents matching assigned IDS
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="assignedIDs">The assigned i ds.</param>
        /// <param name="inverse">if set to <c>true</c> if will keep only given assigned IDS</param>
        public static void DataSetSubSet(this IList<WebSiteDocumentsSet> input, List<String> assignedIDs, Boolean inverse = true, Boolean applyToUnknownCategory = false)
        {
            foreach (WebSiteDocumentsSet category in input)
            {
                if (category.name == SpaceLabel.UNKNOWN)
                {
                    if (!applyToUnknownCategory) continue;
                }

                foreach (WebSiteDocuments site in category)
                {
                    List<WebSiteDocument> toRemove = new List<WebSiteDocument>();
                    foreach (WebSiteDocument page in site.documents)
                    {
                        if (!assignedIDs.Contains(page.AssignedID))
                        {
                            toRemove.Add(page);
                        }
                    }
                    foreach (WebSiteDocument page in toRemove)
                    {
                        site.documents.Remove(page);
                    }
                }
            }
        }


        public static String GetPageURL(WebSiteDocument page, WebSiteDocuments site)
        {
            String url = WebSiteDocumentsSetTools.GetRequestURL(page.HTTPHeader);
            if (url.isNullOrEmpty())
            {
                url = site.domain.add(page.path, "/");
                url = url.ensureStartsWith("http://");
            }
            return url;
        }


        /// <summary>
        /// Replaces forbiden characters
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <returns></returns>
        public static String GetUrlSignature(String url)
        {
            String output = url.ToLower().Replace("http://", "").Replace("https://", "");
            output = output.Replace("/", "");
            output = output.Replace("\\", "");

            output = output.Replace(".", "");
            return output;
        }

        public const Int32 filenameMaxLength = 100;

        public static Boolean IsSafeFilename(String input)
        {
            Boolean output = input.Length > filenameMaxLength;

            if (output)
            {

            }
            return output;
        }


        public static Regex SelectRequestURL { get; set; } = new Regex("Response-URL: ([\\w:\\?\\=\\-_/\\.&]*)");

        /// <summary>
        /// Extracts request URL information from document header string
        /// </summary>
        /// <param name="headerSource">The header source.</param>
        /// <returns></returns>
        public static String GetRequestURL(String headerSource)
        {
            var mch = SelectRequestURL.Match(headerSource);
            String output = "";
            if (mch.Success)
            {
                if (mch.Groups.Count > 0)
                {
                    output = mch.Groups[1].Value;
                }
            }
            return output;
        }

        /// <summary>
        /// Describes the counts.
        /// </summary>
        /// <param name="dataset">The dataset.</param>
        /// <param name="output">The output.</param>
        /// <returns></returns>
        public static String DescribeCounts(this IEnumerable<WebSiteDocumentsSet> dataset, ITextRender output = null)
        {
            long start = 0;
            if (output == null)
            {
                output = new builderForText();
            }
            else
            {
                start = output.Length;
            }

            Int32 wsc = 0;
            Int32 tdc = 0;
            foreach (var set in dataset)
            {
                Int32 td = set.CountDocumentsTotal();
                wsc += set.Count;
                tdc += td;
                output.AppendLine(set.name + "\t S[" + set.Count.ToString("D5") + "] D[" + td.ToString("D6") + "]");
            }
            output.AppendHorizontalLine();
            output.AppendLine("Total \t S[" + wsc.ToString("D5") + "] D[" + tdc.ToString("D6") + "]");
            output.AppendHorizontalLine();
            output.AppendComment("S = document sets,  D = documents");

            return output.GetContent(start);
        }


        /// <summary>
        /// If filename is longer than predefined maximum it will replace it with version ending with first 8 characters of MD5 hashed input filepath
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns></returns>
        public static String GetSafeFilename(String input)
        {
            if (input.Length > filenameMaxLength)
            {
                input = input.Substring(0, filenameMaxLength) + md5.GetMd5Hash(input).Substring(0, 8);
            }
            return input;
        }

    }
}