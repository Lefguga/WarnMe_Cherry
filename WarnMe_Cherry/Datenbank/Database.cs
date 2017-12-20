using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarnMe_Cherry.Datenbank
{
    class Database
    {
        FileInfo source;
        Dictionary<string, Dictionary<string, Object>> Library = new Dictionary<string, Dictionary<string, Object>>();

        Encoding Encoding = Encoding.UTF8;

        public Database(string Filename)
        {
            source = new FileInfo(Filename);

            if (!source.Exists)
            {
                source.Create().Close();
            }
            else
            {
                string sourceString;
                using (StreamReader sr = new StreamReader(source.OpenRead(), Encoding))
                {
                    sourceString = sr.ReadToEnd();
                    sr.Close();
                }
                Deserialize(sourceString);
            }
        }

        /// <summary>
        /// ______INSERT______
        /// Insert a value to the database, if key already exists it throws an exception
        /// </summary>
        /// <param name="elementValue"></param>
        /// <param name="elementName"></param>
        public void Insert(string tableName, string elementName, Object elementValue)
        {
            if (!Library.ContainsKey(tableName))
                Library.Add(tableName, new Dictionary<string, Object>());
            if (Library[tableName].ContainsKey(elementName))
            {
                throw new InvalidDataException("Element bereits in der Datenbank vorhanden, bitte wählen Sie einen Anderen Namen oder verwenden Sie die \"Update()\" Methode.");
            }
            else
            {
                Library[tableName].Add(elementName, elementValue);
            }
        }

        /// <summary>
        /// ______UPDATE______
        /// 
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="elementName"></param>
        /// <param name="elementValue"></param>
        public void Update(string tableName, string elementName, Object elementValue)
        {
            // check if key already exists
            if (!Library.ContainsKey(tableName))
            {
                Library.Add(tableName, new Dictionary<string, Object>());
            }
                if (!Library[tableName].ContainsKey(elementName))
                {
                    Library[tableName].Add(elementName, elementValue);
                }
                else
                {
                    Library[tableName][elementName] = elementValue;
                }
        }

        /// <summary>
        /// ______SELECT______
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tableName"></param>
        /// <param name="elementName"></param>
        /// <returns></returns>
        public Object Select<T>(string tableName, string elementName)
        {
            // check if key already exists
            if (!Library.ContainsKey(tableName) || !Library[tableName].ContainsKey(elementName))
            {
                throw new KeyNotFoundException("Der Wert konnte im Pfad nicht gefunden werden.");
            }
            return (T)Library[tableName][elementName];
        }

        /// <summary>
        /// ______EXISTS______
        /// 
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public bool Exists(string tableName)
        {
            // check if key already exists
            if (!Library.ContainsKey(tableName))
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// ______EXISTS______
        /// 
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="elementName"></param>
        /// <returns></returns>
        public bool Exists(string tableName, string elementName)
        {
            // check if key already exists
            return (Library.ContainsKey(tableName) && Library[tableName].ContainsKey(elementName));
        }

        /// <summary>
        /// ______DELETE______
        /// 
        /// </summary>
        /// <param name="tableName"></param>
        public void Delete(string tableName)
        {
            if (Library.ContainsKey(tableName))
            {
                Library.Remove(tableName);
            }
        }

        /// <summary>
        /// ______DELETE______
        /// 
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="elementName"></param>
        public void Delete(string tableName, string elementName)
        {
            if (Library.ContainsKey(tableName))
            {
                if (Library[tableName].ContainsKey(elementName))
                {
                    Library[tableName].Remove(elementName);
                }
            }
        }

        /// <summary>
        /// ______COMMIT______
        /// </summary>
        internal void Commit()
        {
            using (StreamWriter sw = new StreamWriter(source.OpenWrite(), Encoding))
            {
                sw.Write(Serialize());
                sw.Close();
            }
        }

        /// <summary>
        /// ______SERIALIZE______
        /// </summary>
        /// <returns></returns>
        string Serialize()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(Library, new Newtonsoft.Json.JsonSerializerSettings()
            {
                Formatting = Newtonsoft.Json.Formatting.Indented,
                TypeNameHandling = Newtonsoft.Json.TypeNameHandling.None
            });
        }

        /// <summary>
        /// ______DESERIALIZE______
        /// </summary>
        /// <param name="data"></param>
        void Deserialize(string data)
        {
            Library = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, Object>>>(data, new Newtonsoft.Json.JsonSerializerSettings()
            {
                TypeNameHandling = Newtonsoft.Json.TypeNameHandling.None
            }) ?? new Dictionary<string, Dictionary<string, Object>>();
        }
    }
}
