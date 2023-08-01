using System.Collections.Generic;
using System.IO;
using System;
using System.Linq;
using UnityEngine;

public static class FileHandler
{
    /// <summary>
    /// Saves the list as a json file.
    /// </summary>
    /// <returns></returns>
    public static void SaveListToJson<T>(List<T> toSave, string fileName)
    {
        string content = JsonHelper.ToJson<T>(toSave.ToArray());
        WriteFile(GetPath(fileName), content);
    }

    /// <summary>
    /// Saves the variable with the given type as a json file.
    /// </summary>
    /// <returns></returns>
    public static void SaveToJson<T>(T toSave, string fileName)
    {
        string content = JsonUtility.ToJson(toSave);
        WriteFile(GetPath(fileName), content);
    }
    
    /// <summary>
    /// returns a list with the given file name.
    /// </summary>
    /// <returns></returns>
    public static List<T> ReadListFromJson<T>(string fileName)
    {
        string content = ReadFile(GetPath(fileName));

        if (string.IsNullOrEmpty(content) || content == "{}") return new List<T>();

        List<T> res = JsonHelper.FromJson<T>(content).ToList();
        return res;
    }
    
    /// <summary>
    /// returns a variable with the desired type.
    /// </summary>
    /// <returns></returns>
    public static T ReadFromJson<T>(string fileName)
    {
        string content = ReadFile(GetPath(fileName));

        if (string.IsNullOrEmpty(content) || content == "{}") return default(T);

        T res = JsonUtility.FromJson<T>(content);
        return res;
    }

    /// <summary>
    /// Returns the save path as a string.
    /// </summary>
    /// <returns></returns>
    private static string GetPath(string fileName)
    {
        return Application.persistentDataPath + "/" + fileName;
    }

    /// <summary>
    /// Saves the file into the given path.
    /// </summary>
    /// <returns></returns>
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
    /// Returns the file as a string from the given path.
    /// </summary>
    /// <returns></returns>
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
    public static T[] FromJson<T>(string json)
    {
        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(json);
        return wrapper.Items;
    }

    public static string ToJson<T>(T[] array)
    {
        Wrapper<T> wrapper = new Wrapper<T>();
        wrapper.Items = array;
        return JsonUtility.ToJson(wrapper);
    }

    public static string ToJson<T>(T[] array, bool prettyPrint)
    {
        Wrapper<T> wrapper = new Wrapper<T>();
        wrapper.Items = array;
        return JsonUtility.ToJson(wrapper, prettyPrint);
    }

    [Serializable]
    private class Wrapper<T>
    {
        public T[] Items;
    }
}
