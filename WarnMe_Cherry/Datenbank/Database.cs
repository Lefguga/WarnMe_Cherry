using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using static WarnMe_Cherry.Global;

namespace WarnMe_Cherry.Datenbank
{
    class Datenbank
    {
        public static JsonSerializerSettings DefaultSetting = new JsonSerializerSettings()
        {
            Formatting = Formatting.Indented,
            TypeNameHandling = TypeNameHandling.None,
            DateFormatString = "dd.MM.yyyy"
        };


        FileInfo source;
#if GEN_LIBRARY
        JObject Library = new JObject();
#else
        public THIS THIS = new THIS();
#endif
        readonly Encoding Encoding = Encoding.UTF8;

        string LastCommittedString = "";
        public bool HasUncommitedChanges => Serialize().Equals(LastCommittedString); //{ get => (LastCommittedString != null && LastCommittedString.Equals(Serialize())); }

        public Datenbank(string Filename)
        {
#if TRACE
            INFO($"Generating Datenbank from file '{Filename}'");
#endif
            source = new FileInfo(Filename);

            if (!source.Exists)
            {
                source.Create().Close();
            }
            else
            {
                string sourceString = File.ReadAllText(source.FullName, Encoding);
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
        public void Insert(string tableName, string elementName, object elementValue)
        {
#if TRACE
            INFO($"Database.Insert Value in {tableName}, {elementName} with ){elementValue}]{elementValue.GetType()}");
#endif
#if GEN_LIBRARY
            if (!Library.ContainsKey(tableName))
                Library.Add(tableName, new JObject());
            if (((JObject)Library[tableName]).ContainsKey(elementName))
            {
                throw new InvalidDataException("Element bereits in der Datenbank vorhanden, bitte wählen Sie einen Anderen Namen oder verwenden Sie die \"Update()\" Methode.");
            }
            else
            {
                ((JObject)Library[tableName]).Add(elementName, JToken.FromObject(elementValue));
            }
#else
            throw new NotImplementedException("Generic Library not availabel.");
#endif
        }

        /// <summary>
        /// ______UPDATE______
        /// 
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="elementName"></param>
        /// <param name="elementValue"></param>
        public void Update(string tableName, string elementName, object elementValue)
        {
#if TRACE
            INFO($"Database.Update Value in {tableName}, {elementName} to {elementValue} as {elementValue.GetType()}");
#endif
#if GEN_LIBRARY
            // check if key already exists
            if (!Library.ContainsKey(tableName))
            {
                Library.Add(tableName, new JObject());
            }
            if (!((JObject)Library[tableName]).ContainsKey(elementName))
            {
                ((JObject)Library[tableName]).Add(elementName, JToken.FromObject(elementValue));
            }
            else
            {
                Library[tableName][elementName] = JToken.FromObject(elementValue);
            }
#else
            throw new NotImplementedException("Generic Library not availabel.");
#endif
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
#if TRACE
            INFO($"Database.Requested List of Type [{typeof(T)}]");
#endif
#if GEN_LIBRARY
            // check if key exists
            if (!Library.ContainsKey(tableName))
            {
                throw new KeyNotFoundException("Der Wert konnte im Pfad nicht gefunden werden.");
            }
            return (Dictionary<string, T>)Library[tableName].ToObject(typeof(Dictionary<string, T>));
#else
            throw new NotImplementedException("Generic Library not availabel.");
#endif
        }

        public T Select<T>(string tableName, string elementName)
        {
#if TRACE
            INFO($"Database.Requested Type [{typeof(T)}] in Table [{tableName}] with Key [{elementName}].");
#endif
#if GEN_LIBRARY
            // check if key exists
            if (!Exists(tableName, elementName))
            {
                throw new KeyNotFoundException("Der Wert konnte im Pfad nicht gefunden werden.");
            }

            return (T)Library[tableName][elementName].ToObject(typeof(T));
            //}
#else
            throw new NotImplementedException("Generic Library not availabel.");
#endif
        }

        public bool TrySelect<T>(string tableName, string elementName, out T returnVal)
        {
#if TRACE
            INFO($"Database.TrySelect Type [{typeof(T)}] in Library[{tableName}][{elementName}]");
#endif
#if GEN_LIBRARY
            // check if key exists
            if (!Exists(tableName, elementName))
            {
                returnVal = default;
                return false;
            }
            returnVal = (T)Library[tableName][elementName].ToObject(typeof(T));
            return true;
#else
            throw new NotImplementedException("Generic Library not availabel.");
#endif
        }

        /// <summary>
        /// ______EXISTS______
        /// 
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public bool Exists(string tableName)
        {
#if TRACE
            INFO($"Database.Exitsts check for {tableName}");
#endif
#if GEN_LIBRARY
            // check if key already exists
            if (!Library.ContainsKey(tableName))
            {
                return false;
            }
            return true;
#else
            throw new NotImplementedException("Generic Library not availabel.");
#endif
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
#if TRACE
            INFO($"Database.Exitsts check for {tableName}, {elementName}");
#endif
#if GEN_LIBRARY
            // check if key already exists
            return Library.ContainsKey(tableName) && ((JObject)Library[tableName]).ContainsKey(elementName);
#else
            throw new NotImplementedException("Generic Library not availabel.");
#endif
        }

        /// <summary>
        /// ______DELETE______
        /// 
        /// </summary>
        /// <param name="tableName"></param>
        public void Delete(string tableName)
        {
#if TRACE
            INFO($"Database.Delete data in {tableName}");
#endif
#if GEN_LIBRARY
            if (Library.ContainsKey(tableName))
            {
#if DEBUG
                    INFO($"Deleting data in Library[{tableName}]");
#endif
                Library.Remove(tableName);
            }
#else
            throw new NotImplementedException("Generic Library not availabel.");
#endif
        }

        /// <summary>
        /// ______DELETE______
        /// 
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="elementName"></param>
        public void Delete(string tableName, string elementName)
        {
#if TRACE
            INFO($"Database.Delete data in {tableName}, {elementName}");
#endif
#if GEN_LIBRARY
            if (Library.ContainsKey(tableName))
            {
                if (((JObject)Library[tableName]).ContainsKey(elementName))
                {
#if DEBUG
                    INFO($"Deleting data in Library[{tableName}][{elementName}]");
#endif
                    ((JObject)Library[tableName]).Remove(elementName);
                }
            }
#else
            throw new NotImplementedException("Generic Library not availabel.");
#endif
        }

        /// <summary>
        /// ______COMMIT______
        /// </summary>
        internal void Commit()
        {
#if TRACE
            INFO($"Database.Commit Writing data to '{source.FullName}'");
#endif
            string toWrite = Serialize();
            try
            {
                File.WriteAllText($"{source.FullName}_bak", toWrite, Encoding);
                File.Delete(source.FullName);
                File.Move($"{source.FullName}_bak", source.FullName);
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
#if TRACE
            INFO("Database.Serialize data");
#endif
            return JsonConvert.SerializeObject(THIS, DefaultSetting);
        }

        /// <summary>
        /// ______DESERIALIZE______
        /// </summary>
        /// <param name="data"></param>
        void Deserialize(string data)
        {
#if TRACE
            INFO("Database.Deserialize data");
#endif
            THIS = JsonConvert.DeserializeObject<THIS>(data, DefaultSetting);
        }
    }
}
