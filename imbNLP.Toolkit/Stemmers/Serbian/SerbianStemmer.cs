using System;
using System.Collections.Generic;




namespace imbNLP.Toolkit.Stemmers.Serbian
{
    /// <summary>
    /// Ported from original pyton source: Milosevic, N. (2012). [Stemmer for Serbian language](http://arxiv.org/abs/1209.4471). arXiv preprint arXiv:1209.4471.
    /// </summary>
    /// <remarks>
    /// <para>Stemmer for Serbian language created for my master thesis, rewritten in python. It is improvement of Keselj and Šipla's stemmer.</para>
    /// </remarks>
    public class SerbianStemmer : IStemmer
    {

        public SerbianStemmer()
        {

        }

        private static Object _suffixReplacement_lock = new Object();
        private static Dictionary<String, String> _suffixReplacement;
        /// <summary>
        /// Suffix rules
        /// </summary>
        public static Dictionary<String, String> suffixReplacement
        {
            get
            {
                if (_suffixReplacement == null)
                {
                    lock (_suffixReplacement_lock)
                    {

                        if (_suffixReplacement == null)
                        {
                            _suffixReplacement = new Dictionary<String, String>();

                            String rules = @"'ovnicxki':'',
'ovnicxka':'',
'ovnika':'',
'ovniku':'',
'ovnicxe':'',
'kujemo':'',
'ovacyu':'',
'ivacyu':'',
'isacyu':'',
'dosmo':'',
'ujemo':'',
'ijemo':'',
'ovski':'',
'ajucxi':'',
'icizma':'',
'ovima':'',
'ovnik':'',
'ognu':'',
'inju':'',
'enju':'',
'cxicyu':'',
'sxtva':'',
'ivao':'',
'ivala':'',
'ivalo':'',
'skog':'',
'ucxit':'',
'ujesx':'',
'ucyesx':'',
'ocyesx':'',
'osmo':'',
'ovao':'',
'ovala':'',
'ovali':'',
'ismo':'',
'ujem':'',
'esmo':'',
'asmo':'', #pravi gresku kod pevasmo
'zxemo':'',
'cyemo':'',
'cyemo':'',
'bemo':'',
'ovan':'',
'ivan':'',
'isan':'',
'uvsxi':'',
'ivsxi':'',
'evsxi':'',
'avsxi':'',
'sxucyi':'',
'uste':'',
'icxe':'i',#bilo ik
'acxe':'ak',
'uzxe':'ug',
'azxe':'ag',# mozda treba az, pokazati, pokazxe
'aci':'ak',
'oste':'',
'aca':'',
'enu':'',
'enom':'',
'enima':'',
'eta':'',
'etu':'',
'etom':'',
'adi':'',
'alja':'',
'nju':'nj',
'lju':'',
'lja':'',
'lji':'',
'lje':'',
'ljom':'',
'ljama':'',
'zi':'g',
'etima':'',
'ac':'',
'becyi':'beg',
'nem':'',
'nesx':'',
'ne':'',
'nemo':'',
'nimo':'',
'nite':'',
'nete':'',
'nu':'',
'ce':'',
'ci':'',
'cu':'',
'ca':'',
'cem':'',
'cima':'',
'sxcyu':'s',
'ara':'r',
'iste':'',
'este':'',
'aste':'',
'ujte':'',
'jete':'',
'jemo':'',
'jem':'',
'jesx':'',
'ijte':'',
'inje':'',
'anje':'',
'acxki':'',
'anje':'',
'inja':'',
'cima':'',
'alja':'',
'etu':'',
'nog':'',
'omu':'',
'emu':'',
'uju':'',
'iju':'',
'sko':'',
'eju':'',
'ahu':'',
'ucyu':'',
'icyu':'',
'ecyu':'',
'acyu':'',
'ocu':'',
'izi':'ig',
'ici':'ik',
'tko':'d',
'tka':'d',
'ast':'',
'tit':'',
'nusx':'',
'cyesx':'',
'cxno':'',
'cxni':'',
'cxna':'',
'uto':'',
'oro':'',
'eno':'',
'ano':'',
'umo':'',
'smo':'',
'imo':'',
'emo':'',
'ulo':'',
'sxlo':'',
'slo':'',
'ila':'',
'ilo':'',
'ski':'',
'ska':'',
'elo':'',
'njo':'',
'ovi':'',
'evi':'',
'uti':'',
'iti':'',
'eti':'',
'ati':'',
'vsxi':'',
'vsxi':'',
'ili':'',
'eli':'',
'ali':'',
'uji':'',
'nji':'',
'ucyi':'',
'sxcyi':'',
'ecyi':'',
'ucxi':'',
'oci':'',
'ove':'',
'eve':'',
'ute':'',
'ste':'',
'nte':'',
'kte':'',
'jte':'',
'ite':'',
'ete':'',
'cyi':'',
'usxe':'',
'esxe':'',
'asxe':'',
'une':'',
'ene':'',
'ule':'',
'ile':'',
'ele':'',
'ale':'',
'uke':'',
'tke':'',
'ske':'',
'uje':'',
'tje':'',
'ucye':'',
'sxcye':'',
'icye':'',
'ecye':'',
'ucxe':'',
'oce':'',
'ova':'',
'eva':'',
'ava':'av',
'uta':'',
'ata':'',
'ena':'',
'ima':'',
'ama':'',
'ela':'',
'ala':'',
'aka':'',
'aja':'',
'jmo':'',
'oga':'',
'ega':'',
'aća':'',
'oca':'',
'aba':'',
'cxki':'',
'ju':'',
'hu':'',
'cyu':'',
'cu':'',
'ut':'',
'it':'',
'et':'',
'at':'',
'usx':'',
'isx':'',
'esx':'',
'esx':'',
'uo':'',
'no':'',
'mo':'',
'mo':'',
'lo':'',
'ko':'',
'io':'',
'eo':'',
'ao':'',
'un':'',
'an':'',
'om':'',
'ni':'',
'im':'',
'em':'',
'uk':'',
'uj':'',
'oj':'',
'li':'',
'ci':'',
'uh':'',
'oh':'',
'ih':'',
'eh':'',
'ah':'',
'og':'',
'eg':'',
'te':'',
'sxe':'',
'le':'',
'ke':'',
'ko':'',
'ka':'',
'ti':'',
'he':'',
'cye':'',
'cxe':'',
'ad':'',
'ecy':'',
'ac':'',
'na':'',
'ma':'',
'ul':'',
'ku':'',
'la':'',
'nj':'nj',
'lj':'lj',
'ha':'',
'a':'',
'e':'',
'u':'',
'sx':'',
'o':'',
'i':'',
'j':'',
'i':''";

                            _suffixReplacement.SetRules(rules);
                        }
                    }
                }
                return _suffixReplacement;
            }
        }


        private static Object _modalVerbs_lock = new Object();
        private static Dictionary<String, String> _modalVerbs;
        /// <summary>
        /// Modal verbs
        /// </summary>
        public static Dictionary<String, String> modalVerbs
        {
            get
            {
                if (_modalVerbs == null)
                {
                    lock (_modalVerbs_lock)
                    {

                        if (_modalVerbs == null)
                        {
                            _modalVerbs = new Dictionary<String, String>();
                            String verbs = @"'bih':'biti',
'bi':'biti',
'bismo':'biti',
'biste':'biti',
'bisxe':'biti',
'budem':'biti',
'budesx':'biti',
'bude':'biti',
'budemo':'biti',
'budete':'biti',
'budu':'biti',
'bio':'biti',
'bila':'biti',
'bili':'biti',
'bile':'biti',
'biti':'biti',
'bijah':'biti',
'bijasxe':'biti',
'bijasmo':'biti',
'bijaste':'biti',
'bijahu':'biti',
'besxe':'biti',
'sam':'jesam',
'si':'jesam',
'je':'jesam',
'smo':'jesam',
'ste':'jesam',
'su':'jesam',
'jesam':'jesam',
'jesi':'jesam',
'jeste':'jesam',
'jesmo':'jesam',
'jeste':'jesam',
'jesu':'jesam',
'cyu':'hteti',
'cyesx':'hteti',
'cye':'hteti',
'cyemo':'hteti',
'cyete':'hteti',
'hocyu':'hteti',
'hocyesx':'hteti',
'hocye':'hteti',
'hocyemo':'hteti',
'hocyete':'hteti',
'hocye':'hteti',
'hteo':'hteti',
'htela':'hteti',
'hteli':'hteti',
'htelo':'hteti',
'htele':'hteti',
'htedoh':'hteti',
'htede':'hteti',
'htede':'hteti',
'htedosmo':'hteti',
'htedoste':'hteti',
'htedosxe':'hteti',
'hteh':'hteti',
'hteti':'hteti',
'htejucyi':'hteti',
'htevsxi':'hteti',
'mogu':'mocyi',
'možeš':'mocyi',
'može':'mocyi',
'možemo':'mocyi',
'možete':'mocyi',
'mogao':'mocyi',
'mogli':'mocyi',
'moći':'mocyi'
";
                            _modalVerbs.SetRules(verbs);
                            // add here if any additional initialization code is required
                        }
                    }
                }
                return _modalVerbs;
            }
        }


        private static Object _lowEncoding_lock = new Object();
        private static Dictionary<String, String> _lowEncoding;
        /// <summary>
        /// Low-encoding
        /// </summary>
        public static Dictionary<String, String> lowEncoding
        {
            get
            {
                if (_lowEncoding == null)
                {
                    lock (_lowEncoding_lock)
                    {

                        if (_lowEncoding == null)
                        {
                            _lowEncoding = new Dictionary<String, String>();
                            _lowEncoding.Add("š", "sx");
                            _lowEncoding.Add("č", "cx");
                            _lowEncoding.Add("ć", "cy");
                            _lowEncoding.Add("đ", "dx");
                            _lowEncoding.Add("ž", "zx");
                            // add here if any additional initialization code is required
                        }
                    }
                }
                return _lowEncoding;
            }
        }



        public Boolean Stem()
        {
            current = current.ToLower();
            foreach (var lowPair in lowEncoding)
            {
                current = current.Replace(lowPair.Key, lowPair.Value);
            }
            Boolean suffixFound = false;
            foreach (var sufPair in suffixReplacement)
            {
                if (current.EndsWith(sufPair.Key))
                {
                    suffixFound = true;
                    Int32 l = sufPair.Key.Length;
                    current = current.Substring(0, current.Length - l);
                    current += sufPair.Value;
                    break;
                }
            }
            if (!suffixFound)
            {
                foreach (var mv in modalVerbs)
                {
                    if (current == mv.Key)
                    {
                        current = mv.Value;
                        break;
                    }
                }
            }

            return true;
        }

        protected String current { get; set; } = "";

        public void SetCurrent(String input)
        {
            current = input;

        }

        public String GetCurrent()
        {
            return current;
        }
    }
}
