using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Editors
{
    public static class FileHandler
    {
        /// <summary>
        /// Save a list of items as a JSON file.
        /// </summary>
        public static void SaveListToJson<T>(List<T> itemsToSave, string fileName)
        {
            string jsonContent = JsonHelper.ToJson<T>(itemsToSave.ToArray());
            WriteFile(GetPath(fileName), jsonContent);
        }

        /// <summary>
        /// Save a single item of given type as a JSON file.
        /// </summary>
        public static void SaveToJson<T>(T itemToSave, string fileName)
        {
            string jsonContent = JsonUtility.ToJson(itemToSave);
            WriteFile(GetPath(fileName), jsonContent);
        }
    
        /// <summary>
        /// Read a list of items from a JSON file.
        /// </summary>
        public static List<T> ReadListFromJson<T>(string fileName)
        {
            string jsonContent = ReadFile(GetPath(fileName));

            if (string.IsNullOrEmpty(jsonContent) || jsonContent == "{}")
                return new List<T>();

            List<T> result = JsonHelper.FromJson<T>(jsonContent).ToList();
            return result;
        }
    
        /// <summary>
        /// Read a single item of desired type from a JSON file.
        /// </summary>
        public static T ReadFromJson<T>(string fileName)
        {
            string jsonContent = ReadFile(GetPath(fileName));

            if (string.IsNullOrEmpty(jsonContent) || jsonContent == "{}")
                return default(T);

            T result = JsonUtility.FromJson<T>(jsonContent);
            return result;
        }

        /// <summary>
        /// Get the full path for a file in the persistent data path.
        /// </summary>
        private static string GetPath(string fileName)
        {
            return Application.persistentDataPath + "/" + fileName;
        }

        /// <summary>
        /// Write content to a file at the specified path.
        /// </summary>
        private static void WriteFile(string path, string content)
        {
            FileStream fileStream = new FileStream(path, FileMode.Create);

            using (StreamWriter writer = new StreamWriter(fileStream))
            {
                writer.Write(content);
            }
        
            fileStream.Close();
        }

        /// <summary>
        /// Read the content of a file from the specified path.
        /// </summary>
        private static string ReadFile(string path)
        {
            if (File.Exists(path))
            {
                using (StreamReader reader = new StreamReader(path))
                {
                    string content = reader.ReadToEnd();
                    return content;
                }
            }

            return "";
        }
    }

    public static class JsonHelper
    {
        /// <summary>
        /// Convert JSON string to an array of objects of the specified type.
        /// </summary>
        public static T[] FromJson<T>(string json)
        {
            Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(json);
            return wrapper.items;
        }

        /// <summary>
        /// Convert an array of objects of the specified type to JSON string.
        /// </summary>
        public static string ToJson<T>(T[] array)
        {
            Wrapper<T> wrapper = new Wrapper<T>();
            wrapper.items = array;
            return JsonUtility.ToJson(wrapper);
        }

        /// <summary>
        /// Convert an array of objects of the specified type to JSON string with optional formatting.
        /// </summary>
        public static string ToJson<T>(T[] array, bool prettyPrint)
        {
            Wrapper<T> wrapper = new Wrapper<T>();
            wrapper.items = array;
            return JsonUtility.ToJson(wrapper, prettyPrint);
        }

        [Serializable]
        private class Wrapper<T>
        {
            public T[] items;
        }
    }
}