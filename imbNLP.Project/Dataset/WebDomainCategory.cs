using imbACE.Network.tools;
using imbSCI.Core;
using imbSCI.Core.extensions.io;
using imbSCI.Core.files.folders;
using imbSCI.Core.reporting;
using imbSCI.Data;
using imbSCI.Data.collection.graph;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace imbNLP.Project.Dataset
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="imbSCI.Data.collection.graph.graphNodeCustom" />
    public class WebDomainCategory : graphNodeCustom
    {
        protected override bool doAutorenameOnExisting { get { return false; } }

        protected override bool doAutonameFromTypeName { get { return false; } }

        public override string pathSeparator { get { return Path.DirectorySeparatorChar.ToString(); } }

        public WebDomainCategory()
        {
        }

        public WebDomainCategory(String _name)
        {
            name = _name;
        }

        public String description { get; set; } = "";


        public override graphNodeCustom CreateChildItem(string nameForChild)
        {
            WebDomainCategory output = new WebDomainCategory();
            output.name = nameForChild;
            Add(output);
            return output;
        }

        public List<String> sites { get; set; } = new List<string>();

        public const String categorySiteList = "sample.txt";
        public const String categoryAggregateSiteList = "sampleAggregate.txt";


        public List<WebDomainCategory> GetCategories(Int32 depthOffset = 1)
        {
            List<WebDomainCategory> output = new List<WebDomainCategory>();

            Int32 l = level;
            List<WebDomainCategory> newTasks = new List<WebDomainCategory>();
            newTasks.Add(this);

            while (newTasks.Any())
            {
                var tasks = newTasks.ToList();
                newTasks = new List<WebDomainCategory>();
                foreach (WebDomainCategory cat in tasks)
                {
                    if (cat.Count() == 0)
                    {
                        output.Add(cat);
                    }
                    else
                    {
                        if ((cat.level - l) < depthOffset)
                        {
                            foreach (WebDomainCategory subcat in cat)
                            {
                                newTasks.Add(subcat);

                            }
                        }
                        else
                        {
                            foreach (WebDomainCategory subcat in cat)
                            {
                                output.Add(subcat);

                            }
                        }
                    }
                }

            }

            return output;
        }

        /// <summary>
        /// Collects the sites from childer nodes until depth specified
        /// </summary>
        /// <param name="depth">The depth.</param>
        /// <returns></returns>
        public List<String> GetSites(Int32 depth = 1)
        {
            List<String> output = new List<string>();

            //sites.AddRange(sites);

            Int32 l = level;
            List<WebDomainCategory> newTasks = new List<WebDomainCategory>();
            newTasks.Add(this);

            while (newTasks.Any())
            {
                var tasks = newTasks.ToList();
                newTasks = new List<WebDomainCategory>();
                foreach (WebDomainCategory cat in tasks)
                {
                    output.AddRange(cat.sites);

                    if ((cat.level - l) < depth)
                    {
                        foreach (WebDomainCategory subcat in cat)
                        {
                            newTasks.Add(subcat);
                        }
                    }
                }

            }

            return output;

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
            foreach (var fi in di.EnumerateFiles(categorySiteList, SearchOption.AllDirectories))
            {
                c++;
                if (c > 2)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Gets the domain list.
        /// </summary>
        /// <param name="options">The options.</param>
        /// <returns></returns>
        public String GetDomainList(WebDomainCategoryFormatOptions options, ILogBuilder logger = null)
        {
            StringBuilder sb = new StringBuilder();
            foreach (String s in sites)
            {
                String ln = s;
                if (options.HasFlag(WebDomainCategoryFormatOptions.normalizeDomainname))
                {
                    domainAnalysis da = new domainAnalysis(s);

                    ln = da.urlProper;
                }
                sb.AppendLine(ln);
            }

            return sb.ToString();
        }

        public WebDomainCategory AddCategory(List<String> domainList, String _name, String _description, ILogBuilder logger)
        {
            WebDomainCategory output = Add(_name) as WebDomainCategory;
            output.sites.AddRange(domainList);
            output.description = _description;
            return output;

        }


        /// <summary>
        /// Loads the domain list.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="options">The options.</param>
        public void LoadDomainList(String path, WebDomainCategoryFormatOptions options = WebDomainCategoryFormatOptions.saveReadmeFile | WebDomainCategoryFormatOptions.saveAggregate | WebDomainCategoryFormatOptions.normalizeDomainname, ILogBuilder logger = null)
        {
            if (File.Exists(path))
            {
                sites.Clear();
                String[] list = File.ReadAllLines(path);
                foreach (String ln in list)
                {
                    String s = ln;
                    if (options.HasFlag(WebDomainCategoryFormatOptions.normalizeDomainname))
                    {
                        domainAnalysis da = new domainAnalysis(s);
                        s = da.urlProper;
                    }
                    sites.Add(s);
                }
            }
        }

        /// <summary>
        /// Loads the specified folder.
        /// </summary>
        /// <param name="folder">The folder.</param>
        /// <param name="options">The options.</param>
        public void Load(String path, WebDomainCategoryFormatOptions options = WebDomainCategoryFormatOptions.saveReadmeFile | WebDomainCategoryFormatOptions.saveAggregate | WebDomainCategoryFormatOptions.normalizeDomainname, ILogBuilder logger = null)
        {
            DirectoryInfo di = new DirectoryInfo(path);



            FileInfo rootList = di.GetFiles(categorySiteList, SearchOption.TopDirectoryOnly).FirstOrDefault(); //folder.findFile(categorySiteList, SearchOption.TopDirectoryOnly);

            if (rootList!=null)
            {
                LoadDomainList(rootList.FullName, options);
            }

            List<FileInfo> sampleFiles = di.GetFiles(categorySiteList, SearchOption.AllDirectories).ToList();

            foreach (var fi in sampleFiles)
            {

                String pathForCategory = fi.DirectoryName.removeStartsWith(di.FullName); //Path.GetDirectoryName(fi).removeStartsWith(folder.path);



                WebDomainCategory cat = graphTools.ConvertPathToGraph<WebDomainCategory>(this, pathForCategory, false, Path.DirectorySeparatorChar.ToString());  //Add(pathForCategory) as WebDomainCategory;
                cat.LoadDomainList(fi.FullName, options, logger);

                //fi.FullName.Remove(folder.)
            }
        }

        /// <summary>
        /// Saves the specified folder.
        /// </summary>
        /// <param name="folder">The folder.</param>
        /// <param name="options">The options.</param>
        public void Save(String pathToSave, WebDomainCategoryFormatOptions options = WebDomainCategoryFormatOptions.saveReadmeFile | WebDomainCategoryFormatOptions.saveAggregate | WebDomainCategoryFormatOptions.normalizeDomainname, ILogBuilder logger = null)
        {
            DirectoryInfo di = new DirectoryInfo(pathToSave);
            folderNode folder = di;

            String domainList = GetDomainList(options, logger);
            String path = folder.pathFor(categorySiteList, imbSCI.Data.enums.getWritableFileMode.overwrite, "Web sites at this category", true);
            File.WriteAllText(path, domainList);

            StringBuilder sb = new StringBuilder();
            foreach (WebDomainCategory category in this)
            {
                var subFolder = folder.Add(category.name, category.name, "Subcategory");
                category.Save(subFolder.path, options, logger);

                if (options.HasFlag(WebDomainCategoryFormatOptions.saveAggregate))
                {

                    sb.AppendLine(GetDomainList(options, logger));


                }
            }

            if (options.HasFlag(WebDomainCategoryFormatOptions.saveAggregate))
            {
                path = folder.pathFor(categoryAggregateSiteList, imbSCI.Data.enums.getWritableFileMode.overwrite, "Web sites at this category, including subcategories", true);
                File.WriteAllText(path, sb.ToString());
            }

            if (this.root == this)
            {
                if (options.HasFlag(WebDomainCategoryFormatOptions.saveGraphAtRoot))
                {


                    var dgml = GraphConverters.DataSetDomainGraphConverter.Convert(this, 300);  //imbSCI.Graph.Converters.GraphConversionTools.DefaultGraphToDGMLConverterInstance.Convert(this, 300); //.ConvertToDGML(this, 300);
                    dgml.Save(folder.pathFor("dataset.dgml", imbSCI.Data.enums.getWritableFileMode.overwrite, "DirectedGraphMarkupLanguage representation of categories", true), imbSCI.Data.enums.getWritableFileMode.overwrite);

                }

                if (options.HasFlag(WebDomainCategoryFormatOptions.saveReadmeFile))
                {
                    folder.generateReadmeFiles(imbACE.Core.appManager.AppInfo);
                }
            }
        }
    }
}