using System;
using System.Linq;
using System.Collections.Generic;
using imbNLP.Toolkit.Space;

namespace imbNLP.Toolkit.TopicModels.LDA
{

    public class LatentDirichletAllocation : TopicModelBase
    {
        public LatentDirichletAllocationSettings settings { get; set; } = new LatentDirichletAllocationSettings();

        public Int32[][] WordToTopic;

        public List<SpaceTopic> topics { get; set; } = new List<SpaceTopic>();

        public Int32[][] TopicAssigment;
        public Int32[][] dt;

        /// <summary>
        /// The theta - Document -> Topic distributions
        /// </summary>
        public double[][] theta; //Document -> Topic Distributions
        /// <summary>
        /// The phi - Topic -> Word distributions
        /// </summary>
        public double[][] phi; // Topic->Word Distributions

        public Dictionary<SpaceTopic, List<Int32>> TopicToWords { get; set; } = new Dictionary<SpaceTopic, List<int>>();

        public Dictionary<SpaceDocumentModel, Dictionary<SpaceTopic, Int32>> DocumentTopicCounts { get; set; } = new Dictionary<SpaceDocumentModel, Dictionary<SpaceTopic, int>>();

        public Random rnd { get; set; } = new Random();

        public Int32 TotalWordCount { get; set; } = 0;
        public Int32[] Words { get; set; }

        public override void PrepareTheModel(SpaceModel space)
        {

            WordToTopic = new int[settings.K][];
            dt = new Int32[space.documents.Count][];

            // creating topic instances
            for (int i = 0; i < settings.K; i++)
            {
                SpaceTopic topic = new SpaceTopic(i.ToString());
                topics.Add(topic);
                WordToTopic[i] = new Int32[space.terms_known_label.Count];
            }

            Int32 d = 0;
            foreach (SpaceDocumentModel document in space.documents)
            {
                Dictionary<SpaceTopic, Int32> topicCount = new Dictionary<SpaceTopic, int>();
                topics.ForEach(x => topicCount.Add(x, 0));
                DocumentTopicCounts.Add(document, topicCount);
                TotalWordCount += document.Length;
                dt[d] = new int[settings.K];
                d++;
            }

            Words = new int[TotalWordCount];

            // -------------

            TopicAssigment = new int[space.documents.Count][];

            d = 0;

            Int32 wgi = 0;
            TotalWordToTopic = new Int32[TotalWordCount];

            foreach (SpaceDocumentModel document in space.documents)
            {
                TopicAssigment[d] = new Int32[document.Length];


                for (int w = 0; w < document.Length; w++)
                {
                    Int32 topicID = rnd.Next(settings.K);
                    TotalWordToTopic[wgi] = topicID;
                    TopicAssigment[d][w] = topicID;

                    Int32 ti = TopicAssigment[d][w];
                    Int32 wi = document.Words[w];
                    TotalWordToDocument[wgi] = d;

                    WordToTopic[ti][wi]++;

                    Words[wgi] = document.Words[w];
                    wgi++;
                }


                for (int t = 0; t < settings.K; t++)
                {
                    Int32 sum = 0;
                    for (int wi = 0; wi < TopicAssigment[d].Length; wi++)
                    {
                        if (TopicAssigment[d][wi] == t)
                        {
                            sum++;
                        }
                    }
                    dt[d][t] = sum;
                }

                d++;
            }



            theta = new double[space.documents.Count][];
            for (int m = 0; m < space.documents.Count; m++)
            {
                theta[m] = new double[settings.K];
            }
            phi = new double[settings.K][];
            for (int k = 0; k < settings.K; k++)
            {
                phi[k] = new double[space.terms_known_label.Count];
            }


            for (int iter = 1; iter <= settings.iterations; iter++)
            {

                for (int i = 0; i < space.terms_known_label.Count; i++)
                {
                    int topic = DoSampling(i, space);
                    TotalWordToTopic[i] = topic;
                }
            }

        }

        protected int[] nwsum;
        protected int[] ndsum;

        public Int32[] TotalWordToTopic { get; set; }
        public Int32[] TotalWordToDocument { get; set; }

        protected double[] p;

        private int DoSampling(int i, SpaceModel space)
        {
            int oldZ = TotalWordToTopic[i];
            int w = Words[i]; // //words[i];
            int d = TotalWordToDocument[i];

            WordToTopic[w][oldZ] -= 1;
            dt[d][oldZ] -= 1;


            nwsum[oldZ] -= 1;
            ndsum[d] -= 1;

            double Vbeta = space.terms_known_label.Count * settings.eta; // Beta;
            double Kalpha = settings.K * settings.alpha; // Alpha;

            for (int k = 0; k < settings.K; k++)
            {
                p[k] = (WordToTopic[w][k] + settings.eta) / (nwsum[k] + Vbeta) * (dt[d][k] + settings.alpha) / (ndsum[d] + Kalpha);
            }
            for (int k = 1; k < settings.K; k++)
            {
                p[k] += p[k - 1];
            }
            Random rnd = new Random();
            double cp = rnd.NextDouble() * p[settings.K - 1];

            int newZ;
            for (newZ = 0; newZ < settings.K; newZ++)
            {
                if (p[newZ] > cp)
                {
                    break;
                }
            }
            if (newZ == settings.K) newZ--;
            WordToTopic[w][newZ] += 1;
            dt[d][newZ] += 1;
            nwsum[newZ] += 1;
            ndsum[d] += 1;
            return newZ;
        }

        protected void CalcParameter(SpaceModel space)
        {
            for (int m = 0; m < space.documents.Count; m++)
            {
                for (int k = 0; k < settings.K; k++)
                {
                    theta[m][k] = (dt[m][k] + settings.alpha) / (ndsum[m] + settings.K * settings.alpha);
                }
            }

            for (int k = 0; k < settings.K; k++)
            {
                for (int w = 0; w < space.terms_known_label.Count; w++)
                {
                    phi[k][w] = (WordToTopic[w][k] + settings.eta) / (nwsum[k] + space.terms_known_label.Count * settings.eta);
                }
            }
        }

        public void ProjectTopics(SpaceModel space)
        {
            for (int k = 0; k < settings.K; k++)
            {
                var wordsProbsList = new Dictionary<int, double>();

                for (int w = 0; w < space.terms_known_label.Count; w++)
                {
                    wordsProbsList.Add(w, phi[k][w]);
                }

                double ans = 0;
                for (int w = 0; w < space.terms_known_label.Count; w++)
                {
                    ans += phi[k][w];
                }
                if (Math.Abs(ans - 1.00) > 0.1)
                {
                    throw (new Exception("Phi Calculation Error"));
                }

                //sw.Write("Topic " + k + "th:\n");
                var wordsProbsListOrdered = wordsProbsList.OrderBy(e => -e.Value).ToList();

                //for (int i = 0; i < tw; i++)
                //{
                //    string word = cor.GetStringByID(wordsProbsListOrdered[i].Key);
                //    sw.WriteLine("\t" + word + " " + wordsProbsListOrdered[i].Value);
                //}
            }
        }

    }

}