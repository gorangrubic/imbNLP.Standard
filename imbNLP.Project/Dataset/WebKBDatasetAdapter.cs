
using imbNLP.Toolkit.Documents;
using imbSCI.Core.files.folders;
using imbSCI.Core.reporting;
using imbSCI.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace imbNLP.Project.Dataset
{

    /// <summary>
    /// Adapter for loading a WebKB-formatted dataset
    /// </summary>
    /// <seealso cref="imbNLP.Project.Dataset.IWebDocumentsCategoryAdapter" />
    public class WebKBDatasetAdapter : IWebDocumentsCategoryAdapter
    {
        //public Boolean generateReadmeFiles { get; set; } = true;
        //public Boolean generateGraphFile { get; set; } = true;
        //public Boolean generateDomainListAtCategory { get; set; } = true;



        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>
        /// The description.
        /// </value>
        public String description { get; set; } = "WebKB 7Sectors dataset format adapter - blog.veles.rs - imbNLP framework.";

        /// <summary>
        /// Gets the URL path from filename.
        /// </summary>
        /// <param name="filename">The filename.</param>
        /// <returns></returns>
        private String GetURLPathFromFilename(String filename)
        {
            String output = filename;
            output = output.Replace("_", ":");
            output = output.Replace("^", "/");
            return output;
        }

        /// <summary>
        /// Gets the filename from URL path.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <returns></returns>
        private String GetFilenameFromURLPath(String url)
        {
            String output = url;
            output = output.Replace(":", "_");
            output = output.Replace("/", "^");
            return output;
        }

        /// <summary>
        /// Gets or sets the name of the select domain.
        /// </summary>
        /// <value>
        /// The name of the select domain.
        /// </value>
        private Regex SelectDomainName { get; set; } = new Regex("//([\\w\\d\\.\\-]*)/");
        /// <summary>
        /// Gets or sets the select path.
        /// </summary>
        /// <value>
        /// The select path.
        /// </value>
        private Regex SelectPath { get; set; } = new Regex("//[\\w\\d\\.\\-]*/([\\s\\w\\/\\d\\.\\-_]*)");

        /// <summary>
        /// Loads the web site document.
        /// </summary>
        /// <param name="fi">The fi.</param>
        /// <param name="webSite">The web site.</param>
        /// <returns></returns>
        private WebSiteDocument LoadWebSiteDocument(FileInfo fi, WebSiteDocuments webSite, WebDomainCategoryFormatOptions options)
        {


            WebSiteDocument output = null;
            String path = GetURLPathFromFilename(fi.Name);


            path = path.Substring(path.IndexOf(webSite.domain) + webSite.domain.Length);

            //path = path.TrimStart('//');
            //if (SelectPath.IsMatch(path))
            //{
            //    path = SelectPath.Match(path).Value;
            //} else
            //{
            //    path = path.removeStartsWith("http://" + webSite.domain );
            //}

            output = new WebSiteDocument(path, options.HasFlag(WebDomainCategoryFormatOptions.lazyLoading), fi.FullName);






            return output;
        }

        /// <summary>
        /// Loads the web sites.
        /// </summary>
        /// <param name="category">The category.</param>
        /// <param name="di">The di.</param>
        /// <param name="logger">The logger.</param>
        private void LoadWebSites(WebDocumentsCategory category, DirectoryInfo di, WebDomainCategoryFormatOptions options, ILogBuilder logger = null)
        {
            FileInfo[] fileList = di.GetFiles();

            Dictionary<String, List<FileInfo>> siteFilesIndex = new Dictionary<string, List<FileInfo>>();

            if (fileList.Length > 1)
            {
                foreach (FileInfo fi in fileList)
                {

                    String path = GetURLPathFromFilename(fi.Name);
                    if (path.StartsWith("http"))
                    {
                        Match m = SelectDomainName.Match(path);
                        if (m.Success)
                        {
                            String domain = m.Groups[1].Value;
                            if (!siteFilesIndex.ContainsKey(domain)) siteFilesIndex.Add(domain, new List<FileInfo>());

                            siteFilesIndex[domain].Add(fi);
                        }
                    }
                }

                if (logger != null)
                {
                    logger.log("Web sites detected: [" + siteFilesIndex.Count + "]");
                }

                foreach (String k in siteFilesIndex.Keys)
                {
                    WebSiteDocuments webSite = new WebSiteDocuments(k);

                    foreach (FileInfo fi in siteFilesIndex[k])
                    {

                        webSite.documents.Add(LoadWebSiteDocument(fi, webSite, options));
                    }

                    category.siteDocuments.Add(webSite);

                    if (logger != null)
                    {
                        logger.log(category.path + " -> [" + webSite.domain + "] -> pages [" + webSite.documents.Count + "]");
                    }
                }
            }
        }

        /// <summary>
        /// Loads the directory.
        /// </summary>
        /// <param name="category">The category.</param>
        /// <param name="di">The di.</param>
        /// <param name="options">The options.</param>
        /// <param name="logger">The logger.</param>
        private void LoadDirectory(WebDocumentsCategory category, DirectoryInfo di, WebDomainCategoryFormatOptions options, ILogBuilder logger = null)
        {
            DirectoryInfo[] dirList = di.GetDirectories();
            foreach (DirectoryInfo dir in dirList)
            {
                WebDocumentsCategory child = category.CreateChildItem(dir.Name) as WebDocumentsCategory;
                LoadWebSites(child, dir, options, logger);
                LoadDirectory(child, dir, options, logger);
            }
        }

        /// <summary>
        /// Loads the dataset.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="logger">The logger.</param>
        /// <returns></returns>
        public WebDocumentsCategory LoadDataset(String path, WebDomainCategoryFormatOptions options, ILogBuilder logger = null)
        {
            WebDocumentsCategory output = new WebDocumentsCategory();

            DirectoryInfo dir = new DirectoryInfo(path);

            output.name = dir.Name;

            if (!dir.Exists)
            {
                if (logger != null) logger.log("Directory " + path + " not found!");
                return output;
            }

            LoadDirectory(output, dir, options, logger);

            return output;
        }

        /// <summary>
        /// Gets the web document source.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <returns></returns>
        protected String GetWebDocumentSource(WebSiteDocument page, ILogBuilder logger = null)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(page.HTTPHeader);
            sb.AppendLine(page.HTMLSource);
            return sb.ToString();
        }

        /// <summary>
        /// Saves the subcategories.
        /// </summary>
        /// <param name="category">The category.</param>
        /// <param name="rootFolder">The root folder.</param>
        protected void SaveSubcategories(WebDocumentsCategory category, folderNode rootFolder, WebDomainCategoryFormatOptions options, ILogBuilder logger = null)
        {
            foreach (WebDocumentsCategory subcat in category)
            {
                SaveWebSites(subcat, rootFolder, options);
            }
        }



        /// <summary>
        /// Saves the web sites.
        /// </summary>
        /// <param name="category">The category.</param>
        /// <param name="rootFolder">The root folder.</param>
        protected void SaveWebSites(WebDocumentsCategory category, folderNode rootFolder, WebDomainCategoryFormatOptions options, ILogBuilder logger = null)
        {
            folderNode folder = rootFolder.Add(category.name, category.name, category.description);
            StringBuilder domainList = new StringBuilder();

            foreach (WebSiteDocuments site in category.siteDocuments)
            {
                domainList.AppendLine(site.domain);
                foreach (WebSiteDocument page in site.documents)
                {
                    String source = GetWebDocumentSource(page);
                    String filename = site.domain.add(page.path, "/");
                    filename = filename.Replace("//", "/");
                    filename = "http://" + filename;
                    filename = GetFilenameFromURLPath(filename);
                    filename = WebSiteDocumentsSetTools.GetSafeFilename(filename);

                    File.WriteAllText(folder.pathFor(filename, imbSCI.Data.enums.getWritableFileMode.overwrite, "Page of [" + site.domain + "] at path [" + page.path + "]", true), source);
                }
            }

            if (options.HasFlag(WebDomainCategoryFormatOptions.saveDomainList))
            {
                File.WriteAllText(folder.pathFor(WebDomainCategory.categorySiteList, imbSCI.Data.enums.getWritableFileMode.overwrite, "Domains in category [" + category.path + "]", true), domainList.ToString());
            }

            SaveSubcategories(category, folder, options);

        }


        /// <summary>
        /// Designates if the path seems to contain a WebKB formatted dataset
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        public Boolean Probe(String path, ILogBuilder logger = null)
        {
            DirectoryInfo di = new DirectoryInfo(path);
            if (!di.Exists) return false;
            Int32 c = 0;
            foreach (var fi in di.EnumerateFiles("http*.*", SearchOption.AllDirectories))
            {
                c++;
                if (c > 5)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Saves the dataset.
        /// </summary>
        /// <param name="dataset">The dataset.</param>
        /// <param name="path">The path.</param>
        public void SaveDataset(WebDocumentsCategory dataset, String path, WebDomainCategoryFormatOptions options, ILogBuilder logger = null)
        {
            folderNode folder = new DirectoryInfo(path);
            folder.description = description;


            SaveWebSites(dataset, folder, options);

            if (options.HasFlag(WebDomainCategoryFormatOptions.saveGraphAtRoot))
            {
                var dmgl = GraphConverters.documentsConverter.Convert(dataset, 300); // imbSCI.Graph.Converters.GraphConversionTools.ConvertToDGML<WebDocumentsCategory>(dataset, 300);
                dmgl.Save(folder.pathFor("dataset", imbSCI.Data.enums.getWritableFileMode.overwrite, "Directed graph of categories in the dataset", true));
                var dot = imbSCI.Graph.Converters.GraphConversionTools.ConvertToDOT(dmgl);
                dot.Save(folder.pathFor("dataset_dot", imbSCI.Data.enums.getWritableFileMode.existing, "DOT graph of categories in the dataset", true));
                //var mxgraph = imbSCI.Graph.MXGraph.directedGraphToMXGraph.ConvertToMXGraph(dmgl);

            }

            if (options.HasFlag(WebDomainCategoryFormatOptions.saveReadmeFile))
            {
                folder.generateReadmeFiles(imbACE.Core.appManager.AppInfo);
            }



        }
    }
}