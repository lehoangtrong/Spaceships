using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public static class FileHandler {

    //public static void SaveToJSON<T> (List<T> toSave, string filename) {
    //    Debug.Log (GetPath (filename));
    //    string content = JsonHelper.ToJson<T> (toSave.ToArray ());
    //    WriteFile (GetPath (filename), content);
    //}

    //public static void SaveToJSON<T> (T toSave, string filename) {
    //    string content = JsonUtility.ToJson (toSave);
    //    WriteFile (GetPath (filename), content);
    //}

    public static void SaveToJSON<T>(List<T> toSave, string filename)
    {
        if (string.IsNullOrEmpty(filename))
        {
            Debug.LogError("Filename is null or empty!");
            return;
        }

        try
        {
            string path = GetPath(filename);
            Debug.Log($"Saving to path: {path}");
            string content = JsonHelper.ToJson<T>(toSave.ToArray());
            WriteFile(path, content);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to save JSON file '{filename}': {e.Message}");
        }
    }

    public static void SaveToJSON<T>(T toSave, string filename)
    {
        if (string.IsNullOrEmpty(filename))
        {
            Debug.LogError("Filename is null or empty!");
            return;
        }

        try
        {
            string content = JsonUtility.ToJson(toSave);
            WriteFile(GetPath(filename), content);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to save JSON file '{filename}': {e.Message}");
        }
    }

    //public static List<T> ReadListFromJSON<T> (string filename) {
    //    string content = ReadFile (GetPath (filename));

    //    if (string.IsNullOrEmpty (content) || content == "{}") {
    //        return new List<T> ();
    //    }

    //    List<T> res = JsonHelper.FromJson<T> (content).ToList ();

    //    return res;

    //}

    public static List<T> ReadListFromJSON<T>(string filename)
    {
        if (string.IsNullOrEmpty(filename))
        {
            Debug.LogError("Filename is null or empty!");
            return new List<T>();
        }

        try
        {
            string content = ReadFile(GetPath(filename));

            if (string.IsNullOrEmpty(content) || content == "{}")
            {
                return new List<T>();
            }

            List<T> res = JsonHelper.FromJson<T>(content).ToList();
            return res;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to read JSON file '{filename}': {e.Message}");
            return new List<T>();
        }
    }

    //public static T ReadFromJSON<T> (string filename) {
    //    string content = ReadFile (GetPath (filename));

    //    if (string.IsNullOrEmpty (content) || content == "{}") {
    //        return default (T);
    //    }

    //    T res = JsonUtility.FromJson<T> (content);

    //    return res;

    //}

    public static T ReadFromJSON<T>(string filename)
    {
        if (string.IsNullOrEmpty(filename))
        {
            Debug.LogError("Filename is null or empty!");
            return default(T);
        }

        try
        {
            string content = ReadFile(GetPath(filename));

            if (string.IsNullOrEmpty(content) || content == "{}")
            {
                return default(T);
            }

            T res = JsonUtility.FromJson<T>(content);
            return res;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to read JSON file '{filename}': {e.Message}");
            return default(T);
        }
    }

    //private static string GetPath (string filename) {
    //    return Path.Combine(Application.persistentDataPath, filename);
    //}

    private static string GetPath(string filename)
    {
        return @$"D:\\{filename}";
    }

    //private static string GetPath(string filename)
    //{
    //    if (string.IsNullOrEmpty(filename))
    //    {
    //        Debug.LogError("Cannot create path: filename is null or empty!");
    //        return "";
    //    }

    //    string path = Path.Combine(Application.persistentDataPath, filename).Replace('\\', '/');

    //    // ✅ Tạo thư mục nếu chưa tồn tại
    //    string directory = Path.GetDirectoryName(path);
    //    if (!Directory.Exists(directory))
    //    {
    //        Directory.CreateDirectory(directory);
    //        Debug.Log($"Created directory: {directory}");
    //    }

    //    return path;
    //}

    //private static void WriteFile (string path, string content) {
    //    FileStream fileStream = new FileStream (path, FileMode.Create);

    //    using (StreamWriter writer = new StreamWriter (fileStream)) {
    //        writer.Write (content);
    //    }
    //}

    private static void WriteFile(string path, string content)
    {
        if (string.IsNullOrEmpty(path))
        {
            Debug.LogError("Cannot write file: path is null or empty!");
            return;
        }

        try
        {
            FileStream fileStream = new FileStream(path, FileMode.Create);

            using (StreamWriter writer = new StreamWriter(fileStream))
            {
                writer.Write(content);
            }
            Debug.Log($"Successfully wrote file: {path}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to write file '{path}': {e.Message}");
        }
    }

    //private static string ReadFile (string path) {
    //    if (File.Exists (path)) {
    //        using (StreamReader reader = new StreamReader (path)) {
    //            string content = reader.ReadToEnd ();
    //            return content;
    //        }
    //    }
    //    return "";
    //}
    private static string ReadFile(string path)
    {
        if (string.IsNullOrEmpty(path))
        {
            Debug.LogError("Cannot read file: path is null or empty!");
            return "";
        }

        try
        {
            if (File.Exists(path))
            {
                using (StreamReader reader = new StreamReader(path))
                {
                    string content = reader.ReadToEnd();
                    Debug.Log($"Successfully read file: {path}");
                    return content;
                }
            }
            else
            {
                Debug.LogWarning($"File does not exist: {path}");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to read file '{path}': {e.Message}");
        }

        return "";
    }
}

public static class JsonHelper {
    public static T[] FromJson<T> (string json) {
        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>> (json);
        return wrapper.Items;
    }

    public static string ToJson<T> (T[] array) {
        Wrapper<T> wrapper = new Wrapper<T> ();
        wrapper.Items = array;
        return JsonUtility.ToJson (wrapper);
    }

    public static string ToJson<T> (T[] array, bool prettyPrint) {
        Wrapper<T> wrapper = new Wrapper<T> ();
        wrapper.Items = array;
        return JsonUtility.ToJson (wrapper, prettyPrint);
    }

    [Serializable]
    private class Wrapper<T> {
        public T[] Items;
    }
}