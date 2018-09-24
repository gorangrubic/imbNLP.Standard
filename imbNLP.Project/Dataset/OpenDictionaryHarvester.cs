using HtmlAgilityPack;
using imbSCI.Core.extensions.data;
using imbSCI.Core.reporting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace imbNLP.Project.Dataset
{
    public class OpenDictionaryHarvester
    {
        private Regex SelectDomainName { get; set; } = new Regex("//([\\w\\d\\.\\-]*)/");
        private Regex SelectPath { get; set; } = new Regex("//[\\w\\d\\.\\-]*/([\\s\\w\\/\\d\\.\\-_]*)");

        public String HomeDomain { get; set; } = "dmoztools.net";
        public String HomeURL { get; set; } = "/Business";

        public String HTTPPrefix { get; set; } = "http";

        public Int32 DepthLimit { get; set; } = 3;

        public WebDomainCategory result { get; set; } = new WebDomainCategory();

        public String[] BlacklistDomains { get; set; } = new string[] {
            "twitter.com", "facebook.com", "aol.com", "ask.com", "bing.com", "duckduckgo.com", "google.com", "ixquick.com", "yahoo.com", "yandex.com", "new.yippy.com", "gigablast.com", "xml.com", "mp3.com",
        "consumersearch.com"};

        public OpenDictionaryHarvester()
        {
        }

        public OpenDictionaryHarvester(String startURL)
        {
            HomeDomain = SelectDomainName.Match(startURL).Groups[1].Value;
            HomeURL = SelectPath.Match(startURL).Groups[1].Value;
        }

        public void Start(String startingURL, ILogBuilder logger = null)
        {
            WebDomainCategory node = result;

            WebDirectoryIteration iteration = new WebDirectoryIteration();
            Match m = SelectPath.Match(startingURL);
            iteration.URL = startingURL;
            iteration.DirectoryPath = m.Groups[1].Value;

            node.name = iteration.DirectoryPath.Trim('/');

            iteration.DirectoryNode = node;

            List<WebDirectoryIteration> tasks = new List<WebDirectoryIteration>();
            tasks.Add(iteration);

            while (tasks.Any())
            {
                List<WebDirectoryIteration> newTasks = new List<WebDirectoryIteration>();

                foreach (WebDirectoryIteration task in tasks)
                {
                    newTasks.AddRange(Load(task, logger));
                }
                logger.log("Tasks done [" + tasks.Count + "] - new tasks [" + newTasks.Count + "]");
                tasks = newTasks;
            }
        }

        public List<WebDirectoryIteration> Load(WebDirectoryIteration iteration, ILogBuilder logger = null)
        {
            HtmlAgilityPack.HtmlWeb web = new HtmlWeb();

            HtmlDocument htmlDoc = new HtmlDocument();

            htmlDoc = web.Load(iteration.URL);

            Process(htmlDoc, iteration);

            List<WebDirectoryIteration> output = new List<WebDirectoryIteration>();

            foreach (String path in iteration.SubdirectoryList)
            {
                if (iteration.DirectoryNode.level < DepthLimit)
                {
                    WebDirectoryIteration newIteration = new WebDirectoryIteration("https://" + HomeDomain + path);
                    WebDomainCategory subNode = iteration.DirectoryNode.CreateChildItem(path.Replace(iteration.DirectoryPath, "").Trim('/')) as WebDomainCategory;
                    newIteration.DirectoryNode = subNode;
                    newIteration.DirectoryPath = path;
                    output.Add(newIteration);
                }
            }

            iteration.DirectoryNode.sites.AddRange(iteration.WebsiteList);

            return output;
        }

        public void Process(HtmlDocument page, WebDirectoryIteration iteration)
        {
            HtmlNodeCollection links = page.DocumentNode.SelectNodes("//a");
            if (links == null) return;

            IEnumerable<string> pathList = links.Select(x => x.GetAttributeValue("href", ""));

            foreach (String path in pathList)
            {
                if (path.StartsWith(HTTPPrefix))
                {
                    if (path.Contains(HomeDomain))
                    {
                        iteration.OtherInnerLinks.Add(path);
                    }
                    else
                    {
                        if (!path.ContainsAny(BlacklistDomains))
                        {
                            iteration.WebsiteList.Add(path);
                        }
                    }
                }
                else if (path.StartsWith("/" + iteration.DirectoryPath) || path.StartsWith(iteration.DirectoryPath))
                {
                    iteration.SubdirectoryList.Add(path);
                }
                else
                {
                    iteration.OtherInnerLinks.Add(path);
                }
            }
        }
    }
}