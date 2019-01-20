
using imbNLP.Toolkit.Documents;
using imbSCI.Data.collection.graph;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace imbNLP.Project.Dataset
{
    /// <summary>
    /// Hierarchical category model, dataset Data Object Model oriented
    /// </summary>
    /// <seealso cref="imbSCI.Data.collection.graph.graphNodeCustom" />
    public class WebDocumentsCategory : graphNodeCustom
    {

        public string description { get; set; } = "";

        protected override bool doAutorenameOnExisting { get { return false; } }

        protected override bool doAutonameFromTypeName { get { return false; } }

        public override string pathSeparator { get { return Path.DirectorySeparatorChar.ToString(); } set { } }

        public WebDocumentsCategory()
        {
        }

        public WebDocumentsCategory(string _name) : base()
        {
            name = _name;
        }

        public override graphNodeCustom CreateChildItem(string nameForChild)
        {
            WebDocumentsCategory output = new WebDocumentsCategory();
            output.name = nameForChild;
            Add(output);
            return output;
        }

        /// <summary>
        /// Creates dataset list, with all entities, groupped into first level categories.
        /// </summary>
        /// <returns></returns>
        public List<WebSiteDocumentsSet> GetFirstLevelCategories()
        {
            List<WebSiteDocumentsSet> dataset = new List<WebSiteDocumentsSet>();
            foreach (WebDocumentsCategory subcat in this)
            {
                WebSiteDocumentsSet ds = new WebSiteDocumentsSet();
                ds.AddRange(subcat.GetAllSites());
                ds.name = subcat.name;
                dataset.Add(ds);
            }
            return dataset;
        }

        /// <summary>
        /// Gets flat list of categories, where names represent category hierarchy graph path
        /// </summary>
        /// <param name="parentCatName">Name of the parent category, leave blank if this category should be considered as root</param>
        /// <returns></returns>
        public List<WebSiteDocumentsSet> GetAllCategories(String parentCatName = "")
        {
            List<WebSiteDocumentsSet> dataset = new List<WebSiteDocumentsSet>();
            foreach (WebDocumentsCategory subcat in this)
            {
                WebSiteDocumentsSet ds = new WebSiteDocumentsSet();
                ds.AddRange(subcat.siteDocuments);
                ds.name = parentCatName + pathSeparator + subcat.name;
                dataset.Add(ds);

                dataset.AddRange(subcat.GetAllCategories(ds.name));
            }
            return dataset;
        }


        /// <summary>
        /// Collects the sites from childer nodes until depth specified
        /// </summary>
        /// <param name="depth">The depth.</param>
        /// <returns></returns>
        public List<WebSiteDocuments> GetSites(Int32 depth = 1)
        {
            List<WebSiteDocuments> output = new List<WebSiteDocuments>();

            //sites.AddRange(sites);

            Int32 l = level;
            var newTasks = new List<WebDocumentsCategory>();
            newTasks.Add(this);

            while (newTasks.Any())
            {
                var tasks = newTasks.ToList();
                newTasks = new List<WebDocumentsCategory>();
                foreach (WebDocumentsCategory cat in tasks)
                {
                    output.AddRange(cat.siteDocuments);

                    if ((cat.level - l) < depth)
                    {
                        foreach (WebDocumentsCategory subcat in cat)
                        {
                            newTasks.Add(subcat);
                        }
                    }
                }

            }

            return output;

        }

        /// <summary>
        /// Gets or sets the sites.
        /// </summary>
        /// <value>
        /// The sites.
        /// </value>
        public List<WebSiteDocuments> siteDocuments { get; set; } = new List<WebSiteDocuments>();


        public WebSiteDocuments GetOrAdd(String domainName)
        {
            if (!siteDocuments.Any(x => x.domain == domainName))
            {
                WebSiteDocuments output = new WebSiteDocuments(domainName);
                siteDocuments.Add(output);
                return output;
            }
            else
            {
                return siteDocuments.FirstOrDefault(x => x.domain == domainName);
            }
        }

        /// <summary>
        /// Collects all web sites from the children nodes
        /// </summary>
        /// <returns></returns>
        public List<WebSiteDocuments> GetAllSites()
        {
            List<WebSiteDocuments> output = new List<WebSiteDocuments>();
            output.AddRange(siteDocuments);

            foreach (WebDocumentsCategory cat in this)
            {

                output.AddRange(cat.GetAllSites());
            }
            return output;
        }


        public WebDocumentsCategory GetOrAdd(String __path, Boolean isAbsolute)
        {
            WebDocumentsCategory cat = graphTools.ConvertPathToGraph<WebDocumentsCategory>(this, __path, isAbsolute, pathSeparator, true);  //Add(pathForCategory) as WebDomainCategory;

            if (cat == this)
            {
                
            }

            return cat;
        }



        /// <summary>
        /// Sets the categories by enumerable dataset
        /// </summary>
        /// <param name="categorySet">The category set.</param>
        public void SetCategoryByDataset(IEnumerable<WebSiteDocumentsSet> categorySet)
        {

            foreach (WebSiteDocumentsSet category in categorySet)
            {
                WebDocumentsCategory catChild = GetOrAdd(category.name, false);

                foreach (WebSiteDocuments site in category)
                {
                    var existingSite = catChild.siteDocuments.FirstOrDefault(x => x.domain == site.domain);

                    if (existingSite != null)
                    {
                        catChild.siteDocuments.Remove(existingSite);
                    }

                    catChild.siteDocuments.Add(site);
                    
                }
            }



        }



        /// <summary>
        /// Gets the domain category.
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <returns></returns>
        public WebDomainCategory GetDomainCategory(WebDomainCategory parent = null)
        {
            if (parent == null) parent = new WebDomainCategory(name);
            parent.sites.AddRange(siteDocuments.Select(x => x.domain));

            foreach (WebDocumentsCategory cat in this)
            {
                WebDomainCategory subParent = parent.CreateChildItem(cat.name) as WebDomainCategory;
                cat.GetDomainCategory(subParent);
            }
            return parent;
        }
    }
}