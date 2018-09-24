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
    public interface ITextCached
    {
        String ToString();
        void FromString(String text);
    }

    public static class CacheServiceHelper
    {

        public static String GetDataSetSignature(this List<WebSiteDocumentsSet> dataset)
        {
            StringBuilder sb = new StringBuilder();
            foreach (WebSiteDocumentsSet ws in dataset)
            {
                sb.Append(ws.name + ":" + ws.Count);
            }
            return md5.GetMd5Hash(sb.ToString());
        }
    }

    public class CacheServiceProvider
    {
        public DirectoryInfo folder;

        public CacheServiceProvider()
        {

        }

        public void Deploy(DirectoryInfo _folder)
        {
            folder = _folder;
        }

        public CacheServiceProvider(DirectoryInfo _folder)
        {
            folder = _folder;
        }

        public Boolean IsReady
        {
            get
            {
                if (folder == null) return false;
                return true;
            }
        }

        private Object GetCacheLock = new Object();


        public String GetFilename(string setupSignature, string datasetSignature, string itemName, String typename)
        {
            String path = Path.Combine(typename, datasetSignature, setupSignature);
            path = path.add(itemName + "xml", ".");

            return path;

        }


        private static Object GetCachedLock = new Object();

        protected Dictionary<String, Byte[]> loaded { get; set; } = new Dictionary<string, Byte[]>();

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        public void Dispose()
        {
            loaded.Clear();
        }


        private static Object SetCachedLock = new Object();


        public void SetCached<T>(string setupSignature, string datasetSignature, string itemName, T item)
        {

            if (!IsReady) return;
            Type t = typeof(T);

            String path = GetFilename(setupSignature, datasetSignature, itemName, t.Name);
            String subpath = Path.Combine(folder.FullName, path);
            lock (SetCachedLock)
            {
                if (!loaded.ContainsKey(path))
                {
                    try
                    {
                        var fp = subpath.getWritableFile(imbSCI.Data.enums.getWritableFileMode.existing, null).FullName;

                        if (!File.Exists(subpath))
                        {

                            IFormatter formatter = new BinaryFormatter();
                            Stream stream = new FileStream(fp, FileMode.Create, FileAccess.Write, FileShare.None);
                            formatter.Serialize(stream, item);
                            stream.Close();

                        }

                        if (File.Exists(subpath))
                        {
                            var bt = File.ReadAllBytes(fp);
                            loaded.Add(path, bt);
                        }
                    }
                    catch (Exception ex)
                    {

                    }

                }

            }
        }


        public T GetCached<T>(string setupSignature, string datasetSignature, string itemName)
        {
            if (!IsReady) return default(T);

            Type t = typeof(T);
            String path = GetFilename(setupSignature, datasetSignature, itemName, t.Name);
            T output = default(T);

            lock (GetCachedLock)
            {

                if (loaded.ContainsKey(path))
                {
                    var bt = loaded[path];
                    IFormatter formatter = new BinaryFormatter();
                    if (bt.Length != 0)
                    {
                        Stream st = new MemoryStream(bt);
                        output = (T)formatter.Deserialize(st);
                        st.Close();
                    }
                    else
                    {
                        loaded.Remove(path);
                    }
                    //output = (T)formatter.Deserialize()
                    //stream.Close();
                    // output = objectSerialization.ObjectFromXML<T>(loaded[path]);
                }
                else
                {
                    String subpath = Path.Combine(folder.FullName, path);



                    if (File.Exists(subpath))
                    {
                        IFormatter formatter = new BinaryFormatter();
                        Stream stream = new FileStream(subpath, FileMode.Open, FileAccess.Read, FileShare.Read);
                        if (stream.Length == 0)
                        {

                        }
                        else
                        {
                            try
                            {
                                output = (T)formatter.Deserialize(stream);
                                stream.Close();

                                //String xml = File.ReadAllText(subpath);
                                //output = new T();
                                //output.FromString(xml);

                                var bt = File.ReadAllBytes(subpath);
                                loaded.Add(path, bt);
                            }
                            catch (Exception ex)
                            {

                            }
                        }
                    }
                }

            }

            return output;
        }

    }

}