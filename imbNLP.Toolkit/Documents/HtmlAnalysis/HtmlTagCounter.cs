using imbSCI.Core.extensions.data;
using imbSCI.Data;
using imbSCI.Data.collection.graph;
using System;
using System.Collections.Generic;

namespace imbNLP.Toolkit.Documents.HtmlAnalysis
{



    //    public List<String> DistinctUnknownTags { get; set; } = new List<string>();

    public class HtmlTagCounter : graphNodeCustom
    {


        public HtmlTagCounter GetOrAddCategory(String categoryPath, String _description = "", Double _weight = 1)
        {
            var tagParts = categoryPath.SplitSmart(pathSeparator, "", true, true);
            var head = this;
            foreach (String tp in tagParts)
            {
                if (this.ContainsKey(tp))
                {
                    head = this[tp] as HtmlTagCounter;
                }
                else
                {
                    HtmlTagCounter htc = new HtmlTagCounter(tp, _description, _weight);
                    head.Add(htc);
                    head = htc;
                }
            }
            return head;
        }

        protected void ScoreUp(Int32 _score)
        {
            Score += _score;

        }

        public Double GetScore(Boolean collectChildScore = true, Boolean applyWeight = true)
        {
            Double output = Score;
            if (applyWeight)
            {
                output = output * weight;
            }
            if (collectChildScore)
            {
                foreach (HtmlTagCounter child in children.Values)
                {
                    output += child.GetScore(collectChildScore, applyWeight);
                }
            }
            return output;
        }

        public void ScoreUp(String _tagName, Int32 _score = 1)
        {
            if (name == _tagName)
            {
                ScoreUp(_score);
                return;
            }
            if (tag == _tagName)
            {
                ScoreUp(_score);
                return;
            }
            if (alias.Contains(_tagName))
            {
                ScoreUp(_score);
            }
        }

        /// <summary>
        /// Defines the tags.
        /// </summary>
        /// <param name="_tags">The tags.</param>
        public void DefineTags(params String[] _tags)
        {
            foreach (String _t in _tags)
            {
                if (tag.isNullOrEmpty())
                {
                    tag = _t;
                    continue;
                }
                else if (!alias.Contains(_t))
                {
                    alias.Add(_t);
                }
            }
        }

        protected override bool doAutorenameOnExisting { get { return false; } }

        protected override bool doAutonameFromTypeName { get { return false; } }

        public override string pathSeparator { get { return "/"; } set { } }


        public string tag { get; set; } = "";
        public List<String> alias { get; set; } = new List<String>();

        public Int32 Score { get; set; } = 0;

        public Double weight { get; set; } = 1;

        //public Boolean Is

        public String description { get; set; } = "";

        public HtmlTagCounter(String _name, String _description, Double _weight)
        {
            name = _name;
            description = _description;
            weight = _weight;
        }

        public HtmlTagCounter(string _tag, string[] _alias)
        {
            name = _tag;
            tag = _tag;
            alias.AddRange(_alias);
        }
    }
}
