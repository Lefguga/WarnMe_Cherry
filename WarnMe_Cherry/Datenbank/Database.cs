using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace WarnMe_Cherry.Datenbank
{
    class Datenbank
    {
        public static Newtonsoft.Json.JsonSerializerSettings DefaultSetting = new Newtonsoft.Json.JsonSerializerSettings()
        {
            TypeNameHandling = Newtonsoft.Json.TypeNameHandling.None,
            DateFormatString = "dd.MM.yyyy HH:mm:ss.ffff"
        };

        FileInfo source;
        Dictionary<string, Dictionary<string, Object>> Library = new Dictionary<string, Dictionary<string, Object>>();
        readonly Encoding Encoding = Encoding.UTF8;

        string LastCommittedString = "";
        public bool HasUncommitedChanges => Serialize().Equals(LastCommittedString); //{ get => (LastCommittedString != null && LastCommittedString.Equals(Serialize())); }

        public Datenbank(string Filename)
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
                LastCommittedString = sourceString;
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
#if DEBUG
            Console.WriteLine("LOG: Updated Value [{0}, {1}] = [({3}){2}]", tableName, elementName, elementValue, elementValue.GetType());
#endif
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
        public Dictionary<string, T> Select<T>(string tableName)
        {
            // check if key exists
            if (!Library.ContainsKey(tableName))
            {
                throw new KeyNotFoundException("Der Wert konnte im Pfad nicht gefunden werden.");
            }
            return Library[tableName].ToDictionary(item => item.Key, item => Select<T>(tableName, item.Key));
        }

        public T Select<T>(string tableName, string elementName)
        {
            // check if key exists
            if (!Exists(tableName, elementName))
            {
                throw new KeyNotFoundException("Der Wert konnte im Pfad nicht gefunden werden.");
            }
            //try
            //{
            //    Console.WriteLine("Log: {0} [{1}, {2}]\nVALUE:{3}\n", typeof(T).ToString(), tableName, elementName, Library[tableName][elementName]);
            //    return (T)Library[tableName][elementName];
            //}
            //catch (InvalidCastException) // Some custom objects cant deserialized correctly and stored as objects. These must deserialized explictly
            //{
#if DEBUG
            Console.WriteLine("LOG: Requested Type [{0}] existing Type [{1}]", Library[tableName][elementName].GetType(), typeof(T));
#endif
            if (typeof(T) == Library[tableName][elementName].GetType())
                return (T)Library[tableName][elementName];
            else
                return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(Library[tableName][elementName].ToString());
            //}
        }

        public bool TrySelect<T>(string tableName, string elementName, out T returnVal)
        {
            // check if key exists
            if (!Exists(tableName, elementName))
            {
                returnVal = default(T);
                return false;
            }
#if DEBUG
            Console.WriteLine("LOG: Try Requested Type [{0}] existing Type [{1}]", Library[tableName][elementName].GetType(), typeof(T));
#endif
            if (typeof(T) == Library[tableName][elementName].GetType())
                returnVal = (T)Library[tableName][elementName];
            else
                returnVal = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(Library[tableName][elementName].ToString());
            return true;
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
            string toWrite = Serialize();
            try
            {
                using (StreamWriter sw = new StreamWriter(source.OpenWrite(), Encoding))
                {
                    sw.Write(toWrite);
                    sw.Close();
                }
            }
            catch (IOException)
            {
                return;
            }
            finally
            {
                LastCommittedString = toWrite;
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
                TypeNameHandling = Newtonsoft.Json.TypeNameHandling.None,
                DateFormatString = "dd.MM.yyyy HH:mm:ss.ffff"
            });
        }

        /// <summary>
        /// ______DESERIALIZE______
        /// </summary>
        /// <param name="data"></param>
        void Deserialize(string data)
        {
            Library = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, Object>>>(data, DefaultSetting) ?? new Dictionary<string, Dictionary<string, Object>>();
        }
    }
}
